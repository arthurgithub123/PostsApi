using System;

namespace PostsApi.Models.Token
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
