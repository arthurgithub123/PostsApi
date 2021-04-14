using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostsApi.Migrations
{
    public partial class AddPostAcceptedUserIdproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AcceptedUserId",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedUserId",
                table: "Posts");
        }
    }
}
