using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AmandaFE.Migrations
{
    public partial class KeywordsManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_PostId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Post");

            migrationBuilder.CreateTable(
                name: "Keyword",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keyword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostKeyword",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false),
                    KeywordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostKeyword", x => new { x.PostId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_PostKeyword_Keyword_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keyword",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostKeyword_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostKeyword_KeywordId",
                table: "PostKeyword",
                column: "KeywordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostKeyword");

            migrationBuilder.DropTable(
                name: "Keyword");

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Post",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Post",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostId",
                table: "Post",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_PostId",
                table: "Post",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
