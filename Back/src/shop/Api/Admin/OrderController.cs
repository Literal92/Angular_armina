using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Extension;
using shop.Services.Contracts;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.Services.Identity;
using shop.ViewModels.Reservation;

namespace shop.Api.Admin.Controllers
{
    [Route("api/Admin/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class OrderController : Controller
    {
        #region fields
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;
        private readonly IProductOptionService _productOptionService;
        private readonly IUnitOfWork _uow;

        private IWebHostEnvironment _hostingEnvironment;
        private readonly int _userId;

        #endregion

        #region Ctor
        public OrderController(IOrderService orderService,
            IOrderProductService orderProductService,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment hostingEnvironment,
            IProductOptionService productOptionService,
            IUnitOfWork uow
             )
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
            _userId = httpContextAccessor.UserId();
            _hostingEnvironment = hostingEnvironment;
            _productOptionService = productOptionService;
            _uow = uow;

        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="categoryId"></param>        
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = ConstantPolicies.OrderView)]
        public async Task<PagingViewModel<OrderViewModel>> Get(int? id, string username, DateTime? from = null, DateTime? to = null,
            OrderType? orderType = null,
            OrderSendType? orderSendType = null,
            string productName = null,
            string reciverName = null,
            string reciverMobile = null,
            string senderName = null,
            string senderMobile = null,
            string address = null,
            int pageIndex = 0, int pageSize = 20)
        {

            var inculde = new List<Expression<Func<Order, object>>> { };

            inculde.Add(c => c.User);


            var output = await _orderService.Get(id: id, userName: username, from: from,
                to: to,
                reciverMobile: reciverMobile,
                reciverName: reciverName,
                senderMobile: senderMobile,
                senderName: senderName,
                orderSendType: orderSendType,
                address: address,
                productName: productName,
                orderType: orderType,
                pageIndex: pageIndex, pageSize: pageSize,
                includes: inculde.ToArray());

            var response = new PagingViewModel<OrderViewModel>
            {
                Pages = output.Orders.Select(a => OrderViewModel.FromEntity(a)).ToList(),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }

        [HttpPost]
        [Authorize(Policy = ConstantRoles.Admin)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([FromBody] OrderViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");

            try
            {
                var files = new List<(string base64File, string filename, bool ValidSize)>();

                //// get filename


                var output = await _orderService.Create(model.ToEntity());

                if (!output.success)
                    return StatusCode((int)output.status, new { message = output.error });


                // return Created("Create", output.product.Id);

                return Ok(true);

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = ConstantRoles.Admin)]
        [DisableRequestSizeLimit]

        public async Task<IActionResult> Update(int id, [FromBody] OrderViewModel model)
        {
            if (id != model.Id)
                return BadRequest("عدم اعتبار داده های ورودی !");
            try
            {

                var product = await _orderService.GetById(id: id);

                if (product == null)
                    return NotFound(new { message = "ایتمی یافت نشد !" });

                var productToUpdate = model.ToEntity();


                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }

        }


        [HttpDelete("{id:int}")]
        [Authorize(Policy = ConstantRoles.Admin)]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _orderService.Delete(id);

                if (!result.success)
                    return StatusCode((int)result.status, new { message = result.error });

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }



        [HttpGet]
        [Authorize(Policy = ConstantRoles.Admin)]

        public async Task<PagingViewModel<OrderProductViewModel>> GetOrderProduct(int? orderId)
        {

            var inculde = new List<Expression<Func<OrderProduct, object>>> { };
            inculde.Add(c => c.OptionColor);
            inculde.Add(c => c.ProductOption);


            inculde.Add(c => c.Product);

            var output = await _orderProductService.Get(orderId: orderId,
                includes: inculde.ToArray());

            var response = new PagingViewModel<OrderProductViewModel>
            {
                Pages = output.OrderProducts.Select(a => OrderProductViewModel.FromEntity(a)).ToList(),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }





        /// <summary>
        /// تکمیل فرایند خریدهای ناقص
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = ConstantRoles.Admin)]

        public async Task<IActionResult> Complete([FromBody]OrderBasketSaveViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");
            try
            {

                //if (_userId == 1)
                {
                    // update order
                    var order = await _orderService.GetById(id: model.Id, includes: new Expression<Func<Order, object>>[] { c => c.OrderProducts });


                    order.PaidDateTime = DateTime.UtcNow;
                    order.OrderType = OrderType.Paid;
                    if (order.OrderProducts == null)
                        return View(new PaymentResultViewModel
                        {
                            Success = false,
                            Message = "درخواست نا معتبر است !",
                            TransactionCode = string.Empty
                        });

                    // کم کردن از موجودی محصول
                    var orderProducts = order.OrderProducts.ToList();
                    var listOptions = orderProducts.Select(c => c.ProductOptionId.Value).ToList();

                    var options = await _productOptionService.GetByIds(Ids: listOptions,
                                 includes: new Expression<Func<ProductOption, object>>[] { c => c.OptionColors });

                    var colors = options.SelectMany(c => c.OptionColors).ToList();
                    foreach (var item in orderProducts)
                    {
                        if (item.OptionColorId == null)
                        {
                            var option = options.FirstOrDefault(c => c.Id == item.ProductOptionId);
                            if (option != null)
                                option.Count -= item.Count;

                            continue;
                        }

                        var color = colors.FirstOrDefault(c => c.Id == item.OptionColorId);
                        if (color != null)
                            color.Count -= item.Count;

                    }
                    await _uow.SaveChangesAsync();

                    return Ok(new { result = "admin" });
                }


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا در ثبت محصول در سبد خرید !");
            }
        }


        /// <summary>
        /// تکمیل فرایند خریدهای ناقص
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = ConstantPolicies.OrderAdmin)]

        public async Task<IActionResult> ChangeSenderType([FromBody]OrderBasketSaveViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");


            await _orderService.ChangeSender(model.Id);
            return Ok(new { result = "true" });

        }

        [HttpGet]
        [Authorize(Policy = ConstantPolicies.OrderAdmin)]

        public async Task<List<GetOrderDontSendWithAddressViewModel>> GetOrdersByAddress()
        {
            return await _orderService.GetOrderDontSendWithAddress();
        }
        [HttpPut("{id:int}")]
        [Authorize(Policy = ConstantRoles.Admin)]

        public async Task<IActionResult> EditAddress(int id, [FromBody]EditAddressViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");

            try
            {
                var output = await _orderService.EditAddress(model);
                if (!output.success)
                    return StatusCode((int)output.status, output.error);

                return Ok(true);
            }
            catch (Exception Ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }
        [HttpDelete("{id:int}")]
        [Authorize(Policy = ConstantRoles.Admin)]
        public async Task<IActionResult> DeleteByOrderProductId(int id)
        {
            try
            {
                var output = await _orderService.DeleteByOrderProductId(id);
                if (!output.success)
                    return StatusCode((int)output.status, output.error);

                return Ok(true);
            }
            catch (Exception Ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }
        [HttpPut("{id}")]
        [Authorize(Policy = ConstantPolicies.AccountantAdmin)]
        public async Task<IActionResult> AcceptPaymnet(int id, [FromBody] AcceptOrderViewModel model)
        {
            try
            {
                if (id != model.Id)
                    return BadRequest("عدم اعتبار داده های ورودی !");

                var output = await _orderService.AcceptPaymnet(model.Id, model.Accept);

                if (!output.success)
                    return StatusCode((int)output.status, output.error);

                return Ok(true);
            }
            catch (Exception Ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }

        }

        [HttpGet]
        [Authorize(Policy = ConstantPolicies.ReportAdmin)]
        public async Task<List<ReportViewModel>> GetDailyReport (DateTime? from, DateTime? to)
        {
            try
            {
                return await _orderService.GetReportDaily(from, to); 
            }
            catch
            {
                return new List<ReportViewModel>();
            }

        }
        [HttpGet]
        [Authorize(Policy = ConstantPolicies.ReportAdmin)]
        public async Task<List<ReportMonthViewModel>> GetMonthReport(int? from, int? to, int year)
        {
            try
            {
                return await _orderService.GetReportMonthly(from, to, year);
            }
            catch
            {
                return new List<ReportMonthViewModel>();
            }
        }
        [HttpGet]
        public async Task<bool> SetShamsiDate()
        {
            try
            {
                var output= await _orderService.SetShamsiDate();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}