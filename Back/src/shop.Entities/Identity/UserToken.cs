using shop.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace shop.Entities.Identity
{
    public class UserToken : IdentityUserToken<int>, IAuditableEntity
    {
        public virtual User User { get; set; }
    }
}