using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AmandaFE.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Post> LastTen { get; set; }
        public ICollection<Post> Posts { get; set; }
        public DbSet<PostKeyword> PostKeywords { get; set; }
    }
}
