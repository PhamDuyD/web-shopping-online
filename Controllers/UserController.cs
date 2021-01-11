using ShoppingOnline.Models.Business;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmLogin(User model)
        {
            var res = new UserBusiness().checkLogin(model);
            if (res)
            {
                Session["user"] = new UserBusiness().findUser(model);
                return Redirect("/");
            }
            else
            {
                TempData["error"] = "Tài khoản hoặc mật khẩu không chính xác.";
                return Redirect("/User/Login");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult frmRegister(User entity)
        {
            var res = new UserBusiness().register(entity);
            if (res)
            {
                TempData["error"] = "Bạn vui lòng đăng nhập để sử dụng dịch vụ của E-Shopper.";
                return Redirect("/dang-nhap");
            }
            else
            {
                TempData["error"] = "Đã có lỗi xảy ra, vui lòng thử lại sau.";
                return Redirect("/dang-ky");
            }
        }

        public ActionResult changPass(long ID)
        {
            return View();
        }

        [HttpPost]
        public ActionResult frmchangePass(long ID, string ex_password, string Password)
        {
            if(new UserBusiness().checkPass(ex_password, ID))
            {
                new UserBusiness().changePass(ID, Password);
                TempData["error"] = "Bạn vui lòng đăng nhập lại sau khi đổi mật khẩu.";
                return Redirect("/dang-nhap");
            }
            else
            {
                TempData["error"] = "Mật khẩu cũ không trùng, vui lòng nhập lại.";
                return Redirect("/doi-mat-khau/" + ID);
            }
            
        }

        public ActionResult logout()
        {
            Session["user"] = null;
            return Redirect("/");
        }
    }
}