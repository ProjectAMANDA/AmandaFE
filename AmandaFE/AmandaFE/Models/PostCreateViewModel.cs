using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class PostCreateViewModel
    {
        [Display(Name = "User Name")]
        [MinLength(3)]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Blog Post Title")]
        [MinLength(3)]
        [RegularExpression(@".*(.*\w.*){3}.*", ErrorMessage = "Title must contain at least 3 non-whitespace characters")]
        [Required]
        public string PostTitle { get; set; }

        [Display(Name = "Blog Text")]
        [MinLength(10)]
        // TODO(taylorjoshuaw): Need to fix the RegEx below for multiline
        //[RegularExpression(@"(?m)^.*(.*\w.*){10}.*$", ErrorMessage = "Post must contain at least 10 non-whitespace characters")]
        [Required]
        public string PostContent { get; set; }

        [Display(Name = "Enrich Post", Description = "Enriches post using Azure Cognitive Services and Related Articles")]
        public bool EnrichPost { get; set; }

        [Display(Name = "Blog Post Keywords (Comma Separated)")]
        public string Keywords { get; set; }
    }
}
