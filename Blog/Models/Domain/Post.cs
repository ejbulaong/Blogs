using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Blog.Models.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public bool Published { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Slug { get; set; }
        public List<Comment> Comments{get; set;}

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public Post()
        {            
            DateCreated = DateTime.Now;
            Comments = new List<Comment>();
        }

        public string GetSlug(string value)
        {
            var valueHolder = new StringBuilder();
            foreach (var c in value)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c ==' '))
                {
                    valueHolder.Append(c);
                }
            }

            string newValue = valueHolder.ToString().ToLower();

            var slug =newValue.Replace(" ", "-");           

            var lastChar = slug.Substring(slug.Length - 1);

            if (lastChar == "-")
            {
                slug = slug.Substring(0, slug.Length - 1);
            }

            return slug;
        }       
    }
}