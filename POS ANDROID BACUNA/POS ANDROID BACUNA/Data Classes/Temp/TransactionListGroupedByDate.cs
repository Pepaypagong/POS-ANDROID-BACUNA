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
    class TransactionListGroupedByDate
    {
        public int id { get; set; }
        public int ticketNumber { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int CustomerOrRunnerId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string TransactionType { get; set; } //ORDER [for customers], PAYMENT [for customers cash/check/split], PAYLATER [for runners]
        public bool isHeader { get; set; }
        public int TransactionCount { get; set; }
        public bool isPaid { get; set; }
    }
}