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

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentPaymentNumpadActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentPaymentNumpadActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        Button mBtnNext;

        LinearLayout mLlContents;
        LinearLayout mLlSaveButtonContainer;
        EditText mEtAmountReceived;
        TextView mTxtBalanceAndChange;
        TextView mTxtAmountToPay;
        GridLayout mGridlayoutNumpad;

        float mDpVal;
        double mTotalSaleAmount;
        private bool mIsPaymentModeCash;
        string mPesoSign = "\u20b1";
        float TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = 60;
        int MAX_AMOUNT_RECEIVED_LENGTH = 15;
        bool isInitialPriceChange = true;
        bool mIsBalanceOrChangeVisible = false;
        bool mIsAmtToPayVisible = false;
        bool mIsInitialCall;
        double mTotalSplitPaymentAmount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_payment_numpad);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mLlContents, mToolBar);
            });
            SetButtonAppearance(GetCurrentAmtReceivedInput(), GetCurrentTotalSaleAmount(), mIsInitialCall);
        }
        private void SetContentsLayoutHeight(LinearLayout layout, SupportToolbar layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int layoutHeight = layout.Height;

            int _topMargin = dpToPixel(5);
            int _bottomMargin = dpToPixel(5);
            int _leftMargin = 0;
            int _rightMargin = 0;

            int calculatedHeight = layoutHeight - checkoutButtonHeight;
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

        private void FnGetData()
        {
            mIsInitialCall = Intent.GetBooleanExtra("isInitialCall", true);
            mTotalSplitPaymentAmount = Intent.GetDoubleExtra("totalSplitPaymentsAmount", 0);
            mTotalSaleAmount = Intent.GetDoubleExtra("totalSaleAmount", 0);
            mIsPaymentModeCash = Intent.GetBooleanExtra("isPaymentModeCash", true);
        }
        private void FnSetEvents()
        {
            mBtnNext.Click += MBtnNext_Click;
            mEtAmountReceived.AddTextChangedListener(new CurrencyTextWatcher(mEtAmountReceived, this, mPesoSign));
            NumpadClickEvent(mEtAmountReceived);
            mEtAmountReceived.AfterTextChanged += MEtAmountReceived_AfterTextChanged;
        }

        private decimal GetCurrentAmtReceivedInput()
        {
            return Convert.ToDecimal(mEtAmountReceived.Text.Replace(mPesoSign, "").Replace(",", ""));

        }
        private decimal GetCurrentTotalSaleAmount()
        {
            return Convert.ToDecimal(mTotalSaleAmount);
        }

        private void MEtAmountReceived_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            int characterCount = mEtAmountReceived.Text.Count();
            TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = characterCount > 14 ? 35 : 50;
            mEtAmountReceived.SetTextSize(Android.Util.ComplexUnitType.Sp, TOTAL_SALE_AMOUNT_TEXT_SIZE_SP);

            ChangeAmountsAferTextChanged();
        }
        private void ChangeAmountsAferTextChanged()
        {
            decimal currentAmtReceivedInput = GetCurrentAmtReceivedInput();
            decimal currentBalance = mIsInitialCall ? GetCurrentTotalSaleAmount() :
                GetCurrentTotalSaleAmount() - Convert.ToDecimal(mTotalSplitPaymentAmount);
            if (currentBalance > currentAmtReceivedInput)
            {
                mIsBalanceOrChangeVisible = true;
                mIsAmtToPayVisible = true;
                mTxtBalanceAndChange.Text = "Balance: " + mPesoSign + string.Format("{0:n}", currentBalance - currentAmtReceivedInput);
            }
            else if (currentBalance < currentAmtReceivedInput)
            {
                mIsBalanceOrChangeVisible = true;
                mIsAmtToPayVisible = true;
                mTxtBalanceAndChange.Text = mIsPaymentModeCash ?
                    "Change: " + mPesoSign + string.Format("{0:n}", currentAmtReceivedInput - currentBalance) :
                    "The amount entered can't exceed the amount to pay.";
            }
            else if (currentBalance == currentAmtReceivedInput)
            {
                mIsBalanceOrChangeVisible = false;
                mIsAmtToPayVisible = false;
            }
            mTxtAmountToPay.Text = "Total sale amount: " + mPesoSign + string.Format("{0:n}", GetCurrentTotalSaleAmount());
            SetBalanceAndAmtToPayVisibility(mIsBalanceOrChangeVisible, mIsAmtToPayVisible);
            SetButtonAppearance(currentAmtReceivedInput, currentBalance, mIsInitialCall);
        }

        private void MBtnNext_Click(object sender, EventArgs e)
        {
            if (mIsInitialCall)
            {
                decimal currentInputAmount = GetCurrentAmtReceivedInput();
                decimal totalSaleAmount = GetCurrentTotalSaleAmount();
                if (!HasErrors(currentInputAmount, totalSaleAmount))
                {
                    //use request code 25 for payment method
                    //use request code 26 for payment done page
                    if (totalSaleAmount > currentInputAmount)
                    {
                        if (!mDialogShown)
                        {
                            mDialogShown = true;
                            Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentMethodsActivity));
                            intent.PutExtra("totalSaleAmount", mTotalSaleAmount);
                            intent.PutExtra("isCash", mIsPaymentModeCash);
                            intent.PutExtra("inputAmount", Convert.ToDouble(currentInputAmount));
                            StartActivityForResult(intent, 25);
                        }
                    }
                    else if (totalSaleAmount <= currentInputAmount)
                    {
                        if (!mDialogShown) //avoid double click
                        {
                            mDialogShown = true;
                            Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentDoneActivity));
                            intent.PutExtra("TotalSaleAmount", Convert.ToDouble(totalSaleAmount));
                            intent.PutExtra("ChangeAmount", Convert.ToDouble(currentInputAmount - totalSaleAmount));
                            intent.PutExtra("TransactionType", "PAYMENT");
                            StartActivityForResult(intent, 26);
                        }
                    }
                }
            }
            else
            {
                decimal currentInputAmount = GetCurrentAmtReceivedInput();
                decimal currentBalance = GetCurrentTotalSaleAmount() - Convert.ToDecimal(mTotalSplitPaymentAmount);
                if (!HasErrors(currentInputAmount, currentBalance))
                {
                    PassDataToPaymentMethods(true);
                    Finish();
                }
            }
        }

        public override void OnBackPressed()
        {
            PassDataToPaymentMethods(false);
            base.OnBackPressed();
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        private void PassDataToPaymentMethods(bool _isBackPressed)
        {
            var result = new Intent();
            result.PutExtra("inputAmount", Convert.ToDouble(GetCurrentAmtReceivedInput()));
            result.PutExtra("isPaymentModeCash", mIsPaymentModeCash);
            result.PutExtra("isBackPressed", _isBackPressed);
            SetResult(Result.Ok, result);
        }

        private bool HasErrors(decimal _amountReceived, decimal _totalSaleAmount)
        {
            bool retVal = false;
            if (_amountReceived == 0)
            {
                DialogMessageService.MessageBox(this, "", "Please enter an amount.");
                return true;
            }
            else if (!mIsPaymentModeCash && (_amountReceived > _totalSaleAmount))
            {
                DialogMessageService.MessageBox(this, "", "The amount entered can't exceed the amount to pay.");
                return true;
            }
            return retVal;
        }

        void NumpadClickEvent(EditText _edittext)
        {
            int childCount = mGridlayoutNumpad.ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                TextView numberPad = (TextView)mGridlayoutNumpad.GetChildAt(i);
                numberPad.Click += delegate (object sender, EventArgs e) {
                    if (isInitialPriceChange)
                    {
                        isInitialPriceChange = false;
                        _edittext.Text = mPesoSign + "0.00";
                    }
                    if (numberPad.Text == "Del")
                    {
                        if (_edittext.Text.Length <= 5)
                        {
                            string holder = _edittext.Text;

                            string firstDigit = holder.Substring(1, 1);
                            string secondDigit = holder.Substring(3, 1);

                            holder = holder.Remove(1, 1).Insert(1, "0"); //first digit always 0 on del
                            holder = holder.Remove(3, 1).Insert(3, firstDigit);
                            holder = holder.Remove(4, 1).Insert(4, secondDigit);
                            _edittext.Text = holder;
                        }
                        else
                        {
                            _edittext.Text = _edittext.Text.Remove(_edittext.Text.Length - 1, 1);
                        }

                    }
                    else
                    {
                        if (numberPad.Text == "0")
                        {
                            if (_edittext.Text == mPesoSign + "0.00")
                            {
                                //mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                            else
                            {
                                _edittext.Text = _edittext.Text + numberPad.Text;
                            }
                        }
                        else
                        {
                            _edittext.Text = _edittext.Text + numberPad.Text;
                        }
                    }
                };
            }
        }

        private void FnSetControls()
        {
            mLlContents = FindViewById<LinearLayout>(Resource.Id.llContents);
            mEtAmountReceived = FindViewById<EditText>(Resource.Id.etAmountReceived);
            mTxtBalanceAndChange = FindViewById<TextView>(Resource.Id.txtBalanceAndChange);
            mTxtAmountToPay = FindViewById<TextView>(Resource.Id.txtAmountToPay);
            mGridlayoutNumpad = FindViewById<GridLayout>(Resource.Id.glNumpad);
            mBtnNext = FindViewById<Button>(Resource.Id.btnSave);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);

            mEtAmountReceived.Text = mPesoSign + string.Format("{0:n}", mIsInitialCall ? mTotalSaleAmount : mTotalSaleAmount - mTotalSplitPaymentAmount);
            SetTxtAmountReceived();
            SetBalanceAndAmtToPayVisibility(mIsBalanceOrChangeVisible, mIsAmtToPayVisible);
        }

        private void SetTxtAmountReceived()
        {
            int characterCount = mEtAmountReceived.Text.Count();
            TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = characterCount > 13 ? 35 : 50;
            mEtAmountReceived.SetTextSize(Android.Util.ComplexUnitType.Sp, TOTAL_SALE_AMOUNT_TEXT_SIZE_SP);
            mEtAmountReceived.InputType = Android.Text.InputTypes.Null;
            mEtAmountReceived.SetSelection(mEtAmountReceived.Text.Length);
            mEtAmountReceived.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(MAX_AMOUNT_RECEIVED_LENGTH) });//set max length
            mEtAmountReceived.SetBackgroundColor(ColorHelper.ResourceIdToColor(Android.Resource.Color.Transparent, this));
        }

        private void SetBalanceAndAmtToPayVisibility(bool mIsBalanceOrChangeVisible, bool mIsAmtToPayVisible)
        {
            mTxtBalanceAndChange.Visibility = mIsBalanceOrChangeVisible ? ViewStates.Visible : ViewStates.Invisible;
            mTxtAmountToPay.Visibility = mIsAmtToPayVisible ? ViewStates.Visible : ViewStates.Invisible;
        }

        private void SetButtonAppearance(decimal _totalAmtReceived, decimal _totalSaleAmount, bool _isInitialCall)
        {
            if (_isInitialCall)
            {
                if (_totalAmtReceived == 0)
                {
                    mBtnNext.SetTextColor(ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
                    mBtnNext.Background = this.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderNoItem);
                    mBtnNext.Text = "Next";
                }
                else if (_totalAmtReceived >= _totalSaleAmount)
                {
                    mBtnNext.SetTextColor(ColorHelper.ResourceIdToColor(mIsPaymentModeCash ?
                        Resource.Color.colorPrimary :
                        _totalAmtReceived == _totalSaleAmount ?
                        Resource.Color.colorPrimary : Resource.Color.colorAccent, this));
                    mBtnNext.Background = this.GetDrawable(mIsPaymentModeCash ?
                        Resource.Drawable.buttonCheckoutRoundBorderWithItem :
                        _totalAmtReceived == _totalSaleAmount ?
                        Resource.Drawable.buttonCheckoutRoundBorderWithItem : Resource.Drawable.buttonCheckoutRoundBorderNoItem);
                    mBtnNext.Text = "Charge " + mPesoSign + String.Format("{0:n}", _totalSaleAmount);
                }
                else if (_totalAmtReceived < _totalSaleAmount)
                {
                    mBtnNext.SetTextColor(ColorHelper.ResourceIdToColor(Resource.Color.colorPrimary, this));
                    mBtnNext.Background = this.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderWithItem);
                    mBtnNext.Text = "Next";
                }
            }
            else
            {
                //_totalSaleAmount is current balance
                mBtnNext.SetTextColor(ColorHelper.ResourceIdToColor(_totalAmtReceived == 0 || !mIsPaymentModeCash && (_totalAmtReceived > _totalSaleAmount) ?
                    Resource.Color.colorAccent : Resource.Color.colorPrimary, this));
                mBtnNext.Background = this.GetDrawable(_totalAmtReceived == 0 || !mIsPaymentModeCash && (_totalAmtReceived > _totalSaleAmount) ?
                    Resource.Drawable.buttonCheckoutRoundBorderNoItem : Resource.Drawable.buttonCheckoutRoundBorderWithItem);
                mBtnNext.Text = "Next";
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    PassDataToPaymentMethods(false);
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 25 || requestCode == 26)
            {
                try
                {
                    mDialogShown = false;
                    bool isStartNewTransaction = data.GetBooleanExtra("isStartNewTransaction", false);
                    if (isStartNewTransaction)
                    {
                        var result = new Intent();
                        result.PutExtra("isStartNewTransaction", isStartNewTransaction);
                        SetResult(Result.Ok, result);
                        Finish();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = mIsPaymentModeCash ? "Payment: Cash" : "Payment: Check";
        }
    }
}