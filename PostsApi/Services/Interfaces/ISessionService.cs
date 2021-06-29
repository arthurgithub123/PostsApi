using PostsApi.Models.Token;
using PostsApi.Models.ViewModels.User;
using System.Threading.Tasks;

namespace PostsApi.Services.Interfaces
{
    public interface ISessionService
    {
        public Task CreateAdministrator(AdminCreateViewModel adminCreateViewModel, bool isModelStateValid);
        public Task<UserToken> CreateCommom(UserCreateViewModel userCreateViewModel, bool isModelStateValid);
        public Task CreatePassword(PasswordCreateViewModel passwordCreateViewModel, bool isModelStateValid);
        public Task<UserToken> Login(UserLoginViewModel userLoginViewModel, bool isModelStateValid);
    }
}
