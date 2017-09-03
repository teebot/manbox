using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ProductSelectionViewModel
    {
        public int ModelId { get; set; }

        public int ProductId { get; set; }

        public string ProductTitle { get; set; }

        public string UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string SubTotalPrice { get; set; }

        public string ModelName { get; set; }

        public string ProductReference { get; set; }

        public IEnumerable<ModelViewModel> Models { get; set; }
    }
}
