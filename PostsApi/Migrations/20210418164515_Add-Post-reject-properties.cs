using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostsApi.Migrations
{
    public partial class AddPostrejectproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedUserId",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RejectedUserId",
                table: "Posts");
        }
    }
}
