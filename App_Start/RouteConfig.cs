﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoppingOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "order product",
            url: "dat-hang",
            defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional }
        );

            routes.MapRoute(
             name: "product search",
             url: "tim-kiem",
             defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
             namespaces: new[] { "ShoppingOnline.Controllers" }
         );

            routes.MapRoute(
             name: "product detail",
             url: "san-pham/{Metatitle}-{ID}",
             defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
             namespaces: new[] { "ShoppingOnline.Controllers" }
         );

            routes.MapRoute(
              name: "list cart",
              url: "gio-hang",
              defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "Change password",
              url: "doi-mat-khau/{ID}",
              defaults: new { controller = "User", action = "changPass", id = UrlParameter.Optional }
          );

            routes.MapRoute(
               name: "sign out",
               url: "dang-ky",
               defaults: new { controller = "User", action = "Register", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "log in",
                url: "dang-nhap",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ShoppingOnline.Controllers" }
            );
        }
    }
}
