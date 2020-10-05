using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Extension;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.Services.Identity;
using shop.ViewModels.Identity.Settings;
using shop.ViewModels.Reservation;
using static shop.Entities.Identity.Enum.UserTypeEnum;

namespace shop.Api.Client
{
    [Route("api/client/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Client)]
    [ApiExplorerSettings(GroupName = "v1.0")]
    public class OrderController : Controller
    {
        #region fields
        private readonly IOrderService _orderService;
        private readonly IProductOptionService _productOptionService;
        private readonly IProductService _productService;
        private IApplicationUserManager _usersService;
        private readonly int _userId;
        private readonly IOrderProductService _orderProductService;
        private readonly IOptionsSnapshot<PaymentSetting> _paymentSetting;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWork _uow;
        public readonly IApplicationRoleManager _applicationRoleManager;

        #endregion

        #region Ctor
        public OrderController(IOrderService orderService,
            IProductOptionService productOptionService,
            IProductService productService,
            IApplicationUserManager usersService,
            IHttpContextAccessor httpContextAccessor,
            IOrderProductService orderProductService,
            IUploadService uploadService,
            IOptionsSnapshot<PaymentSetting> paymentSetting,
            IUnitOfWork uow,
            IApplicationRoleManager applicationRoleManager
            )
        {
            _uploadService = uploadService;
            _orderService = orderService;
            _productOptionService = productOptionService;
            _productService = productService;
            _usersService = usersService;
            _userId = httpContextAccessor.UserId();
            _orderProductService = orderProductService;
            _paymentSetting = paymentSetting;
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _applicationRoleManager = applicationRoleManager;
        }
        #endregion

        #region Methods
        /// <summary>
        /// ثبت اولیه سبد خرید
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Initial([FromBody]OrderProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");
            try
            {
                var user = await _usersService.FindByIdAsync(_userId.ToString());
                bool isReseller = user.UserType == UserType.Reseller ? true : false;
                var discount = isReseller ? user.PercentDiscount : 0;
                if (discount == null)
                {
                    discount = 0;
                }
                // سبد پرداخت نشده
                bool oldBasket = false;
                var order = new Order();
                order.UserId = _userId;
                // 1 - ابتدا بررسی سبد های مشتری
                var exist = await _orderService.Get(userId: _userId, orderType: OrderType.Unpaid);

                if (exist.Count > 0)
                {
                    order = exist.Orders.FirstOrDefault();
                    if (order.IsReserved)
                        return BadRequest("ابتدا نسبت به پرداخت سبد رزرو شده اقدام نمایید");

                    oldBasket = true;
                }
                order.OrderProducts = new List<OrderProduct>();
                // یعنی محصول بدون فیلد بوده است
                if (model.ProductOptionId == null)
                {
                    var output = await _productService.ComputPrice(model.ProductId, model.Count);
                    if (!output.success)
                        return StatusCode((int)output.status, output.error);

                    order.OrderProducts.Add(new OrderProduct
                    {
                        Count = model.Count,
                        // fieldId = model.fieldId, چون نداره در این حالت
                        // OptionColorId = model.OptionColorId, چون نداره در این حالت
                        OrderId = oldBasket ? order.Id : 0,
                        ProductId = model.ProductId,
                        TotalPrice = output.price,
                        UnitPrice = output.unitPrice,
                        //ProductOptionId = model.ProductOptionId چون نداره در این حالت
                    });
                    order.TotalPrice += output.price;
                    order.DiscountPercent = discount.Value;
                    order.DiscountPrice = (order.TotalPrice * discount.Value) / 100;
                    order.TotalWithDiscountPrice = order.TotalPrice - order.DiscountPrice;
                    order.OrderType = OrderType.Unpaid;
                    order.OrderSendType = OrderSendType.Pending;

                }
                // محاسبه قیمت بر اساس اپشن ها
                else
                {
                    var output = await _productOptionService.ComputPrice(model.ProductId, model.ProductOptionId.Value, model.OptionColorId, model.Count, isReseller: isReseller);

                    if (!output.success)
                        return StatusCode((int)output.status, output.error);

                    order.OrderProducts.Add(new OrderProduct
                    {
                        Count = model.Count,
                        fieldId = model.fieldId,
                        OptionColorId = model.OptionColorId,
                        OrderId = oldBasket ? order.Id : 0,
                        ProductId = model.ProductId,
                        TotalPrice = output.price,
                        UnitPrice = output.unitPrice,
                        TotalForReseller = output.priceReseller,
                        ProductOptionId = model.ProductOptionId
                    });
                    if (isReseller)
                    {
                        order.TotalPrice += output.priceReseller;

                    }
                    else
                    {
                        order.TotalPrice += output.price;

                    }

                    order.DiscountPercent = discount == null ? 0 : discount.Value;
                    order.DiscountPrice = (order.TotalPrice * discount.Value) / 100;
                    order.TotalWithDiscountPrice = order.TotalPrice - order.DiscountPrice;
                    order.OrderType = OrderType.Unpaid;
                    order.OrderSendType = OrderSendType.Pending;
                    //order.ShippingCost = 11000;
                }
                // سبد خرید قبلی رو ویرایش کن
                if (oldBasket)
                {
                    var output = await _orderService.Update(order);
                    if (!output.success)
                        return StatusCode((int)output.status, output.error);
                }
                // سبد جدید ایجاد کن
                else
                {
                    var output = await _orderService.Create(order);

                    if (!output.success)
                        return StatusCode((int)output.status, output.error);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا در ثبت محصول در سبد خرید !");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagingViewModel<OrderViewModel>> Get(int? id, int pageIndex, int pageSize)
        {

            var inculde = new List<Expression<Func<Order, object>>> { };

            var output = await _orderService.Get(id: id, userId: _userId, pageIndex: pageIndex, pageSize: pageSize,
                includes: inculde.ToArray());

            var response = new PagingViewModel<OrderViewModel>
            {
                Pages = output.Orders.Select(a => OrderViewModel.FromEntity(a)).ToList(),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }
        /// <summary>
        /// نمایش سبد خرید برای مشتری
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<OrderBasketViewModel> GetForUser()
        {
            /// <summary>
            ///  چون ممکنه درصد تخیف که به کاربر اختصاص داده میشه، تغییر کنه
            ///  یا اصلا کسی قبلا تخفیف نداشته الان داره
            /// </summary>
            var user = await _usersService.FindByIdAsync(_userId.ToString());
            bool isReseller = user.UserType == UserType.Reseller ? true : false;
            var discount = isReseller ? user.PercentDiscount : 0;

            var output = await _orderService.GetByUserID(userId: _userId, orderType: OrderType.Unpaid);
            if (output == null)
                return output;
            if (discount == null)
            {
                discount = 0;
            }



            output.DiscountPercent = discount.Value;
            output.DiscountPrice = (output.TotalPrice * discount.Value) / 100;
            output.TotalWithDiscountPrice = output.TotalPrice - output.DiscountPrice;
            return output;
        }


        [HttpGet]
        public async Task<OrderViewModel> GetOrderProduct(int? orderId)
        {
            var inculdeOrder = new List<Expression<Func<Order, object>>> { };

            var inculde = new List<Expression<Func<OrderProduct, object>>> { };
            inculde.Add(c => c.OptionColor);
            inculde.Add(c => c.ProductOption);


            inculde.Add(c => c.Product);

            var output = await _orderProductService.Get(orderId: orderId,
                includes: inculde.ToArray());

            var order = (await _orderService.Get(id: orderId, userId: _userId, pageIndex: 0, pageSize: 0,
                includes: inculdeOrder.ToArray())).Orders.FirstOrDefault();


            var response = new OrderViewModel
            {

                OrderProducts = output.OrderProducts.Select(a => OrderProductViewModel.FromEntity(a)).ToList(),
                DiscountPercent = order.DiscountPercent,
                DiscountPrice = order.DiscountPrice,
                Id = order.Id,
                OrderSendType = order.OrderSendType,
                OrderType = order.OrderType,
                PaidDateTime = order.PaidDateTime,
                ReciverAddress = order.ReciverAddress ?? "",
                ReciverMobile = order.ReciverMobile ?? "",
                ReciverName = order.ReciverName ?? "",
                SenderMobile = order.SenderMobile ?? "",
                SenderName = order.SenderName ?? "",
                TotalPrice = order.TotalPrice,
                TotalWithDiscountPrice = order.TotalWithDiscountPrice



            };

            return response;
        }




        /// <summary>
        /// ثبت تغییرات سبد و مشخصات ادرس فرستنده و گیرنده
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]

        public async Task<IActionResult> Save([FromBody]OrderBasketSaveViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");
            try
            {
                var order = model.ToEntity();

                var filesIcon = new List<(string base64File, string filename, bool ValidSize)>();

                #region validFile
                if (model.Picture != default)
                {
                    filesIcon = await _uploadService.GetFileName(model.Picture, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 4048000);
                    if (filesIcon.FirstOrDefault().ValidSize != true)
                        return BadRequest($"حداکثر سایز مجاز برای عکس KB 2000 است");

                    order.Picture = filesIcon.FirstOrDefault().filename;
                }
                #endregion
                var roles = _applicationRoleManager.FindUserRoles(_userId);

                var output = await _orderService.SaveBasket(order, _userId, roles.ToList());
                if (!output.success)
                    return StatusCode((int)output.status, output.error);


                if (model.Picture != null)
                {
                    var file = (filesIcon.FirstOrDefault().base64File, filesIcon.FirstOrDefault().filename);
                    await _uploadService.Upload(id: (output.order.Id).ToString(), file: file, uploadType: UploadType.Order, width: 850);
                }

                // حالت رزرو
                if (model.IsReserved)
                    return Ok(new { message = "تا یک ساعت زمان دارید نسبت به پرداخت محصول عمل نمایید." });

                if (roles.Any(c => c.Name == ConstantRoles.SuperAdmin || c.Name == ConstantRoles.ClientAdmin))
                    return Ok(new { result = "admin" });

                //if (_userId == 1 || _userId == 1374|| _userId == 1935)


                return Ok(new { gatewayUrl = $"{_paymentSetting.Value.Url}/{output.order.Id}" });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا در ثبت محصول در سبد خرید !");
            }
        }

        /// <summary>
        /// حذف ایتم از سبد خرید براساس
        /// orderProductId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<RemoveItemBasketViewModel> Delete(int id)
        {
            try
            {
                // حذف orderproduct
                var output = await _orderProductService.Delete(id);
                var user = await _usersService.FindByIdAsync(userId: _userId.ToString());
                if (!output.success)
                    return new RemoveItemBasketViewModel
                    {
                        TotalPrice = 0,
                        DiscountPrice = 0,
                        totalWithDiscountPrice = 0
                    };


                bool isReseller = user.UserType == UserType.Reseller ? true : false;


                // تاثیر بر روی قیمت order
                var result = await _orderService.ChangePrice(id: output.orderId,
                    isReseller ?
                    output.resellerPrice : output.price

                    , _userId);
                // خروجی قیمت جدید سبد 
                return new RemoveItemBasketViewModel
                {
                    TotalPrice = result.totalPrice,
                    DiscountPrice = result.discountPrice,
                    totalWithDiscountPrice = result.totalWithDiscountPrice
                };
            }
            catch (Exception ex)
            {
                return new RemoveItemBasketViewModel
                {
                    TotalPrice = 0,
                    DiscountPrice = 0,
                    totalWithDiscountPrice = 0
                };
            }
        }
        #endregion
    }
}