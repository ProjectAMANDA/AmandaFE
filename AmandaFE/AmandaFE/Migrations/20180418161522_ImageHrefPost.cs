using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AmandaFE.Migrations
{
    public partial class ImageHrefPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageIds",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "ImageHref",
                table: "Post",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageHref",
                table: "Post");

            migrationBuilder.AddColumn<int>(
                name: "ImageIds",
                table: "Post",
                nullable: false,
                defaultValue: 0);
        }
    }
}
