using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class Post
    {
        public int ID { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Post")]
        public string Body { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public Post()
        {
            this.Tags = new List<Tag>();
        }
    }
}