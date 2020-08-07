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
    public class MultiSizeAddProductsHolder
    {
        public int productId { get; set; }
        public string productSize { get; set; }
        public int quantity { get; set; }
        public decimal productPrice { get; set; }
        public bool isSelected { get; set; }
    }
}