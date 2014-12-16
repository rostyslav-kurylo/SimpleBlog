using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SimpleBlog.Models
{
    public class SimpleBlogDbInitializer: DropCreateDatabaseIfModelChanges<SimpleBlogDbContext>
    {
        protected override void Seed(SimpleBlogDbContext context)
        {
            //--Initial Administrator's credentials---------------------------
            string defaultUserName = "Administrator";
            string defaultPassword = "password";
            string defaultEmail = "admin@host.com";
            //----------------------------------------------------------------
            Admin admin = new Admin();

            admin.UserName = defaultUserName;
            admin.PasswordHash = Controllers.AccountController.CreatePasswordHash(defaultPassword);
            admin.Email = defaultEmail;

            context.Admins.Add(admin);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}