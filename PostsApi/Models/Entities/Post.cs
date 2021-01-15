﻿using Microsoft.AspNetCore.Http;
using PostsApi.Models.Entities.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsApi.Models.Entities
{
    public class Post : BaseModel
    {
        public string Description { get; set; }
        public string ImageName { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime? AcceptedUserId { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public bool IsCreatedByAdmin { get; set; }
    }
}
