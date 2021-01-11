using ShoppingOnline.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingOnline.Models.Business
{
    public class OrderBusiness
    {
        private Shopping_OnlineEntities db = new Shopping_OnlineEntities();

        //thêm hóa đơn
        public bool addOrder(Order entity)
        {
            try
            {
                var order = new Order();
                var maxid = db.Orders.Max(x => x.ID);
                order.Order_Code = "ORDER" + (maxid + 1).ToString() + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year;
                order.CustomerName = entity.CustomerName;
                order.CustomerAddress = entity.CustomerAddress;
                order.CustomerPhone = entity.CustomerPhone;
                order.Email = entity.Email;
                order.TotalMoney = entity.TotalMoney;
                order.TotalQuantity = entity.TotalQuantity;
                order.CreatedDate = DateTime.Now;
                order.Status = false;

                db.Orders.Add(order);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //thêm chi tiết hóa đơn
        public void addOrder_Detail(Order_Detail entity)
        {
                db.Order_Detail.Add(entity);
                db.SaveChanges();
        }

        //lấy Order ID lớn nhất
        public long findMaxID()
        {
            return db.Orders.Max(x => x.ID);
        }
    }
}