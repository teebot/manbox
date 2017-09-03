using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManBox.Model;
using ManBox.Model.ViewModels;

namespace ManBox.Model.ViewModels
{
    public class CatalogOverviewViewModel
    {
        public string JsonOverviewData { get; set; }

        public bool HasToken { get; set; }
    }
}