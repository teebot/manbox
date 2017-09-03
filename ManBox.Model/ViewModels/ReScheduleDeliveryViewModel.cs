using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class ReScheduleDeliveryViewModel
    {
        /// <summary>
        /// will be false if asked to rush and stock is insufficient 
        /// </summary>
        public bool ShippableTomorrow { get; set; }

        public DateTime NewDate { get; set; }

        public bool CannotSnoozeYet { get; set; }
    }
}
