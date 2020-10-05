using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using shop.ViewModels.Reservation;
using System.Net;
using DNTPersianUtils.Core;
using shop.Services.Contracts.Identity;
using static shop.Entities.Identity.Enum.UserTypeEnum;
using shop.Entities.Identity;
using shop.Services.Identity;

namespace shop.Services.Reservation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Order> _order;
        private readonly IOrderProductService _orderProductService;
        private IApplicationUserManager _usersService;
        private IProductOptionService _productOptionService;
        private IOptionColorService _optionColorService;

        public OrderService(IUnitOfWork uow, IOrderProductService orderProductService, IApplicationUserManager usersService, IProductOptionService productOptionService, IOptionColorService optionColorService)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _order = _uow.Set<Order>();
            _orderProductService = orderProductService;
            _usersService = usersService;
            _productOptionService = productOptionService;
            _optionColorService = optionColorService;
        }
        /// <summary>
        /// </summary>
        /// <returns></returns>

        public async Task<(List<Order> Orders, int Count, int TotalPages, HttpStatusCode status)> Get(
         int? id = 0,
         int? userId = null,
         string userName = null,
         string reciverName = null,
         string reciverMobile = null,
         string senderName = null,
         string senderMobile = null,
         string address = null,
         string productName = null,
         OrderSendType? orderSendType = null,

         OrderType? orderType = null,
         DateTime? from = null, DateTime? to = null,
         int pageIndex = 0, int pageSize = 0, Expression<Func<Order, object>>[] includes = null)
        {
            var queryable = _order.AsQueryable();

            queryable = id != null && id > 0 ? queryable.Where(c => c.Id == id) : queryable;

            queryable = userId != null ? queryable.Where(c => c.UserId == userId) : queryable;


            //queryable = categoryId != null ? queryable.Where(p => p.CategoryId == categoryId) : queryable;
            // title
            queryable = (userName != null && userName != "null") ?
                queryable.Where(c => EF.Functions.Like(c.SenderMobile, $"%{userName.Trim()}%")
                || EF.Functions.Like(c.ReciverMobile, $"%{userName.Trim()}%")
                ||
                 c.User.UserName == userName
                ) : queryable;


            queryable = (address != null && address != "null" && address != "") ?
              queryable.Where(c => EF.Functions.Like(c.ReciverAddress, $"%{address.Trim()}%")
              ) : queryable;

            queryable = (reciverMobile != null && reciverMobile != "null" && reciverMobile != "") ?
            queryable.Where(c => EF.Functions.Like(c.ReciverMobile, $"%{reciverMobile.Trim()}%")
            ) : queryable;

            queryable = (reciverName != null && reciverName != "null" && reciverName != "") ?
           queryable.Where(c => EF.Functions.Like(c.ReciverName, $"%{reciverName.Trim()}%")
           ) : queryable;

            queryable = (senderName != null && senderName != "null" && senderName != "") ?
          queryable.Where(c => EF.Functions.Like(c.SenderName, $"%{senderName.Trim()}%")
          ) : queryable;


            queryable = (senderMobile != null && senderMobile != "null" && senderMobile != "") ?
          queryable.Where(c => EF.Functions.Like(c.SenderMobile, $"%{senderMobile.Trim()}%"))
          : queryable;

            queryable = !string.IsNullOrEmpty(productName) && productName != "null" ?
                         queryable.Where(c => c.OrderProducts.Any(z => EF.Functions.Like(z.Product.Title.Trim(), $"%{productName.Trim()}%")))
                        : queryable;

            queryable = orderSendType != null ? queryable.Where(c => c.OrderSendType == orderSendType) : queryable;


            queryable = orderType != null ? queryable.Where(c => c.OrderType == orderType) : queryable;

            queryable = from != null ? queryable.Where(c => c.PaidDateTime >= from.Value.Date) : queryable;
            queryable = to != null ? queryable.Where(c => c.PaidDateTime <= to.Value.Date) : queryable;

            if (orderType == OrderType.Paid)
            {
                queryable = queryable//.OrderByDescending(c => c.Id)
               .OrderByDescending(p => p.PaidDateTime);
            }

            else //if (orderType == OrderType.Unpaid)
            {
                queryable = queryable//.OrderByDescending(c => c.Id)
               .OrderByDescending(p => p.Id);
            }

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize > 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (Expression<Func<Order, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.AsNoTracking().ToListAsync();

            return (Orders: list, Count: count, TotalPages: totalPages, status: HttpStatusCode.OK);
        }
        public async Task<Order> GetById(int id = 0, Expression<Func<Order, object>>[] includes = null)
        {
            var queryable = _order.AsQueryable();

            queryable = queryable.Where(c => c.Id == id);

            if (includes != null)
                foreach (Expression<Func<Order, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            return await queryable.FirstOrDefaultAsync();
        }
        public async Task<(Order product, bool success, HttpStatusCode status, string error)> Create(Order model)
        {
            try
            {
                foreach (var item in model.OrderProducts)
                {

                    if (item.OptionColorId != null)
                    {
                        var color = await _optionColorService.GetById(item.OptionColorId.Value);
                        if (color != null)
                        {
                            // تعداد سفارش داده شده
                            var countBuy = item.Count;
                            //تعداد موجود
                            var count = color.Count;

                            if (countBuy > count)
                                return (null, false, HttpStatusCode.BadRequest, "موجودی کمتر از تقاضا !");
                        }
                    }
                }


                await _order.AddAsync(model);
                await _uow.SaveChangesAsync();
                return (model, true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, status: HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }

        public async Task<(Order product, string oldFile, bool success, HttpStatusCode status, string error)> Update(Order model)
        {
            try
            {
                var queryable = _order.AsQueryable();

                var find = await queryable.Where(c => c.Id == model.Id).Include(c => c.OrderProducts).FirstOrDefaultAsync();
                if (find == null)
                    return (product: null, oldFile: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                _uow.Entry(find).CurrentValues.SetValues(model);

                if (find.OrderProducts != null
                    //&& find.OrderProducts.Any() 
                    && model.OrderProducts != null)
                {
                    foreach (var item in model.OrderProducts)
                    {
                        var exist = find.OrderProducts.FirstOrDefault(c => c.fieldId == item.fieldId &&
                                                                       c.ProductOptionId == item.ProductOptionId &&
                                                                       c.OptionColorId == item.OptionColorId);

                        if (item.OptionColorId != null)
                        {
                            var color = await _optionColorService.GetById(item.OptionColorId.Value);
                            if (color != null)
                            {
                                // تعداد سفارش داده شده
                                var countBuy = exist != null ? exist.Count + item.Count : item.Count;
                                //تعداد موجود
                                var count = color.Count;

                                if (countBuy > count)
                                    return (null, null, false, HttpStatusCode.BadRequest, "موجودی کمتر از تقاضا !");
                            }
                        }


                        if (exist != null)
                        {

                            exist.Count += item.Count;
                            exist.TotalPrice += item.TotalPrice;
                            exist.TotalForReseller += item.TotalForReseller;

                            await _orderProductService.Update(exist);
                        }
                        else
                            await _orderProductService.Create(item);
                    }
                }

                await _uow.SaveChangesAsync();
                return (find, oldFile: "", success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, oldFile: null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }





        public async Task<(Order product, string oldFile, bool success, HttpStatusCode status, string error)> ChangeSender(int id)
        {
            try
            {
                var queryable = _order.AsQueryable();

                var find = await queryable.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    return (product: null, oldFile: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");
                if (find.OrderSendType == OrderSendType.Pending)
                {
                    find.OrderSendType = OrderSendType.Accepted;
                }

                else if (find.OrderSendType == OrderSendType.Accepted)
                {
                    find.OrderSendType = OrderSendType.Pending;
                }


                await _uow.SaveChangesAsync();
                return (find, oldFile: "", success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, oldFile: null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }






        public async Task<(Order order, bool success, HttpStatusCode status, string error)>
                                                            SaveBasket(Order model, int userId,List<Role> roles)
        {
            try
            {
                var user = await _usersService.FindByIdAsync(userId.ToString());
                bool isReseller = user.UserType == UserType.Reseller ? true : false;
                var percentDiscount = isReseller ? user.PercentDiscount : 0;
                if (percentDiscount == null)
                {
                    percentDiscount = 0;
                }
                model.UserId = user.Id;
                var queryable = _order.AsQueryable();

                var find = await queryable.Where(c => c.Id == model.Id).Include(c => c.OrderProducts).FirstOrDefaultAsync();
                if (find == null)
                    return (order: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");


                var total = 0;

                if (find.OrderProducts != null && find.OrderProducts.Any() && model.OrderProducts != null)
                {
                    if (find.IsReserved)
                    {
                        // مجموع قبلی سبد
                        var countOld = find.OrderProducts.Sum(c => c.Count);
                        // مجموع جدید سبد
                        var countModel = model.OrderProducts.Sum(c => c.Count);
                        // ایدی ایتم های قبل
                        var itemsOld = find.OrderProducts.Select(c => c.ProductId);
                        // ایدی ایتم های جدید
                        var itemsNew = model.OrderProducts.Select(c => c.ProductId);
                        // ایا محصول جدیدی به سبد اضافه شده است ؟
                        var addNewPrd = itemsOld.Any(c => itemsNew.All(z => z != c));
                        //ایا مجموع کم یا زیاد شده یا محصول جدیدی به سبد اضافه شده ؟
                        if (countOld != countModel || addNewPrd)
                            return (null, false, HttpStatusCode.BadRequest, error: "ابتدا نسبت به پرداخت سبد رزرو شده اقدام نمایید");
                    }

                    foreach (var item in model.OrderProducts)
                    {
                        var pOrder = find.OrderProducts.FirstOrDefault(c => c.Id == item.Id);

                        if (pOrder == null)
                            return (order: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                        var price = 0;
                        var priceReseller = 0;
                        // حالتی که اپشن شامل رنگ است
                        if (pOrder.ProductOptionId != null && pOrder.OptionColorId != null)
                        {
                            var output = await _productOptionService.Get(id: pOrder.ProductOptionId,
                                           includes: new Expression<Func<ProductOption, object>>[] { c => c.OptionColors });
                            if (output.Count > 0)
                            {
                                var option = output.options.FirstOrDefault();
                                if (option.OptionColors != null && option.OptionColors.Any())
                                {
                                    var color = option.OptionColors.FirstOrDefault(c => c.Id == pOrder.OptionColorId);

                                    if (color == null || color.Count == 0)
                                        return (order: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                                    if (color.Count < item.Count)
                                        return (null, false, HttpStatusCode.BadRequest, "موجودی کمتر از تقاضا !");

                                    price = color.Price.Value;
                                    priceReseller = color.ResellerPrice;
                                }
                                else
                                    return (order: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                            }
                        }
                        // حالتی که شامل رنگ نیست
                        else if (pOrder.ProductOptionId != null)
                        {
                            var output = await _productOptionService.Get(id: pOrder.ProductOptionId);
                            if (output.Count == 0)
                                return (order: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                            var option = output.options.FirstOrDefault();

                            if (option.Count < item.Count)
                                return (null, false, HttpStatusCode.BadRequest, "موجودی کمتر از تقاضا !");

                            price = option.Price.Value;

                        }


                        pOrder.Count = item.Count;
                        pOrder.TotalPrice = item.Count * price;

                        pOrder.TotalForReseller = item.Count * priceReseller;
                        await _orderProductService.Update(pOrder);

                        //total += pOrder.TotalForReseller != null && pOrder.TotalForReseller.Value>0 ?
                        //         pOrder.TotalForReseller.Value :
                        //         pOrder.TotalPrice;
                        if (isReseller)
                        {
                            total += pOrder.TotalForReseller.Value;
                        }
                        else
                        {
                            total += pOrder.TotalPrice;

                        }
                    }
                }
                model.TotalPrice = total;
                model.DiscountPercent = percentDiscount.Value;
                model.DiscountPrice = (total * model.DiscountPercent) / 100;
                model.TotalWithDiscountPrice = total - model.DiscountPrice;//هزینه ارسال
                model.TotalPrice = model.TotalPrice;//هزینه ارسال

                _uow.Entry(find).CurrentValues.SetValues(model);

                // model.ShippingCost = 11000;


                #region Admin || IsReserve
                if (roles.Any(c => c.Name == ConstantRoles.SuperAdmin || c.Name == ConstantRoles.ClientAdmin))
                    //if (userId == 1
                  // || user.UserName == "9981415928" || user.UserName == "9018091511"|| user.UserName == "9372197297"
                    //|| model.IsReserved  //  )
                {
                    // update order
                    var order = find;

                    if (model.IsReserved)
                        order.ReserveTo = DateTime.UtcNow.AddHours(1);

                    if (!model.IsReserved && 
                        (
                        roles.Any(c => c.Name == ConstantRoles.SuperAdmin || c.Name == ConstantRoles.ClientAdmin)
                        //userId == 1 || user.UserName == "9981415928" 
                        //|| user.UserName == "9018091511"|| user.UserName == "9372197297")
                        ))
                    {
                        order.PaidDateTime = DateTime.UtcNow;
                        order.OrderType = OrderType.Paid;
                    }

                    // کم کردن از موجودی محصول
                    var orderProducts1 = order.OrderProducts.ToList();
                    var listOptions1 = orderProducts1.Select(c => c.ProductOptionId.Value).ToList();

                    var options = await _productOptionService.GetByIds(Ids: listOptions1,
                                 includes: new Expression<Func<ProductOption, object>>[] { c => c.OptionColors });

                    var colors = options.SelectMany(c => c.OptionColors).ToList();
                    foreach (var item in orderProducts1)
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

                #endregion


                await _uow.SaveChangesAsync();
                return (find, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }



        public async Task<(bool success, HttpStatusCode status, string error)> Delete(int id)
        {
            try
            {
                var queryable = _order.AsQueryable();

                var provider = await queryable.FirstOrDefaultAsync(a => a.Id == id);
                if (provider == null)
                    return (success: false, status: HttpStatusCode.NotFound, error: "ایتمی یافت نشد !");

                provider.IsDeleted = true;

                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }

        public virtual async Task<OrderBasketViewModel> GetByUserID(int userId, OrderType? orderType = null, bool isReseller = false)
        {
            var queryable = _order.AsQueryable();

            queryable = queryable.Where(c => c.UserId == userId);
            queryable = queryable.Where(c => c.OrderType == orderType);

            queryable = queryable.Include(c => c.OrderProducts)
                                  .ThenInclude(c => c.Product);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.Field);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.ProductOption);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.OptionColor);

            var order = await queryable.FirstOrDefaultAsync();

            if (order == null)
                return OrderBasketViewModel.FromEntity(order);

            if (order.OrderProducts.Count == 0)
            {
                order.TotalPrice = 0;
                order.DiscountPrice = 0;
                order.TotalWithDiscountPrice = 0;
                await _uow.SaveChangesAsync();
            }

            var result = OrderBasketViewModel.FromEntity(order);
            var listDelete = new List<OrderProductForBasketViewModel>();
            foreach (var item in result.OrderProducts)
            {

                if (item.OptionColor.Count <= 0 && result.IsReserved == false)
                {
                    await _orderProductService.Delete(item.Id);
                    listDelete.Add(item);
                }
                item.Field.ProductOptions = null;
                item.ProductOption.Product = null;
                item.ProductOption.OptionColors = null;
                item.ProductOption.Field = null;
                item.OptionColor.ProductOption = null;
            }
            foreach (var item in listDelete)
            {
                result.OrderProducts.Remove(item);
            }

            var totoalPrice = 0;
            foreach (var item in result.OrderProducts)
            {
                if (order.User.UserType == UserType.Reseller)
                {
                    totoalPrice += Convert.ToInt32(item.TotalForReseller);
                }
                if (order.User.UserType == UserType.Client)
                {
                    totoalPrice += Convert.ToInt32(item.TotalPrice);
                }
            }
            if (totoalPrice != order.TotalPrice)
            {
                order.TotalPrice = totoalPrice;
                order.TotalWithDiscountPrice = totoalPrice - order.DiscountPercent;
                result.TotalPrice = totoalPrice;
                await _uow.SaveChangesAsync();
            }

            return result;
        }

        public async Task<(int totalPrice, int discountPrice, int totalWithDiscountPrice)> ChangePrice(int id, int decresePrice, int userId)
        {
            try
            {
                var user = await _usersService.FindByIdAsync(userId.ToString());
                var discount = user.PercentDiscount ?? 0;
                var order = await GetById(id);
                if (order == null)
                    return (0, 0, 0);

                order.TotalPrice -= decresePrice;
                order.DiscountPrice = (order.TotalPrice * discount) / 100;
                order.DiscountPercent = discount;
                order.TotalWithDiscountPrice = order.TotalPrice == 0 ? 0 : order.TotalPrice - order.DiscountPrice;
                if (order.TotalPrice == 0)
                {
                    order.ReserveTo = null;
                    order.IsReserved = false;
                    order.OrderProducts = null;
                }
                await _uow.SaveChangesAsync();
                return (order.TotalPrice, order.DiscountPrice, order.TotalWithDiscountPrice);
            }
            catch
            {
                return (0, 0, 0);
            }
        }


        public async Task CancellReserve()
        {
            try
            {
                var queryable = _order.AsQueryable();
                queryable = queryable.Where(c => c.IsReserved == true &&
                                      c.OrderType == OrderType.Unpaid &&
                                   c.ReserveTo != null && c.ReserveTo <= DateTime.UtcNow);
                queryable = queryable.Include(c => c.OrderProducts);

                var orders = await queryable.ToListAsync();

                if (orders != null && orders.Any())
                {
                    foreach (var order in orders)
                    {
                        var orderProducts = order.OrderProducts.ToList();
                        if (orderProducts != null && orderProducts.Any())
                        {
                            //var listOptions = orderProducts.Select(c => c.ProductOptionId.Value).ToList();

                            //var options = await _productOptionService.GetByIds(Ids: listOptions,
                            //             includes: new Expression<Func<ProductOption, object>>[] { c => c.OptionColors });

                            //var colors = options.SelectMany(c => c.OptionColors).ToList();
                            foreach (var item in orderProducts)
                            {
                                //if (item.OptionColorId == null)
                                //{
                                //    var option = options.FirstOrDefault(c => c.Id == item.ProductOptionId);
                                //    if (option != null)
                                //        option.Count += item.Count;

                                //    continue;
                                //}

                                await _optionColorService.Update(Convert.ToInt32(item.OptionColorId), item.Count);
                                //var color = colors.FirstOrDefault(c => c.Id == item.OptionColorId);
                                //if (color != null)
                                //    color.Count += item.Count;

                            }

                        }
                        order.IsReserved = false;
                        order.ReserveTo = null;
                    }
                    await _uow.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }


        public async Task<List<GetOrderDontSendWithAddressViewModel>> GetOrderDontSendWithAddress()
        {
            var queryable = _order.AsQueryable();
            queryable = queryable.Where(c => c.OrderType == OrderType.Paid &&
                                        c.OrderSendType == OrderSendType.Pending
                                        && c.AcceptPayment == true);

            queryable = queryable.Include(c => c.OrderProducts)
                                  .ThenInclude(c => c.Product);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.Field);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.ProductOption);

            queryable = queryable.Include(c => c.OrderProducts)
                                 .ThenInclude(c => c.OptionColor);

            var orders = await queryable.AsNoTracking()
                                        .OrderByDescending(c => c.Id)
                                        .ToListAsync();

            if (orders == null || !orders.Any())
                return new List<GetOrderDontSendWithAddressViewModel>();

            var result = new List<GetOrderDontSendWithAddressViewModel>();
            foreach (var order in orders)
            {

                //if (order.OrderProducts.Count == 0)
                //{
                //    order.TotalPrice = 0;
                //    order.DiscountPrice = 0;
                //    order.TotalWithDiscountPrice = 0;
                //}
                var temp = GetOrderDontSendWithAddressViewModel.FromEntity(order);
                result.Add(temp);

                //var listDelete = new List<OrderProductForBasketViewModel>();

                //foreach (var item in temp.OrderProducts)
                //{

                //    if (item.Count <= 0)
                //    {
                //        listDelete.Add(item);
                //    }
                //    item.Field.ProductOptions = null;
                //    item.ProductOption.Product = null;
                //    item.ProductOption.OptionColors = null;
                //    item.ProductOption.Field = null;
                //    item.OptionColor.ProductOption = null;
                //}
                //foreach (var item in listDelete)
                //{
                //    temp.OrderProducts.Remove(item);
                //}
            }
            return result;



        }

        public async Task<(bool success, HttpStatusCode status, string error)> EditAddress(EditAddressViewModel model)
        {

            try
            {
                var find = await _order.FirstOrDefaultAsync(c => c.Id == model.Id);
                if (find == null)
                    return (false, HttpStatusCode.NotFound, "ایتمی یافت نشد !");

                find.ReciverAddress = model.ReciverAddress;
                // find.ReciverMobile = model.ReciverMobile;
                //find.ReciverName = model.ReciverName;
                //find.SenderMobile = model.SenderMobile;
                //find.SenderName = model.SenderName;

                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا سمت سرور");
            }

        }

        public async Task<(bool success, HttpStatusCode status, string error)> DeleteByOrderProductId(int id)
        {
            try
            {
                var query = await _orderProductService.Get(id, includes: new Expression<Func<OrderProduct, object>>[] { c => c.OptionColor });
                var find = query.OrderProducts.FirstOrDefault();
                if (find == null)
                    return (false, HttpStatusCode.NotFound, "ایتمی یافت نشد !");

                find.OptionColor.Count += find.Count;

                var order = await _order.FirstOrDefaultAsync(c => c.Id == find.OrderId);
                order.TotalPrice -= find.TotalPrice;
                order.TotalWithDiscountPrice -= find.TotalPrice;

                find.IsDeleted = true;

                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا سمت سرور");
            }

        }

        public async Task<(bool success, HttpStatusCode status, string error)> AcceptPaymnet(int id, bool accept)
        {
            var order = await _order.FindAsync(id);
            if (order == null)
                return (false, HttpStatusCode.NotFound, "ایتمی یافت نشد !");

            order.AcceptPayment = accept;
            await _uow.SaveChangesAsync();

            return (true, HttpStatusCode.OK, "با موفقیت ثبت شد !");

        }

        public async Task<List<ReportViewModel>> GetReportDaily(DateTime? from, DateTime? to)
        {
            var queryable = _order.AsQueryable();

            queryable = queryable.Where(c => c.OrderType == OrderType.Paid && c.PaidDateTime != null);
            if (from == null && to == null)
            {
                from = DateTime.UtcNow.AddDays(-30);
                to = DateTime.UtcNow;
            }

            queryable = from != null ? queryable.Where(c => c.PaidDateTime.Value.Date >= from.Value.Date) : queryable;
            queryable = to != null ? queryable.Where(c => c.PaidDateTime.Value.Date <= to.Value.Date) : queryable;

            //queryable = queryable.Where(p => p.OrganizationId == organizationId);
            List<ReportViewModel> query = new List<ReportViewModel>();

                query = await queryable.GroupBy(o => new { o.PaidDateTime.Value.Date })
                                   .Select(g => new ReportViewModel
                                   {
                                       Day = g.Key.Date.Day,
                                       DateTime = g.Key.Date,
                                       Count = g.Sum(o => o.TotalWithDiscountPrice)
                                   }).OrderBy(c=>c.DateTime).ToListAsync();

            return query;
        }

        public async Task<List<ReportMonthViewModel>> GetReportMonthly(int? from, int? to, int year)
        {
            var queryable = _order.AsQueryable();

            if (from == null && to == null)
            {
                from = 1;
                to = 12;
            }
            if (from == to)
            {
                to = from + 1;
            }
            queryable = queryable.Where(c => c.OrderType == OrderType.Paid && c.PaidDateTime != null
                                            && c.MonthOfYear != null && c.Year != null);

            queryable = queryable.Where(c => c.MonthOfYear.Value >= from && c.MonthOfYear <= to && c.Year == year);
            List<ReportMonthViewModel> query = new List<ReportMonthViewModel>();

            query = await queryable.GroupBy(o => new { o.MonthOfYear })
                               .Select(g => new ReportMonthViewModel
                               {
                                   Month = g.Key.MonthOfYear.Value,
                                   Count = g.Sum(o => o.TotalWithDiscountPrice)
                               }).OrderBy(c => c.Month).ToListAsync();

            return query;
        }


        public async Task<bool> SetShamsiDate()
        {
            var queryable = _order.Where(c => c.PaidDateTime != null && c.Year == null && c.MonthOfYear == null);
            var count = await queryable.LongCountAsync();
            var start = 0; 
            while(count>0)
            {
               var list =  await queryable.Skip(start).Take(100).ToListAsync();
                foreach (var item in list)
                {
                    item.Year = item.PaidDateTime.Value.GetPersianYear();
                    item.MonthOfYear = item.PaidDateTime.Value.GetPersianMonth();
                }
                await _uow.SaveChangesAsync();
                start += 100;
                count -= 100;
            }
            return true;
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }

}
