﻿using shop.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;

namespace shop.Entities.Identity
{
    public class RoleClaim : IdentityRoleClaim<int>, IAuditableEntity
    {
        public virtual Role Role { get; set; }
    }
}