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
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CustomersFragmentAddCustomerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CustomersFragmentAddCustomerActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        RelativeLayout mRlContents;
        CardView mCardviewCustomerAppearance;
        TextInputLayout mTxtInputLayoutCustomerName;
        TextInputLayout mTxtInputLayoutMobileNumber;
        TextInputLayout mTxtInputLayoutCustomerAddress;
        EditText mEtCustomerName;
        EditText mEtMobileNumber;
        EditText mEtCustomerAddress;
        
        LinearLayout mLlSaveButtonContainer;
        Button mBtnSaveCustomer;
        float mDpVal;

        int CUSTOMER_NAME_MAX_LENGTH = 20;
        int MOBILE_NUMBER_MAX_LENGTH = 14;
        int ADDRESS_MAX_LENGTH = 50;

        CustomersDataAccess mCustomersDataAccess;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.customers_fragment_customer_info);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnSetUpData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mRlContents, mToolBar);
            });
        }

        private void FnSetUpData()
        {
            mCustomersDataAccess = new CustomersDataAccess();
        }

        private void FnSetControls()
        {
            mRlContents = FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mCardviewCustomerAppearance = FindViewById<CardView>(Resource.Id.cardviewCustomerAppearance);
            mTxtInputLayoutCustomerName = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerName);
            mTxtInputLayoutMobileNumber = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutMobileNumber);
            mTxtInputLayoutCustomerAddress = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerAddress);
            mEtCustomerName = FindViewById<EditText>(Resource.Id.etCustomerName);
            mEtMobileNumber = FindViewById<EditText>(Resource.Id.etMobileNumber);
            mEtCustomerAddress = FindViewById<EditText>(Resource.Id.etCustomerAddress);

            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnSaveCustomer = FindViewById<Button>(Resource.Id.btnSaveCustomer);

            mEtCustomerName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(CUSTOMER_NAME_MAX_LENGTH) });
            mEtMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MOBILE_NUMBER_MAX_LENGTH) });
            mEtCustomerAddress.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(ADDRESS_MAX_LENGTH) });
        }
        private void FnSetEvents()
        {
            mBtnSaveCustomer.Click += MBtnSaveCustomer_Click;
        }

        private void MBtnSaveCustomer_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                mCustomersDataAccess.InsertIntoTable(CustomerToSave());
                Finish();
            }
        }

        private bool HasErrors()
        {
            bool retVal = false;
            string _customerName = mEtCustomerName.Text.Trim();
            if (_customerName == "")
            {
                retVal = true;
                mTxtInputLayoutCustomerName.Error = "Enter customer name";
            }
            else if (mCustomersDataAccess.CustomerNameExists(_customerName))
            {
                retVal = true;
                mTxtInputLayoutCustomerName.Error = "Customer name already taken";
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        private CustomersModel CustomerToSave()
        {
            CustomersModel customer = new CustomersModel
            {
                FullName = mEtCustomerName.Text.Trim(),
                Contact = mEtMobileNumber.Text.Trim(),
                Address = mEtCustomerAddress.Text.Trim(),
                DateCreated = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT),
                DateModified = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT)
            };
            return customer;
        }

        private void SetContentsLayoutHeight(RelativeLayout layout, SupportToolbar layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int recyclerViewHeight = layout.Height;

            int _topMargin = dpToPixel(5);
            int _bottomMargin = dpToPixel(5);
            int _leftMargin = 0;
            int _rightMargin = 0;

            int calculatedHeight = recyclerViewHeight - checkoutButtonHeight;
            calculatedHeight = calculatedHeight - _bottomMargin;

            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, calculatedHeight);
            layoutParams.SetMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            layout.LayoutParameters = layoutParams;
            layout.RequestLayout();
        }

        private int dpToPixel(int val)
        {
            return val * (int)Math.Ceiling(mDpVal);
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
            actionBar.Title = "NEW CUSTOMER";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}