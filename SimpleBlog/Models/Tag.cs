using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.Models
{
    public class Tag
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Tag")]
        public string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public Tag()
        {
            this.Posts = new List<Post>();
        }
    }
}