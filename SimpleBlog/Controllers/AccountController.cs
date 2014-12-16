using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using SimpleBlog.Models;

namespace SimpleBlog.Controllers
{
    public class AccountController : Controller
    {
        private SimpleBlogDbContext db = new SimpleBlogDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName, Password, RememberMe")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userID = ValidateUser(model.UserName, model.Password);
                if (userID > 0)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    Session.Add("UserID", userID);

                    return RedirectToAction("Index", "Admin");
                }
                else
                    ModelState.AddModelError("", "Ooops... Invalid username or password! Try again?");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Remove("UserID");
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }

        /// <summary>
        /// Edit administrator's data
        /// </summary>
        [Authorize]
        public ActionResult Edit()
        {
            Admin admin = db.Admins.Find((int)Session["UserID"]);
            EditViewModel model = new EditViewModel();

            model.UserName = admin.UserName;
            model.Email = admin.Email;
            model.About = admin.About;

            return View(model);
        }

        /// <summary>
        /// Edit administrator's data
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName, Email, Password, ConfirmPassword, About")] EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Admin admin = db.Admins.Find((int)Session["UserID"]);

                admin.UserName = model.UserName ?? admin.UserName;
                admin.Email = model.Email ?? admin.Email;
                admin.PasswordHash = (model.Password != null) ? CreatePasswordHash(model.Password) : admin.PasswordHash;
                admin.About = model.About ?? admin.About;

                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }

            return View(model);
        }

        /// <summary>
        /// Is user's credentials valid
        /// </summary>
        /// <param name="userName">user's name</param>
        /// <param name="password">user's password</param>
        /// <returns>UserID - user is valid; -1 - if not valid</returns>
        private int ValidateUser(string userName, string password)
        {
            using (SimpleBlogDbContext _db = new SimpleBlogDbContext())
            {
                Admin admin = (from a in _db.Admins
                             where a.UserName == userName
                             select a).FirstOrDefault();

                if (admin != null)
                    return VerifyHashedPassword(admin.PasswordHash, password) == true ? admin.ID : -1;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Hashing user's password
        /// </summary>
        /// <param name="password">user's password</param>
        /// <returns></returns>
        public static string CreatePasswordHash(string password, string hashAlgorithm = "sha256")
        {
            return Crypto.HashPassword(password);
        }

        /// <summary>
        /// Is hashed password match
        /// </summary>
        /// <returns>true if match</returns>
        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            return Crypto.VerifyHashedPassword(hashedPassword, password);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}