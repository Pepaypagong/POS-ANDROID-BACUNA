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
using SupportSearchBar = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using POS_ANDROID_BACUNA.Fragments;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "CheckoutSelectCustomerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutSelectCustomerActivity : AppCompatActivity
    {
        private Button mBtnSelectCustomer;
        private List<CustomersModel> mItems;
        private List<RunnersModel> mRunners;
        private ListView mListView;
        private SupportSearchBar searchBar;
        private SupportToolbar toolBar;
        private bool mIsCustomer;
        private CustomersDataAccess mCustomersDataAccess;
        private RunnersDataAccess mRunnersDataAccess;
        private TransactionsDataAccess mTransactionsDataAccess;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.checkout_select_customer);

            FnGetData();
            FnSetUpControls();
            FnSetUpToolbar();
            FnSetUpEvents();
            if (mIsCustomer)
            {
                FnPopulateCustomers("");
            }
            else
            {
                FnPopulateRunners("");
            }
            //SearchBar
            searchBar.OnActionViewExpanded(); //show edit mode of searchview
            searchBar.ClearFocus(); //clear focus and hide keyboard
        }

        private void FnGetData()
        {
            mIsCustomer = Intent.GetBooleanExtra("isCustomer", true);
            if (mIsCustomer)
            {
                mCustomersDataAccess = new CustomersDataAccess();
            }
            else
            {
                mRunnersDataAccess = new RunnersDataAccess();
            }
            mTransactionsDataAccess = new TransactionsDataAccess();
        }

        private void FnSetUpEvents()
        {
            mBtnSelectCustomer.Click += MBtnSelectCustomer_Click;
            mListView.ItemClick += MListView_ItemClick;
            searchBar.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchBar); };
            searchBar.QueryTextChange += SearchBar_QueryTextChange;
        }

        private void SearchBar_QueryTextChange(object sender, SupportSearchBar.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            if (mIsCustomer)
            {
                FnPopulateCustomers(queryString);
            }
            else
            {
                FnPopulateRunners(queryString);
            }
            
            mListView.Invalidate();
        }

        private void FnSetUpToolbar()
        {
            SetSupportActionBar(toolBar);
            SupportActionBar ab = SupportActionBar;
            //ab.SetHomeAsUpIndicator(Resource.Drawable.left_icon_thin);
            ab.SetDisplayShowHomeEnabled(true);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.Title = mIsCustomer ? "Select customer" : "Select runner";
        }

        private void FnSetUpControls()
        {
            toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            mBtnSelectCustomer = FindViewById<Button>(Resource.Id.btnSelectCustomer);
            mListView = FindViewById<ListView>(Resource.Id.lvCustomers);
            searchBar = FindViewById<SupportSearchBar>(Resource.Id.searchBar);
            searchBar.QueryHint = mIsCustomer ? "Search customers" : "Search runners";
            mBtnSelectCustomer.Text = mIsCustomer ? "REMOVE CUSTOMER" : "REMOVE RUNNER";
        }

        private void FnPopulateRunners(string _queryString)
        {
            mRunners = mRunnersDataAccess.SelectTable();
            if (_queryString != "" && mRunners != null)
            {
                mRunners = mRunners
                    .Where(x => x.FullName.ToLower().Contains(_queryString))
                    .ToList();
            }
            RunnersListViewAdapter adapter = new RunnersListViewAdapter(this, mRunners, mTransactionsDataAccess.SelectTable());
            mListView.Adapter = adapter;
        }

        private void FnPopulateCustomers(string _queryString)
        {
            mItems = mCustomersDataAccess.SelectTable();
            if (_queryString != "" && mItems != null)
            {
                mItems = mItems
                    .Where(x => x.FullName.ToLower().Contains(_queryString))
                    .ToList();
            }
            CustomersListViewAdapter adapter = new CustomersListViewAdapter(this, mItems);
            mListView.Adapter = adapter;
        }

        private void SearchBar_Click(object sender, EventArgs e, SupportSearchBar s)
        {
            s.OnActionViewExpanded();
        }

        //hide keyboard on remove focus of searchbar edittext
        public override bool OnTouchEvent(MotionEvent e)
        {
            //closeKeyBoard();

            //remove focus of searchbar
            searchBar.ClearFocus();

            return base.OnTouchEvent(e);
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string fullname = mIsCustomer ? mItems[e.Position].FullName : mRunners[e.Position].FullName;
            int id = mIsCustomer ? mItems[e.Position].Id : mRunners[e.Position].Id;
            //var result = new Intent(); 
            //result.PutExtra("Key", firstName + " " + lastName);
            GlobalVariables.mCurrentSelectedCustomerOnCheckout = fullname; //set current selected customer
            GlobalVariables.mCurrentSelectedCustomerIdOrRunnerIdOnCheckout = id;
            GlobalVariables.mHasSelectedCustomerOnCheckout = true;
            SetResult(Result.Ok); //SetResult(Result.Ok, result);

            Finish();
        }

        private void MBtnSelectCustomer_Click(object sender, EventArgs e) //remove customer
        {
            GlobalVariables.mCurrentSelectedCustomerOnCheckout = "";
            GlobalVariables.mCurrentSelectedCustomerIdOrRunnerIdOnCheckout = 0;
            GlobalVariables.mHasSelectedCustomerOnCheckout = false;
            SetResult(Result.Ok);

            Finish();
            //remove the action layout on checkout
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.add_new_item:
                    if (mIsCustomer)
                    {
                        Intent intent = new Intent(this, typeof(CustomersFragmentAddCustomerActivity));
                        StartActivityForResult(intent, 35);
                    }
                    else
                    {
                        Intent intent = new Intent(this, typeof(RunnersFragmentAddRunnerActivity));
                        StartActivityForResult(intent, 36);
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_products_categories_search, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 35)
            {
                FnPopulateCustomers("");
            }
            else
            {
                FnPopulateRunners("");
            }
        }

    }
}