using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class CategoryViewModel
    {
        public string Title { get; set; }

        public IQueryable<ProductViewModel> Products { get; set; }

        public bool HasSizeChart { get; set; }

        public string TitleStd { get; set; }

        public int CategoryId { get; set; }
    }
}
