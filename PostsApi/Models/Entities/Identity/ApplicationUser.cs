using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PostsApi.Models.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public static string GetAvatar(ApplicationUser applicationUser)
        {
            if (string.IsNullOrEmpty(applicationUser.Avatar))
            {
                return "default.png";
            }

            return applicationUser.Avatar;
        }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
