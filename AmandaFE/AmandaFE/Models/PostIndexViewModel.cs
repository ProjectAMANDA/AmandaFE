using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class PostIndexViewModel
    {
        public string SearchUserName { get; set; }
        public int SearchUserId { get; set; }

        public string SearchKeywordString { get; set; }

        public ICollection<Post> Posts { get; set; }
        public IEnumerable<Post> LastTen { get; set; }
        public DbSet<PostKeyword> PostKeywords { get; set; }
    }
}
