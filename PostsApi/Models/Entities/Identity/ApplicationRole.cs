using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PostsApi.Models.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {

        }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
