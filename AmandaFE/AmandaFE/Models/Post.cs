﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class Post
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int RelatedPostIds { get; set; }
        public ICollection<Post> RelatedPosts { get; set; }

        public int ImageIds { get; set; }

        [StringLength(60, MinimumLength = 1)]
        [Required]
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string RelatedArticles { get; set; }
        public float Sentiment { get; set; }

        [StringLength(120, MinimumLength = 1)]
        [Required]
        public string Keywords { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
