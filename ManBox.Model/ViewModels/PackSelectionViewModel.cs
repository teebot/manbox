using System.Collections.Generic;

namespace ManBox.Model.ViewModels
{
    public class PackSelectionViewModel
    {
        public int PackId { get; set; }
        
        public string Title { get; set; }

        public string Price { get; set; }

        public List<ProductSelectionViewModel> SelectedProducts { get; set; }
    }
}
