using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class EditCommentViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        [Required]
        public string UpdatedReason { get; set; }
        public string Slug { get; set; }
        public ApplicationUser User { get; set; }
    }
}