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
    public class ProductsOnCart
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public int productCategoryId { get; set; }
        public decimal productPrice { get; set; }
        public decimal productOrigPrice { get; set; }
        public decimal productDiscountAmount { get; set; }
        public decimal productDiscountPercentage { get; set; }
        public decimal productSubTotalPrice { get; set; }
        public int productCountOnCart { get; set; }
        public string productSize { get; set; }
    }
}