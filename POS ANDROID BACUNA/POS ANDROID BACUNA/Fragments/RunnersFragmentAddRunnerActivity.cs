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
    [Activity(Label = "RunnersFragmentAddRunnerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RunnersFragmentAddRunnerActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        RelativeLayout mRlContents;
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

        int Runner_NAME_MAX_LENGTH = 20;
        int MOBILE_NUMBER_MAX_LENGTH = 14;
        int ADDRESS_MAX_LENGTH = 50;

        RunnersDataAccess mRunnersDataAccess;
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
            mRunnersDataAccess = new RunnersDataAccess();
        }

        private void FnSetControls()
        {
            mRlContents = FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mCardviewRunnerAppearance = FindViewById<CardView>(Resource.Id.cardviewCustomerAppearance);
            mTxtInputLayoutRunnerName = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerName);
            mTxtInputLayoutMobileNumber = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutMobileNumber);
            mTxtInputLayoutRunnerAddress = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutCustomerAddress);
            mEtRunnerName = FindViewById<EditText>(Resource.Id.etCustomerName);
            mEtMobileNumber = FindViewById<EditText>(Resource.Id.etMobileNumber);
            mEtRunnerAddress = FindViewById<EditText>(Resource.Id.etCustomerAddress);

            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnSaveRunner = FindViewById<Button>(Resource.Id.btnSaveCustomer);

            mEtRunnerName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(Runner_NAME_MAX_LENGTH) });
            mEtMobileNumber.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MOBILE_NUMBER_MAX_LENGTH) });
            mEtRunnerAddress.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(ADDRESS_MAX_LENGTH) });

            mTxtInputLayoutRunnerName.Hint = "Runner name";
            mBtnSaveRunner.Text = "SAVE RUNNER";
        }
        private void FnSetEvents()
        {
            mBtnSaveRunner.Click += MBtnSaveRunner_Click;
        }

        private void MBtnSaveRunner_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                mRunnersDataAccess.InsertIntoTable(RunnerToSave());
                Finish();
            }
        }

        private bool HasErrors()
        {
            bool retVal = false;
            string _RunnerName = mEtRunnerName.Text.Trim();
            if (_RunnerName == "")
            {
                retVal = true;
                mTxtInputLayoutRunnerName.Error = "Enter Runner name";
            }
            else if (mRunnersDataAccess.RunnerNameExists(_RunnerName))
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
                FullName = mEtRunnerName.Text.Trim(),
                Contact = mEtMobileNumber.Text.Trim(),
                Address = mEtRunnerAddress.Text.Trim(),
                DateCreated = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT),
                DateModified = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT)
            };
            return Runner;
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
            actionBar.Title = "NEW RUNNER";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}