using System;
using System.Collections.Generic;
using ManBox.Model.ViewModels;
namespace ManBox.Business
{
    public interface IShopRepository
    {
        ProductOverviewViewModel GetProductOverview();
        SubscriptionSelectionViewModel GetActiveSelection();
        ProductVisualsViewModel GetProductVisuals(string productReference);

        SubscriptionSelectionViewModel AddToSubscription(int modelId, int quantity);
        SubscriptionSelectionViewModel AddPackToSubscription(int packId);
        SubscriptionSelectionViewModel RemoveFromSubscription(int modelId);
        SubscriptionSelectionViewModel RemovePackFromSubscription(int packId);
        SubscriptionSelectionViewModel ApplyCouponToDelivery(string code, string token);
        SubscriptionSelectionViewModel ChangeSelectionModel(int newModelId, int replacedModelId, int? packId);

        ReScheduleDeliveryViewModel ReScheduleDelivery(int deliveryId, bool rushNow, int? snoozeWeeks);

        Dictionary<string, int> GetAvailableLanguages();

        GiftPacksViewModel GetGiftPacks();

        SubscriptionSelectionViewModel AddGift(int giftPackId);
    }
}
