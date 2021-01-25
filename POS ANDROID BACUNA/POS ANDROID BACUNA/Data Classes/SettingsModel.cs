using SQLite;


namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class SettingsModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string ReceiptCompanyName { get; set; }
        public string ReceiptAddressLine1 { get; set; }
        public string ReceiptAddressLine2 { get; set; }
        public string ReceiptContactNumber { get; set; }
        public string ReceiptFooterNote { get; set; }
        public string ReceiptPrinterAddress { get; set; }
        public string ReceiptPrinterName { get; set; }
    }
}