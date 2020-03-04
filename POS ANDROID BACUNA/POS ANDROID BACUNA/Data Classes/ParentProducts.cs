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
    class ParentProducts
    {
        public int parentProductId { get; set; }
        public string parentProductName { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; } 
    }
}