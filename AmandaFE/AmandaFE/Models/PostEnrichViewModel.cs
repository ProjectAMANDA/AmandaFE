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
        public int PostId { get; set; }

        public JToken[] Images { get; set; }
        public string SelectedImageHref { get; set; }

        public float Sentiment { get; set; }
    }
}
