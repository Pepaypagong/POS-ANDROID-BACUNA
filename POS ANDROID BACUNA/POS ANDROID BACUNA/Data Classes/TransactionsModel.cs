using SQLite;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class TransactionsModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string TransactionDateTime { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public int CustomerOrRunnerId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string TransactionType { get; set; } //ORDER [for customers], PAYMENT [for customers cash/check/split], PAYLATER [for runners]
        public string CashierName { get; set; }
        public bool IsPaid { get; set; }
        public decimal PaymentCashAmount { get; set; } //not yet implemented
        public decimal PaymentCheckAmount { get; set; } //not yet implemented
    }
}