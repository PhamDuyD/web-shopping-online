using PagedList;
using ShoppingOnline.Models.Business;
using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();
        // GET: Admin/Order
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var model = db.Orders.OrderByDescending(x => DbFunctions.TruncateTime(x.CreatedDate)).ToPagedList(page, pageSize);
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        public JsonResult autocompleteProduct(string q)
        {
            var product = new ProductBusiness().searchProduct(q);
                            
            return Json(new
            {
                data = product,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult addOrderProduct(string product_name, int quantity, string color, string size)
        {
            var product = new ProductBusiness().searchProduct(product_name);
            var cart = Session["add_order"];
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
                    if (color != null)
                        item.Color = color;
                    if (size != null)
                        item.Size = size;
                    list.Add(item);
                }
            }
            else//nếu giỏ hàng trống
            {
                var item = new CartDTO();
                item.Product = product;
                item.Quantity = quantity;
                if (color != null)
                    item.Color = color;
                if (size != null)
                    item.Size = size;
                var list = new List<CartDTO>();
                list.Add(item);

                Session["add_order"] = list;
            }
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);

        }


        //Xóa từng sản phẩm
        public JsonResult Delete_OrderProduct(long ID)
        {
            var cartSec = (List<CartDTO>)Session["add_order"];
            cartSec.RemoveAll(x => x.Product.ID == ID);
            Session["add_order"] = cartSec;
            return Json(new
            {
                status = true
            });
        }

        //Sửa số lượng sp trong giỏ hàng
        public JsonResult Edit(long ID, int Quantity)
        {
            var productSec = (List<CartDTO>)Session["add_order"];

            foreach (var item in productSec)
            {
                if (item.Product.ID == ID)
                {
                    item.Quantity = Quantity;
                }

            }

            Session["add_order"] = productSec;
            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public ActionResult OrderPayment(Order order)
        {
            var res = new OrderBusiness().addOrder(order);
            if (res)
            {
                var cart = (List<CartDTO>)Session["add_order"];
                foreach (var item in cart)
                {
                    var od = new Order_Detail();
                    od.Product_ID = item.Product.ID;
                    od.Quantity = item.Quantity;
                    od.Size = item.Size;
                    od.Color = item.Color;
                    od.Order_ID = new OrderBusiness().findMaxID();
                    if (item.Product.Price != null)
                    {
                        od.Price = item.Product.Price;
                        od.Amount = (int)item.Product.Price * item.Quantity;
                    }
                    else
                    {
                        od.Price = item.Product.Promotion_Price;
                        od.Amount = (int)item.Product.Promotion_Price * item.Quantity;
                    }
                    new OrderBusiness().addOrder_Detail(od);
                }
                //SendEmail(order.Email, order);
                Session["add_order"] = null;
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Add");
            }
        }

        public ActionResult Edit_Order(long ID)
        {
            var order = from order_detail in db.Order_Detail
                        join or in db.Orders on order_detail.Order_ID equals or.ID
                        join pro in db.Products on order_detail.Product_ID equals pro.ID
                        where order_detail.Order_ID == ID
                        select new CartDTO()
                        {
                            Product = pro,
                            Quantity = (int)order_detail.Quantity,
                            Color = order_detail.Color,
                            Size = order_detail.Size,
                            OrderDetail_ID = order_detail.ID
                        };
            ViewBag.order_edit = order.ToList();
            ViewBag.Order_info = db.Orders.Find(ID);
            return View();
        }

        [HttpPost]
        public ActionResult Edit_Order(Order entity)
        {
            var order = db.Orders.Find(entity.ID);
            order.CustomerName = entity.CustomerName;
            order.CustomerAddress = entity.CustomerAddress;
            order.CustomerPhone = entity.CustomerPhone;
            order.Email = entity.Email;
            order.TotalMoney = entity.TotalMoney;
            order.TotalQuantity = entity.TotalQuantity;

            db.SaveChanges();
            return RedirectToAction("Index");

        }


        //Xóa đơn hàng
        public JsonResult Delete(long ID)
        {
            try
            {
                var order = db.Orders.Find(ID);
                db.Orders.Remove(order);
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

        //kích hoạt đã thanh toán
        public JsonResult changStatus(long ID)
        {
            var order = db.Orders.Find(ID);
            order.Status = true;

            //Trừ tồn kho
            var lst = db.Order_Detail.Where(x => x.Order_ID == ID);
            foreach(var item in lst)
            {
                var product = db.Products.Find(item.Product_ID);
                if(item.Quantity > product.Quantity)//Kiểm tra lượng hàng có còn k??
                {
                    return Json(new
                    {
                        status = false
                    });
                }
                else
                {
                    new ProductBusiness().Subtract_Quantity((int)item.Product_ID, (int)item.Quantity);
                }
            }

            db.SaveChanges();
            return Json(new
            {
                status = true
            });
        }

        //Chi tiết đơn hàng
        public ActionResult Order_Detail(long ID)
        {
            var query = from order_detail in db.Order_Detail
                        join pro in db.Products on order_detail.Product_ID equals pro.ID
                        where order_detail.Order_ID == ID
                        select new Order_DetailDTO()
                        {
                            ID = order_detail.ID,
                            Product_Name = pro.Product_Name,
                            Quantity = order_detail.Quantity,
                            Size = order_detail.Size,
                            Color = order_detail.Color,
                            Price = order_detail.Price,
                            Amount = order_detail.Amount
                        };
            ViewBag.order = db.Orders.Find(ID);
            return View(query.OrderByDescending(x => x.ID).ToList());
        }

        //lấy serial_type
        public JsonResult getSerial(string product_name)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var product = new ProductBusiness().searchProduct(product_name);
            var query = db.Serial_Type.Where(x => x.Product_ID == product.ID).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        //send email
        public void SendEmail(string address, Order order)
        {
            string email = "superjunior25251325@gmail.com";
            string password = "HungCong15021996";

            var loginInfo = new NetworkCredential(email, password);
            var msg = new MailMessage();
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            msg.IsBodyHtml = true;
            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = "E-Shopper - Thông báo đơn hàng";
            msg.Body = "<div style='width: 600px; height: 100 %; margin: auto'>";
            msg.Body += "<div style ='margin:0 250px'>";
            //msg.Body += "<a style='color:#871fff;text-decoration:none' target='blank'" +
            //                    "style='vertical-align:middle;width:45%;height:auto;max-width:100%;border-width:0'" +
            //                    "class='CToWUd'>" +
            //                        "<img src='https://lh3.googleusercontent.com/1LIw-IQwGPyWHez1tuBM7QwgHKAhvGfnpKjTsT1wf0Onk4ADg20r2PMwCFOXb9-getwcPhUS0zrwRQJwBX4fn1uGkNM-Rf7vTS1ONms0SLGL6Vi6wd8gnu3bBQViLa6Nxp-K_k3f7n_5NeI1j57MaT-HURsxPo70zQfQm36axC62bQUlvNUqj03KRi_lmJVAIPVvXIx7ZtfJaVgG4evv8GexGHL5ngmVuR_OG7OZdzpr8HuzJ9weMsoR4Y7EghQ9odxkgOtyShkSLUnEWCkvXsIrDbdGYsI-mPMox-fVpDJXo80RDdtiBy9lH_9bkQ-O5fCWGUvc1RFjVrn50IxGaBwdRuFD1Yzj3B6Z_ejZDhILGX_ptYOPjeAUGFngSV5JfT4TN22v8GisY7v8dLOhG6uW8bg8BgGynua5PA1gxoNQRGyjmGDREbV2f6loMv4xDLMrlf4ND2gf04Iapxh8_AePp9EHBzORmCVIB2ZSGMYdARYAl61mQnXBjBesdMdAGVOgIzL4LoADQtjQWRwOC5re_R3puuYJLLAHaF1rUrDhKgaxir0_ilGhY22o1mHmlEphXGyDuAdKV3plJgDp57J7wzorQhK8A69VR1yjxe9xY8rJin3miMMlYRItmaGoDkzB-y8ZmKdTlMCzyUF3YYiAwi5Z9Tejv_UwNNmjrS1qUsGcISZKFkZuSyA=w235-h66-no'" +
            //                        "style='vertical-align:middle;border-width:0'" +
            //                        "class='CToWUd'>" +
            //                "</a>";
            msg.Body += "<div>" +
                            "<div style='font-weight: normal;line-height: 48px'>" +
                                "<h1>Thông báo đơn hàng</h1>" +
                            "</div>" +
                        "</div>";
            msg.Body += "<div>" +
                            "<p style='font-size:16px;margin-bottom:16px;line-height:24px'> " +
                                "Họ và tên: <span style='color: blue'>" + order.CustomerName + "</span>.</p>" +
                             "<p style='font-size:16px;margin-bottom:16px;line-height:24px'> " +
                                "Địa chỉ: <span style='color: blue'>" + order.CustomerAddress + "</span>.</p>" +
                             "<p style='font-size:16px;margin-bottom:16px;line-height:24px'> " +
                                "Số điện thoại: <span style='color: blue'>" + order.CustomerPhone + "</span>.</p>" +
                             "<p style='font-size:16px;margin-bottom:16px;line-height:24px'> " +
                                "Tổng tiền đơn hàng: <span style='color: blue'>" + order.TotalMoney.Value.ToString("N0") + "</span>.</p>" +
                            "<p style='font-size:16px;margin-bottom:16px;line-height:24px'> Cảm ơn bạn đã đặt hàng tại E-Shopper </p>" +
                        "</div>";
            msg.Body += "<p style='font-family: 'Roboto', sans-serif;'>" +
                            "<span style='line-height:24px;font-size:16px'> Trang mua sắm online E-Shopper,</span><br>" +
                            "<span style='line-height:24px;font-size:16px'> Dành cho bạn những sản phẩm với chất lượng tốt nhất</span >" +
                        "</p>" +

                        "<hr style='margin:40px 0 20px 0;display:block;height:1px;border:0;border-top:1px solid #c4cdd5;padding:0'>" +
                        "<footer style='font-family: 'Roboto', sans-serif; margin-bottom:40px'>" +
                            "<span style='color:#919eab;line-height:28px;font-size:12px'> E-SHOPPER</span><br>" +
                            "<span style='color:#919eab;line-height:28px;font-size:12px'> Mail thông báo đơn hàng. Do not reply </span ><br >";
            msg.Body += "</div>";
            msg.Body += "</div>";
            msg.BodyEncoding = UTF8Encoding.UTF8;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);

        }
    }
}