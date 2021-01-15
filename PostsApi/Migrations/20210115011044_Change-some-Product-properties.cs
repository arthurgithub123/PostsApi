using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PostsApi.Migrations
{
    public partial class ChangesomeProductproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecomendDate",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RecomendUserId",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedUserId",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCreatedByAdmin",
                table: "Posts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedUserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsCreatedByAdmin",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecomendDate",
                table: "Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RecomendUserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
