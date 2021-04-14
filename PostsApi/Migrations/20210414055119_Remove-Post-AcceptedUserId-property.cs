using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostsApi.Migrations
{
    public partial class RemovePostAcceptedUserIdproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedUserId",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedUserId",
                table: "Posts",
                type: "datetime2",
                nullable: true);
        }
    }
}
