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
    
    public partial class Address
    {
        public Address()
        {
            this.Subscriptions = new HashSet<Subscription>();
            this.Subscriptions1 = new HashSet<Subscription>();
        }
    
        public int AddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int CountryId { get; set; }
        public string Province { get; set; }
    
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<Subscription> Subscriptions1 { get; set; }
    }
}