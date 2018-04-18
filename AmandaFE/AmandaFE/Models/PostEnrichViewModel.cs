using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class PostEnrichViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<string> ImageHrefs { get; set; }
    }
}

