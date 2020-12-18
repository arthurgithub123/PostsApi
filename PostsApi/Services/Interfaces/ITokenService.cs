using PostsApi.Models.Entities.Identity;
using PostsApi.Models.Token;

namespace PostsApi.Services.Interfaces
{
    public interface ITokenService
    {
        UserToken BuildToken(ApplicationUser applicationUser, string role, string jwtSecretKey);
    }
}
