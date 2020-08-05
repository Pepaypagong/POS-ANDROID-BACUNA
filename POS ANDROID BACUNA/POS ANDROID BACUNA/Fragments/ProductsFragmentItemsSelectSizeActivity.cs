using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportSearchBar = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes;
using Java.Util;
using Android.Graphics;
using POS_ANDROID_BACUNA.Adapters;


namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "ProductsFragmentItemsSelectSizeActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentItemsSelectSizeActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        ListView mLvSizes;
        List<ProductSizes> mProductSizes;
        private SupportSearchBar searchBar;
        bool mDialogShown = false;
        Button mBtnCreateSize;
        string productName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_items_size_select);
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            FnGetData();
            FnSetListViewAdapter("");
            //SearchBar
            searchBar.OnActionViewExpanded(); //show edit mode of searchview
            searchBar.ClearFocus(); //clear focus and hide keyboard
        }

        private void FnGetData()
        {
            productName = Intent.GetStringExtra("productName");
        }

        private void FnSetListViewAdapter(string _queryString)
        {
            ProductsSelectSizesAdapter adapter = new ProductsSelectSizesAdapter(this, PopulateProductSizes(_queryString));
            mLvSizes.Adapter = adapter;
        }
        private List<ProductSizes> PopulateProductSizes(string _queryString)
        {
            var sizes = GlobalVariables.globalSizesList;
            if (_queryString != "")
            {
                mProductSizes = sizes
                    .Where(x => x.productSizeName.ToLower().Contains(_queryString))
                    .ToList();
            }
            else
            {

                mProductSizes = sizes;
            }
            return mProductSizes;
        }

        private void FnSetEvents()
        {
            mLvSizes.ItemClick += MLvSizes_ItemClick;
            searchBar.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchBar); };
            searchBar.QueryTextChange += SearchBar_QueryTextChange;
        }

        private void SearchBar_Click(object sender, EventArgs e, SupportSearchBar s)
        {
            s.OnActionViewExpanded();
        }

        private void SearchBar_QueryTextChange(object sender, SupportSearchBar.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            FnSetListViewAdapter(queryString);
            mLvSizes.Invalidate();
        }

        private void MLvSizes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int sizeId = mProductSizes[e.Position].productSizeId;
            string sizeName = mProductSizes[e.Position].productSizeName;
            bool alreadyExists = GlobalVariables.newProductSizesList.Any(x => x.productSize == sizeName);

            if (!alreadyExists)
            {
                if (!mDialogShown)
                {
                    mDialogShown = true;
                    Intent intent = new Intent(this, typeof(ProductsFragmentItemsAddSizeActivity));
                    intent.PutExtra("productSizeId", sizeId);
                    intent.PutExtra("productSizeName", sizeName);
                    intent.PutExtra("productName", productName);
                    StartActivityForResult(intent, 11);
                    //Finish();
                }
            }  
        }

        private void FnSetControls()
        {
            mLvSizes = FindViewById<ListView>(Resource.Id.lvSizes);
            searchBar = FindViewById<SupportSearchBar>(Resource.Id.searchBar);
            searchBar.QueryHint = "Search sizes";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = "SELECT SIZE";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 11)
            {
                mDialogShown = false;
                if (GlobalVariables.mIsCreateProductSizeClicked)
                {
                    GlobalVariables.mIsCreateProductSizeClicked = false;
                    Finish();
                }
            }
        }

    }
}