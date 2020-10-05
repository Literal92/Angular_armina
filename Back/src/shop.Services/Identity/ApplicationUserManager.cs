using shop.Common.IdentityToolkit;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using DNTCommon.Web.Core;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Net;
using shop.ViewModels.Identity.Settings;

namespace shop.Services.Identity
{
    public class ApplicationUserManager :
        UserManager<User>,
        IApplicationUserManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IUsedPasswordsService _usedPasswordsService;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger<ApplicationUserManager> _logger;
        private readonly IOptions<IdentityOptions> _optionsAccessor;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IServiceProvider _services;
        private readonly DbSet<User> _users;
        private readonly DbSet<Role> _roles;
        private readonly IApplicationUserStore _userStore;
        private readonly IEnumerable<IUserValidator<User>> _userValidators;
        private User _currentUserInScope;
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly DbSet<UserRole> _userRoles;
        private readonly IOptionsSnapshot<SiteSettings> _settingOptions;

        public ApplicationUserManager(
            IApplicationUserStore store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<ApplicationUserManager> logger,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork uow,
            IUsedPasswordsService usedPasswordsService,
              IOptionsSnapshot<SiteSettings> _adminUserSeedOptions,
             IOptionsSnapshot<SiteSettings> settingOptions)
            : base(
                (UserStore<User, Role, ApplicationDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>)store,
                  optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = store ?? throw new ArgumentNullException(nameof(_userStore));
            _optionsAccessor = optionsAccessor ?? throw new ArgumentNullException(nameof(_optionsAccessor));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(_passwordHasher));
            _userValidators = userValidators ?? throw new ArgumentNullException(nameof(_userValidators));
            _passwordValidators = passwordValidators ?? throw new ArgumentNullException(nameof(_passwordValidators));
            _keyNormalizer = keyNormalizer ?? throw new ArgumentNullException(nameof(_keyNormalizer));
            _errors = errors ?? throw new ArgumentNullException(nameof(_errors));
            _services = services ?? throw new ArgumentNullException(nameof(_services));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(_contextAccessor));
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _usedPasswordsService = usedPasswordsService ?? throw new ArgumentNullException(nameof(_usedPasswordsService));
            _users = uow.Set<User>();
            _roles = uow.Set<Role>();
            _userRoles = uow.Set<UserRole>();
            _settingOptions = settingOptions;

            this._adminUserSeedOptions = _adminUserSeedOptions;
        }

        #region BaseClass

        string IApplicationUserManager.CreateTwoFactorRecoveryCode()
        {
            return base.CreateTwoFactorRecoveryCode();
        }

        Task<PasswordVerificationResult> IApplicationUserManager.VerifyPasswordAsync(IUserPasswordStore<User> store, User user, string password)
        {
            return base.VerifyPasswordAsync(store, user, password);
        }

        public override async Task<IdentityResult> CreateAsync(User user)
        {
            user.SerialNumber = Guid.NewGuid().ToString();

            var result = await base.CreateAsync(user);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            user.SerialNumber = Guid.NewGuid().ToString();
            var result = await base.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await base.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            return result;
        }

        #endregion

        #region CustomMethods

        public User FindById(int userId)
        {
            return _users.Find(userId);
        }

        public Task<User> FindByIdIncludeUserRolesAsync(int userId)
        {
            return _users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return Users.ToListAsync();
        }

        public User GetCurrentUser()
        {
            if (_currentUserInScope != null)
            {
                return _currentUserInScope;
            }

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return null;
            }

            var userId = int.Parse(currentUserId);
            return _currentUserInScope = FindById(userId);
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return _currentUserInScope ??
                (_currentUserInScope = await GetUserAsync(_contextAccessor.HttpContext.User));
        }

        public string GetCurrentUserId()
        {
            return _contextAccessor.HttpContext.User.Identity.GetUserId();
        }

        public int? CurrentUserId
        {
            get
            {
                var userId = _contextAccessor.HttpContext.User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                return !int.TryParse(userId, out int result) ? (int?)null : result;
            }
        }

        IPasswordHasher<User> IApplicationUserManager.PasswordHasher { get => base.PasswordHasher; set => base.PasswordHasher = value; }

        IList<IUserValidator<User>> IApplicationUserManager.UserValidators => base.UserValidators;

        IList<IPasswordValidator<User>> IApplicationUserManager.PasswordValidators => base.PasswordValidators;

        IQueryable<User> IApplicationUserManager.Users => base.Users;

        public string GetCurrentUserName()
        {
            return _contextAccessor.HttpContext.User.Identity.GetUserName();
        }

        public async Task<bool> HasPasswordAsync(int userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user?.PasswordHash != null;
        }

        public async Task<bool> HasPhoneNumberAsync(int userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user?.PhoneNumber != null;
        }

        public async Task<byte[]> GetEmailImageAsync(int? userId)
        {
            if (userId == null)
                return "?".TextToImage(new TextToImageOptions());

            var user = await FindByIdAsync(userId.Value.ToString());
            if (user == null)
                return "?".TextToImage(new TextToImageOptions());

            if (!user.IsEmailPublic)
                return "?".TextToImage(new TextToImageOptions());

            return user.Email.TextToImage(new TextToImageOptions());
        }

        public async Task<PagedUsersListViewModel> GetPagedUsersListAsync(SearchUsersViewModel model, int pageNumber)
        {
            var skipRecords = pageNumber * model.MaxNumberOfRows;
            var query = _users.Include(x => x.Roles).AsNoTracking();

            if (!model.ShowAllUsers)
            {
                query = query.Where(x => x.IsActive == model.UserIsActive);
            }

            if (!string.IsNullOrWhiteSpace(model.TextToFind))
            {
                model.TextToFind = model.TextToFind.ApplyCorrectYeKe();

                if (model.IsPartOfEmail)
                {
                    query = query.Where(x => x.Email.Contains(model.TextToFind));
                }

                if (model.IsUserId)
                {
                    if (int.TryParse(model.TextToFind, out int userId))
                    {
                        query = query.Where(x => x.Id == userId);
                    }
                }

                if (model.IsPartOfName)
                {
                    query = query.Where(x => x.FirstName.Contains(model.TextToFind));
                }

                if (model.IsPartOfLastName)
                {
                    query = query.Where(x => x.LastName.Contains(model.TextToFind));
                }

                if (model.IsPartOfUserName)
                {
                    query = query.Where(x => x.UserName.Contains(model.TextToFind));
                }

                if (model.IsPartOfLocation)
                {
                    query = query.Where(x => x.Location.Contains(model.TextToFind));
                }
            }

            if (model.HasEmailConfirmed)
            {
                query = query.Where(x => x.EmailConfirmed);
            }

            if (model.UserIsLockedOut)
            {
                query = query.Where(x => x.LockoutEnd != null);
            }

            if (model.HasTwoFactorEnabled)
            {
                query = query.Where(x => x.TwoFactorEnabled);
            }

            query = query.OrderBy(x => x.Id);
            return new PagedUsersListViewModel
            {
                Paging =
                {
                    TotalItems = await query.CountAsync()
                },
                Users = await query.Skip(skipRecords).Take(model.MaxNumberOfRows).ToListAsync(),
                Roles = await _roles.ToListAsync()
            };
        }

        public async Task<PagedUsersListViewModel> GetPagedUsersListAsync(
            int pageNumber, int recordsPerPage,
            string sortByField, SortOrder sortOrder,
            bool showAllUsers)
        {
            var skipRecords = pageNumber * recordsPerPage;
            var query = _users.Include(x => x.Roles).AsNoTracking();

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
                Roles = await _roles.ToListAsync()
            };
        }

        public async Task<IdentityResult> UpdateUserAndSecurityStampAsync(int userId, Action<User> action)
        {
            var user = await FindByIdIncludeUserRolesAsync(userId);
            user.SerialNumber = Guid.NewGuid().ToString();

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }

            action(user);

            var result = await UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateSecurityStampAsync(user);
        }

        public async Task<IdentityResult> AddOrUpdateUserRolesAsync(int userId, IList<int> selectedRoleIds, Action<User> action = null)
        {
            var user = await FindByIdIncludeUserRolesAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }

            var currentUserRoleIds = user.Roles.Select(x => x.RoleId).ToList();

            if (selectedRoleIds == null)
            {
                selectedRoleIds = new List<int>();
            }

            var newRolesToAdd = selectedRoleIds.Except(currentUserRoleIds).ToList();
            foreach (var roleId in newRolesToAdd)
            {
                user.Roles.Add(new UserRole { RoleId = roleId, UserId = user.Id });
            }

            var removedRoles = currentUserRoleIds.Except(selectedRoleIds).ToList();
            foreach (var roleId in removedRoles)
            {
                var userRole = user.Roles.SingleOrDefault(ur => ur.RoleId == roleId);
                if (userRole != null)
                {
                    user.Roles.Remove(userRole);
                }
            }

            action?.Invoke(user);

            var result = await UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateSecurityStampAsync(user);
        }

        Task<IdentityResult> IApplicationUserManager.UpdatePasswordHash(User user, string newPassword, bool validatePassword)
        {
            return base.UpdatePasswordHash(user, newPassword, validatePassword);
        }
        public async Task<string> GetSerialNumberAsync(int userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            return user.SerialNumber;
        }

        public async Task UpdateUserLastActivityDateAsync(int userId)
        {
            var user = FindById(userId);
            if (user.LastLoggedIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }
            user.LastLoggedIn = DateTime.UtcNow;
            await _uow.SaveChangesAsync();
        }


        public User FindByRefererCode(string refererCode)
        {
            return _users.FirstOrDefault(a => a.RefererCode.Equals(refererCode));
        }

        public User FindByUsername(string username)
        {
            return _users.FirstOrDefault(a => a.UserName == username);
        }
        public virtual async Task<User> FindUserAsync(string username, string password)
        {
            //var user = await FindByNameAsync(username);
            var user = await _uow.Set<User>().FirstOrDefaultAsync(a => a.UserName == username);

            if (user != null/* && await CheckPasswordAsync(user, password.ToLower())*/)
                return user;
            else
                return null;
        }
        public virtual async Task<User> FindUserAsync2(string username, string password)
        {
            var user = await _uow.Set<User>().Include(z => z.Roles).FirstOrDefaultAsync(a => a.UserName == username);

            if (user != null && await CheckPasswordAsync(user, password))
                return user;
            else
                return null;
        }
        public virtual async Task<User> FindBySerialNumber(string serialNumber)
        {
            return await _users.FirstOrDefaultAsync(x => x.SerialNumber == serialNumber);
        }


        public async virtual Task<IEnumerable<User>> GetUserAsync(Expression<Func<User, bool>> where = null,
       Func<IQueryable<User>, IOrderedQueryable<User>> orderby = null, int skip = 0, int take = 0,
       params Expression<Func<User, object>>[] includeProperties)
        {
            IQueryable<User> queryable = _uow.Set<User>();
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
            foreach (Expression<Func<User, object>> includeProperRoley in includeProperties)
            {

                queryable = queryable.Include<User, object>(includeProperRoley);
            }

            return await queryable.AsNoTracking().ToListAsync();
        }


        public virtual async Task<(List<User> Users, int Count, int TotalPages)> Get(int? id = null, int pageSize = 0, int pageIndex = 0, string Mobile = null, string displayName = null,
            int? status = null, Expression<Func<User, object>>[] includes = null)
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            var queryable = _users.Where(c => ((c.UserName).ToLower() != (adminUserSeed.Username).ToLower()));
            //IQueryable<User> queryable = _uow.Set<User>();
            queryable = Mobile != null ? queryable.Where(x => EF.Functions.Like(x.PhoneNumber, $"%{Mobile}%")) : queryable;
            queryable = displayName != null ? queryable.Where(x => EF.Functions.Like(x.DisplayName, $"%{displayName}%")) : queryable;
            queryable = status != null ? queryable.Where(c => c.IsActive == Convert.ToBoolean(status)) : queryable;
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            queryable = queryable.OrderByDescending(c => c.Id);

            if (pageIndex > 0 && pageSize > 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }

            if (includes != null)
                foreach (Expression<Func<User, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.AsNoTracking().ToListAsync();
            return (Users: list, Count: count, TotalPages: totalPages);

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

        public virtual async Task<int> Count(Expression<Func<User, bool>> where = null)
        {
            return await _uow.Set<User>().Where(where).CountAsync();
        }

        public virtual async Task<IdentityResult> UpdateUserAsync(User user, string newpass)
        {
            if (!string.IsNullOrEmpty(user.UserName))
            {
                if (user.UserName.Length < 6)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "BadRequest",
                        Description = "نام کاربری باید حداقل شامل 6 حرف باشد !"
                    });
                }
                var exist = FindByUsername(user.UserName);
                // نام کاربری تکراری
                if (exist != null && exist.Id != user.Id)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "Conflict",
                        Description = "نام کاربری معتبر نیست !"
                    });
                }
            }
            var old = await base.FindByIdAsync((user.Id).ToString());

            if (old == default(User))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }
            if (newpass != null)
            {
                var outRemovePass = await RemovePasswordAsync(old);
                if (!outRemovePass.Succeeded)
                {
                    return outRemovePass;
                }
                var outResetPass = await AddPasswordAsync(old, newpass);
                if (!outResetPass.Succeeded)
                {
                    return outResetPass;
                }
            }
            // _uow.Entry(old).CurrentValues.SetValues(user);
            if (user.UserName != null)
                old.UserName = user.UserName;
            if (user.PhoneNumber != null)
                old.PhoneNumber = user.PhoneNumber;
            if (user.FirstName != null)
                old.FirstName = user.FirstName;
            if (user.LastName != null)
                old.LastName = user.LastName;
            if (user.Mobile != null)
                old.Mobile = user.Mobile;
            if (user.Email != null)
                old.Email = user.Email;
            old.IsActive = user.IsActive;
            old.SerialNumber = Guid.NewGuid().ToString();
            var outUpdate = await base.UpdateAsync(old);
            return outUpdate;

        }


        public virtual async Task<User> ChangePasswordAsync(User user, string newpass)
        {

            var old = await base.FindByIdAsync((user.Id).ToString());
            if (old == default(User))
            {
                return null;
            }
            var outRemovePass = await RemovePasswordAsync(old);
            if (!outRemovePass.Succeeded)
            {
                return null;
            }
            var outResetPass = await AddPasswordAsync(old, newpass);
            if (!outResetPass.Succeeded)
            {
                return null;
            }
            return user;

        }



        public virtual async Task<IdentityResult> AddUserRoleAsync(int userId, List<Role> roles)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user == default(User))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "کاربر مورد نظر یافت نشد."
                });
            }

            var exist = (await GetUserRoleAsync(c => c.UserId == userId,
                                     includeProperties: c => c.Role))
                                    .Select(s => s.Role).ToList();

            // remove
            var listRemove = await _uow.Set<UserRole>().Where(c => c.UserId == userId).ToListAsync();
            if (listRemove.Any())
            {
                _uow.Set<UserRole>().RemoveRange(listRemove);
            }
            // add
            if (roles.Any())
            {
                var listAdd = new List<UserRole>();
                listAdd.AddRange(roles.Select(s => new UserRole { UserId = userId, RoleId = s.Id }));
                await _uow.Set<UserRole>().AddRangeAsync(listAdd);
            }

            await _uow.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public virtual async Task<IdentityResult> AddUserClaimsAsync(User user, List<UserClaim> claims)
        {
            try
            {

                var exist = (await GetClaimsAsync(user)).ToList();
                //var exist = await GetUserAsync(c => c.Id == user.Id, includeProperties: c => c.Claims);
                if (exist.Any())
                {
                    foreach (var item in exist)
                    {
                        await RemoveClaimAsync(user, item);
                    }
                }
                if (claims.Any())
                {
                    foreach (var item in claims)
                    {
                        item.Id = 0;
                        //item.UserId = user.Id;
                        await _uow.Set<UserClaim>().AddAsync(item);
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
        public virtual async Task AddClaimAsync(User user, List<Claim> claims)
        {
            var exist = (await GetClaimsAsync(user)).ToList();
            //var addlist = claims.Select(x => new { x.Type, x.Value }).Except(existClaims.Select(x => new { x.Type, x.Value })).ToList();
            //var removedlist = existClaims.Select(x => new { x.Type, x.Value }).Except(claims.Select(x => new { x.Type, x.Value })).ToList();
            if (exist.Any())
            {
                foreach (var item in exist)
                {
                    await RemoveClaimAsync(user, item);
                }
            }
            if (claims.Any())
            {
                foreach (var item in claims)
                {
                    var createClaim = new UserClaim() { UserId = user.Id, ClaimType = item.Type, ClaimValue = item.Value };
                    await _uow.Set<UserClaim>().AddAsync(createClaim);
                }
            }
        }



        public async Task<(HttpStatusCode status, bool success, string error)> ChangeTypeUser(int id)
        {
            try
            {
                var find = await _users.FirstOrDefaultAsync(c => c.Id == id);
                if (find == null)
                    return (HttpStatusCode.NotFound, false, "کاربری یافت نشد !");

                var user = find;
                if (user.UserType == Entities.Identity.Enum.UserTypeEnum.UserType.Client)
                {
                    user.UserType = Entities.Identity.Enum.UserTypeEnum.UserType.Reseller;
                    user.PercentDiscount = 0;
                }
                else
                {
                    user.UserType = Entities.Identity.Enum.UserTypeEnum.UserType.Client;
                    user.PercentDiscount =0 ;

                }
                await _uow.SaveChangesAsync();
                return (HttpStatusCode.OK, true, null);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, false, "خطا سمت سرور !");
            }
        }

        public async Task<(HttpStatusCode status, bool success, string error)> UpdateActive(int id)
        {
            try
            {

                var find = await _users.FirstOrDefaultAsync(c => c.Id == id);
                if (find == null)
                    return (HttpStatusCode.NotFound, false, "کاربری یافت نشد !");

                var user = find;
                user.IsActive = !find.IsActive;
                _uow.Entry(find).CurrentValues.SetValues(user);
                await _uow.SaveChangesAsync();
                return (HttpStatusCode.OK, true, null);
            }
            catch
            {
                return (HttpStatusCode.InternalServerError, false, "خطا سمت سرور !");
            }
        }

        public async Task<(IdentityResult result, int id)> CreateWithRoleAsync(User user, string password, string role)
        {
            user.SerialNumber = Guid.NewGuid().ToString();
            var result = await base.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _usedPasswordsService.AddToUsedPasswordsListAsync(user);
            }
            var roleAdmin = await _roles.Where(c => c.Name.ToLower() == role.ToLower()).ToListAsync();
            if (roleAdmin != null)
            {
                var resultRole = await AddUserRoleAsync(user.Id, roleAdmin);
            }

            return (result, user.Id);
        }


        public virtual async Task<IdentityResult> Update(User user)
        {

            var outUpdate = await base.UpdateAsync(user);
            return outUpdate;

        }


        public async Task<(List<User> list, int count, int totalPages)> SearchAdmin(int? id = null, string displayName = null, string userName = null, string mobile = null, int pageIndex = 1, int pageSize = 10, Expression<Func<User, object>>[] includes = null)
        {

            var queryable = _userRoles.AsQueryable();
            var users = queryable.Where(c => c.Role.Name.ToLower() == "Admin".ToLower()).Select(c => c.User);


            // بجز ادمین اصلی
            users = users.Where(c => c.UserName != _settingOptions.Value.AdminUserSeed.Username);
            users = id != null ? users.Where(c => c.Id == id) : users;
            users = userName != null && userName != "null" && mobile != "undefined" ? users.Where(c => c.UserName == userName) : users;
            users = mobile != null && mobile != "null" && mobile != "undefined" ? users.Where(c => c.Mobile == mobile) : users;
            users = displayName != null && displayName != "null" && displayName != "undefined" ? users.Where(c => EF.Functions.Like(c.DisplayName, $"{displayName}%")) : users;

            var count = await users.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                users = users.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes.ToList())
                    users = users.Include(item);

            var list = await users.ToListAsync();
            return (list, count, totalPages);

        }

        public async Task<(List<User> list, int count, int totalPages)> Search(int? id = null, string displayName = null, string userName = null, string mobile = null, int pageIndex = 1, int pageSize = 10, Expression<Func<User, object>>[] includes = null)
        {

            var queryable = _users.AsQueryable();
            // بجز ادمین اصلی
            queryable = queryable.Where(c => c.UserName != _settingOptions.Value.AdminUserSeed.Username);
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;
            queryable = userName != null && userName != "null" && mobile != "undefined" ? queryable.Where(c => c.UserName == userName) : queryable;
            queryable = mobile != null && mobile != "null" && mobile != "undefined" ? queryable.Where(c => c.Mobile == mobile) : queryable;
            queryable = displayName != null && displayName != "null" && displayName != "undefined" ? queryable.Where(c => EF.Functions.Like(c.DisplayName, $"{displayName}%")) : queryable;

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
        public async Task<User> GetByUserId(int id, Expression<Func<User, object>>[] includes = null)
        {

            var queryable = _users.AsQueryable();
            queryable = queryable.Where(c => c.Id == id);

            if (includes != null)
                foreach (var item in includes.ToList())
                    queryable = queryable.Include(item);

            var user = await queryable.FirstOrDefaultAsync();
            return user;

        }


        public async Task<User> GetVerifyCode(string username)
        {
            var rand = new Random();
            var code = rand.Next(1000, 9999);
           // code = 1234;
            var roleName = ConstantRoles.Client;
            var user = await _users.FirstOrDefaultAsync(a => a.UserName == username);
            if (user == null)
            {
                user = new User()
                {
                    UserName = username,
                    PhoneNumber = username,
                    IsRegister = true,
                    IsActive = true,
                    VerifyCode = code,
                    ExpireDateVerifyCode = DateTime.Now.AddDays(1),
                    SerialNumber = Guid.NewGuid().ToString(),
                    UserType = Entities.Identity.Enum.UserTypeEnum.UserType.Client

                };
                await CreateAsync(user, "Shop@321");
            }

            var isRole = await IsInRoleAsync(user, roleName);
            if (!isRole)
            {
                var role = await _roles.FirstOrDefaultAsync(p => p.Name == roleName);
                if (role == null)
                {
                    role = new Role { Name = roleName, NormalizedName = roleName.ToUpper() };
                    await _roles.AddAsync(role);
                    await _uow.SaveChangesAsync();
                }
                await AddToRoleAsync(user, roleName);
            }



            user.VerifyCode = code;
            user.ExpireDateVerifyCode = DateTime.Now.AddDays(1);
            //await UpdateAsync(user);
           await _uow.SaveChangesAsync();
            return user;
        }
        /// <summary>
        /// درخواست همکاری
        /// </summary>
        /// <returns></returns>
        public async Task<User> CooperationRequest(User model)
        {
            var rand = new Random();
            var code = rand.Next(1000, 9999);
            // code = 1234;
            var roleName = ConstantRoles.Client;
            var user = await _users.FirstOrDefaultAsync(a => a.UserName == model.UserName);
            if (user == null)
            {
                user = new User()
                {
                    UserName = model.UserName,
                    PhoneNumber = model.UserName,
                    Mobile= model.Mobile,
                    IsRegister = true,
                    IsActive = true,
                    SerialNumber = Guid.NewGuid().ToString(),
                    UserType = Entities.Identity.Enum.UserTypeEnum.UserType.Client,
                    IsRequest = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    WhatsApp = model.WhatsApp,
                    Telegram = model.Telegram,
                    Instagram = model.Instagram,
                    WebSite = model.WebSite,
                    Picture = model.Picture

            };
                await CreateAsync(user, "Shop@321");
                var isRole = await IsInRoleAsync(user, roleName);
                if (!isRole)
                {
                    var role = await _roles.FirstOrDefaultAsync(p => p.Name == roleName);
                    if (role == null)
                    {
                        role = new Role { Name = roleName, NormalizedName = roleName.ToUpper() };
                        await _roles.AddAsync(role);
                        await _uow.SaveChangesAsync();
                    }
                    await AddToRoleAsync(user, roleName);
                }
            }
            else
            {
                    user.IsRequest = true;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.WhatsApp = model.WhatsApp;
                    user.Telegram = model.Telegram;
                    user.Instagram = model.Instagram;
                    user.WebSite = model.WebSite;
                    user.Picture = model.Picture;

            }
            await _uow.SaveChangesAsync();
            return user;
        }

        #endregion


    }
}