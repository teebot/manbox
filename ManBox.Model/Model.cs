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
    
    public partial class Model
    {
        public Model()
        {
            this.SubscriptionDeliveryModels = new HashSet<SubscriptionDeliveryModel>();
        }
    
        public int ModelId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public decimal ShopPrice { get; set; }
        public Nullable<decimal> SupplierPrice { get; set; }
        public Nullable<int> AmountInStock { get; set; }
        public Nullable<decimal> AdvisedPrice { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ICollection<SubscriptionDeliveryModel> SubscriptionDeliveryModels { get; set; }
    }
}
