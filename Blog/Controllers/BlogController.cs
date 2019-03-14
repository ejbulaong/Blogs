using Blog.Models;
using Blog.Models.Domain;
using Blog.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class BlogController : Controller
    {
        private List<string> AllowedExtenions = new List<string>
                { ".jpeg", ".jpg", ".gif", ".png" };

        private ApplicationDbContext DbContext;

        public BlogController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ManagePosts()
        {
            if (User.Identity.Name != "admin@blog.com")
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var userId = User.Identity.GetUserId();
            var model = (from post in DbContext.Posts
                         where post.UserId == userId
                         select new ManagePostsViewModel
                         {
                             Id = post.Id,
                             Title = post.Title,
                             Published = post.Published,
                             DateCreated = post.DateCreated
                         }).ToList();

            if(model == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }            

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            var fileExtension = Path.GetExtension(model.Photo.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            var post = new Post();
            post.Title = model.Title;
            post.Subtitle = model.Subtitle;
            post.Body = model.Body;
            post.Published = model.Published;
            post.PhotoUrl = UploadFile(model.Photo);

            var userId = User.Identity.GetUserId();
            post.UserId = userId;            

            DbContext.Posts.Add(post);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.ManagePosts));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int? id, EditViewModel model)
        {
            var fileExtension = Path.GetExtension(model.Photo.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            var post = new Post();
            post.Title = model.Title;
            post.Subtitle = model.Subtitle;
            post.Body = model.Body;
            post.Published = model.Published;
            post.PhotoUrl = UploadFile(model.Photo);

            var userId = User.Identity.GetUserId();
            post.UserId = userId;

            DbContext.Posts.Add(post);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.ManagePosts));
        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload/";
                var mappedFolder = Server.MapPath(uploadFolder);

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file.FileName);

                return uploadFolder + file.FileName;
            }

            return null;
        }
    }
}