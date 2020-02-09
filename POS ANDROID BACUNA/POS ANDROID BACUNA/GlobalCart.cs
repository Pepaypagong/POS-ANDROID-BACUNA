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
using Product = POS_ANDROID_BACUNA.Data_Classes.Product;

namespace POS_ANDROID_BACUNA
{
    public static class GlobalCart
    {
        public static List<Product> globalProductsCart = new List<Product>();
    }
}