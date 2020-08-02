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
using Product = POS_ANDROID_BACUNA.Data_Classes.Product;
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

        public override void OnCreate(Bundle savedInstanceState)
        {
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
            if (e.Tab.Text == "ITEMS")
            {
            }
            thisFragmentView.RequestFocus();
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(ChildFragmentManager);
            adapter.AddFragment(new ProductsFragmentItemsFragment(), "ITEMS");
            adapter.AddFragment(new ProductsFragmentStocksFragment(), "STOCK");
            adapter.AddFragment(new ProductsFragmentCategoriesFragment(), "CATEGORIES");
            adapter.AddFragment(new ProductsFragmentSizesFragment(), "SIZES");
            viewPager.Adapter = adapter;
        }

        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }

            public override int Count
            {
                get
                {
                    return Fragments.Count;
                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentNames[position]);
            }

        }
    }
}