using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class AccountMenuViewModel
    {
        public AccountMenuItem ActiveMenuItem { get; set; }
    }

    public enum AccountMenuItem
    { 
        Orders,
        Address,
        Password
    }
}
