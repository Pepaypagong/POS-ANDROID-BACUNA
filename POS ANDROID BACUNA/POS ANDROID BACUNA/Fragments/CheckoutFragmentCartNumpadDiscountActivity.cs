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
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes;
using Java.Util;
using Android.Graphics;
using POS_ANDROID_BACUNA.Adapters;
using Android.Text;
using Java.Lang;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentCartNumpadDiscountActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentCartNumpadDiscountActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;

        TextView mTxtItemNameAndQty;
        TextView mTxtDiscountedPrice;
        TextView mTxtOriginalPrice;

        TextView mTxtCashDiscount;
        EditText mEtCashDiscount;
        ImageView mImgCashDiscountCheck;

        TextView mTxtPercentageDiscount;
        EditText mEtPercentageDiscount;
        ImageView mImgPercentageDiscount;

        int cashDiscountMaxLength = 14;
        int percentageDiscountMaxLength = 8;
        string mPesoSign = "\u20b1";
        GridLayout mGridNumpad;
        TextView mTxtOK; //ok on numpad

        bool mIsCashDiscountEventAdded = true;
        bool mIsPercentDiscountEventAdded = false;
        MoneyTextWatcher cashDiscountTextWatch;
        MoneyTextWatcher percentDiscountTextWatch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_cart_child_numpad_discount);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarCheckoutCartNumpadDiscount);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            //actionBar.SetHomeAsUpIndicator(Resource.Drawable.abc_ic_ab_back_material);
            actionBar.SetDisplayHomeAsUpEnabled(true); 
            actionBar.SetDisplayShowHomeEnabled(true); //back icon
            actionBar.Title = GlobalVariables.mCurrentSelectedItemNameOnCart;

            SetViews();
            GlobalVariables.mIsCashDiscountSelected = true;
            FnSubscribeGridToEvent(); 
            FnPassDataFromCaller();
            
            cashDiscountTextWatch = new MoneyTextWatcher(mEtCashDiscount, mEtPercentageDiscount, mEtCashDiscount, mTxtOriginalPrice, mTxtDiscountedPrice, this);
            percentDiscountTextWatch = new MoneyTextWatcher(mEtPercentageDiscount, mEtPercentageDiscount, mEtCashDiscount, mTxtOriginalPrice, mTxtDiscountedPrice, this);

            //initial subscription to textchanged event
            mEtCashDiscount.AddTextChangedListener(cashDiscountTextWatch);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout_cart_discount, menu);

            IMenuItem selectedCustomerItem = menu.FindItem(Resource.Id.menuItem_clearDiscount);
            return base.OnCreateOptionsMenu(menu);
        }

        void SetViews()
        {
            mTxtItemNameAndQty = FindViewById<TextView>(Resource.Id.txtItemNameAndQty);
            mTxtDiscountedPrice = FindViewById<TextView>(Resource.Id.txtDiscountedPrice);
            mTxtOriginalPrice = FindViewById<TextView>(Resource.Id.txtOriginalPrice);

            mTxtCashDiscount = FindViewById<TextView>(Resource.Id.txtCashDiscount);
            mImgCashDiscountCheck = FindViewById<ImageView>(Resource.Id.imgCashDiscountCheck);
            mEtCashDiscount = FindViewById<EditText>(Resource.Id.etCashDiscount);
            mEtCashDiscount.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(cashDiscountMaxLength) });//set max length
            mEtCashDiscount.InputType = Android.Text.InputTypes.Null;
            mEtCashDiscount.Focusable = true;
            mEtCashDiscount.FocusableInTouchMode = true;
            mEtCashDiscount.RequestFocus(); //focus on load
            mEtCashDiscount.Click += MEtCashDiscount_Click;

            mTxtPercentageDiscount = FindViewById<TextView>(Resource.Id.txtPercentageDiscount);
            mImgPercentageDiscount = FindViewById<ImageView>(Resource.Id.imgPercentageDiscountCheck);
            mEtPercentageDiscount = FindViewById<EditText>(Resource.Id.etPercentageDiscount);
            mEtPercentageDiscount.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(percentageDiscountMaxLength) });//set max length
            mEtPercentageDiscount.InputType = Android.Text.InputTypes.Null;
            mEtPercentageDiscount.Focusable = false;
            mEtPercentageDiscount.FocusableInTouchMode = false;
            mEtPercentageDiscount.Click += MEtPercentageDiscount_Click;

            mGridNumpad = FindViewById<GridLayout>(Resource.Id.glNumPadDiscount);
            mTxtOK = FindViewById<TextView>(Resource.Id.txtNumpadCheckDiscount);
        }

        private void MEtPercentageDiscount_Click(object sender, EventArgs e)
        {
            FnSetEditableAppearance(false);
            GlobalVariables.mIsCashDiscountSelected = false;

            if (!mIsPercentDiscountEventAdded)
            {
                mEtCashDiscount.RemoveTextChangedListener(cashDiscountTextWatch);
                mEtPercentageDiscount.AddTextChangedListener(percentDiscountTextWatch);
            }
            mIsCashDiscountEventAdded = false;
            mIsPercentDiscountEventAdded = true;
        }

        private void MEtCashDiscount_Click(object sender, EventArgs e)
        {
            FnSetEditableAppearance(true);
            GlobalVariables.mIsCashDiscountSelected = true;

            if (!mIsCashDiscountEventAdded)
            {
                mEtCashDiscount.AddTextChangedListener(cashDiscountTextWatch);
                mEtPercentageDiscount.RemoveTextChangedListener(percentDiscountTextWatch);
            }
            mIsCashDiscountEventAdded = true;
            mIsPercentDiscountEventAdded = false;
        }

        void FnSetEditableAppearance(bool _isCashEnabled)
        {
            mImgCashDiscountCheck.Background = _isCashEnabled ? this.GetDrawable(Resource.Drawable.check_icon) : null;
            mEtCashDiscount.BackgroundTintList = GetColorStateList(_isCashEnabled ? Resource.Color.colorAccent : Resource.Color.colorBlurred);
            mEtCashDiscount.SetTextColor(ColorResourceToColorInt(_isCashEnabled ? Resource.Color.colorLightBlack : Resource.Color.colorBlurred,this));

            mImgPercentageDiscount.Background = _isCashEnabled ? null : this.GetDrawable(Resource.Drawable.check_icon);
            mEtPercentageDiscount.BackgroundTintList = GetColorStateList(_isCashEnabled ? Resource.Color.colorBlurred : Resource.Color.colorAccent);
            mEtPercentageDiscount.SetTextColor(ColorResourceToColorInt(_isCashEnabled ? Resource.Color.colorBlurred : Resource.Color.colorLightBlack, this));

            if (_isCashEnabled)
            {
                mEtCashDiscount.Focusable = true;
                mEtCashDiscount.FocusableInTouchMode = true;
                mEtPercentageDiscount.FocusableInTouchMode = false;
                mEtPercentageDiscount.Focusable = false;

                mEtCashDiscount.RequestFocus();
            }
            else
            {
                mEtCashDiscount.Focusable = false;
                mEtCashDiscount.FocusableInTouchMode = false;
                mEtPercentageDiscount.FocusableInTouchMode = true;
                mEtPercentageDiscount.Focusable = true;

                mEtPercentageDiscount.RequestFocus();
            }
        }

        void FnPassDataFromCaller()
        {
            string currentItemName = GlobalVariables.mCurrentSelectedItemNameOnCart;
            int currentItemCount = GlobalVariables.mCurrentSelectedItemQtyOnCart;
            decimal currentSubtotalDiscount = GlobalVariables.mCurrentSelectedItemDiscountAmountOnCart;
            decimal currentItemPrice = GlobalVariables.mCurrentSelectedItemPriceOnCart;
            decimal currentOriginalSubtotal = currentItemCount * currentItemPrice;
            decimal discountedSubtotal = currentOriginalSubtotal - currentSubtotalDiscount;

            mTxtItemNameAndQty.Text = currentItemName + " X " + currentItemCount.ToString();
            mTxtDiscountedPrice.Text = mPesoSign + string.Format("{0:n}", discountedSubtotal);

            if (currentOriginalSubtotal == discountedSubtotal)
            {
                mTxtOriginalPrice.Visibility = ViewStates.Invisible;
            }
            mTxtOriginalPrice.Text = mPesoSign +  string.Format("{0:n}", currentOriginalSubtotal);

            mEtCashDiscount.Text = mPesoSign + currentSubtotalDiscount.ToString();

            if (currentSubtotalDiscount == 0)
            {
                mEtPercentageDiscount.Text = "0.00%";
            }
            else
            {
                mEtPercentageDiscount.Text = string.Format("{0:n}", (currentSubtotalDiscount / currentOriginalSubtotal) * 100) + "%"; 
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    GlobalVariables.mIsFromRemoveItem = false;
                    Finish();
                    return true;
                case Resource.Id.menuItem_clearDiscount:
                    GlobalVariables.mIsFromRemoveItem = false;
                    ClearDiscount();
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void ClearDiscount()
        {
            foreach (var item in GlobalVariables.globalProductsOnCart)
            {
                if (item.productId == GlobalVariables.mCurrentSelectedItemIdOnCart)
                {
                    item.productDiscountAmount = 0.00M;
                    item.productSubTotalPrice = item.productCountOnCart * item.productPrice;
                }
            }
        }

        void FnSubscribeGridToEvent()
        {

            int childCount = mGridNumpad.ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                TextView numberPad = (TextView)mGridNumpad.GetChildAt(i);
                numberPad.Click += delegate (object sender, EventArgs e)
                {
                    EditText _selectedEdittext = GlobalVariables.mIsCashDiscountSelected ? mEtCashDiscount : mEtPercentageDiscount;

                    if (numberPad.Text == "Del")
                    {
                        if (_selectedEdittext.Text.Length <= 5 & GlobalVariables.mIsCashDiscountSelected == true)
                        {
                            string holder = _selectedEdittext.Text.Replace("\u20b1", "");

                            string firstDigit = holder.Substring(0, 1);
                            string secondDigit = holder.Substring(2, 1);

                            holder = holder.Remove(0, 1).Insert(0, "0"); //first digit always 0 on del
                            holder = holder.Remove(2, 1).Insert(2, firstDigit);
                            holder = holder.Remove(3, 1).Insert(3, secondDigit);
                            _selectedEdittext.Text = mPesoSign+holder;
                        }
                        else if (_selectedEdittext.Text.Length <= 5 & GlobalVariables.mIsCashDiscountSelected == false)
                        {
                            string holder = _selectedEdittext.Text.Replace("%", "");

                            string firstDigit = holder.Substring(0, 1);
                            string secondDigit = holder.Substring(2, 1);

                            holder = holder.Remove(0, 1).Insert(0, "0"); //first digit always 0 on del
                            holder = holder.Remove(2, 1).Insert(2, firstDigit);
                            holder = holder.Remove(3, 1).Insert(3, secondDigit);
                            _selectedEdittext.Text = holder+"%";
                        }
                        else
                        {
                            if (GlobalVariables.mIsCashDiscountSelected == true)
                            {
                                _selectedEdittext.Text = _selectedEdittext.Text.Remove(_selectedEdittext.Text.Length - 1, 1);
                            }
                            else
                            {
                                string holder = _selectedEdittext.Text;
                                holder = holder.Replace("%", "");
                                holder = holder.Remove(holder.Length - 1, 1);

                                _selectedEdittext.Text = holder;
                            }
                        }

                    }
                    else if (numberPad.Text == "Apply")
                    {
                        decimal newDiscount = Convert.ToDecimal(mEtCashDiscount.Text.Replace(",", "").Replace("\u20b1", ""));
                        GlobalVariables.mCurrentSelectedItemDiscountAmountOnCart = newDiscount;
                        foreach (var item in GlobalVariables.globalProductsOnCart)
                        {
                            if (item.productId == GlobalVariables.mCurrentSelectedItemIdOnCart)
                            {
                                item.productDiscountAmount = newDiscount;
                                item.productSubTotalPrice = (item.productCountOnCart * item.productPrice) - item.productDiscountAmount;
                            }
                        }
                        GlobalVariables.mIsFromRemoveItem = false;
                        Finish();
                    }
                    else
                    {
                        if (numberPad.Text == "0")
                        {
                            if (_selectedEdittext.Text == mPesoSign + "0.00")
                            {
                                //_selectedEdittext.Text = _selectedEdittext.Text + numberPad.Text;
                            }
                            else if (_selectedEdittext.Text == "0.00%")
                            {

                            }
                            else
                            {
                                _selectedEdittext.Text = _selectedEdittext.Text + numberPad.Text;
                            }
                        }
                        else
                        {
                            _selectedEdittext.Text = _selectedEdittext.Text + numberPad.Text;
                        }
                    }
                };
            }
        }


        Color ColorResourceToColorInt(int _colorResourceId, Context _context)
        {
            int colorInt = _context.GetColor(_colorResourceId);
            return new Color(colorInt);
        }

        public class MoneyTextWatcher : Java.Lang.Object, ITextWatcher
        {
            private bool mIsCashDiscountSelected;
            private EditText mEditText;
            private EditText mEtPercentageDiscount;
            private EditText mEtCashDiscount;
            private TextView mTxtOrigPrice;
            private TextView mTxtDiscountedPrice;
            private decimal currentOriginalSubtotal;

            public MoneyTextWatcher(EditText _mEditText, EditText _mEtPercentageDiscount, EditText _mEtCashDiscount,
                TextView _txtOrigPrice, TextView _txtDiscountedPrice,
                Context _context)
            {
                mEditText = _mEditText;
                mEtPercentageDiscount = _mEtPercentageDiscount;
                mEtCashDiscount = _mEtCashDiscount;
                mTxtOrigPrice = _txtOrigPrice;
                mTxtDiscountedPrice = _txtDiscountedPrice;
                currentOriginalSubtotal = GlobalVariables.mCurrentSelectedItemQtyOnCart * GlobalVariables.mCurrentSelectedItemPriceOnCart;
            }

            public void AfterTextChanged(IEditable s)
            {

            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {

            }

            public decimal GetPercentageDiscount(decimal _cashDiscount, decimal _originalPrice)
            {
                decimal retVal = 0.0M;
                retVal = (_cashDiscount / _originalPrice) * 100;
                return retVal;
            }

            public decimal GetCashDiscount(decimal _percentageDiscount, decimal _originalPrice)
            {
                decimal retVal = 0.0M;
                retVal = (_percentageDiscount * _originalPrice) / 100;
                return retVal;
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                mIsCashDiscountSelected = GlobalVariables.mIsCashDiscountSelected;

                string value = mIsCashDiscountSelected ? mEditText.Text.Replace(",", "").Replace("\u20b1", "").Replace(".", "").TrimStart('0') :
                    mEditText.Text.Replace(",", "").Replace("%", "").Replace(".", "").TrimStart('0');

                decimal ul;
                if (decimal.TryParse(value, out ul))
                {
                    ul /= 100;
                    mEditText.RemoveTextChangedListener(this);
                    if (mIsCashDiscountSelected)
                    {
                        if (ul >= currentOriginalSubtotal)
                        {
                            ul = currentOriginalSubtotal;
                        }
                        mEditText.Text = "\u20b1" + string.Format("{0:n}", ul);
                    }
                    else
                    {
                        if (ul >= 100)
                        {
                            ul = 100;
                        }
                        mEditText.Text = string.Format("{0:n}", ul) + "%";
                    }
                    
                    mEditText.AddTextChangedListener(this);
                }
                if (ul == 0)
                {
                    mTxtOrigPrice.Visibility = ViewStates.Invisible;
                }
                else
                {
                    mTxtOrigPrice.Visibility = ViewStates.Visible;
                }
                if (mIsCashDiscountSelected)
                {
                    mEtPercentageDiscount.Text = string.Format("{0:n}", GetPercentageDiscount(ul, currentOriginalSubtotal)) + "%";
                    mTxtDiscountedPrice.Text = "\u20b1" + string.Format("{0:n}", (currentOriginalSubtotal - ul));
                }
                else
                {
                    mEtCashDiscount.Text = "\u20b1" + string.Format("{0:n}", GetCashDiscount(ul, currentOriginalSubtotal));
                    decimal cashDiscountAmtmEtCashDiscount = Convert.ToDecimal(mEtCashDiscount.Text.Replace("\u20b1",""));
                    mTxtDiscountedPrice.Text = "\u20b1" + string.Format("{0:n}", (currentOriginalSubtotal - cashDiscountAmtmEtCashDiscount));
                }
                
            }

        }
    }
}