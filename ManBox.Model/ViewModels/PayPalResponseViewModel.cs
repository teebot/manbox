
namespace ManBox.Model.ViewModels
{
    public class PayPalResponseViewModel
    {
        public string Payment_Status { get; set; }
        public string Payer_Status { get; set; }
        public string Payer_Email { get; set; }
        public string Payment_Type { get; set; }
        public string Item_Number { get; set; } // subscription id 
        public string Txn_Type { get; set; } //subscr_payment
        public string Period3 { get; set; } // 3 M
        public string Mc_Amount3 { get; set; } // 10.00
        public string Action_Type { get; set; }
        public string Pay_Key { get; set; }
        public string Preapproval_Key { get; set; }
        public string Sender_Email { get; set; }
        public string Memo { get; set; }
        public string Payer_Id { get; set; }


        public int SubscriptionId { get; set; } // CUSTOM


        public override string ToString()
        {
            return string.Format(
                @"payment status: {0}
                payer status: {1}
                payer email: {2}
                payment type: {3}
                item number: {4}
                txn type: {5}
                period3: {6}
                mcamount3: {7}
                number of transactions: {8}
                action type: {9}
                pay key: {10}
                sender email: {11}
                memo: {12}
                payer Id: {13},
                subscriptionId: {14}
                "
                , Payment_Status, Payer_Status, Payer_Email, Payment_Type, Item_Number,
                Txn_Type, Period3, Mc_Amount3, Action_Type, Pay_Key, Preapproval_Key,
                Sender_Email, Memo, Payer_Id, SubscriptionId);
        }
    }
}