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
    
    public partial class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }
    
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public bool HasSizeChart { get; set; }
        public System.Guid TitleTrId { get; set; }
        public bool IsVisible { get; set; }
        public int Position { get; set; }
    
        public virtual ICollection<Product> Products { get; set; }
        public virtual Translation Translation { get; set; }
    }
}
