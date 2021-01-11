using ShoppingOnline.Models.Business;
using ShoppingOnline.Models.DTO;
using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Controllers
{
    public class CartController : Controller
    {

        private const string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            //hiển thị giỏ        
            var cart = Session[CartSession];
            var list = new List<CartDTO>();

            if (cart != null)
            {
                list = (List<CartDTO>)cart;
            }
            return View(list);
        }


        public JsonResult AddCart(long product_ID, int quantity, string color = null, string size = null)
        {
            var product = new ProductBusiness().findID(product_ID);
            var cart = Session[CartSession];
            if (cart != null)//Nếu giỏ đã chứa sản phẩm
            {
                var list = (List<CartDTO>)cart;
                if (list.Exists(x => x.Product.ID == product_ID))//nếu giỏ đã chứa sản phẩm có ID = BookID
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ID == product_ID)
                        {
                            if(item.Quantity + quantity > item.actual_number)
                            {
                                return Json(new
                                {
                                    status = false
                                }, JsonRequestBehavior.AllowGet);
                            }
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
                    item.actual_number = (int)product.Quantity;//Lấy lượng hàng tồn
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
                item.actual_number = (int)product.Quantity;
                if (color != null)
                    item.Color = color;
                if (size != null)
                    item.Size = size;
                var list = new List<CartDTO>();
                list.Add(item);

                Session[CartSession] = list;
            }
            return Json(new
            {
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        //Xóa từng sản phẩm
        public JsonResult Delete(long id)
        {
            var cartSec = (List<CartDTO>)Session[CartSession];
            cartSec.RemoveAll(x => x.Product.ID == id);
            Session[CartSession] = cartSec;
            return Json(new
            {
                status = true
            });
        }

        //Sửa số lượng sp trong giỏ hàng
        public JsonResult Edit(long product_ID, int quantity)
        {
            var productSec = (List<CartDTO>)Session[CartSession];

            foreach (var item in productSec)
            {
                if (item.Product.ID == product_ID)
                {
                    item.Quantity = quantity;
                }

            }

            Session[CartSession] = productSec;
            return Json(new
            {
                status = true
            });
        }


        public ActionResult Payment()
        {
            return View();
        }

        //Thanh toán hóa đơn
        [HttpPost]
        public ActionResult OrderPayment(Order order)
        {
            var res = new OrderBusiness().addOrder(order);
            if (res)
            {
                var cart = (List<CartDTO>)Session[CartSession];
                foreach(var item in cart)
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
                Session[CartSession] = null;
                return RedirectToAction("Order_success");
            }
            else
            {
                return RedirectToAction("Payment");
            }
        }

        public ActionResult Order_success()
        {
            return View();
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
                                "Họ và tên: <span style='color: blue'>"+ order.CustomerName +"</span>.</p>" +
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