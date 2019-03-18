using Blog.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UserName { get; set; }
        public List<Comment> Comments { get; set; }
        public string NewComment { get; set; }
    }
}