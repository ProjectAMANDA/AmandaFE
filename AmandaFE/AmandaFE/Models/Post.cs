using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RelatedPostIds { get; set; }
        public int ImageIds { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string RelatatedArticles { get; set; }
        public float Sentiment { get; set; }
        public string Keywords { get; set; }
        public DateTime dateTime { get; set; }
    }
}
