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

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "RunnersFragmentPaymentDoneActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RunnersFragmentPaymentDoneActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        
        TextView mTxtTotalSaleAmount;
        TextView mTxtChange;
        Button mBtnReceipt;
        Button mBtnStartNewSale;
        ImageView mImgIcon;
        TextView mTxtDone;
        RelativeLayout mRlRunnerBalance;
        TextView mTxtRunnerName;
        TextView mTxtUpdatedBalance;

        double mTotalSaleAmount;
        double mChangeAmount;
        string mTransactionType; //PAYMENT, ORDER, PAYLATER

        string mPesoSign = "\u20b1";
        float TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = 50;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_payment_done);
            FnGetData();
            FnSetControls();
            FnSetEvents();
        }
        private void FnGetData()
        {
            mTotalSaleAmount = Intent.GetDoubleExtra("TotalSaleAmount", 0);
            mChangeAmount = Intent.GetDoubleExtra("ChangeAmount", 0);
            mTransactionType = Intent.GetStringExtra("TransactionType");
        }
        private void FnSetEvents()
        {
            mBtnReceipt.Click += MBtnReceipt_Click;
            mBtnStartNewSale.Click += MBtnStartNewSale_Click;
        }

        private void MBtnReceipt_Click(object sender, EventArgs e)
        {
        }

        private void MBtnStartNewSale_Click(object sender, EventArgs e)
        {
            var result = new Intent();
            result.PutExtra("isStartNewTransaction", true);
            SetResult(Result.Ok, result);
            Finish();
        }

        private void FnSetControls()
        {
            mImgIcon = FindViewById<ImageView>(Resource.Id.imgCheckDone);
            mTxtDone = FindViewById<TextView>(Resource.Id.txtDone);
            mTxtTotalSaleAmount = FindViewById<TextView>(Resource.Id.txtTotalSaleAmount);
            mTxtChange = FindViewById<TextView>(Resource.Id.txtChange);
            mBtnReceipt = FindViewById<Button>(Resource.Id.btnReceipt);
            mBtnStartNewSale = FindViewById<Button>(Resource.Id.btnNewSale);
            mRlRunnerBalance = FindViewById<RelativeLayout>(Resource.Id.rlRunnerBalance);
            mTxtRunnerName = FindViewById<TextView>(Resource.Id.txtRunnerName);
            mTxtUpdatedBalance = FindViewById<TextView>(Resource.Id.txtUpdatedBalance);

            mTxtChange.Text = "Change: " + mPesoSign + string.Format("{0:n}", mChangeAmount);
            mTxtChange.Visibility = mChangeAmount == 0 ? ViewStates.Invisible : ViewStates.Visible;
            mTxtTotalSaleAmount.Text = mPesoSign + string.Format("{0:n}", mTotalSaleAmount);
            int characterCount = mTxtTotalSaleAmount.Text.Count();
            TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = characterCount > 13 ? 35 : 50;
            mTxtTotalSaleAmount.SetTextSize(Android.Util.ComplexUnitType.Sp, TOTAL_SALE_AMOUNT_TEXT_SIZE_SP);
            if (mTransactionType == "ORDER")
            {
                mImgIcon.SetBackgroundResource(Resource.Drawable.paste_icon);
                mTxtDone.Text = "Order placed !";
            }
            else if (mTransactionType == "PAYLATER")
            {
                mRlRunnerBalance.Visibility = ViewStates.Visible;
                mTxtRunnerName.Text = GlobalVariables.mCurrentSelectedCustomerOnCheckout;
                mTxtUpdatedBalance.Text = GetCurrentRunnerBalance();
            }
        }

        private string GetCurrentRunnerBalance()
        {
            return "- " + mPesoSign + string.Format("{0:n}",mTotalSaleAmount); // fetch from db the updated bal of the customer
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 0)
            {
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

    }
}