﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PostsApi.Models.Entities.Identity;
using System;

namespace PostsApi.Context
{
    public class PostsContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public PostsContext(DbContextOptions<PostsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(applicationUser => applicationUser.Posts)
                .WithOne(post => post.User);
        }
    }
}
