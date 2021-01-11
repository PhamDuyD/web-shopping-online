using PagedList;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/User
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var model = db.Users.Where(x => x.Type == null).OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult adminLogin(User model)
        {
            var result = db.Users.Where(x => x.Account == model.Account && x.Password == model.Password && x.Type == 1 ).ToList();
            if(result.Count > 0)
            {
                Session["admin_login"] = model.Account;
                return Redirect("/Admin/Home");
            }
            else
            {
                TempData["error_login"] = "Tài khoản hoặc mật khẩu không chính xác";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {
            Session["admin_login"] = null;
            return RedirectToAction("Login");
        }

        //Xóa tài khoản
        public JsonResult Delete(long ID)
        {
            var user = db.Users.Find(ID);
            db.Users.Remove(user);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        public JsonResult changeStatus(long ID)
        {
            var user = db.Users.Find(ID);
            if (user.Status == true)
                user.Status = false;
            else
                user.Status = true;
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
    }
}