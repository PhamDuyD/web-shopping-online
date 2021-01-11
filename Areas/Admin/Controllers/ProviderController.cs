using PagedList;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class ProviderController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/Provider
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var model = db.Providers.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
            return View(model);
        }


        //Xóa tài khoản
        public JsonResult Delete(long ID)
        {

            try
            {
                var provider = db.Providers.Find(ID);
                db.Providers.Remove(provider);
                db.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }

        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addProvider(Provider entity)
        {
            try
            {
                var maxid = db.Providers.Max(x => x.ID);
                var prv = new Provider();
                prv.Code = "VENDOR" + (maxid + 1).ToString() + DateTime.Now.ToString("ddMMyyyy");
                prv.Name = entity.Name;
                prv.Description = entity.Description;
                prv.Metatitle = Str_Metatitle(entity.Name);
                db.Providers.Add(prv);
                db.SaveChanges();
                TempData["add_success"] = "Thêm nhà cung cấp thành công";
                return RedirectToAction("Index");

            }
            catch
            {
                TempData["add_success"] = "Thêm nhà cung cấp KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult editProvider(Provider entity)
        {
            try
            {
                var prv = db.Providers.Find(entity.ID);
                prv.Name = entity.Name;
                prv.Description = entity.Description;
                prv.Metatitle = Str_Metatitle(entity.Name);
                db.SaveChanges();
                TempData["add_success"] = "Sửa nhà cung cấp thành công";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["add_success"] = "Sửa nhà cung cấp KHÔNG thành công";
                return RedirectToAction("Index");
            }
        }

        public JsonResult GetProviderByID(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var provider = db.Providers.Find(ID);
            return Json(provider, JsonRequestBehavior.AllowGet);
        }


        public string Str_Metatitle(string str)
        {
            string[] VietNamChar = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ:/"
            };
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            string str1 = str.ToLower();
            string[] name = str1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string meta = null;
            //Thêm dấu '-'
            foreach (var item in name)
            {
                meta = meta + item + "-";
            }
            return meta;
        }
    }
}