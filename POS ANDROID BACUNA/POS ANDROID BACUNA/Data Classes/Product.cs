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
    class Product
    {
        public int productId {get; set;}
        public string productName { get; set; }
        public int productCategoryId { get; set; }
        public decimal productRetailPrice { get; set; }
        public decimal productWholesalePrice { get; set; }
        public decimal productRunnerPrice { get; set; }
        public string productColorBg { get; set; }
        public System.Drawing.Image productImage { get; set; }

    }
}