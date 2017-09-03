using System;
using System.Web;
using System.Web.Mvc;
using ManBox.Business;
using ManBox.Common;
using ManBox.Model.ViewModels;
using ManBox.Web.Filters;
using Newtonsoft.Json;
using Segmentio;
using ManBox.Web.Helpers;

namespace ManBox.Web.Controllers
{
    public class BoxController : ManBoxControllerBase
    {
        private readonly IShopRepository shopRepository;

        public BoxController(IShopRepository shopRepoInject)
        {
            shopRepository = shopRepoInject;
        }

        //[StaticHtmlForCrawlersAttribute]
        public ActionResult Compose()
        {
            var catalogCacheKey = WebConstants.CacheKeys.CatalogOverview + CurrentLanguageIso;

            var model = new CatalogOverviewViewModel();

#if !DEBUG
            model.JsonOverviewData = this.HttpContext.Cache[catalogCacheKey] as string;
#endif
            if (model.JsonOverviewData == null)
            {
                var subscriptionOverviewModel = shopRepository.GetProductOverview();
                model.JsonOverviewData = JsonConvert.SerializeObject(subscriptionOverviewModel);
                this.HttpContext.Cache.Insert(catalogCacheKey, model.JsonOverviewData);
            }

            model.HasToken = !string.IsNullOrEmpty(this.CurrentSubscriptionToken);
            
            return View(model);
        }

        public JsonResult GetSubscription()
        {
            var model = shopRepository.GetActiveSelection();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Gifts()
        {
            GiftPacksViewModel model = shopRepository.GetGiftPacks();
            return View(model);
        }

        [HttpPost]
        public JsonResult AddToSubscription(int modelId, int quantity)
        {
            var updatedSelection = shopRepository.AddToSubscription(modelId, quantity);
            if (string.IsNullOrEmpty(this.CurrentSubscriptionToken) || this.CurrentSubscriptionToken != updatedSelection.Token)
            {
                this.CurrentSubscriptionToken = updatedSelection.Token;
            }

            return Json(updatedSelection);
        }

        [HttpPost]
        public ActionResult AddPackToSubscription(int packId)
        {
            var updatedSelection = shopRepository.AddPackToSubscription(packId);
            if (string.IsNullOrEmpty(this.CurrentSubscriptionToken) || this.CurrentSubscriptionToken != updatedSelection.Token)
            {
                this.CurrentSubscriptionToken = updatedSelection.Token;
            }

            return RedirectToAction("Size", "Checkout");
        }

        [HttpPost]
        public ActionResult AddGift(int giftPackId)
        {
            var updatedSelection = shopRepository.AddGift(giftPackId);
            if (string.IsNullOrEmpty(this.CurrentSubscriptionToken) || this.CurrentSubscriptionToken != updatedSelection.Token)
            {
                this.CurrentSubscriptionToken = updatedSelection.Token;
            }

            return RedirectToAction("GiftConfiguration", "Checkout");
        }

        public ActionResult ChangeSelectionModel(ModelSizeSelectViewModel model)
        {
            var updatedSelection = shopRepository.ChangeSelectionModel(model.ModelId, model.ReplacedModelId, model.PackId);
            return Json(updatedSelection);
        }

        [HttpPost]
        public JsonResult RemoveFromSubscription(int modelId)
        {
            var updatedSelection = shopRepository.RemoveFromSubscription(modelId);
            return Json(updatedSelection);
        }

        [HttpPost]
        public JsonResult RemovePackFromSubscription(int packId)
        {
            var updatedSelection = shopRepository.RemovePackFromSubscription(packId);
            return Json(updatedSelection);
        }

        [HttpPost]
        public JsonResult ApplyCoupon(string code)
        {
            var result = shopRepository.ApplyCouponToDelivery(code, this.CurrentSubscriptionToken);
            return Json(result);
        }

        public ActionResult Zoom(string id)
        {
            var result = shopRepository.GetProductVisuals(id);
            return PartialView(result);
        }

        public ActionResult Chuck()
        {
            return View();
        }

        /// <summary>
        /// ascii manbox
        /// </summary>
        public void AddBoxer()
        {
            Response.Write(@"

    HERE BE DRAGONS
    ___________________________________________

            
                         ^\    ^                  
                        / \\  / \                 
                       /.  \\/   \      |\___/|   
    *----*           / / |  \\    \  __/  O  O\   
    |   /          /  /  |   \\    \_\/  \     \     
   / /\/         /   /   |    \\   _\/    '@___@      
  /  /         /    /    |     \\ _\/       |U
  |  |       /     /     |      \\\/        |
  \  |     /_     /      |       \\  )   \ _|_
  \   \       ~-./_ _    |    .- ; (  \_ _ _,\'
  ~    ~.           .-~-.|.-*      _        {-,
   \      ~-. _ .-~                 \      /\'
    \                   }            {   .*
     ~.                 '-/        /.-~----.
       ~- _             /        >..----.\\\
           ~ - - - - ^}_ _ _ _ _ _ _.-\\\


    Your Boxer was added to your subscription!

       ________________
      ||||||||||||||||||
      |       |:       |
      | .-.-. |: .-.-. |
      | '. .' |: '. .' |
      |   '   |/   '   |
      |       /\       |
      |______|  |______|


    ascii manbox coming soon for real??
    *********** who knows?! ***********
    .___  ___.      ___      .__   __. .______     ______   ___   ___ 
    |   \/   |     /   \     |  \ |  | |   _  \   /  __  \  \  \ /   /
    |  \  /  |    /  ^  \    |   \|  | |  |_)  | |  |  |  |  \  V  /  
    |  |\/|  |   /  /_\  \   |  . `  | |   _  <  |  |  |  |   >   <   
    |  |  |  |  /  _____  \  |  |\   | |  |_)  | |  `--'  |  /  .  \  
    |__|  |__| /__/     \__\ |__| \__| |______/   \______/  /__/ \__\   

    AS A REWARD HERE'S A TIP: you should put something in your cart and then try the konami code
");
        }
    }
}
