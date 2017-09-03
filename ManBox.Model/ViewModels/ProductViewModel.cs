using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ProductViewModel
    {
        public string Description { get; set; }

        public IEnumerable<ModelViewModel> Models { get; set; }

        public decimal Price { get; set; }

        public string Title { get; set; }

        public string Reference { get; set; }

        public string DescriptionDetail { get; set; }

        public int ProductId { get; set; }

        public string Brand { get; set; }

        public string PriceCurrency { get; set; }

        public decimal? AdvisedPrice { get; set; }

        public string AdvisedPriceCurrency { get; set; }

        public int Quantity { get; set; }
    }
}
