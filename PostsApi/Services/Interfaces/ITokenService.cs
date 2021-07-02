using PostsApi.Models.Entities.Identity;
using PostsApi.Models.JsonWebToken;

namespace PostsApi.Services.Interfaces
{
    public interface ITokenService
    {
        UserToken BuildToken(ApplicationUser applicationUser, string role, string jwtSecretKey);
    }
}
