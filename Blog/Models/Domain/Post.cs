using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.Domain
{
    public class Post
    {
        public int Id { get; set;}
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Post()
        {
            DateCreated = DateTime.Now;
        }
    }
}