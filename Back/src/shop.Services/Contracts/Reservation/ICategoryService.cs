using shop.Entities.Reservation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Contracts.Reservation
{
    public interface ICategoryService  : IDisposable
    {
        Task<(List<Category> categories, int count, int totalPages)> Get(string title=null,string title_phrase=null, int? id = null, int? parentId = null, int? productId = null, bool? isChkParent = null,bool? contionRoot=true, int? pageIndex = 0, int? pageSize = 0, Expression<Func<Category, object>>[] includes = null);

        Task<List<Category>> Search(string title, int pageIndex = 0, int pageSize = 0);
        Task<Category> GetByTitle(string title);
        Task<Category> GetById(int id);

        Task<(Category category,bool success, HttpStatusCode status, string error)> Create(Category category);

        Task<(Category category, string oldFile, bool success,HttpStatusCode status, string error)> Update(int id, Category category);
        Task<(bool success, HttpStatusCode statusCode, string error)> Delete(int Id);

        IEnumerable<Category> FindAllParents(List<Category> all_data, Category child);

    }
}
