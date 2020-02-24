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
using Android.Bluetooth;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public static class GlobalVariables
    {
        public static List<Product> globalProductsCart = new List<Product>();
        public static List<ProductsOnCart> globalProductsOnCart = new List<ProductsOnCart>();
        public static bool mIsPrinterSet = false;
        public static BluetoothDevice mSelectedDevice;
        public static View mCurrentSelectedCustomerButtonLayout = null;
        public static string mCurrentSelectedCustomerOnCheckout = "";
        public static bool mHasSelectedCustomerOnCheckout = false;
        public static bool mIsAllCollapsed = true; //check if all listview items on cart is closed

        public static int mCurrentSelectedItemIdOnCart; //for changing qty or price
        public static string mCurrentSelectedItemNameOnCart; //for changing qty or price
        public static int mCurrentSelectedItemQtyOnCart; //for changing qty or price
        public static decimal mCurrentSelectedItemPriceOnCart; //for changing qty or price

        public static bool mIsFromRemoveItem; //flag for collapsing cart list on activity result
    }
}