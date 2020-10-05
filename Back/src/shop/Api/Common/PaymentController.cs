using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.ViewModels.Identity.Settings;
using shop.ViewModels.Reservation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zarinpal;
using static shop.Entities.Identity.Enum.UserTypeEnum;

namespace shop.Api.Common
{
    [Route("[controller]/[action]")]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    //[Authorize(Policy = "Client")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class PaymentController : Controller
    {
        #region Fields
        private readonly IUnitOfWork _uow;

        private readonly IOptionsSnapshot<PaymentSetting> _paymentSetting;
        private readonly IGateWayService _gateWayService;
        private readonly IOrderService _orderService;
        private readonly IProductOptionService _productOptionService;
        private IApplicationUserManager _usersService;

        #endregion


        #region Ctor
        public PaymentController(IUnitOfWork uow, IOptionsSnapshot<PaymentSetting> paymentSetting,
            IGateWayService gateWayService, IOrderService orderService,
            IProductOptionService productOptionService,
            IApplicationUserManager usersService
            )
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _paymentSetting = paymentSetting;
            _gateWayService = gateWayService;
            _orderService = orderService;
            _productOptionService = productOptionService;
            _usersService = usersService;
        }
        #endregion


        #region Methods     


        //public IActionResult Pay()
        //{
        //    return View(new PayViewModel());
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> Pay(int id)
        {
            var callbackUrl = _paymentSetting.Value.CallBackUrl;

            try
            {
                // دریافت مرچنت کد از تنظیمات 
                var merchantID = _paymentSetting.Value.MerchentId;
                var order = await _orderService.GetById(id, new Expression<Func<Order, object>>[] { c => c.User, c => c.OrderProducts, c => (c.OrderProducts as OrderProduct).OptionColor });

                // اتصال به درگاه
                if (order == null)
                {
                    var success = false;
                    return Redirect($"{callbackUrl}?success={success}");
                }
                var user = await _usersService.FindByIdAsync(order.UserId.ToString());
                if (user== null)
                {
                    var success = false;
                    string error = "کاربر یافت نشد !";
                    return Redirect($"{callbackUrl}?success={success}&error={error}");
                }
                var isReseller = user.UserType == UserType.Reseller ? true : false;
                //عدم اتصال به درگاه،لطفا با پشتیبانی تماس بگیرید
                // ثیت اولیه در gateWay 

                var gateWay = await _gateWayService.Create(new GateWay
                {
                    Amount = order.TotalWithDiscountPrice + 11000,
                    UserId = order.UserId,
                    OrderId = order.Id
                });

                if (gateWay == null)
                {
                    var success = false;
                    return Redirect($"{callbackUrl}?success={success}");
                }

                if (order.OrderProducts == null || !order.OrderProducts.Any())
                {
                    var success = false;
                    string error = "cart is empty !";
                    return Redirect($"{callbackUrl}?success={success}&error={error}");
                }
                string product = "";


                if (order.OrderProducts != null && order.OrderProducts.Any())
                {
                    foreach (var item in order.OrderProducts)
                    {
                        product += "pid:" + item.ProductId + "-poid:" + item.ProductOptionId + "-coid:" + item.OptionColorId + "-count:" + item.Count + "*";
                    }
                }

                // بررسی قیمت

                int compute = 0;
                foreach (var item in order.OrderProducts)
                {
                    var optionPrice = isReseller? item.OptionColor.ResellerPrice : item.OptionColor.Price;
                    var mul = optionPrice * item.Count;
                    compute += mul.Value;
                }
                if (compute != order.TotalWithDiscountPrice)
                {
                    var success = false;
                    string error = "عدم تطابق سبد خرید با مجموع قیمت !";
                    return Redirect($"{callbackUrl}?success={success}&error={error}");
                }


                var amount = order.TotalWithDiscountPrice + 11000;
                if (amount <= 11000)
                {
                    var success = false;
                    string error = "عدم تطابق سبد خرید با مجموع قیمت !";

                    return Redirect($"{callbackUrl}?success={success}&error={error}");
                }

                var callback = callbackUrl + "?" + "id=" + gateWay.Id + "&" + $"orderId={order.Id}";

                var outPayment = await new Payment(merchantID, amount)
                          .PaymentRequest(
                           //"پرداخت " + gateWay.Id.ToString()
                           "oid:" + order.Id + " " + order.User.UserName + " " + product
                          ,
                              callbackUrl: callback,
                              order.User?.Email,
                              order.User?.UserName);

                #region zarinpal v3

                //var payment = new Payment();
                //// برای سند باکس باید کتابخانه ازمایش زرین پال را نصب کنید
                //var mode = _paymentSetting.Value.IsSandboxZarin == true ?
                //           Payment.Mode.sandbox :
                //          Payment.Mode.zarinpal;
                ////  متاسفانه برای این ورژن جدید تغییراتی داده است
                //// از جمله این که ابجکت زیر نوعش رو فقط نوشته ابجک، 
                //// نگفته از نوع کدوم کلاس است
                //// که از تویه نمونه کدش کلاسش رو برداشتم
                //var data = new PaymentRequestViewModel
                //(
                //    MerchantID: merchantID,
                //    Amount: amount,
                //    CallbackURL: callbackUrl + $"?id={gateWay.Id}&orderId={order.Id}",
                //    Description: gateWay.Id.ToString()
                //);
                // var outPayment = await payment.Request(data: data, mode);

                #endregion

                if (outPayment.Status == 100)
                {
                    return Redirect(outPayment.Link);
                }
                else
                {
                    var success = false;
                    return Redirect($"{callbackUrl}?id={gateWay.Id}&orderId={order.Id}&authority={outPayment.Authority}&success={success}&status={outPayment.Status}");

                }
            }
            catch (Exception ex)
            {
                var success = false;
                return Redirect($"{callbackUrl}?success={success}");

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">GatWayId</param>
        /// <param name="authority"></param>
        /// <param name="status"></param>
        /// <param name="reserveId"></param>
        /// <param name="appFromReserve"></param>
        /// <param name="walletApp"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ResultPay(int? id, string authority, string status, int? orderId,
                                                    bool success = false, string error=null)
        {
            try
            {

                //if (!success)
                //{
                //    return View(new PaymentResultViewModel
                //    {
                //        Success = false,
                //        Message = !string.IsNullOrEmpty(error)? error: "درخواست نامعتبر" + " success",
                //        TransactionCode = authority,
                //        Id= orderId.ToString()
                //    });
                //}

                // عددي كه نشان دهنده موفق بودن يا عدم موفق بودن پرداخت ميباشد.
                //var status = HttpContext.Request.Query["Status"];
                //var reserveId = string.IsNullOrEmpty(HttpContext.Request.Query["reserveId"].ToString()) ? null : HttpContext.Request.Query["reserveId"].ToString();
                ////كد يكتاي شناسه مرجع درخواست.
                //var authority = HttpContext.Request.Query["Authority"];
                if (id == null || orderId == null || status == null)
                    return View(new PaymentResultViewModel
                    {
                        Success = false,
                        Message = "درخواست نامعتبر",
                        TransactionCode = authority,
                        Id = orderId.ToString()

                    });

                var gateWay = await _gateWayService.GetById(id: id.Value,
                    includes: new Expression<Func<GateWay, object>>[] { c => c.User });


                if (gateWay == null)
                {
                    return View(new PaymentResultViewModel
                    {
                        Success = false,
                        Message = "درخواست نا معتبر است !",
                        TransactionCode = authority,
                        Id = orderId.ToString()

                    });

                }
                if (status != "" && (status.ToString().ToLower() == "ok" || status.ToString().ToLower() == "100") && authority != "")
                {
                    var payment = new Payment(merchantId: _paymentSetting.Value.MerchentId, amount: (int)gateWay.Amount);

                    var res = payment.Verification(authority: authority.ToString()).Result;
                    if (res.Status == 100)
                    {
                        // update getway
                        gateWay.Status = res.Status;
                        gateWay.Authority = authority.ToString();
                        // کدرهگیری
                        gateWay.RefId = res.RefId;
                        gateWay.Authority = authority;
                        gateWay.Success = true;

                        // update order
                        var order = await _orderService.GetById(id: orderId.Value, includes: new Expression<Func<Order, object>>[] { c => c.OrderProducts, c => c.User });
                        if (order == null)
                            return View(new PaymentResultViewModel
                            {
                                Success = false,
                                Message = "درخواست نا معتبر است !",
                                TransactionCode = authority
                            });

                        order.PaidDateTime = DateTime.UtcNow;
                        order.Year = DateTime.UtcNow.GetPersianYear();
                        order.MonthOfYear = DateTime.UtcNow.GetPersianMonth();
                        order.OrderType = OrderType.Paid;
                        order.RefId = res.RefId;
                        if (order.OrderProducts == null)
                            return View(new PaymentResultViewModel
                            {
                                Success = false,
                                Message = "درخواست نا معتبر است !",
                                TransactionCode = string.Empty,
                                Id = orderId.ToString()

                            });

                        if (!order.IsReserved)
                        {
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


                        }

                        await _uow.SaveChangesAsync();

                        try
                        {
                            if (order.User != null && order.User.UserName.IsValidIranianMobileNumber())
                            {
                                var api = new Kavenegar.KavenegarApi("41723573576F786B63616A7263594F6346734D584E52395036536A7836456279");
                                var res2 = api.Send("10002020022000", order.User.UserName, "با سلام سفارش شما با کد " + order.Id + " در نی نی حراجی ثبت شد ");
                            }
                        }
                        catch (Exception)
                        {

                        }



                        return View(new PaymentResultViewModel
                        {
                            Success = true,
                            Message = "تراکنش با موفقیت انجام شد !",
                            TransactionCode = res.RefId.ToString(),
                            Id = orderId.ToString()

                        });


                    }
                    else
                    {
                        gateWay.Status = res.Status;
                        gateWay.Authority = authority.ToString();
                        gateWay.RefId = res.RefId;
                        gateWay.Success = false;
                        var output = await _gateWayService.Update(Id: gateWay.Id, gateWay: gateWay);
                        //result = (succeed: false, error: "خطا در درگاه بانک", gateWay: gateWay);
                        return View(new PaymentResultViewModel
                        {
                            Success = false,
                            Message = "خطا در درگاه بانک",
                            TransactionCode = string.Empty,
                            Id = orderId.ToString()

                        });
                    }
                }
                else
                {
                    if (status == null)
                    {
                        status = "";
                    }
                    gateWay.Status = -1;
                    gateWay.Authority = authority.ToString() + " " + status.ToString().ToLower();

                    var output = await _gateWayService.Update(Id: gateWay.Id, gateWay: gateWay);
                    //result = (succeed: false, error: "خطا در درگاه بانک", gateWay: gateWay);
                    return View(new PaymentResultViewModel
                    {
                        Success = false,
                        Message = "خطا در درگاه بانک",
                        TransactionCode = string.Empty,
                        Id = orderId.ToString()

                    });
                }


            }
            catch (Exception ex)
            {
                return View(new PaymentResultViewModel
                {
                    Success = false,
                    Message = "خطا در درگاه بانک",
                    TransactionCode = string.Empty,
                    Id = orderId.ToString()

                });
            }
        }


        #endregion

    }
}
