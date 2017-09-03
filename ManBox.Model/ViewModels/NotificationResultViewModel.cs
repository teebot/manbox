using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class NotificationResultViewModel
    {
        public int NotificationsSent { get; set; }

        public DateTime EndedDateTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public string MessageType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} notifications sent in {2}. Finished at {3}", NotificationsSent, this.MessageType, this.ElapsedTime, this.EndedDateTime);
        }
    }
}
