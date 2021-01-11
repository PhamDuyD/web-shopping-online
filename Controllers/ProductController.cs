using PagedList;
using ShoppingOnline.Models.Business;
using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ShoppingOnline.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(long ID)
        {
            var model = new ProductBusiness();
            var product = model.getProductDetail(ID);
            var serial_type = model.GetSerial_TypesByID(ID);
            ViewBag.product_detail = product;
            ViewBag.serial_type_product = serial_type;
            var lstColor = new List<Serial_TypeDTO>();
            bool isSize = false;
            foreach(var item in serial_type)
            {
                if(item.Color != null)
                {
                    var ser = new Serial_TypeDTO();
                    ser.Color = item.Color;
                    ser.ID = item.ID;
                    lstColor.Add(ser);
                }

                if(item.Size != null)
                {
                    isSize = true;
                }

            }

            ViewBag.lstColor = lstColor;
            ViewBag.isSize = isSize;
            ViewBag.lstProductRecommend_1 = model.getProductRecommend();
            ViewBag.lstProductRecommend_2 = model.getProductRecommend();
            ViewBag.lstSameCategory = model.getProductByCategoryID(product.Category_ID);
            ViewBag.lstReview = model.getReview(ID);
            return View();
        }

        public JsonResult addReview(string json_review)
        {
            var JsonReview = new JavaScriptSerializer().Deserialize<List<ReviewDTO>>(json_review);
            var review = new Review();
            foreach (var item in JsonReview)
            {
                review.Content = item.Content;
                review.Rating = item.Rating;
                review.CreatedDate = DateTime.Now;
                review.User_ID = item.User_ID;
                review.Product_ID = item.Product_ID;
                review.Status = true;
            }
            
            var res = new ProductBusiness().addReview(review);
            if (res)
            {
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }

        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        public JsonResult ListName(string q)
        {
            var query = db.Products.Where(x => x.Product_Name.Contains(q)).Select(x => x.Product_Name);
            //var query = from pro in db.Products
            //            where pro.Product_Name.Contains(q)
            //            select new
            //            {
            //                pro.Product_Name,
            //                pro.Image
            //            };
            return Json(new
            {
                data = query,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string keyword, int page = 1, int pagesize = 6)
        {
            string[] key = keyword.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var product_name = new List<Product>();//Tìm theo tên sản phẩm
            foreach (var item in key)
            {
                product_name = (from b in db.Products
                          where b.Product_Name.Contains(item)
                          select b).ToList();
            }
            ViewBag.KeyWord = keyword;
            return View(product_name.ToPagedList(page, pagesize));
        }


    }
}