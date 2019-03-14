using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class ManagePostsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Published { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}