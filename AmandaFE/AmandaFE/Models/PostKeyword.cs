using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class PostKeyword
    {
        public Post Post { get; set; }
        public int PostId { get; set; }

        public Keyword Keyword { get; set; }
        public int KeywordId { get; set; }
    }
}
