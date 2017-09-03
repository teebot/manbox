using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class OrdersOverviewViewModel {
        public List<OrderDeliveryViewModel> Orders { get; set; }
    }

    public class OrderDeliveryViewModel
    {
        public DateTime? Date { get; set; }

        public int DeliveryId { get; set; }

        public decimal Amount { get; set; }

        public string DeliveryState { get; set; }

        public decimal CouponAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public List<DeliveryProductViewModel> Products { get; set; }
    }

    public class DeliveryProductViewModel
    { public decimal Price { get; set; }
    public int Quantity { get; set; }

    public string ProductName { get; set; }

    public string ModelName { get; set; }

    public decimal TotalPrice { get; set; }

    public string ProductReference { get; set; }
    }
}
