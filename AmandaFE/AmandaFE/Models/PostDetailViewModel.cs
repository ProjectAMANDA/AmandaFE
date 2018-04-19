using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Keyword> Keywords { get; set; }
    }
}
