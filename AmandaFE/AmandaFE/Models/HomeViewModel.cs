using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Post> LastTen { get; set; }
    }
}
