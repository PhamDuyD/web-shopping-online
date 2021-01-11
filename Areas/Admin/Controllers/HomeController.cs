using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["admin_login"] == null)
                return Redirect("/Admin/User/Login");
            //thống kê sp bán ra
            var sell = from detail in db.Order_Detail
                       join order in db.Orders on detail.Order_ID equals order.ID
                       where order.Status == true
                       select new
                       {
                           detail.Quantity,
                           detail.Amount
                       };
            ViewBag.Count_sell = sell.Select(x => x.Quantity).Sum();

            //Thống kê doanh thu
            ViewBag.Count_money = sell.Select(x => x.Amount).Sum();

            //Thống kê đơn đặt hàng
            ViewBag.Count_Order = db.Orders.ToList().Count();


            //Thống kê tồn ko
            ViewBag.quantity_product = db.Products.Where(x => x.Quantity > 0).OrderByDescending(x => x.Quantity).Take(10).ToList();

            //Thống kê lượng hàng bán ra
            var lstproduct_id = db.Order_Detail.Select(x => x.Product_ID).Distinct();
            var listProduct_sell = new List<Order_DetailDTO>();
            foreach(var item in lstproduct_id)
            {
                var product = db.Products.Find(item);
                var productsell = new Order_DetailDTO();
                productsell.Product_Name = product.Product_Name;
                productsell.Quantity = 0;
                productsell.Amount = 0;
                foreach(var jtem in db.Order_Detail.Where(x => x.Product_ID == item))
                {
                    productsell.Quantity += jtem.Quantity;
                    productsell.Amount += jtem.Amount;
                }
                listProduct_sell.Add(productsell);
            }
            ViewBag.product_sell = listProduct_sell.OrderByDescending(x => x.Quantity).Take(10).ToList();

            //Thống kê đánh giá hôm nay
            ViewBag.Review_today = db.Reviews.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê đơn đạt hàng hôm nay
            ViewBag.Order_today = db.Orders.Where(x => DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)).Count();

            //Thống kê user đã đăng ký
            ViewBag.user = db.Users.Count();

            //Thống kê nhà cung cấp
            ViewBag.provider = db.Providers.Count();

            //Thống kê nhập kho
            var input = (from detail in db.Inward_Detail
                        join inward in db.Inwards on detail.Inward_ID equals inward.ID
                        where DbFunctions.TruncateTime(inward.Createdate) == DbFunctions.TruncateTime(DateTime.Now)
                        select new
                        {
                            detail.Product_ID
                        });
            ViewBag.inward = input.Select(x => x.Product_ID).Distinct().Count();
            return View();
        }
    }
}