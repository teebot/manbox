using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManBox.Web.Controllers
{
    public class ErrorController : ManBoxControllerBase
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View("Error");
        }

    }
}
