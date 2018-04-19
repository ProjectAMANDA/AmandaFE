using AmandaFE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Data
{
    public class BlogDBContext : DbContext
    {
        public DbSet<Post> Post { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Keyword> Keyword { get; set; }
        public DbSet<PostKeyword> PostKeyword { get; set; }

        public BlogDBContext(DbContextOptions<BlogDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostKeyword>()
                .HasKey(k => new { k.PostId, k.KeywordId });
        }
    }
}
