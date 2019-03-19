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

        [HttpGet]
        public ActionResult Index()
        {
            var user = User.IsInRole("Admin");

            var model = (from b in DbContext.Posts
                         where (user ? (b.Published == true || b.Published == false) : b.Published == true)
                         select new IndexBlogViewModel
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Subtitle = b.Subtitle,
                             Body = b.Body,
                             UserName = b.User.UserName,
                             DateCreated = b.DateCreated,
                             DateUpdated = b.DateUpdated,
                             Slug = b.Slug,
                         }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string searchInput)
        {
            if (searchInput == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var user = User.IsInRole("Admin");

            var model = (from b in DbContext.Posts
                         where (user ? (b.Published == true || b.Published == false) : b.Published == true)
                         select new IndexBlogViewModel
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Subtitle = b.Subtitle,
                             Body = b.Body,
                             UserName = b.User.UserName,
                             DateCreated = b.DateCreated,
                             DateUpdated = b.DateUpdated,
                             Slug = b.Slug,
                         }).ToList();

            var filteredModel = (from b in model
                                 where (b.Title.ToLower()).Contains(searchInput.ToLower())
                                 select b).ToList();

            return View(filteredModel);
        }

        [HttpGet]
        public ActionResult SamplePost()
        {
            var userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            return View();
        }

        [HttpGet]
        public ActionResult BlogPost(string slug)
        {
            if (slug == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = (from b in DbContext.Posts
                         where b.Slug == slug
                         select new BlogPostViewModel
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Subtitle = b.Subtitle,
                             Body = b.Body,
                             UserName = b.User.UserName,
                             PhotoUrl = b.PhotoUrl,
                             DateCreated = b.DateCreated,
                             DateUpdated = b.DateUpdated,
                             Comments = b.Comments,
                             Slug = b.Slug
                         }).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult BlogPost(BlogPostViewModel model)
        {
            var userId = User.Identity.GetUserId();

            if (userId == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if (model == null || model.NewComment == null || model.NewComment == "")
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var commentToSave = new Comment();

            commentToSave.Body = model.NewComment;
            commentToSave.DateCreated = DateTime.Now;
            commentToSave.DateUpdated = DateTime.Now;
            commentToSave.PostId = model.Id;
            commentToSave.UserId = userId;
            commentToSave.Slug = model.Slug;

            DbContext.Comments.Add(commentToSave);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.BlogPost), new { slug = model.Slug });
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
                             DateCreated = post.DateCreated,
                             DateUpdated = post.DateUpdated
                         }).ToList();

            if (model == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            if (User.Identity.Name != "admin@blog.com")
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (model.Body == null || model.Photo == null)
            {
                return View();
            }

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

            var slugDuplicate = (from b in DbContext.Posts
                                 where b.Title == post.Title
                                 select b).FirstOrDefault();

            if (slugDuplicate != null)
            {
                post.Slug = $"{post.GetSlug(post.Title)}-{post.Id}";
            }
            else
            {
                post.Slug = post.GetSlug(post.Title);
            }

            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.ManagePosts));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.ManagePosts));
            }

            var model = (from b in DbContext.Posts
                         where b.Id == id
                         select new EditViewModel
                         {
                             Title = b.Title,
                             Subtitle = b.Subtitle,
                             Body = b.Body,
                             Published = b.Published,
                             PhotoUrl = b.PhotoUrl
                         }).FirstOrDefault();

            if (model == null)
            {
                ModelState.AddModelError("", "Post not found.");
                return View();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int? id, EditViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.ManagePosts));
            }

            if (model.Body == null || model.Photo == null)
            {
                return View();
            }

            var fileExtension = Path.GetExtension(model.Photo.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            var userId = User.Identity.GetUserId();

            var postToEdit = (from b in DbContext.Posts
                              where b.Id == id && b.UserId == userId
                              select b).FirstOrDefault();

            postToEdit.Title = model.Title;
            postToEdit.Subtitle = model.Subtitle;
            postToEdit.Body = model.Body;
            postToEdit.Published = model.Published;
            postToEdit.DateUpdated = DateTime.Now;
            postToEdit.PhotoUrl = UploadFile(model.Photo);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.ManagePosts));
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.ManagePosts));
            }

            var userId = User.Identity.GetUserId();

            var postToDelete = (from b in DbContext.Posts
                                where b.Id == id && b.UserId == userId
                                select b).FirstOrDefault();

            if (postToDelete != null)
            {
                DbContext.Posts.Remove(postToDelete);
                DbContext.SaveChanges();
            }

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

        [HttpGet]
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = (from c in DbContext.Comments
                         where c.Id == id
                         select new EditCommentViewModel
                         {
                             Id = c.Id,
                             Body = c.Body,
                             DateUpdated = c.DateUpdated,
                             DateCreated = c.DateCreated,
                             UpdatedReason = c.UpdatedReason,
                             User = c.User,
                             Slug = c.Slug
                         }).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult EditComment(int? id, EditCommentViewModel model, string userName)
        {
            if (!ModelState.IsValid)
            {
                var user = (from u in DbContext.Users
                            where u.UserName == userName
                            select u).FirstOrDefault();

                model.User = user;

                return View(model);
            }

            if (id == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var commentToEdit = (from c in DbContext.Comments
                                 where c.Id == id
                                 select c).FirstOrDefault();

            if (commentToEdit == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            commentToEdit.Body = model.Body;
            commentToEdit.DateUpdated = DateTime.Now;
            commentToEdit.UpdatedReason = model.UpdatedReason;
            commentToEdit.Slug = model.Slug;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.BlogPost), new { slug = commentToEdit.Slug });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteComment(int? id, string postSlug)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var commentToDelete = (from c in DbContext.Comments
                                   where c.Id == id
                                   select c).FirstOrDefault();

            if (commentToDelete != null)
            {
                DbContext.Comments.Remove(commentToDelete);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogController.BlogPost), new { slug = postSlug });
        }
    }
}