namespace jcamacho_budgeter1.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<jcamacho_budgeter1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jcamacho_budgeter1.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Household Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Household Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Household User"))
            {
                roleManager.Create(new IdentityRole { Name = "Household User" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "jcamacho1964@optonline.net"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "James",
                    LastName = "Camacho",
                    DisplayName = "James Camacho",
                    PhoneNumber = "(704) 816-0913",
                    UserName = "jcamacho1964@optonline.net",
                    Email = "jcamacho1964@optonline.net",
                }, "Empire111!");
            }

            if (!context.Users.Any(u => u.Email == "jamescamacho821@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Jim",
                    LastName = "Camacho",
                    DisplayName = "Jim Camacho",
                    PhoneNumber = "(704) 000-0000",
                    UserName = "jamescamacho821@gmail.com",
                    Email = "jamescamacho821@gmail.com",
                }, "Password1!");
            }

            if (!context.Users.Any(u => u.Email == "jrnasf@optonline.net"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Ralph",
                    LastName = "Felice",
                    DisplayName = "Ralph Felice",
                    PhoneNumber = "(516) 000-0000",
                    UserName = "jrnasf@optonline.net",
                    Email = "jrnasf@optonline.net",
                }, "Password1!");
            }

            var userIdAdmin = userManager.FindByEmail("jcamacho1964@optonline.net").Id;
            userManager.AddToRole(userIdAdmin, "Admin");

            var userIdHAdmin = userManager.FindByEmail("jamescamacho821@gmail.com").Id;
            userManager.AddToRole(userIdHAdmin, "Household Admin");

            var userIdUser = userManager.FindByEmail("jrnasf@optonline.net").Id;
            userManager.AddToRole(userIdUser, "Household User");

            context.Types.AddOrUpdate(t => t.Id,
               new Models.Type() { Id = 1, Name = "Income" },
               new Models.Type() { Id = 2, Name = "Expense" }

           );

            context.Categories.AddOrUpdate(c => c.Id,
                new Models.Category() { Id = 1, Name = "Rent/Utilities" },
                new Models.Category() { Id = 2, Name = "Food" },
                new Models.Category() { Id = 3, Name = "Entertainment" },
                new Models.Category() { Id = 4, Name = "Travel" },
                new Models.Category() { Id = 5, Name = "Commuting Cost" },
                new Models.Category() { Id = 6, Name = "Misc" }
            );

        }
    }
}

