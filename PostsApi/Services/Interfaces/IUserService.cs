using Microsoft.AspNetCore.Http;

namespace PostsApi.Services.Interfaces
{
    public interface IUserService
    {
        string SaveProfileImage(IFormFile formFile);
    }
}
