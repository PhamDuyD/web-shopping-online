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
    
    public partial class Inward
    {
        public Inward()
        {
            this.Inward_Detail = new HashSet<Inward_Detail>();
        }
    
        public long ID { get; set; }
        public Nullable<long> TotalQuantity { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<long> Provider_ID { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public string RecivedInfo { get; set; }
    
        public virtual ICollection<Inward_Detail> Inward_Detail { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
