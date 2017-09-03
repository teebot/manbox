using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManBox.Web.Controllers
{
    public class AboutController : ManBoxControllerBase
    {
        //
        // GET: /About/

        public ActionResult Conditions()
        {
            return View();
        }

        public ActionResult Privacy() 
        {
            return View();
        }

        public ActionResult Imprint()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
