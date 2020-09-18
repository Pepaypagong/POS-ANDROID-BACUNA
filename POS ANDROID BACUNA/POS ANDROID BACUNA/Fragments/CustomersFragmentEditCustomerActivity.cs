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
using Android.Graphics.Drawables;
using Android.Text;
using Android.Support.V4.View;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CustomersFragmentEditCustomerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CustomersFragmentEditCustomerActivity : AppCompatActivity
    {
        TabLayout mTabs;
        SupportToolbar mToolbar;
        ViewPager mViewPager;
        CustomersDataAccess mCustomerDataAccess;
        public int selectedCustomerId;
        public List<CustomersModel> selectedCustomer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.customers_fragment_customer_holder);
            FnGetData();
            FnSetUpToolbar();
            FnSetControls();
            FnSetEvents();
        }

        public List<CustomersModel> DataToFragments()
        {
            return selectedCustomer;
        }

        private void FnSetUpToolbar()
        {
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolbar);
            SupportActionBar actionBar = SupportActionBar;

            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = selectedCustomer[0].FullName;
        }

        private void FnGetData()
        {
            mCustomerDataAccess = new CustomersDataAccess();
            selectedCustomerId = Intent.GetIntExtra("customerId",0);
            selectedCustomer = mCustomerDataAccess.SelectRecord(selectedCustomerId);
        }

        private void FnSetControls()
        {
            mTabs = FindViewById<TabLayout>(Resource.Id.tabsMenu);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            SetUpViewPager(mViewPager);
            mTabs.SetupWithViewPager(mViewPager);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_customers_edit, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menuItem_delete:
                    DeleteCustomer();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void DeleteCustomer()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Delete Customer?");
            alert.SetMessage("Do you want to delete this customer?"); //indicate here if the customer has existing transactions

            alert.SetButton2("CANCEL", (c, ev) =>
            {
                //cancel button
            });
            alert.SetButton("YES", (c, ev) =>
            {
                mCustomerDataAccess.DeleteFromTable(selectedCustomerId);
                Finish();
            });
            alert.Show();
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabFragmentAdapter adapter = new TabFragmentAdapter(SupportFragmentManager);
            adapter.AddFragment(new CustomersFragmentEditInfoFragment(), "INFO");
            adapter.AddFragment(new CustomersFragmentEditSalesFragment(), "SALES");
            adapter.AddFragment(new CustomersFragmentEditOrdersFragment(), "ORDERS");
            viewPager.Adapter = adapter;
        }
        private void FnSetEvents()
        {
            mTabs.TabSelected += MTabs_TabSelected;
        }

        private void MTabs_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {

        }
    }
}