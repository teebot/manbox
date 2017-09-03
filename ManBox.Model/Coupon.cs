//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManBox.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Coupon
    {
        public Coupon()
        {
            this.SubscriptionDeliveries = new HashSet<SubscriptionDelivery>();
        }
    
        public int CouponId { get; set; }
        public string Code { get; set; }
        public Nullable<decimal> Percentage { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int NumberAvailable { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public bool Enabled { get; set; }
    
        public virtual ICollection<SubscriptionDelivery> SubscriptionDeliveries { get; set; }
    }
}