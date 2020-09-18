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
using SQLitePCL;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class NewProduct
    {
        public int ProductId {get; set;}
        public string ProductName { get; set; }
        public int ParentProductId { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategory { get; set; }
        public int ProductSizeId { get; set; }
        public string ProductSize { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ProductRetailPrice { get; set; }
        public decimal ProductWholesalePrice { get; set; }
        public decimal ProductRunnerPrice { get; set; }
        public string ProductColorBg { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductAlias { get; set; }
        public string ProductCode { get; set; }
    }
}