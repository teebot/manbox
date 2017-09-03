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
    
    public partial class User
    {
        public User()
        {
            this.Subscriptions = new HashSet<Subscription>();
        }
    
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CountryId { get; set; }
        public string SignInTypeCV { get; set; }
        public bool IsOptin { get; set; }
        public System.DateTime CreatedDatetime { get; set; }
        public string Phone { get; set; }
        public int LanguageId { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual Language Language { get; set; }
    }
}
