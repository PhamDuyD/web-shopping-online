using ShoppingOnline.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingOnline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new ProductBusiness();
            ViewBag.LstProduct = model.getAll();
            ViewBag.lstPhuKienThoiTrang = model.getProductBy_PhuKienThoiTrang();
            ViewBag.lstQuanAoNam = model.GetProductBy_QuanAoNam();
            ViewBag.lstQuanApNu = model.GetProductBy_QuanAoNu();
            ViewBag.lstValiBalo = model.GetProductBy_ValiBalo();
            ViewBag.lstProductRecommend_1 = model.getProductRecommend();
            ViewBag.lstProductRecommend_2 = model.getProductRecommend();

            return View();
        }

    }
}