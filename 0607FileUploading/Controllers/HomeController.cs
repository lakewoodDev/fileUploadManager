using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Uploading.Data;
using _0607FileUploading.Models;

namespace _0607FileUploading.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            var viewModel = new ViewModel();
            viewModel.Top5Recent = fum.GetTop5RecentlyAdded();
            viewModel.Top5Viewed = fum.GetTop5Viewed();
            viewModel.Message = (string)TempData["message"];
            viewModel.ErrorMessage = (string)TempData["errorMessage"];
            if (User.Identity.IsAuthenticated)
            {
                viewModel.User = User.Identity.Name;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(HttpPostedFileBase image)
        {
            var ext = Path.GetFileName(image.FileName);
            var g = Guid.NewGuid();
            image.SaveAs(Server.MapPath("~/Images/") + g + ext);
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            fum.AddImage(g + ext, DateTime.Now);
            TempData["message"] = "Image Uploaded Successfully";
            return Redirect("/home/index");
        }

        public ActionResult GetCount(int id)
        {
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            var count = fum.GetCountById(id);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewImage(int? imageid)
        {
            if (imageid == null)
            {
                return Redirect("/home/index");
            }
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            fum.UpdateCountById(imageid.Value);
            Image i = fum.GetImageById(imageid);
            return View(i);
        }

        [HttpPost]
        public ActionResult GenerateLink(int? id, DateTime exp)
        {
            if (id == null)
            {
                return Redirect("/home/index");
            }
            var g = Guid.NewGuid();
            var link = "/gotoimage?imagelink=" + g;
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            fum.GenerateLink(new Link
            {
                Expiration = exp,
                ShareLink = g.ToString(),
                ImageId = id
            });
            return Json(link);
        }

        public ActionResult GotoImage(string imagelink)
        {
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            var id = fum.GetIdByLink(imagelink);
            if (id == null)
            {
                TempData["errorMessage"] = "Sorry that link is null";
                return Redirect("/home/index");
            }

            if (fum.CheckExpiration(id.Value, imagelink) <= DateTime.Now)
            {
                TempData["errorMessage"] = "Sorry that link is expired";
                fum.DeleteLink(id.Value, imagelink);
                return Redirect("/home/index");
            }
            Image i = fum.GetImageById(id);
            fum.UpdateCountById(id.Value);
            return View(i);
        }

        public ActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["errorMessage"] = User.Identity.Name + " please log out to log in with another account";
                return Redirect("/home/index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(string username, string password)
        {
            var user = new UserManager(Properties.Settings.Default.Constr);
            var check = user.Login(username, password);
            if (check == null)
            {
                TempData["errorMessage"] = "That email or password was incorrect";
                return View((string)TempData["errorMessage"]);
            }
            FormsAuthentication.SetAuthCookie(username, true);
            return Redirect("/home/index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string passwordMatch)
        {
            var manager = new UserManager(Properties.Settings.Default.Constr);
            if (manager.CheckUserAvailability(username) != null)
            {
                TempData["errorMessage"] = "That username is taken";
                return View(TempData["errorMessage"]);
            }
            if (password != passwordMatch)
            {
                TempData["errorMessage"] = "Passwords don't match";
                return View(TempData["errorMessage"]);
            }
            manager.AddUser(username, password);
            TempData["Message"] = "You may now sign in using your username and password";
            return Redirect("/home/signin");
        }

        public ActionResult CheckIfAvailable(string username)
        {
            var manager = new UserManager(Properties.Settings.Default.Constr);
            return Json(manager.CheckUserAvailability(username), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/home/index");
        }


        [HttpPost]
        public ActionResult Like(int id)
        {
            var fum = new FileUploadingManager(Properties.Settings.Default.Constr);
            fum.Like(id, User.Identity.Name);
            return Json(null);
        }


    }
}