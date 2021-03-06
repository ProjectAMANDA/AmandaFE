﻿// <auto-generated />
using AmandaFE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AmandaFE.Migrations
{
    [DbContext(typeof(BlogDBContext))]
    [Migration("20180419040109_PostKeywordJunction")]
    partial class PostKeywordJunction
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AmandaFE.Models.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Keyword");
                });

            modelBuilder.Entity("AmandaFE.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("ImageHref");

                    b.Property<string>("RelatedArticles");

                    b.Property<float>("Sentiment");

                    b.Property<string>("Summary");

                    b.Property<string>("Title");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("AmandaFE.Models.PostKeyword", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("KeywordId");

                    b.HasKey("PostId", "KeywordId");

                    b.HasIndex("KeywordId");

                    b.ToTable("PostKeyword");
                });

            modelBuilder.Entity("AmandaFE.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("AmandaFE.Models.Post", b =>
                {
                    b.HasOne("AmandaFE.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AmandaFE.Models.PostKeyword", b =>
                {
                    b.HasOne("AmandaFE.Models.Keyword", "Keyword")
                        .WithMany("PostKeywords")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AmandaFE.Models.Post", "Post")
                        .WithMany("PostKeywords")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
