using Newtonsoft.Json;
using PagedList;
using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/Product
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var query = from pro in db.Products
                        join pvd in db.Providers on pro.Provider_ID equals pvd.ID
                        select new ProductDTO()
                        {
                            ID = pro.ID,
                            Product_Name = pro.Product_Name,
                            Product_Code = pro.Product_Code,
                            Promotion_Price = pro.Promotion_Price,
                            Price = pro.Price,
                            Image = pro.Image,
                            Quantity = pro.Quantity,
                            Provider_Name = pvd.Name
                        };
            return View(query.OrderByDescending(x => x.ID).ToPagedList(page, pageSize));
        }


        // GET: Admin/Product/Create
        public ActionResult Add()
        {
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstProvider = db.Providers.ToList();
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                long maxid = db.Products.Max(x => x.ID);
                var pro = new Product();
                pro.Product_Name = entity.Product_Name;
                pro.Product_Code = "PRO" + (maxid + 1).ToString() + DateTime.Now.ToString("ddMMyyyy");
                pro.Metatitle = Str_Metatitle(entity.Product_Name);
                pro.Promotion_Price = entity.Promotion_Price;
                pro.Price = entity.Price;
                pro.Provider_ID = entity.Provider_ID;
                pro.Category_ID = entity.Category_ID;
                pro.Desription = entity.Desription;
                pro.Quantity = 0;
               
                //Thêm hình ảnh
                var path = Path.Combine(Server.MapPath("~/Assets/images/product"), Image.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extensionName = Path.GetExtension(Image.FileName);
                    string filename = Image.FileName +DateTime.Now.ToString("ddMMyyyy") + extensionName;
                    path = Path.Combine(Server.MapPath("~/Assets/images/product/"), filename);
                    Image.SaveAs(path);
                    pro.Image = filename;
                }
                else
                {
                    Image.SaveAs(path);
                    pro.Image = Image.FileName;
                }

                db.Products.Add(pro);
                db.SaveChanges();
                TempData["add_success"] = "Thêm sản phẩm thành công.";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Add");
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(long ID)
        {
            ViewBag.product = db.Products.Find(ID);
            ViewBag.lstCategory = db.Categories.ToList();
            ViewBag.lstProvider = db.Providers.ToList();
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product entity, HttpPostedFileBase Image)
        {
            try
            {
                var pro = db.Products.Find(entity.ID);
                pro.Product_Name = entity.Product_Name;
                pro.Metatitle = Str_Metatitle(entity.Product_Name);
                pro.Promotion_Price = entity.Promotion_Price;
                pro.Price = entity.Price;
                pro.Provider_ID = entity.Provider_ID;
                pro.Category_ID = entity.Category_ID;
                pro.Desription = entity.Desription;

                try
                {
                    if (pro.Image != Image.FileName)
                    {
                        //Xóa file cũ
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/images/product"), pro.Image));
                        //Thêm hình ảnh
                        var path = Path.Combine(Server.MapPath("~/Assets/images/product"), Image.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            string extensionName = Path.GetExtension(Image.FileName);
                            string filename = Image.FileName + DateTime.Now.ToString("ddMMyyyy") + extensionName;
                            path = Path.Combine(Server.MapPath("~/Assets/images/product/"), filename);
                            Image.SaveAs(path);
                            pro.Image = filename;
                        }
                        else
                        {
                            Image.SaveAs(path);
                            pro.Image = Image.FileName;
                        }
                    }
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    db.SaveChanges();
                    TempData["add_success"] = "Sửa sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                
                

                
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Product/Delete/5
        public JsonResult Delete(long ID)
        {
            try
            {
                var product = db.Products.Find(ID);
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Assets/images/product"), product.Image));

                db.Products.Remove(product);
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

        //Chuyển tên sản phẩm thành metatitle
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
