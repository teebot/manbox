using ManBox.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Business.Batch
{
    public interface IBatchRepository
    {
        NotificationResultViewModel SendUpcomingBoxNotifications();
    }
}
