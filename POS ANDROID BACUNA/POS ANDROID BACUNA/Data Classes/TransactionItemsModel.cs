
using SQLite;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class TransactionItemsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductOrigPrice { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        public decimal ProductDiscountPercentage { get; set; }
        public decimal ProductSubTotalPrice { get; set; }
        public int ProductCountOnCart { get; set; }
    }
}