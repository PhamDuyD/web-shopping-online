//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShoppingOnline.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.Order_Detail = new HashSet<Order_Detail>();
        }
    
        public long ID { get; set; }
        public string Order_Code { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string Email { get; set; }
        public Nullable<decimal> TotalMoney { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> User_ID { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual ICollection<Order_Detail> Order_Detail { get; set; }
        public virtual User User { get; set; }
    }
}
