using Microsoft.AspNetCore.Http;
using PostsApi.Models.Entities.Identity;
using System;

namespace PostsApi.Models.Entities
{
    public class Post : BaseModel
    {
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid? RecomendUserId { get; set; }
        public DateTime? RecomendDate { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
