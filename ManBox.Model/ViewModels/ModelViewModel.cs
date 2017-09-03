using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ModelViewModel
    {
        public int ModelId { get; set; }

        public string Name { get; set; }

        public decimal ShopPrice { get; set; }

        public int ProductId { get; set; }

        public int? PackId { get; set; }
    }

    public class ModelSizeSelectViewModel
    {
        public int ModelId { get; set; }

        public int ReplacedModelId { get; set; }

        public int ProductId { get; set; }

        public int? PackId { get; set; }
    }
}
