using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AmandaFE.Migrations
{
    public partial class CollectionIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostIds",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RelatedPostIds",
                table: "Post");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostIds",
                table: "User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RelatedPostIds",
                table: "Post",
                nullable: false,
                defaultValue: 0);
        }
    }
}
