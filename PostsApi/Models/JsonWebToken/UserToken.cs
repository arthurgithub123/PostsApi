namespace PostsApi.Models.JsonWebToken
{
    public class UserToken
    {
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }
}
