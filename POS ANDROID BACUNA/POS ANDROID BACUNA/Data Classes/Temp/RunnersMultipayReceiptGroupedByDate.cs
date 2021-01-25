using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace POS_ANDROID_BACUNA.Data_Classes.Temp
{
    class RunnersMultipayReceiptGroupedByDate
    {
        public int Id { get; set; }
        public string TransactionDateTime { get; set; }
        public int RunnerId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CashierName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductOrigPrice { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        public decimal ProductDiscountPercentage { get; set; }
        public decimal ProductSubTotalPrice { get; set; }
        public int ProductCountOnCart { get; set; }
        public decimal FooterTotalAmount { get; set; }
        public int ViewType { get; set; } //0 = header, 1 = item, 2 = footer
    }
}