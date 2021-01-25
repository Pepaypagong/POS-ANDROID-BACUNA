using SQLite;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class RunnersMultipayRecordsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TransactionDateTime { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public int RunnerId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CashierName { get; set; }
        public decimal PaymentCashAmount { get; set; } //not yet implemented
        public decimal PaymentCheckAmount { get; set; } //not yet implemented
        public string TransactionIds { get; set; } //#10, #11, #12, #13
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}