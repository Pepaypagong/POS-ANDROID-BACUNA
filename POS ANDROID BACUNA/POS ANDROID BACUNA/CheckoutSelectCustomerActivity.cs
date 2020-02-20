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

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "CheckoutSelectCustomerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutSelectCustomerActivity : AppCompatActivity
    {

        private Button mBtnSelectCustomer;
        private List<Customers> mItems;
        private ListView mListView;
        private SupportSearchBar searchBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.checkout_select_customer);

            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.left_icon);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetTitle(Resource.String.select_customer);

            //SearchBar
            searchBar = FindViewById<SupportSearchBar>(Resource.Id.searchBar);
            searchBar.QueryHint = "Search customers";
            searchBar.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchBar); };
            searchBar.OnActionViewExpanded(); //show edit mode of searchview
            searchBar.ClearFocus(); //clear focus and hide keyboard

            mBtnSelectCustomer = FindViewById<Button>(Resource.Id.btnSelectCustomer);
            mBtnSelectCustomer.Click += MBtnSelectCustomer_Click;

            //populate listview
            mListView = FindViewById<ListView>(Resource.Id.myListView);
            mItems = new List<Customers>();
            mItems.Add(new Customers() { FirstName = "Jeffrey", LastName = "Bacuña", Age = "24", Gender = "Male" });
            mItems.Add(new Customers() { FirstName = "Joan", LastName = "Bacuña", Age = "21", Gender = "Female" });
            mItems.Add(new Customers() { FirstName = "Justine", LastName = "Bacuña", Age = "18", Gender = "Male" });

            CustomersListViewAdapter adapter = new CustomersListViewAdapter(this, mItems);

            mListView.Adapter = adapter;

            mListView.ItemClick += MListView_ItemClick;
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

        //unused because clearFocus already closes the keyboard
        void closeKeyBoard()
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(searchBar.WindowToken, 0);
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string firstName = mItems[e.Position].FirstName;
            string lastName = mItems[e.Position].LastName;

            //var result = new Intent(); 
            //result.PutExtra("Key", firstName + " " + lastName);
            GlobalVariables.mCurrentSelectedCustomerOnCheckout = firstName + " " + lastName; //set current selected customer
            GlobalVariables.mHasSelectedCustomerOnCheckout = true;
            SetResult(Result.Ok); //SetResult(Result.Ok, result);

            Finish();
        }

        private void MBtnSelectCustomer_Click(object sender, EventArgs e) //remove customer
        {
            GlobalVariables.mCurrentSelectedCustomerOnCheckout = "";
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
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

    }
}