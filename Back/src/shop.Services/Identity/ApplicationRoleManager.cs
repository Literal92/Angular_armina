using shop.Common.IdentityToolkit;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using shop.ViewModels.Identity.Settings;

namespace shop.Services.Identity
{
    public class ApplicationRoleManager :
        RoleManager<Role>,
        IApplicationRoleManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationRoleManager> _logger;
        private readonly IEnumerable<IRoleValidator<Role>> _roleValidators;
        private readonly IApplicationRoleStore _store;
        private readonly DbSet<User> _users;
        private readonly DbSet<Role> _roles;
        private readonly IOptionsSnapshot<SiteSettings> _settingOptions;


        public ApplicationRoleManager(
            IApplicationRoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<ApplicationRoleManager> logger,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork uow,
             IOptionsSnapshot<SiteSettings> settingOptions) :
            base((RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>)store, roleValidators, keyNormalizer, errors, logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(_store));
            _roleValidators = roleValidators ?? throw new ArgumentNullException(nameof(_roleValidators));
            _keyNormalizer = keyNormalizer ?? throw new ArgumentNullException(nameof(_keyNormalizer));
            _errors = errors ?? throw new ArgumentNullException(nameof(_errors));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(_contextAccessor));
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _users = _uow.Set<User>();
            _roles = _uow.Set<Role>();
            _settingOptions = settingOptions;

        }

        #region BaseClass

        #endregion

        #region CustomMethods

        public IList<Role> FindCurrentUserRoles()
        {
            var userId = getCurrentUserId();
            return FindUserRoles(userId);
        }

        public IList<Role> FindUserRoles(int userId)
        {
            var userRolesQuery = from role in Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return userRolesQuery.OrderBy(x => x.Name).ToList();
        }

        public Task<List<Role>> GetAllCustomRolesAsync()
        {
            return Roles.ToListAsync();
        }

        public IList<RoleAndUsersCountViewModel> GetAllCustomRolesAndUsersCountList()
        {
            return Roles.Select(role =>
                                    new RoleAndUsersCountViewModel
                                    {
                                        Role = role,
                                        UsersCount = role.Users.Count()
                                    }).ToList();
        }

        public async Task<PagedUsersListViewModel> GetPagedApplicationUsersInRoleListAsync(
                int roleId,
                int pageNumber, int recordsPerPage,
                string sortByField, SortOrder sortOrder,
                bool showAllUsers)
        {
            var skipRecords = pageNumber * recordsPerPage;

            var roleUserIdsQuery = from role in Roles
                                   where role.Id == roleId
                                   from user in role.Users
                                   select user.UserId;
            var query = _users.Include(user => user.Roles)
                              .Where(user => roleUserIdsQuery.Contains(user.Id))
                         .AsNoTracking();

            if (!showAllUsers)
            {
                query = query.Where(x => x.IsActive);
            }

            switch (sortByField)
            {
                case nameof(User.Id):
                    query = sortOrder == SortOrder.Descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                    break;
                default:
                    query = sortOrder == SortOrder.Descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                    break;
            }

            return new PagedUsersListViewModel
            {
                Paging =
                {
                    TotalItems = await query.CountAsync()
                },
                Users = await query.Skip(skipRecords).Take(recordsPerPage).ToListAsync(),
                Roles = await Roles.ToListAsync()
            };
        }

        public IList<User> GetApplicationUsersInRole(string roleName)
        {
            var roleUserIdsQuery = from role in Roles
                                   where role.Name == roleName
                                   from user in role.Users
                                   select user.UserId;
            return _users.Where(applicationUser => roleUserIdsQuery.Contains(applicationUser.Id))
                         .ToList();
        }

        public IList<Role> GetRolesForCurrentUser()
        {
            var userId = getCurrentUserId();
            return GetRolesForUser(userId);
        }

        public IList<Role> GetRolesForUser(int userId)
        {
            var roles = FindUserRoles(userId);
            if (roles == null || !roles.Any())
            {
                return new List<Role>();
            }

            return roles.ToList();
        }

        public IList<UserRole> GetUserRolesInRole(string roleName)
        {
            return Roles.Where(role => role.Name == roleName)
                             .SelectMany(role => role.Users)
                             .ToList();
        }
        public virtual async Task<Role> GetRoleByName(string name) => await FindByNameAsync(name.ToLower().Trim());
        public virtual async Task<IEnumerable<Role>> GetRolesAsync(Expression<Func<Role, bool>> where = null,
     Func<IQueryable<Role>, IOrderedQueryable<Role>> orderby = null, int skip = 0, int take = 0,
     params Expression<Func<Role, object>>[] includeProperRoleies)
        {
            IQueryable<Role> queryable = _uow.Set<Role>();
            if (where != null)
            {
                queryable = queryable.Where(where);
            }
            if (orderby != null)
            {
                queryable = orderby(queryable);
            }
            if (skip >= 0 && take != 0)
            {
                queryable = queryable.Skip(skip).Take(take);
            }
            foreach (Expression<Func<Role, object>> includeProperRoley in includeProperRoleies)
            {

                queryable = queryable.Include<Role, object>(includeProperRoley);
            }

            return await queryable.AsNoTracking().ToListAsync();
        }
        public virtual async Task<int> Count(Expression<Func<Role, bool>> where = null)
        {
            return await _uow.Set<Role>().Where(where).CountAsync();
        }
        public virtual async Task<IEnumerable<UserRole>> GetUserRoleAsync(Expression<Func<UserRole, bool>> where = null,
       Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>> orderby = null, int skip = 0, int take = 0,
       params Expression<Func<UserRole, object>>[] includeProperties)
        {
            IQueryable<UserRole> queryable = _uow.Set<UserRole>();
            if (where != null)
            {
                queryable = queryable.Where(where);

            }
            if (orderby != null)
            {
                queryable = orderby(queryable);
            }
            if (skip > 0 && take != 0)
            {
                queryable = queryable.Skip(skip).Take(take);
            }
            foreach (Expression<Func<UserRole, object>> includeProperRoley in includeProperties)
            {

                queryable = queryable.Include<UserRole, object>(includeProperRoley);
            }

            return await queryable.AsNoTracking().ToListAsync();
        }
        public bool IsCurrentUserInRole(string roleName)
        {
            var userId = getCurrentUserId();
            return IsUserInRole(userId, roleName);
        }
        public virtual async Task<Role> CreateAsync(string roleName)
        {
            //var roleCheck= await RoleExistsAsync(roleName);
            //if (!roleCheck)
            //{
            var result = await CreateAsync(new Role() { Name = roleName });
            if (!result.Succeeded)
            {
                return new Role();
            }
            else
            {
                return await FindByNameAsync(roleName);
            }
            //}
            //    return (role: new Role(), exist: false);
        }
        public virtual async Task<IdentityResult> AddClaimAsync(object roleId, List<Claim> claims)
        {
            var FindRole = await FindByIdAsync(roleId.ToString());
            if (FindRole == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = "نقش مورد نظر یافت نشد."
                });
            }
            var existClaims = await _uow.Set<RoleClaim>().Where(x => x.RoleId == FindRole.Id).ToListAsync();
            if (existClaims.Any())
            {
                _uow.Set<RoleClaim>().RemoveRange(existClaims);
            }

            if (claims.Any())
            {
                var createClaim = new List<RoleClaim>();
                foreach (var item in claims)
                {
                    createClaim.Add(new RoleClaim() { RoleId = FindRole.Id, ClaimType = item.Type, ClaimValue = item.Value });
                }
                if (createClaim.Any())
                {
                    await _uow.Set<RoleClaim>().AddRangeAsync(createClaim);
                }
            }

            return IdentityResult.Success;
        }
        public async Task<Role> EditAsync(Role role)
        {
            var roleFind = await FindByIdAsync((role.Id).ToString());

            if (roleFind == null)
                return new Role();

            role.NormalizedName = role.Name;


            _uow.Entry(roleFind).CurrentValues.SetValues(role);
            // await _uow.SaveChangesAsync();
            return role;
        }
        public bool IsUserInRole(int userId, string roleName)
        {
            var userRolesQuery = from role in Roles
                                 where role.Name == roleName
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;
            var userRole = userRolesQuery.FirstOrDefault();
            return userRole != null;
        }
        public virtual async Task<Role> GetRoleById(int id) => await FindByIdAsync(id.ToString());
        public virtual async Task<List<Claim>> GetClaimsAsync(int roleId)
        {
            var role = await GetRoleById(roleId);
            if (role == null)
                return new List<Claim>();
            return (await GetClaimsAsync(role)).ToList();

        }
        public virtual async Task<List<Claim>> GetClaimsAsync(List<Role> roles)
        {
            var output = new List<Claim>();
            foreach (var item in roles)
            {
                var claims = await GetClaimsAsync(item);
                output.AddRange(claims.ToList());
            }
            return output;
        }
        public Task<Role> FindRoleIncludeRoleClaimsAsync(int roleId)
        {
            return Roles.Include(x => x.Claims).FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public async Task<IdentityResult> AddOrUpdateRoleClaimsAsync(
            int roleId,
            string roleClaimType,
            IList<string> selectedRoleClaimValues)
        {
            var role = await FindRoleIncludeRoleClaimsAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = "نقش مورد نظر یافت نشد."
                });
            }

            var currentRoleClaimValues = role.Claims.Where(roleClaim => roleClaim.ClaimType == roleClaimType)
                                                    .Select(roleClaim => roleClaim.ClaimValue)
                                                    .ToList();

            if (selectedRoleClaimValues == null)
            {
                selectedRoleClaimValues = new List<string>();
            }
            var newClaimValuesToAdd = selectedRoleClaimValues.Except(currentRoleClaimValues).ToList();
            foreach (var claimValue in newClaimValuesToAdd)
            {
                role.Claims.Add(new RoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = roleClaimType,
                    ClaimValue = claimValue
                });
            }

            var removedClaimValues = currentRoleClaimValues.Except(selectedRoleClaimValues).ToList();
            foreach (var claimValue in removedClaimValues)
            {
                var roleClaim = role.Claims.SingleOrDefault(rc => rc.ClaimValue == claimValue &&
                                                                  rc.ClaimType == roleClaimType);
                if (roleClaim != null)
                {
                    role.Claims.Remove(roleClaim);
                }
            }

            return await UpdateAsync(role);
        }

        private int getCurrentUserId() => _contextAccessor.HttpContext.User.Identity.GetUserId<int>();
        public virtual async Task<bool> IsClaim(string roleName, string type, string value)
        {
            //var role = await FindByNameAsync(roleName);

            var roles = await _uow.Set<Role>().Where(c => c.Name == roleName).ToListAsync();
            if (roles.Count == 0)
            {
                return false;
            }
            var role = roles.FirstOrDefault();
            var claims = (await GetClaimsAsync(role)).ToList();
            return claims.Any(c => c.Type.ToLower() == type.ToLower() && c.Value.ToLower() == value.ToLower());
        }
        public virtual async Task<IList<Role>> FindUserRolesAsync(int userId)
        {
            return await _uow.Set<UserRole>()
              .Where(a => a.UserId == userId)
              .Select(a => a.Role)
              .AsNoTracking()
              .ToListAsync();
        }

        public virtual async Task<IdentityResult> AddRoleClaimsAsync(Role role, List<RoleClaim> claims)
        {
            try
            {

                var exist = (await GetClaimsAsync(role)).ToList();
                //var exist = await GetUserAsync(c => c.Id == user.Id, includeProperties: c => c.Claims);
                if (exist.Any())
                {
                    foreach (var item in exist)
                    {
                        await RemoveClaimAsync(role, item);
                    }
                }
                if (claims.Any())
                {
                    foreach (var item in claims)
                    {
                        item.Id = 0;
                        //item.UserId = user.Id;
                        await _uow.Set<RoleClaim>().AddAsync(item);
                    }
                    await _uow.SaveChangesAsync();
                }
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InternalServerError",
                    Description = "خطا در اعمال دسترسی"
                });
            }
        }

        public async Task<(List<Role> list, int count, int totalPages)> Search(int? id = null, string description = null, string name = null, int pageIndex = 1, int pageSize = 10, Expression<Func<Role, object>>[] includes = null)
        {

            var queryable = _roles.AsQueryable();
            // بجز ادمین اصلی
            queryable = queryable.Where(c => c.Name != _settingOptions.Value.AdminUserSeed.RoleName);
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;
            queryable = name != null && name != "null" && name != "undefined" ? queryable.Where(c => c.Name.ToLower() == name.ToLower()) : queryable;
            queryable = description != null && description != "null" && description != "undefined" ? queryable.Where(c => c.Description.ToLower() == description.ToLower()) : queryable;

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.ToListAsync();
            return (list, count, totalPages);

        }



        #endregion
    }
}
