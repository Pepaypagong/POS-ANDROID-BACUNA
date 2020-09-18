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
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Support.V7.Widget;
using Product = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;
using Android.Support.Design.Widget;
using Android.Text;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class CustomersFragmentEditInfoFragment : SupportFragment
    {
        View thisFragmentView;
        bool mDialogShown = false;
        RelativeLayout mRlContents;
        SupportToolbar mToolbar;
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

        List<CustomersModel> mSelectedCustomerRow;
        CustomersDataAccess mCustomersDataAccess;

        int CUSTOMER_NAME_MAX_LENGTH = 20;
        int MOBILE_NUMBER_MAX_LENGTH = 14;
        int ADDRESS_MAX_LENGTH = 50;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.customers_fragment_customer_info, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            FnGetData();
            FnSetUpControls();
            FnSetUpEvents();
            ShowSelectedCustomerData();
            return thisFragmentView;
        }

        private void ShowSelectedCustomerData()
        {
            mEtCustomerName.Text = mSelectedCustomerRow[0].FullName;
            mEtMobileNumber.Text = mSelectedCustomerRow[0].Contact;
            mEtCustomerAddress.Text = mSelectedCustomerRow[0].Address;
        }
        private void FnGetData()
        {
            mCustomersDataAccess = new CustomersDataAccess();
            mSelectedCustomerRow = ((CustomersFragmentEditCustomerActivity)this.Activity).DataToFragments();
        }

        private void FnSetUpEvents()
        {
            mBtnSaveCustomer.Click += MBtnSaveCustomer_Click;
        }

        private void MBtnSaveCustomer_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                mCustomersDataAccess.UpdateTable(CustomerToSave());
                ((CustomersFragmentEditCustomerActivity)this.Activity).Finish();
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
            else if (mSelectedCustomerRow[0].FullName != _customerName && mCustomersDataAccess.CustomerNameExists(_customerName))
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
                Id = mSelectedCustomerRow[0].Id,
                FullName = mEtCustomerName.Text.Trim(),
                Contact = mEtMobileNumber.Text.Trim(),
                Address = mEtCustomerAddress.Text.Trim(),
                DateModified = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT)
            };
            return customer;
        }

        private void FnSetUpControls()
        {
            mToolbar = thisFragmentView.FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            mRlContents = thisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mCardviewCustomerAppearance = thisFragmentView.FindViewById<CardView>(Resource.Id.cardviewCustomerAppearance);
            mTxtInputLayoutCustomerName = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerName);
            mTxtInputLayoutMobileNumber = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutMobileNumber);
            mTxtInputLayoutCustomerAddress = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerAddress);
            mEtCustomerName = thisFragmentView.FindViewById<EditText>(Resource.Id.etCustomerName);
            mEtMobileNumber = thisFragmentView.FindViewById<EditText>(Resource.Id.etMobileNumber);
            mEtCustomerAddress = thisFragmentView.FindViewById<EditText>(Resource.Id.etCustomerAddress);

            mLlSaveButtonContainer = thisFragmentView.FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnSaveCustomer = thisFragmentView.FindViewById<Button>(Resource.Id.btnSaveCustomer);

            mToolbar.Visibility = ViewStates.Gone;
            mEtCustomerName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(CUSTOMER_NAME_MAX_LENGTH) });
            mEtMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MOBILE_NUMBER_MAX_LENGTH) });
            mEtCustomerAddress.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(ADDRESS_MAX_LENGTH) });
        }
    }
}