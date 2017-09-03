using System.Collections.Generic;

namespace ManBox.Model.ViewModels
{
    public class PackViewModel
    {
        public int PackId { get; set; }
        
        public string Title { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }

        public string Description { get; set; }

        public string PriceCurrency { get; set; }

        public decimal? AdvisedPrice { get; set; }

        public string AdvisedPriceCurrency { get; set; }

        public bool FreeShipping { get; set; }

        public string SavingCurrency { get; set; }

        public decimal? GiftVoucherValue { get; set; }
    }
}
