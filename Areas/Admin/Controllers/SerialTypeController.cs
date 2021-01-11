using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class SerialTypeController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();

        // GET: Admin/SerialType
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SerialType_Product(long ID)
        {
            ViewBag.lstSerial = db.Serial_Type.Where(x => x.Product_ID == ID).ToList();
            ViewBag.product_ID = ID;
            return View();
        }


        public JsonResult Delete_Serial(long ID)
        {
            var serial = db.Serial_Type.Find(ID);
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/images/product-detail"), serial.Image));

            db.Serial_Type.Remove(serial);
            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        //Lấy chi tiết serial_type
        public ActionResult Serial_Detail(long ID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var serial = db.Serial_Type.Single(x => x.ID == ID);
            //string json = JsonConvert.SerializeObject(serial);
            return Json(serial, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult addSerial(Serial_Type entity, HttpPostedFileBase Image)
        {
            var ID = entity.Product_ID;
            try
            {
                var serial = new Serial_Type();
                serial.Color = entity.Color;
                serial.Size = entity.Size;
                serial.Product_ID = entity.Product_ID;
                var path = Path.Combine(Server.MapPath("~/Assets/images/product-detail"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/images/product-detail/"), filename);
                    Image.SaveAs(path);
                    serial.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    serial.Image = Image.FileName;
                }

                db.Serial_Type.Add(serial);
                db.SaveChanges();
                TempData["add_success"] = "Thêm chi tiết sản phẩm thành công.";
                return Redirect("/Admin/SerialType/SerialType_Product/" + ID);
            }
            catch
            {
                TempData["add_success"] = "Đã có lỗi xảy ra";
                return Redirect("/Admin/SerialType/SerialType_Product/" + ID);
            }
        }

        [HttpPost]
        public ActionResult editSerial(Serial_Type entity, HttpPostedFileBase Image)
        {
            var ID = entity.Product_ID;
            try
            {
                var serial = db.Serial_Type.Find(entity.ID);
                serial.Color = entity.Color;
                serial.Size = entity.Size;
                serial.Product_ID = entity.Product_ID;

                try
                {
                    if ( serial.Image != Image.FileName)
                    {
                        //Xóa file cũ
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/images/product-detail"), serial.Image));
                        //Thêm hình ảnh
                        var path = Path.Combine(Server.MapPath("~/Assets/images/product-detail"), Image.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            string extensionName = Path.GetExtension(Image.FileName);
                            string filename = Path.GetFileNameWithoutExtension(Image.FileName) + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                            path = Path.Combine(Server.MapPath("~/Assets/images/product-detail/"), filename);
                            Image.SaveAs(path);
                            serial.Image = filename;
                        }
                        else
                        {
                            Image.SaveAs(path);
                            serial.Image = Image.FileName;
                        }
                    }
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa chi tiết sản phẩm thành công.";
                    return Redirect("/Admin/SerialType/SerialType_Product/" + ID);
                }
                catch
                {
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa chi tiết sản phẩm thành công.";
                    return Redirect("/Admin/SerialType/SerialType_Product/" + ID);
                }
                
            }
            catch
            {
                TempData["add_success"] = "Đã có lỗi xảy ra";
                return Redirect("/Admin/SerialType/SerialType_Product/" + ID);
            }
        }

    }
}