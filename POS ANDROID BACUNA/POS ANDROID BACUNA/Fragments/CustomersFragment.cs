using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SearchView = Android.Support.V7.Widget.SearchView;
using POS_ANDROID_BACUNA.Adapters;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class CustomersFragment : SupportFragment
    {
        View mThisFragmentView;
        public SearchView searchViewSearchCustomers;
        ListView mLvCustomers;
        CustomersDataAccess mCustomersDataAccess;
        List<CustomersModel> mCustomersList;
        bool mDialogShown;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FnSetUpMainView(inflater, container);
            FnSetUpData();
            FnSetUpControls();
            FnSetUpEvents();
            FnSetCustomersList("");
            searchViewSearchCustomers.OnActionViewExpanded(); //show edit mode of searchview
            searchViewSearchCustomers.ClearFocus(); //clear focus and hide keyboard
            return mThisFragmentView;
        }

        private void FnSetUpData()
        {
            mCustomersDataAccess = new CustomersDataAccess();
            mCustomersDataAccess.CreateTable();
            mCustomersList = mCustomersDataAccess.SelectTable();
        }

        private void FnSetUpMainView(LayoutInflater inflater, ViewGroup container)
        {
            mThisFragmentView = inflater.Inflate(Resource.Layout.customers_fragment, container, false);
            mThisFragmentView.Clickable = true;
            mThisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(mThisFragmentView);
        }
        private void FnSetUpControls()
        {
            searchViewSearchCustomers = mThisFragmentView.FindViewById<SearchView>(Resource.Id.searchBar);
            mLvCustomers = mThisFragmentView.FindViewById<ListView>(Resource.Id.lvCustomers);
            searchViewSearchCustomers.QueryHint = "Search customers";
        }
        private void FnSetUpEvents()
        {
            mLvCustomers.ItemClick += MLvCustomers_ItemClick;
            searchViewSearchCustomers.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchViewSearchCustomers); };
            searchViewSearchCustomers.QueryTextChange += SearchBar_QueryTextChange;
        }
        private void SearchBar_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            FnSetCustomersList(queryString);
            mLvCustomers.Invalidate();
        }
        private void SearchBar_Click(object sender, EventArgs e, SearchView s)
        {
            s.OnActionViewExpanded();
        }
        private void MLvCustomers_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!mDialogShown) //avoid double click
            {
                mDialogShown = true;
                int customerId = mCustomersList[e.Position].Id;
                Intent intent = new Intent(Context, typeof(CustomersFragmentEditCustomerActivity));
                intent.PutExtra("customerId", customerId);
                StartActivityForResult(intent, 34);
            }
        }

        public void FnSetCustomersList(string _queryString)
        {
            mCustomersList = mCustomersDataAccess.SelectTable();
            var mItems = mCustomersList;  //GlobalVariables.globalCustomersList;
            if (_queryString != "")
            {
                mItems = mItems
                    .Where(x => x.FullName.ToLower().Contains(_queryString))
                    .ToList();
                mCustomersList = mItems;
            }
            CustomersListViewAdapter adapter = new CustomersListViewAdapter((MainActivity)this.Activity, mItems);
            mLvCustomers.Adapter = adapter;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 34)
            {
                mDialogShown = false;
                FnSetCustomersList("");
                searchViewSearchCustomers.SetQuery("", false);
            }
        }
    }
}