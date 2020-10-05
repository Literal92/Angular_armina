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
using shop.Common.PersianToolkit;

namespace shop.Services.Reservation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Product> _product;

        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _product = _uow.Set<Product>();
        }
        /// <summary>
        /// چرا ادمین و کلاینت جدا شد؟
        /// چون نیازمند تغییرات در سمت مشتری داشتیم
        /// </summary>
        /// <returns></returns>

        public async Task<(List<Product> Products, int Count, int TotalPages, HttpStatusCode status)> Get(int? id = 0,
         string title = null, string code= null , string term = null, int? categoryId = null,
         bool? isPublish = null,

         int pageIndex = 0,
          int pageSize = 0, Expression<Func<Product, object>>[] includes = null)
        {
            var queryable = _product.AsQueryable();

            queryable = id != null && id > 0 ? queryable.Where(c => c.Id == id) : queryable;
            queryable = isPublish != null ? queryable.Where(c => c.IsPublish == isPublish) : queryable;

            queryable = categoryId != null ? queryable.Where(p => p.CategoryId == categoryId) : queryable;
            // چون ممکنه در سمت کلاینت سرچ بشه که پارامتر کد داخل پارامتر 
            // title 
            // ارسال میشه
            queryable = (title != null && title != "null") ? queryable.Where(c =>
                EF.Functions.Like(c.Title, $"%{title.Trim()}%") ||
                c.Code.Trim() == title.Trim()) : queryable;
            
            queryable = (!string.IsNullOrEmpty(code) && code != "null") ? queryable.Where(c =>c.Code.Trim() == code.Trim()) : queryable;

            queryable = queryable.OrderByDescending(c => c.ModifiedDateTime).ThenByDescending(p=>p.Id);

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
                foreach (Expression<Func<Product, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.AsNoTracking().ToListAsync();

            return (Products: list, Count: count, TotalPages: totalPages, status: HttpStatusCode.OK);
        }
        public virtual async Task<Product> GetById(int id = 0)
        {
            return await _product.FirstOrDefaultAsync(c => c.Id == id);
        }
        public virtual async Task<ProductDetailsViewModel> GetByID(int id, bool? isPublish = null)
        {
            var queryable = _product.AsQueryable();

            queryable = queryable.Where(c => c.Id == id);
            queryable = isPublish != null ? queryable.Where(c => c.IsPublish == isPublish) : queryable;


            queryable = queryable.Include(c => c.Pictures);

            var withField = await queryable.Include(c => c.ProductOptions)
                                          .ThenInclude(c => c.Field)
                                          .ToListAsync();

            var withColors = await queryable.Include(c => c.ProductOptions)
                                          .ThenInclude(c => c.OptionColors)
                                          .ToListAsync();

            var optionsColors = withColors != null && withColors.Any() ?
                                withColors.SelectMany(c => c.ProductOptions).ToList() :
                                null;

            var count = await queryable.CountAsync();
            if (count == 0)
                return new ProductDetailsViewModel();


            var product = withField.FirstOrDefault();

            var response = ProductDetailsViewModel.FromEntity(product);

            if (product.ProductOptions != null && product.ProductOptions.Any())
            {
                if (optionsColors != null)
                {
                    foreach (var option in product.ProductOptions)
                    {
                        option.OptionColors = optionsColors.Where(c => c.Id == option.Id)
                                             .SelectMany(c => c.OptionColors).ToList();
                    }

                }

                var group = product.ProductOptions
                    .GroupBy(c => c.FieldId,
                            (key, g) => new
                            {
                                field = g.Select(f => { f.Field.ProductOptions = g.ToList(); return f; })
                                        .FirstOrDefault().Field
                            });

                //  var t = product.ProductOptions.ToLookup(p => p.FieldId);
                response.Fields = group.Select(c => FieldGroupByViewModel.FromEntity(c.field)).ToList();

            }

            return response;
        }


        public async Task<(Product product, bool success, HttpStatusCode status, string error)> Create(Product model)
        {
            try
            {
                var exist = await _product.FirstOrDefaultAsync(c => c.Title.Trim() == model.Title.Trim());
                if (exist != default)
                    return (null, false, HttpStatusCode.Conflict, "درج داده یکسان");

                if (!string.IsNullOrEmpty(model.Code))
                {
                    var existCode = await _product.FirstOrDefaultAsync(c => c.Code.Trim() == model.Code.Trim());
                    if (existCode != default)
                        return (null, false, HttpStatusCode.Conflict, "درج کد محصول یکسان");
                }

                await _product.AddAsync(model);
                await _uow.SaveChangesAsync();
                return (model, true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, status: HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }

        public async Task<(Product product, string oldFile, bool success, HttpStatusCode status, string error)> Update(Product model)
        {
            try
            {
                var queryable = _product.AsQueryable();

                var find = await queryable.FirstOrDefaultAsync(a => a.Id == model.Id);
                if (find == null)
                    return (product: null, oldFile: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                if (!string.IsNullOrEmpty(model.Code))
                {
                    var existCode = await _product.FirstOrDefaultAsync(c => c.Code.Trim() == model.Code.Trim());
                    if (existCode != null && existCode.Id != model.Id)
                        return (null,null, false, HttpStatusCode.Conflict, "درج کد محصول یکسان");
                }


                var oldFile = find.Pic;

                _uow.Entry(find).CurrentValues.SetValues(model);
                await _uow.SaveChangesAsync();
                return (find, oldFile: oldFile, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, oldFile: null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }
        public async Task<(bool success, HttpStatusCode status, string error)> Delete(int id)
        {
            try
            {
                var queryable = _product.AsQueryable();

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


        public async Task<(HttpStatusCode status, bool success, string error)> UpdateActive(int id)
        {
            try
            {

                var find = await _product.FirstOrDefaultAsync(c => c.Id == id);
                if (find == null)
                    return (HttpStatusCode.NotFound, false, "کاربری یافت نشد !");

                var product = find;
                product.IsPublish = !find.IsPublish;
                _uow.Entry(find).CurrentValues.SetValues(product);
                await _uow.SaveChangesAsync();
                return (HttpStatusCode.OK, true, null);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, false, "خطا سمت سرور !");
            }
        }

        public async Task<(int price, int unitPrice, bool success, HttpStatusCode status, string error)> ComputPrice(int id, int count)
        {
            var price = 0;
            var queryable = _product.AsQueryable();
            var product = await queryable.FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
                return (0, 0, false, HttpStatusCode.NotFound, "محصولی با این مشخصات یافت نشد!");

            if (product.Count >= count)
                price = product.Price * count;
            else
                return (0, 0, false, HttpStatusCode.NotFound, "موجودی کمتر از تقاضا !");

            return (price, product.Price, true, HttpStatusCode.OK, null);
        }

        public void Dispose()
        {
            _uow.Dispose();
        }
    }

}
