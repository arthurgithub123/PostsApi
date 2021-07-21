using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PostsApi.Models.Entities;
using PostsApi.Models.Entities.Identity;
using System;

namespace PostsApi.Context
{
    public class PostsContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public PostsContext(DbContextOptions<PostsContext> options) : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(configiguration =>
            {
                configiguration
                    .HasMany(applicationUser => applicationUser.Posts)
                    .WithOne(post => post.User);

                configiguration
                    .HasMany(applicationUser => applicationUser.UserRoles)
                    .WithOne(applicationUser => applicationUser.User)
                    .HasForeignKey(userRole => userRole.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(configiguration =>
            {
                configiguration
                    .HasMany(applicationRole => applicationRole.UserRoles)
                    .WithOne(applicationRole => applicationRole.Role)
                    .HasForeignKey(userRole => userRole.RoleId)
                    .IsRequired();
            });
        }
    }
}
