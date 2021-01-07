﻿using Microsoft.AspNetCore.Http;
using PostsApi.Models.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsApi.Models.Entities
{
    public class Post : BaseModel
    {
        public string Description { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid? RecomendUserId { get; set; }
        public DateTime? RecomendDate { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
