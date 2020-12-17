using Microsoft.AspNetCore.Identity;
using System;

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
    }
}
