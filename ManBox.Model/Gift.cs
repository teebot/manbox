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
    
    public partial class Gift
    {
        public Gift()
        {
            this.Subscriptions = new HashSet<Subscription>();
        }
    
        public int GiftId { get; set; }
        public string GuestName { get; set; }
        public string GiftMessage { get; set; }
    
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}