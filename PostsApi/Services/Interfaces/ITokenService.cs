using PostsApi.Models.Entities.Identity;
using PostsApi.Models.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Services.Interfaces
{
    public interface ITokenService
    {
        UserToken BuildToken(ApplicationUser applicationUser, string role, string jwtSecretKey);
    }
}
