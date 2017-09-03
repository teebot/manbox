using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ManBox.Business;
using ManBox.Model.ViewModels;
using Newtonsoft.Json;

namespace ManBox.Web.Controllers
{
    public class ManBoxApiController : ApiController
    {
        private readonly IShopRepository shopRepository;

        public ManBoxApiController(IShopRepository repo)
        {
            shopRepository = repo;
        }

        public SubscriptionSelectionViewModel GetSubscription(string token)
        {
            var model = shopRepository.GetActiveSelection();
            return model;
        }
    }
}
