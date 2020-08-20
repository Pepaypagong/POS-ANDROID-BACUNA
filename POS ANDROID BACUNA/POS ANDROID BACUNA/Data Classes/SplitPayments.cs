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

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class SplitPayments
    {
        public int id { get; set; }
        public string paymentType { get; set; }
        public decimal amount { get; set; }
    }
}