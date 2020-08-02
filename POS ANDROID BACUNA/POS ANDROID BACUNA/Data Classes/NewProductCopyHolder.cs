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
    public class NewProductCopyHolder
    {
        public int productId {get; set;}
        public string productName { get; set; }
        public int parentProductId { get; set; }
        public int productCategoryId { get; set; }
        public string productCategory { get; set; }
        public int productSizeId { get; set; }
        public string productSize { get; set; }
        public decimal productCost { get; set; }
        public decimal productRetailPrice { get; set; }
        public decimal productWholesalePrice { get; set; }
        public decimal productRunnerPrice { get; set; }
        public string productColorBg { get; set; }
        public System.Drawing.Image productImage { get; set; }
        public string productAlias { get; set; }
        public string productCode { get; set; }
    }
}