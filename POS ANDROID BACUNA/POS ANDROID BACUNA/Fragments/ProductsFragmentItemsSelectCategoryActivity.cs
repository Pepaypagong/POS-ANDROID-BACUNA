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
    [Activity(Label = "ProductsFragmentItemsSelectCategoryActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentItemsSelectCategoryActivity : AppCompatActivity
    {
        private SupportToolbar mToolBar;
        private List<ProductCategories> mProductCategories;
        private ListView mLvCategories;
        private SupportSearchBar searchBar;
        bool mDialogShown = false;
        string selectedCategory;
        string caller;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_items_category_select);
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
            selectedCategory = Intent.GetStringExtra("productCatNameInit");
            caller = Intent.GetStringExtra("caller");
        }

        private void FnSetListViewAdapter(string _queryString)
        {
            if (caller == "ProductsFragmentItemsSelectCategoryActivity")
            {
                if (_queryString != "")
                {
                    mProductCategories = GlobalVariables.globalProductCategoryList
                        .OrderBy(x => x.productCategoryId)
                        .Where(x => x.productCategoryName.ToLower().Contains(_queryString))
                        .ToList();
                }
                else
                {
                    mProductCategories = GlobalVariables.globalProductCategoryList.OrderBy(x => x.productCategoryId).ToList();
                }
            }
            else
            {
                if (_queryString != "")
                {
                    mProductCategories = GlobalVariables.globalProductCategoryList
                        .OrderBy(x => x.productCategoryId)
                        .Where(x => x.productCategoryName != "All")
                        .Where(x => x.productCategoryName.ToLower().Contains(_queryString))
                        .ToList();
                }
                else
                {
                    mProductCategories = GlobalVariables.globalProductCategoryList
                        .OrderBy(x => x.productCategoryId).Where(x => x.productCategoryName != "All").ToList();
                }
            }
            ProductsItemSelectCategoryAdapter adapter = new ProductsItemSelectCategoryAdapter(this, mProductCategories);
            mLvCategories.Adapter = adapter;
        }

        private void FnSetEvents()
        {
            mLvCategories.ItemClick += MLvSizes_ItemClick;
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
            mLvCategories.Invalidate();
        }

        private void MLvSizes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int categoryId = mProductCategories[e.Position].productCategoryId;
            string categoryName = mProductCategories[e.Position].productCategoryName;

            var result = new Intent(); 
            result.PutExtra("productCategoryName", categoryName);
            result.PutExtra("productCategoryId", categoryId);
            SetResult(Result.Ok, result);

            Finish();
        }

        private void FnSetControls()
        {
            mLvCategories = FindViewById<ListView>(Resource.Id.lvCategories);
            searchBar = FindViewById<SupportSearchBar>(Resource.Id.searchBar);
            searchBar.QueryHint = "Search categories";
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    var result = new Intent();
                    result.PutExtra("productCategoryName", selectedCategory);
                    SetResult(Result.Ok, result);
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
            actionBar.Title = "Categories";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        public override void OnBackPressed()
        {
            var result = new Intent();
            result.PutExtra("productCategoryName", selectedCategory);
            SetResult(Result.Ok, result);
            base.OnBackPressed();
        }

        protected override void OnResume()
        {
            base.OnResume();
            OverridePendingTransition(0, 0);//removeanimation
        }
    }
}