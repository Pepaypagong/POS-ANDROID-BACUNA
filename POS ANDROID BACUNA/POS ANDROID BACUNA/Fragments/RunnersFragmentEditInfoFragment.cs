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
    public class RunnersFragmentEditInfoFragment : SupportFragment
    {
        View thisFragmentView;
        bool mDialogShown = false;
        RelativeLayout mRlContents;
        SupportToolbar mToolbar;
        CardView mCardviewRunnerAppearance;
        TextInputLayout mTxtInputLayoutRunnerName;
        TextInputLayout mTxtInputLayoutMobileNumber;
        TextInputLayout mTxtInputLayoutRunnerAddress;
        EditText mEtRunnerName;
        EditText mEtMobileNumber;
        EditText mEtRunnerAddress;

        LinearLayout mLlSaveButtonContainer;
        Button mBtnSaveRunner;
        float mDpVal;

        List<RunnersModel> mSelectedRunnerRow;
        RunnersDataAccess mRunnersDataAccess;

        int Runner_NAME_MAX_LENGTH = 20;
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
            ShowSelectedRunnerData();
            return thisFragmentView;
        }

        private void ShowSelectedRunnerData()
        {
            mEtRunnerName.Text = mSelectedRunnerRow[0].FullName;
            mEtMobileNumber.Text = mSelectedRunnerRow[0].Contact;
            mEtRunnerAddress.Text = mSelectedRunnerRow[0].Address;
        }
        private void FnGetData()
        {
            mRunnersDataAccess = new RunnersDataAccess();
            mSelectedRunnerRow = ((RunnersFragmentEditRunnerActivity)this.Activity).DataToFragments();
        }

        private void FnSetUpEvents()
        {
            mBtnSaveRunner.Click += MBtnSaveRunner_Click;
        }

        private void MBtnSaveRunner_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                mRunnersDataAccess.UpdateTable(RunnerToSave());
                ((RunnersFragmentEditRunnerActivity)this.Activity).Finish();
            }
        }
        private bool HasErrors()
        {
            bool retVal = false;
            string _RunnerName = mEtRunnerName.Text.Trim();
            if (_RunnerName == "")
            {
                retVal = true;
                mTxtInputLayoutRunnerName.Error = "Enter runner name";
            }
            else if (mSelectedRunnerRow[0].FullName != _RunnerName && mRunnersDataAccess.RunnerNameExists(_RunnerName))
            {
                retVal = true;
                mTxtInputLayoutRunnerName.Error = "Runner name already taken";
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        private RunnersModel RunnerToSave()
        {
            RunnersModel Runner = new RunnersModel
            {
                Id = mSelectedRunnerRow[0].Id,
                FullName = mEtRunnerName.Text.Trim(),
                Contact = mEtMobileNumber.Text.Trim(),
                Address = mEtRunnerAddress.Text.Trim(),
                DateModified = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT)
            };
            return Runner;
        }

        private void FnSetUpControls()
        {
            mToolbar = thisFragmentView.FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            mRlContents = thisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mCardviewRunnerAppearance = thisFragmentView.FindViewById<CardView>(Resource.Id.cardviewCustomerAppearance);
            mTxtInputLayoutRunnerName = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerName);
            mTxtInputLayoutMobileNumber = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutMobileNumber);
            mTxtInputLayoutRunnerAddress = thisFragmentView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerAddress);
            mEtRunnerName = thisFragmentView.FindViewById<EditText>(Resource.Id.etCustomerName);
            mEtMobileNumber = thisFragmentView.FindViewById<EditText>(Resource.Id.etMobileNumber);
            mEtRunnerAddress = thisFragmentView.FindViewById<EditText>(Resource.Id.etCustomerAddress);

            mLlSaveButtonContainer = thisFragmentView.FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnSaveRunner = thisFragmentView.FindViewById<Button>(Resource.Id.btnSaveCustomer);

            mToolbar.Visibility = ViewStates.Gone;
            mEtRunnerName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(Runner_NAME_MAX_LENGTH) });
            mEtMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MOBILE_NUMBER_MAX_LENGTH) });
            mEtRunnerAddress.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(ADDRESS_MAX_LENGTH) });
        }
    }
}