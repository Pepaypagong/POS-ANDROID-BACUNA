using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Support.V7.Widget;
using Product = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class ProductsFragment : SupportFragment
    {
        TabLayout mTabs;
        View thisFragmentView;
        ProductsFragmentItemsFragment mProductsFragmentItemsFragment;
        ProductsFragmentStocksFragment mProductsFragmentStocksFragment;
        ProductsFragmentCategoriesFragment mProductsFragmentCategoriesFragment;
        ProductsFragmentSizesFragment mProductsFragmentSizesFragment;

        public override void OnCreate(Bundle savedInstanceState)
        {
            mProductsFragmentItemsFragment = new ProductsFragmentItemsFragment();
            mProductsFragmentStocksFragment = new ProductsFragmentStocksFragment();
            mProductsFragmentCategoriesFragment = new ProductsFragmentCategoriesFragment();
            mProductsFragmentSizesFragment = new ProductsFragmentSizesFragment();

            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            thisFragmentView = inflater.Inflate(Resource.Layout.products_fragment, container, false);
            thisFragmentView.FocusableInTouchMode = true;
            thisFragmentView.Clickable = true;
            mTabs = thisFragmentView.FindViewById<TabLayout>(Resource.Id.tabsMenu);

            ViewPager viewPager = thisFragmentView.FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);

            mTabs.SetupWithViewPager(viewPager);
            FnEvents();
            return thisFragmentView;
        }

        private void FnEvents()
        {
            mTabs.TabSelected += MTabs_TabSelected;
        }

        private void MTabs_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            if (e.Tab.Text == "CATEGORIES")
            {
                mProductsFragmentCategoriesFragment.DeactivateSearch();
            }
            if (e.Tab.Text == "SIZES")
            {
                mProductsFragmentSizesFragment.DeactivateSearch();
            }
            thisFragmentView.RequestFocus();
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabFragmentAdapter adapter = new TabFragmentAdapter(ChildFragmentManager);
            adapter.AddFragment(mProductsFragmentItemsFragment, "ITEMS");
            adapter.AddFragment(mProductsFragmentStocksFragment, "STOCK");
            adapter.AddFragment(mProductsFragmentCategoriesFragment, "CATEGORIES");
            adapter.AddFragment(mProductsFragmentSizesFragment, "SIZES");
            viewPager.Adapter = adapter;
        }
    }
}