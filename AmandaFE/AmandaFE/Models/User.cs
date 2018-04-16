using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class User
    {
        public int Id { get; set; }

        public ICollection<Post> Posts { get; set; }
        public int PostIds { get; set; }

        public string Name { get; set; }
    }
}
