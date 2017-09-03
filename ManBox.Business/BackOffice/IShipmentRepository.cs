using ManBox.Model.ViewModels.BackOffice;
using System;
namespace ManBox.Business.BackOffice
{
    public interface IShipmentRepository
    {
        ShipmentListViewModel GetShipmentsList();
        ShipmentDetailViewModel GetDeliveryDetails(int deliveryId);
        void ConfirmShipmentSent(int deliveryId, bool notify);
        void UpdateShipmentPaymentStatus(int deliveryId, string deliveryPaymentStatus);
        ShipmentPaymentViewModel GetDeliveryPaymentInfo(int deliveryId);
    }
}
