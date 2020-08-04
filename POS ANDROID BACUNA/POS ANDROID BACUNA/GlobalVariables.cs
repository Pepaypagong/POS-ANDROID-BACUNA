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
using SupportFragment = Android.Support.V4.App.Fragment;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public static class GlobalVariables
    {
        public static List<ProductsOnCart> globalProductsOnCart = new List<ProductsOnCart>();
        public static List<Product> globalProductList = new List<Product>();
        public static List<ParentProducts> globalParentProductList = new List<ParentProducts>(); //temporary placeholder
        public static List<ProductCategories> globalProductCategoryList = new List<ProductCategories>();
        public static List<Customers> globalCustomersList = new List<Customers>();
        public static List<Runners> globalRunnersList = new List<Runners>();
        public static List<Options> globalOptionList = new List<Options>();
        public static List<OrderFields> globalSortingFieldList = new List<OrderFields>(); //sorting cart items

        public static SupportFragment mCheckoutFragmentCurrentInstance;

        public static bool mIsPrinterSet = false;
        public static BluetoothDevice mSelectedDevice;

        public static string mCurrentSelectedPricingType = "RT";
        public static View mCurrentSelectedCustomerButtonLayout = null;
        public static string mCurrentSelectedCustomerOnCheckout = "";
        public static bool mHasSelectedCustomerOnCheckout = false;
        public static bool mIsAllCollapsed = true; //check if all listview items on cart is closed

        public static bool mIsCheckoutFragmentMultiSizeAddOpened = false; //prevent double click on CheckoutFragmentMultiSize
        public static string mCurrentSelectedItemNameMultiSize;
        public static int mCurrentSelectedItemIdMultiSize;

        public static int mCurrentSelectedItemIdOnCart; //for changing qty or price
        public static string mCurrentSelectedItemNameOnCart; //for changing qty or price
        public static int mCurrentSelectedItemQtyOnCart; //for changing qty or price
        public static decimal mCurrentSelectedItemPriceOnCart; //for changing qty or price
        public static decimal mCurrentSelectedItemDiscountAmountOnCart; //for changing disc

        public static bool mIsFromRemoveItem; //flag for collapsing cart list on activity result

        public static bool mIsCashDiscountSelected = true; //for text watcher of discount on text changed;

        public static bool mIsCreateProductSizeClicked = false;
        public static List<NewProduct> newProductSizesList = new List<NewProduct>(); //temporary placeholder
        public static List<ParentProductCopyHolder> parentProductCopyHolder = new List<ParentProductCopyHolder>();
        public static List<NewProductCopyHolder> newProductCopyHolder = new List<NewProductCopyHolder>();

        public static bool mIsProductsFragmentEditItemOpened = false; //prevent double click on ProductsFragmentItemEditItem

        public static List<int> productsToDeleteOnEditMode = new List<int>(); //list of product size id deleted from newProductSizeList
    }
}