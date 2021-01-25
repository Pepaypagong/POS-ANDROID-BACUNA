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
    [Activity(Label = "RunnersFragmentPaymentMethodsActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RunnersFragmentPaymentMethodsActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        Button mBtnAddPayment;

        LinearLayout mLlContents;
        LinearLayout mLlSaveButtonContainer;
        TextView mTxtTotalSaleAmount;
        ListView mLvSplitPayments;
        RelativeLayout mRlLabel;
        TextView mTxtSplitpaymentLabel;
        PaymentMethodsListViewAdapter mAdapter;

        float mDpVal;
        double mTotalSaleAmount;
        string mPesoSign = "\u20b1";
        IMenu mCurrentToolbarMenu;

        List<SplitPayments> mSplitPayments;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_payment_methods);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            FnSetUpListView();
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mLlContents, mToolBar);
            });
            SetButtonAppearance(mSplitPayments.Select(x=>x.amount).FirstOrDefault(), Convert.ToDecimal(mTotalSaleAmount));
            SetSplitPaymentLabelVisibility();
        }

        private void FnSetUpListView()
        {
            mAdapter = new PaymentMethodsListViewAdapter(this, mSplitPayments, Convert.ToDecimal(mTotalSaleAmount), mLvSplitPayments);
            mLvSplitPayments.Adapter = mAdapter;
            mAdapter.SetupFooterListView();
        }

        private void SetContentsLayoutHeight(LinearLayout layout, SupportToolbar layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int layoutHeight = layout.Height;

            int _topMargin = dpToPixel(0);
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mCurrentToolbarMenu = menu;
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout_cart, menu);
            IMenuItem item = menu.FindItem(Resource.Id.menuItem_pricingType);
            item.SetTitle(GlobalVariables.mCurrentSelectedPricingType);
            IMenuItem selectedCustomerItem = menu.FindItem(Resource.Id.menuItem_customer_cart);
            selectedCustomerItem.SetActionView(GetButtonLayout(selectedCustomerItem));
            if (GlobalVariables.mHasSelectedCustomerOnCheckout)
            {
                ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
            }
            SetToolbarMenuIconTint(GlobalVariables.mCurrentSelectedPricingType);
            return base.OnCreateOptionsMenu(menu);
        }
        public void SetToolbarMenuIconTint(string _pricingType)
        {
            IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);
            Drawable drawable = item.Icon;
            if (drawable != null)
            {
                drawable.Mutate();
                drawable.SetColorFilter(_pricingType == "RUNR" ?
                    ColorHelper.ResourceIdToColor(Resource.Color.orange, this) :
                    ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this)
                    , PorterDuff.Mode.SrcAtop);
            }

        }
        public void ChangeSelectCustomerIcon(string customerName)
        {
            IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);

            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            View buttonView = inflater.Inflate(Resource.Layout.checkout_fragment_customer_name_button, null);
            LinearLayout borderContainer = buttonView.FindViewById<LinearLayout>(Resource.Id.llBorderContainer);
            TextView txtCustomerNameButtonTitle = buttonView.FindViewById<TextView>(Resource.Id.txtCustomerName);
            ImageView imgIcon = buttonView.FindViewById<ImageView>(Resource.Id.imgCustomerIcon);
            txtCustomerNameButtonTitle.Text = customerName;

            SetActionLayoutColor(borderContainer, imgIcon, txtCustomerNameButtonTitle);
            item.SetActionView(buttonView);
            subscribeCustomLayoutToClick(item);

            //set current selected customer button layout
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = buttonView;
        }
        public void SetActionLayoutColor(LinearLayout _borderContainer, ImageView _imgIcon, TextView _txtCustomerNameButtonTitle)
        {
            _txtCustomerNameButtonTitle.SetTextColor(GetColorStateList(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                Resource.Color.orange : Resource.Color.colorAccent));
            _imgIcon.SetColorFilter(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                        ColorHelper.ResourceIdToColor(Resource.Color.orange, this) :
                        ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
            _borderContainer.Background = GetDrawable(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                Resource.Drawable.roundborderOrange : Resource.Drawable.roundborder);
        }
        public View GetButtonLayout(IMenuItem _selectedCustomerItem)
        {
            View buttonView = null;

            if (GlobalVariables.mHasSelectedCustomerOnCheckout == true)
            {
                LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
                buttonView = inflater.Inflate(Resource.Layout.checkout_fragment_customer_name_button, null);
                TextView txtCustomerNameButtonTitle = buttonView.FindViewById<TextView>(Resource.Id.txtCustomerName);
                txtCustomerNameButtonTitle.Text = GlobalVariables.mCurrentSelectedCustomerOnCheckout;
            }

            return buttonView;
        }
        public void subscribeCustomLayoutToClick(IMenuItem item)
        {
            item.ActionView.Click += (sender, args) => {
                this.OnOptionsItemSelected(item);
            };
        }
        public void removeActionLayout(IMenuItem item)
        {
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = null;
            item.SetActionView(GlobalVariables.mCurrentSelectedCustomerButtonLayout);
        }
        private void FnGetData()
        {
            mTotalSaleAmount = Intent.GetDoubleExtra("totalSaleAmount", 0);
            double inputAmount = Intent.GetDoubleExtra("inputAmount",0);
            bool isCash = Intent.GetBooleanExtra("isCash", true);
            GenerateSplitPaymentList(Convert.ToDecimal(inputAmount),isCash);
        }
        private void GenerateSplitPaymentList(decimal _inputAmount, bool _isCash)
        {
            mSplitPayments = new List<SplitPayments>();
            mSplitPayments.Add(new SplitPayments() { 
                id = 1, 
                paymentType = _isCash ? "Cash" : "Check", 
                amount = _inputAmount
            });
        }
        private void FnSetEvents()
        {
            mBtnAddPayment.Click += MBtnAddPayment_Click;
        }
        private void MBtnAddPayment_Click(object sender, EventArgs e)
        {
            double _totalSaleAmount = mTotalSaleAmount;
            double _totalAmtReceived = GetCurrentSplitPaymentsAmount();

            if (_totalAmtReceived == 0 || (_totalAmtReceived < _totalSaleAmount))
            {
                if (!mDialogShown) //avoid double click
                {
                    mDialogShown = true;
                    //show dialog here
                    SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                    CheckoutFragmentPaymentAddPaymentDialogFragment addPaymentDialog = new CheckoutFragmentPaymentAddPaymentDialogFragment(this);
                    addPaymentDialog.SetStyle((int)DialogFragmentStyle.Normal, Resource.Style.Dialog_FullScreen);
                    var args = new Bundle();
                    args.PutDouble("totalSaleAmount", mTotalSaleAmount);
                    args.PutDouble("totalSplitPaymentsAmount", GetCurrentSplitPaymentsAmount());
                    addPaymentDialog.Arguments = args;
                    addPaymentDialog.Show(transaction, "checkoutFragmentPaymentAddPaymentDialogFragment");
                }
            }
            else if (_totalAmtReceived >= _totalSaleAmount)
            {
                //go to payment done page
                if (!mDialogShown) //avoid double click
                {
                    mDialogShown = true;
                    Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentDoneActivity));
                    intent.PutExtra("TotalSaleAmount", _totalSaleAmount);
                    intent.PutExtra("ChangeAmount", _totalAmtReceived - _totalSaleAmount);
                    intent.PutExtra("TransactionType", "PAYMENT");
                    StartActivityForResult(intent, 29);
                    SaveTransactionToDb();
                }
            }
        }

        private void SaveTransactionToDb()
        {
            //save to db here
        }

        private double GetCurrentSplitPaymentsAmount()
        {
            mSplitPayments = mAdapter.GetUpdatedData();
            return Convert.ToDouble(mSplitPayments.Sum(x => x.amount));
        }
        private void FnSetControls()
        {
            mLlContents = FindViewById<LinearLayout>(Resource.Id.llContents);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mTxtTotalSaleAmount = FindViewById<TextView>(Resource.Id.txtTotalSaleAmount);
            mLvSplitPayments = FindViewById<ListView>(Resource.Id.lvSplitPayments);
            mBtnAddPayment = FindViewById<Button>(Resource.Id.btnSave);
            mRlLabel = FindViewById<RelativeLayout>(Resource.Id.rlLabel);
            mTxtSplitpaymentLabel = FindViewById<TextView>(Resource.Id.txtLabel);
            mTxtSplitpaymentLabel.Text = SetSplitPaymentLabelMessage();
            mTxtTotalSaleAmount.Text = mPesoSign + string.Format("{0:n}", mTotalSaleAmount);
            mLvSplitPayments.SetSelector(Resource.Color.transparent);
        }

        public void SetSplitPaymentLabelVisibility()
        {
            var data = mAdapter.GetUpdatedData();
            bool _isVisible = data.Sum(x => x.amount) == 0 ? true : false;
            mRlLabel.Visibility = _isVisible ? ViewStates.Visible : ViewStates.Invisible;
        }

        private string SetSplitPaymentLabelMessage()
        {
            return "For a split payment, select the first method and enter " +
                "the amount to be paid. Repeat the process until you " +
                "reach up to the total amount of the sale.";
        }

        public void SetButtonAppearance(decimal _totalAmtReceived, decimal _totalSaleAmount)
        {
            if (_totalAmtReceived == 0 || (_totalAmtReceived < _totalSaleAmount))
            {
                mBtnAddPayment.SetTextColor(ColorHelper.ResourceIdToColor(Resource.Color.colorPrimary, this));
                mBtnAddPayment.Background = this.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderWithItem);
                mBtnAddPayment.Text = "Add Payment";
            }
            else if (_totalAmtReceived >= _totalSaleAmount)
            {
                mBtnAddPayment.SetTextColor(ColorHelper.ResourceIdToColor(Resource.Color.colorPrimary , this));
                mBtnAddPayment.Background = this.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderWithItem);
                mBtnAddPayment.Text = "Charge " + mPesoSign + String.Format("{0:n}", _totalSaleAmount);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menuItem_customer_cart:
                    if (!mDialogShown)
                    {
                        mDialogShown = true;
                        Intent intent = new Intent(this, typeof(CheckoutSelectCustomerActivity));
                        intent.PutExtra("isCustomer", GlobalVariables.mCurrentSelectedPricingType == "RUNR" ? false : true);
                        StartActivityForResult(intent, 27);
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void AddPaymentDialogFragmentActivityResult()
        {
            mDialogShown = false;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 27)
            {
                mDialogShown = false;
                if (GlobalVariables.mHasSelectedCustomerOnCheckout == true)
                {
                    ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
                }
                else
                {
                    IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);
                    removeActionLayout(item);
                }
            }
            else if (requestCode == 28)
            {
                double inputAmount = data.GetDoubleExtra("inputAmount", 0);
                bool isCash = data.GetBooleanExtra("isPaymentModeCash", true);
                bool isBackPressed = data.GetBooleanExtra("isBackPressed", true);
                if (isBackPressed)
                {
                    AddSplitPayment(Convert.ToDecimal(inputAmount), isCash);
                }
                SetButtonAppearance(Convert.ToDecimal(GetCurrentSplitPaymentsAmount()), Convert.ToDecimal(mTotalSaleAmount));
            }
            else if (requestCode == 29)
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
        private void AddSplitPayment(decimal _inputAmount, bool _isCash)
        {
            mSplitPayments.Add(new SplitPayments()
            {
                id = GetSplitPaymentId(),
                paymentType = _isCash ? "Cash" : "Check",
                amount = _inputAmount
            });
            mAdapter.RefreshListView();
            SetSplitPaymentLabelVisibility();
        }

        private int GetSplitPaymentId()
        {
            var retval = 0;
            try
            {
                retval = mSplitPayments.Max(x => x.id) + 1;
            }
            catch (Exception)
            {
                retval = 1;
            }
            return retval;
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = "Payment Methods";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}