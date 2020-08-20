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
    [Activity(Label = "CheckoutFragmentPaymentActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentPaymentActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        Button mBtnNext;
        TextView mTxtTotalSaleAmount;
        Button mBtnSaveOrderOrPayLater;

        RelativeLayout mRlCash1,mRlCash2;
        ImageView mImgCash;
        TextView mTxtCash;

        RelativeLayout mRlCheck1,mRlCheck2;
        ImageView mImgCheck;
        TextView mTxtCheck;

        LinearLayout mLlContents;
        LinearLayout mLlSaveButtonContainer;
        IMenu mCurrentToolbarMenu;
        float mDpVal;
        double mTotalSaleAmount;

        string mPesoSign = "\u20b1";
        float TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = 50;
        bool mIsCashSelectedPaymentMode = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_payment);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mLlContents, mToolBar);
            });
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
            mTotalSaleAmount = Intent.GetDoubleExtra("TotalSaleAmount", 0);
        }
        private void FnSetEvents()
        {
            if (IsSaveOrder())
            {
                mBtnSaveOrderOrPayLater.Click += MBtnSaveOrder_Click;
            }
            else
            {
                mBtnSaveOrderOrPayLater.Click += MBtnPayLater_Click;
            }
            mBtnNext.Click += MBtnNext_Click;
            mRlCash1.Click += MRlCash1_Click;
            mRlCash2.Click += MRlCash1_Click;
            mImgCash.Click += MRlCash1_Click;
            mTxtCash.Click += MRlCash1_Click;
            mRlCheck1.Click += MRlCheck1_Click;
            mRlCheck2.Click += MRlCheck1_Click;
            mImgCheck.Click += MRlCheck1_Click;
            mTxtCheck.Click += MRlCheck1_Click;
        }

        private bool IsSaveOrder()
        {
            bool retval = true;
            if (GlobalVariables.mCurrentSelectedPricingType == "RUNR")
            {
                mBtnSaveOrderOrPayLater.Text = "PAY LATER";
                retval = false;
            }
            else //retail wholesale
            {
                mBtnSaveOrderOrPayLater.Text = "SAVE ORDER";
                retval = true;
            }
            return retval;
        }

        private void MBtnPayLater_Click(object sender, EventArgs e)
        {
            if (GlobalVariables.mHasSelectedCustomerOnCheckout)
            {
                if (!mDialogShown) //avoid double click
                {
                    mDialogShown = true;
                    Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentPayLaterActivity));
                    intent.PutExtra("TotalSaleAmount", mTotalSaleAmount);
                    StartActivityForResult(intent, 31);
                }
            }
            else
            {
                DialogMessageService.MessageBox(this, "No runner selected", "Please select a runner first.");
            }
        }

        private void MBtnSaveOrder_Click(object sender, EventArgs e)
        {
            if (!mDialogShown) //avoid double click
            {
                mDialogShown = true;
                Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentDoneActivity));
                intent.PutExtra("TotalSaleAmount", mTotalSaleAmount);
                intent.PutExtra("ChangeAmount", 0);
                intent.PutExtra("TransactionType", "ORDER");
                StartActivityForResult(intent, 30);
            }
        }

        private void MRlCheck1_Click(object sender, EventArgs e)
        {
            mIsCashSelectedPaymentMode = false;
            SetPaymentMethodColors(false);
            mBtnNext.PerformClick();
        }

        private void MRlCash1_Click(object sender, EventArgs e)
        {
            mIsCashSelectedPaymentMode = true;
            SetPaymentMethodColors(true);
            mBtnNext.PerformClick();
        }

        private void SetPaymentMethodColors(bool _isCashSelected)
        {
            mImgCash.SetColorFilter(_isCashSelected ?
                        ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this) :
                        ColorHelper.ResourceIdToColor(Resource.Color.colorLightBlack, this));
            mTxtCash.SetTextColor(_isCashSelected ? ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this) :
                                                    ColorHelper.ResourceIdToColor(Resource.Color.colorLightBlack, this));
            mImgCheck.SetColorFilter(_isCashSelected ?
                        ColorHelper.ResourceIdToColor(Resource.Color.colorLightBlack, this) :
                        ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
            mTxtCheck.SetTextColor(_isCashSelected ? ColorHelper.ResourceIdToColor(Resource.Color.colorLightBlack, this) :
                                                    ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
        }

        private void MBtnNext_Click(object sender, EventArgs e)
        {
            if (GlobalVariables.mCurrentSelectedPricingType == "RUNR")
            {
                if (GlobalVariables.mHasSelectedCustomerOnCheckout)
                {
                    if (!mDialogShown) //avoid double click
                    {
                        mDialogShown = true;
                        Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentNumpadActivity));
                        intent.PutExtra("isInitialCall", true);
                        intent.PutExtra("isPaymentModeCash", mIsCashSelectedPaymentMode);
                        intent.PutExtra("totalSaleAmount", mTotalSaleAmount);
                        StartActivityForResult(intent, 24);
                    }
                }
                else
                {
                    DialogMessageService.MessageBox(this, "No runner selected", "Please select a runner first.");
                }
            }
            else
            {
                if (!mDialogShown)
                {
                    mDialogShown = true;
                    Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentNumpadActivity));
                    intent.PutExtra("isInitialCall", true);
                    intent.PutExtra("isPaymentModeCash", mIsCashSelectedPaymentMode);
                    intent.PutExtra("totalSaleAmount", mTotalSaleAmount);
                    StartActivityForResult(intent, 24);
                }
            }
        }

        private void FnSetControls()
        {
            mTxtTotalSaleAmount = FindViewById<TextView>(Resource.Id.txtAmtToPay);
            mBtnSaveOrderOrPayLater = FindViewById<Button>(Resource.Id.btnSaveOrder);
            mRlCash1 = FindViewById<RelativeLayout>(Resource.Id.rlCash1);
            mRlCash2 = FindViewById<RelativeLayout>(Resource.Id.rlCash2);
            mImgCash = FindViewById<ImageView>(Resource.Id.imgCash);
            mTxtCash = FindViewById<TextView>(Resource.Id.txtCash);

            mRlCheck1 = FindViewById<RelativeLayout>(Resource.Id.rlCheck1);
            mRlCheck2 = FindViewById<RelativeLayout>(Resource.Id.rlCheck2);
            mImgCheck = FindViewById<ImageView>(Resource.Id.imgCheck);
            mTxtCheck = FindViewById<TextView>(Resource.Id.txtCheck);

            mBtnNext = FindViewById<Button>(Resource.Id.btnSave);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mLlContents = FindViewById<LinearLayout>(Resource.Id.llContents);

            mTxtTotalSaleAmount.Text = mPesoSign + string.Format("{0:n}", mTotalSaleAmount);

            int characterCount = mTxtTotalSaleAmount.Text.Count();
            TOTAL_SALE_AMOUNT_TEXT_SIZE_SP = characterCount > 13 ? 35 : 50;
            mTxtTotalSaleAmount.SetTextSize(Android.Util.ComplexUnitType.Sp, TOTAL_SALE_AMOUNT_TEXT_SIZE_SP);
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
                        StartActivityForResult(intent, 23);
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 23 || requestCode == 24 || requestCode == 30 || requestCode == 31)
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
                try
                {
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
            actionBar.Title = "Payment";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}