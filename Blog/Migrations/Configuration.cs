namespace Blog.Migrations
{
    using Blog.Models;
    using Blog.Models.Domain;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //Seeding Users and Roles

            //RoleManager, used to manage roles
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            //Adding moderator role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Moderator"))
            {
                var moderatorRole = new IdentityRole("Moderator");
                roleManager.Create(moderatorRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;

            if (!context.Users.Any(
                p => p.UserName == "admin@blog.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@blog.com";
                adminUser.Email = "admin@blog.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            //Creating the moderator
            ApplicationUser moderator;

            if (!context.Users.Any(
                p => p.UserName == "moderator@blog.com"))
            {
                moderator = new ApplicationUser();
                moderator.UserName = "moderator@blog.com";
                moderator.Email = "moderator@blog.com";

                userManager.Create(moderator, "Password-1");
            }
            else
            {
                moderator = context
                    .Users
                    .First(p => p.UserName == "moderator@blog.com");
            }

            //Make sure the user is on the moderator role
            if (!userManager.IsInRole(moderator.Id, "Moderator"))
            {
                userManager.AddToRole(moderator.Id, "Moderator");
            }
        }
    }
}
