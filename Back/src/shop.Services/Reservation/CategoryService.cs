using DNTPersianUtils.Core;
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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Reservation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        readonly DbSet<Category> _category;
        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _category = _uow.Set<Category>();


        }
        public async Task<(List<Category> categories, int count, int totalPages)> Get(string title=null,string title_phrase=null, int? id = null, int? parentId = null, int? productId = null, bool? isChkParent = null,bool? contionRoot=true, int? pageIndex = 0, int? pageSize = 0, Expression<Func<Category, object>>[] includes = null)
        {

            var queryable = _category.AsQueryable();
            queryable = id != null ? queryable.Where(a => a.Id == id) : queryable;

            queryable = contionRoot != true ? queryable.Where(a => a.Id != 1) : queryable;

            queryable = productId != null ? queryable.Where(a => a.Products.Any(x => x.Id == productId)) : queryable;

            queryable = parentId != null ? queryable.Where(a => a.ParentId == parentId) : queryable;

            queryable = title != null && title != "null" && isChkParent == false ? queryable.Where(c => EF.Functions.Like(c.Title, $"%{title.Trim()}%")) : queryable;
            queryable = title_phrase != null ? queryable.Where(c => c.Title.Trim() == title_phrase.Trim()) : queryable;

            // find parent and Child
            if (title != null && isChkParent == true)
            {
                queryable = queryable.Where(p => EF.Functions.Like(p.Parent.Title, $"%{title.Trim()}%")|| EF.Functions.Like(p.Title, $"%{title.Trim()}%"));
            }

            queryable = queryable.OrderByDescending(c=>c.Id);
            
            var count = isChkParent == false ? await queryable.CountAsync() : queryable.Count();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize.Value)) : 0;

            if (pageIndex >= 0 && pageSize != 0)
            {
                int skip = (pageIndex.Value - 1) * pageSize.Value;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize.Value;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            var list = isChkParent == false ? await queryable.ToListAsync() : queryable.ToList();
            return (categories: list, count: count, totalPages: totalPages);
        }

        public async Task<List<Category>> Search(string title, int pageIndex = 0, int pageSize = 0)
        {
            var queryable = _category.AsQueryable();
            queryable = title != null && title != "null" ? queryable.Where(c => EF.Functions.Like(c.Title, $"%{title.Trim()}%")) : queryable;
            queryable = queryable.Where(p => !p.SubCategory.Any());
            queryable = queryable.Include(p => p.Parent)
                                 .ThenInclude(c => c.Parent);
          
            if (pageIndex >= 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }


            return await queryable.AsNoTracking().ToListAsync();
        }


        public async Task<Category> GetByTitle(string title)
        {
            return await _category.FirstOrDefaultAsync(c => c.Title.Trim() == title.Trim());
        }


        public async Task<Category> GetById(int id)
        {
            return await _category.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(Category category,bool success, HttpStatusCode status, string error)> Create(Category category)
        {
            try
            {
                var exist = await _category.FirstOrDefaultAsync(c=>c.Title.Trim() == category.Title.Trim());
                if(exist != default)
                return (null, false, HttpStatusCode.Conflict, "درج داده یکسان");

                var parents = await Get(title: null, id: category.ParentId);

                var path = string.Empty;
                if (parents.count > 0)
                    path = parents.categories.FirstOrDefault().Path;

                if (category.Title.Trim() == "ریشه")
                {
                    path = category.Title;
                }
                else
                {
                    path = path + "-->" + category.Title;
                }
                category.Path = path;
                await _category.AddAsync(category);
                await _uow.SaveChangesAsync();
                return (category: category, success: true, HttpStatusCode.OK, null);
            }
            catch
            {
                return (category: null, success: false, status: HttpStatusCode.InternalServerError, error:"خطا سمت سرور !");

            }

        }

        public async Task<(Category category, string oldFile, bool success,HttpStatusCode status, string error)> Update(int id, Category category)
        {
            try
            {
                if (category.Title == null || category.Title.Trim() == "undefined")
                    return (category: null, oldFile:null, success: false, status: HttpStatusCode.BadRequest,"عنوان باید تعریف شود");

                var find = await _category.FirstOrDefaultAsync(x => x.Id == id);
                if (find == null)
                    return (category: null, oldFile:null, false, status: HttpStatusCode.NotFound, null);

                string old = find.Title;
                string oldFile= find.Icon;
                var parents = await Get(title: null, id: category.ParentId);

                category.Icon = category.Icon == null ? find.Icon : category.Icon;
                var path = string.Empty;
                if (parents.count > 0)
                    path = parents.categories.FirstOrDefault().Path;

                path = path + "-->" + category.Title;
                category.Path = path;

                _uow.Entry(find).CurrentValues.SetValues(category);

                var childs = await Get(title: null, parentId: find.Id);
                if (childs.count > 0)
                {
                    foreach (var item in childs.categories)
                    {
                        item.Path = item.Path.Replace($"-->{old}-->", $"-->{category.Title}-->");
                    }
                }

                await _uow.SaveChangesAsync();
                return (category: category, oldFile: oldFile, success: true, HttpStatusCode.OK,null);
            }
            catch
            {
                return (category: null, oldFile:null, success: false, HttpStatusCode.InternalServerError ,"خطا سمت سرور!");
            }
        }
        public async Task<(bool success, HttpStatusCode statusCode, string error)> Delete(int Id)
        {
            try
            {
                var rec = await _category.FirstOrDefaultAsync(a => a.Id == Id);
                rec.IsDeleted = true;
                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (success: false, HttpStatusCode.InternalServerError ,error: "خطا سمت سرور!");
            }
        }


        public IEnumerable<Category> FindAllParents(List<Category> all_data, Category child)
        {
            var parent = all_data.FirstOrDefault(x => x.Id == child.ParentId);

            if (parent == null)
                return Enumerable.Empty<Category>();

            return new[] { parent }.Concat(FindAllParents(all_data, parent));
        }
           public void Dispose()
         {
           _uow.Dispose();
        }
    }
}
