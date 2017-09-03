using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ProductOverviewViewModel
    {
        public List<CategoryViewModel> ProductsCategories { get; set; }

        public List<PackViewModel> Packs { get; set; }
    }
}
