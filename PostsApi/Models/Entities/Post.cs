using Microsoft.AspNetCore.Http;
using PostsApi.Models.Entities.Identity;

namespace PostsApi.Models.Entities
{
    public class Post : BaseModel
    {
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
