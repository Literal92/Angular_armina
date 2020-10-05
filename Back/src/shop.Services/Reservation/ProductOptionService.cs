using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Reservation
{
    public class ProductOptionService : IProductOptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ProductOption> _productOption;
        private readonly IOptionColorService _optionColorService;
        public ProductOptionService(IUnitOfWork uow, IOptionColorService optionColorService)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _productOption = _uow.Set<ProductOption>();
            _optionColorService = optionColorService;
        }

        public async Task<(List<ProductOption> options, int Count, int TotalPages)> Get(int? id = null,
         int? fieldId = null, string title = null, int? productId = null,
         int pageIndex = 0, int pageSize = 10,
         Expression<Func<ProductOption, object>>[] includes = null)
        {

            var queryable = _productOption.AsQueryable();
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;

            queryable = productId != null ? queryable.Where(c => c.ProductId == productId) : queryable;

            queryable = title != null && title.Trim() != "null" ? queryable.Where(c => EF.Functions.Like(c.Title, $"%{title.Trim()}%")) : queryable;
            queryable = fieldId != null ? queryable.Where(c => c.FieldId == fieldId.Value) : queryable;

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            queryable = queryable.OrderBy(c => c.Order);

            if (pageIndex > 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            var list = await queryable.Distinct().AsNoTracking().ToListAsync();

            return (options: list, Count: count, TotalPages: totalPages);
        }
        /// <summary>
        /// این تابع جدا نوشته شد
        /// چون نمی خواستیم
        /// AsNoTracking()
        /// باشد
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<List<ProductOption>> GetByIds(List<int> Ids, Expression<Func<ProductOption, object>>[] includes = null)
        {
            var queryable = _productOption.AsQueryable();

            queryable = queryable.Where(c => Ids.Any(z => z == c.Id));

            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            return await queryable.Distinct().ToListAsync();

        }
        public async Task<ProductOption> GetById(int id)
        {
            return await _productOption.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<(ProductOption option, HttpStatusCode status, bool success, string error)> Create(ProductOption model)
        {
            try
            {
                var exist = await _productOption.FirstOrDefaultAsync(c => c.FieldId == model.FieldId &&
                                                  c.ProductId == model.ProductId &&
                                                  c.Title.Trim() == model.Title.Trim());
                if (exist != default)
                    return (null, HttpStatusCode.Conflict, false, "درج داده یکسان");

                await _productOption.AddAsync(model);
                await _uow.SaveChangesAsync();
                return (option: model, status: HttpStatusCode.OK, success: true, error: null);
            }
            catch (Exception ex)
            {
                return (option: null, status: HttpStatusCode.InternalServerError, success: false, error: "خطا در ثبت !");
            }
        }
        public async Task<(ProductOption option, bool success, HttpStatusCode status, string error)> Update(ProductOption option)
        {
            try
            {
                var queryable = _productOption.AsQueryable();

                var find = await queryable.Where(a => a.Id == option.Id)
                    .Include(c => c.OptionColors).FirstOrDefaultAsync();
                if (find == null)
                    return (option: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                _uow.Entry(find).CurrentValues.SetValues(option);

                // رنگ هایی که از قبل بوده است       
                var existId = find.OptionColors != null ? find.OptionColors.Select(c => c.Id).ToList() : new List<int>();
                // رنگ هایی که توسط مدل پاس داده شده اند
                var modelOptionId = option.OptionColors.Select(c => c.Id).ToList();
                // اپشن هایی که وجود دارند و در مدل ورودی هم هستند نباید دوباره درج شوند
                var subscribeOptionsId = existId.Intersect(modelOptionId).ToList();
                // مدل هایی که باید حذف شوند
                var removeOptionsId = existId.Except(subscribeOptionsId).ToList();
                // اپشن هایی که باید اضافه شوند
                var addoptions = option.OptionColors.Where(c => c.Id == 0).ToList();

                // اونایی که حذف شده is delete
                foreach (var item in removeOptionsId)
                {
                    await _optionColorService.Remove(item);
                }
                // اونایی که جدید درج کن 
                foreach (var item in addoptions)
                {
                    await _optionColorService.Create(item);
                }




                await _uow.SaveChangesAsync();
                return (find, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }
        public async Task<(bool success, HttpStatusCode status, string error)> Remove(int id)
        {
            try
            {

                var item = await _productOption.FirstOrDefaultAsync(a => a.Id == id);
                if (item == null)
                    return (success: false, status: HttpStatusCode.NotFound, error: "ایتمی یافت نشد !");

                item.IsDeleted = true;

                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }

        public async Task<(int price, int priceReseller, int unitPrice, bool success, HttpStatusCode status, string error)> ComputPrice(int productId, int optionId, int? optionColorId = null, int count = 0, bool isReseller = false)
        {
            var price = 0;
            var priceReseller = 0;
            var unitPrice = 0;
            var queryable = _productOption.AsQueryable();
            queryable = queryable.Where(c => c.Id == optionId && c.ProductId == productId);
            if (optionColorId == null || optionColorId == 0)
            {
                var option = await queryable.FirstOrDefaultAsync();
                if (option == null)
                    return (0,0, unitPrice, false, HttpStatusCode.NotFound, "محصولی با این مشخصات یافت نشد!");

                if (option.Count >= count)
                {
                    price = option.Price.Value * count;
                    unitPrice = option.Price.Value;
                }
                else
                    return (0,0, unitPrice, false, HttpStatusCode.NotFound, "موجودی کمتر از تقاضا !");
            }
            else
            {
                queryable = queryable.Include(c => c.OptionColors);
                var option = await queryable.FirstOrDefaultAsync();

                if (option == null)
                    return (0,0, unitPrice, false, HttpStatusCode.NotFound, "محصولی با این مشخصات یافت نشد!");
                var color = option.OptionColors != null ?
                            option.OptionColors.FirstOrDefault(c => c.Id == optionColorId)
                            : null;
                if (color == null)
                    return (0,0, unitPrice, false, HttpStatusCode.NotFound, "محصولی با این مشخصات یافت نشد!");

                if (color.Count >= count)
                {
                    price = color.Price.Value * count;
                    priceReseller = color.ResellerPrice * count;
                    unitPrice = color.Price.Value;
                }
                else
                    return (0,0, unitPrice, false, HttpStatusCode.NotFound, "موجودی کمتر از تقاضا !");

            }
            return (price, priceReseller, unitPrice, true, HttpStatusCode.OK, null);
        }

      
        public void Dispose()
        {
            _uow.Dispose();
        }


    }
}
