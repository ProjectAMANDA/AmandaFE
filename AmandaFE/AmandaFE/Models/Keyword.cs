using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class Keyword
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public ICollection<PostKeyword> PostKeywords { get; } = new List<PostKeyword>();
    }
}
