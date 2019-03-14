using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Subtitle { get; set; }
        [AllowHtml, Required]
        public string Body { get; set; }
        [Required]
        public bool Published { get; set; }
        public DateTime? DateUpdated { get; set; }
        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }
}