using ShoppingOnline.Areas.Admin.Models;
using ShoppingOnline.Models.Business;
using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class InwardController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/Inward
        public ActionResult Index()
        {
            
            ViewBag.lstInWard = db.Inwards.OrderByDescending(x => x.Createdate).ToList();
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddInward(Inward entity)
        {
            var res = new InwardBusiness().addInward(entity);
            if (res)
            {
                var cart = (List<CartDTO>)Session["add_inward"];
                foreach (var item in cart)
                {
                    var detail = new Inward_Detail();
                    detail.Inward_ID = db.Inwards.Max(x => x.ID);
                    detail.Product_ID = item.Product.ID;
                    detail.Quantity = item.Quantity;
                    
                    if (item.Product.Price != null)
                    {
                        detail.Price = item.Product.Price;
                        detail.Amount = (int)item.Product.Price * item.Quantity;
                    }
                    else
                    {
                        detail.Price = item.Product.Promotion_Price;
                        detail.Amount = (int)item.Product.Promotion_Price * item.Quantity;
                    }
                    new InwardBusiness().addInward_Detail(detail);

                    //Cộng tồn kho
                    new ProductBusiness().AddQuantity(item.Product.ID, item.Quantity);
                }
                Session["add_order"] = null;
                TempData["add_success"] = "Nhập kho thành công.";
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Add");
            }
        }


        public JsonResult addInwardProduct(string product_name, int quantity)
        {
            var product = new ProductBusiness().searchProduct(product_name);
            var cart = Session["add_inward"];
            if (cart != null)//Nếu giỏ đã chứa sản phẩm
            {
                var list = (List<CartDTO>)cart;
                if (list.Exists(x => x.Product.ID == product.ID))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == product.ID)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //Tạo đối tượng mới
                    var item = new CartDTO();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
            }
            else//nếu giỏ hàng trống
            {
                var item = new CartDTO();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<CartDTO>();
                list.Add(item);

                Session["add_inward"] = list;
            }
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);

        }


        //Xóa từng sản phẩm
        public JsonResult Delete_InwardProduct(long ID)
        {
            var cartSec = (List<CartDTO>)Session["add_inward"];
            cartSec.RemoveAll(x => x.Product.ID == ID);
            Session["add_inward"] = cartSec;
            return Json(new
            {
                status = true
            });
        }

        //Sửa số lượng sp trong giỏ hàng
        public JsonResult Edit(long ID, int Quantity)
        {
            var productSec = (List<CartDTO>)Session["add_inward"];

            foreach (var item in productSec)
            {
                if (item.Product.ID == ID)
                {
                    item.Quantity = Quantity;
                }

            }

            Session["add_inward"] = productSec;
            return Json(new
            {
                status = true
            });
        }


        //Chi tiết nhập kho
        public ActionResult Inward_Detail(long ID)
        {
            var model = from detail in db.Inward_Detail
                        join pro in db.Products on detail.Product_ID equals pro.ID
                        where detail.Inward_ID == ID
                        select new InwardDTO()
                        {
                            InwardDetail_ID = detail.ID,
                            Product_Image = pro.Image,
                            Product_Name = pro.Product_Name,
                            Quantity = detail.Quantity,
                            Price = detail.Price,
                            Amount = detail.Amount
                        };
            ViewBag.Inward = db.Inwards.Find(ID);
            return View(model.ToList());
        }
    }
}