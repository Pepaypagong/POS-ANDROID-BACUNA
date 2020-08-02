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
    [Activity(Label = "CheckoutFragmentCartNumpadActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentCartNumpadActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        TextView mTxtOldQty; //label 1from top top bottom
        TextView mTxtNewQty; //label 2
        TextView mTxtRemoveItem; //label 3

        EditText mEtOldNumpadValue; //edit text 1
        public EditText mEtNumpadValue; //edit text 2
        TextView mTxtOK; //ok on numpad

        GridLayout mGridNumpad;
        string mPesoSign = "\u20b1";

        int changeQtyMaxLength = 4;
        int changePriceMaxLength = 14;

        bool isInitialPriceChange = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_cart_child_numpad);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarCheckoutCartNumpad);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            //actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon_thin);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true); //back icon
            actionBar.Title = GlobalVariables.mCurrentSelectedItemNameOnCart;

            SetViews(NumPadType()); //set labels based on numpad type
            FnSubscribeGridToEvent(NumPadType());
            FnPassDataFromCaller(NumPadType());
        }

        private void MRemoveItem_Click(object sender, EventArgs e)
        {
            int currentSelectedItem = GlobalVariables.mCurrentSelectedItemIdOnCart;
            var itemToRemove = GlobalVariables.globalProductsOnCart.Single(item => item.productId == currentSelectedItem);
            GlobalVariables.globalProductsOnCart.Remove(itemToRemove);

            GlobalVariables.mIsAllCollapsed = true;
            GlobalVariables.mIsFromRemoveItem = true;
            Finish();
        }

        string NumPadType()
        {
            return Intent.GetStringExtra("numpadType") ?? "data not available";
        }

        void SetViews(string _numpadType)
        {
            //change_qty, change_price, change_discount

            mEtNumpadValue = FindViewById<EditText>(Resource.Id.etNumpadValue);
            mEtNumpadValue.InputType = Android.Text.InputTypes.Null;
            mEtNumpadValue.SetSelection(mEtNumpadValue.Text.Length);

            mEtOldNumpadValue = FindViewById<EditText>(Resource.Id.etOldQty);
            mEtOldNumpadValue.InputType = Android.Text.InputTypes.Null;

            mTxtRemoveItem = FindViewById<TextView>(Resource.Id.txtRemoveItem);
            mGridNumpad = FindViewById<GridLayout>(Resource.Id.glNumPad);

            mTxtOldQty = FindViewById<TextView>(Resource.Id.txtOldQty);
            mTxtNewQty = FindViewById<TextView>(Resource.Id.txtNewQty);
            mTxtOK = FindViewById<TextView>(Resource.Id.txtNumpadCheck);

            if (_numpadType == "change_qty")
            {
                mTxtOldQty.Text = "Current quantity";
                mTxtNewQty.Text = "New quantity";
                mTxtRemoveItem.Text = "Remove item";

                mTxtRemoveItem.SetTextColor(ColorResourceToColorInt(Resource.Color.colorRed,this));
                mTxtRemoveItem.Click += MRemoveItem_Click;
                mEtNumpadValue.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(changeQtyMaxLength) });//set max length
                mEtNumpadValue.RequestFocus();
            }
            else if (_numpadType == "change_price")
            {
                mTxtOldQty.Text = "Current item price";
                mTxtNewQty.Text = "Edit item price";
                mTxtRemoveItem.Text = "The price of this product will be changed for this sale only";
                mTxtRemoveItem.SetTextColor(ColorResourceToColorInt(Resource.Color.colorBlurred, this));
                mTxtRemoveItem.Enabled = false;
                mEtNumpadValue.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(changePriceMaxLength) });//set max length
                mEtNumpadValue.RequestFocus();
            }
            else if (_numpadType == "change_discount")
            {
                mTxtOldQty.Text = "Original Subtotal";
                mTxtNewQty.Text = "Edit discount amount";
                mTxtRemoveItem.Text = "";
                mTxtRemoveItem.Enabled = false;
                mEtNumpadValue.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(changePriceMaxLength) });//set max length
                mEtNumpadValue.RequestFocus();
            }
            else
            {
                //data not available error
            }
        }
        void FnPassDataFromCaller(string _numpadType)
        {
            if (_numpadType == "change_qty")
            {
                mEtOldNumpadValue.Text = GlobalVariables.mCurrentSelectedItemQtyOnCart.ToString();
                mEtNumpadValue.Text = "";
            }
            else if (_numpadType == "change_price")
            {
                mEtOldNumpadValue.Text = mPesoSign + string.Format("{0:n}", GlobalVariables.mCurrentSelectedItemPriceOnCart);
                mEtNumpadValue.Text = mPesoSign + string.Format("{0:n}", GlobalVariables.mCurrentSelectedItemPriceOnCart);
            }
            else if (_numpadType == "change_discount")
            {
                //orig subtotal
                mEtOldNumpadValue.Text = mPesoSign + string.Format("{0:n}", GlobalVariables.mCurrentSelectedItemQtyOnCart * GlobalVariables.mCurrentSelectedItemPriceOnCart);
                
                mEtNumpadValue.Text = mPesoSign + string.Format("{0:n}", GlobalVariables.mCurrentSelectedItemDiscountAmountOnCart);
            }
            else
            {
                //data not available error
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
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void FnSubscribeGridToEvent(string _numpadType)
        {
            if (_numpadType == "change_qty")
            {
                ChangeQtyEvent();
            }
            else if (_numpadType == "change_price")
            {
                ChangePriceEvent();
                mEtNumpadValue.AddTextChangedListener(new MoneyTextWatcher("", mEtNumpadValue, mTxtOK,this,true,isInitialPriceChange));
            }
            else if (_numpadType == "change_discount")
            {
                ChangeDiscountEvent();
                mEtNumpadValue.AddTextChangedListener(new MoneyTextWatcher("", mEtNumpadValue, mTxtOK, this,false,false));
            }
            else
            {
                //data not available error
            }
        }

        private void ChangeDiscountEvent()
        {
            int childCount = mGridNumpad.ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                TextView numberPad = (TextView)mGridNumpad.GetChildAt(i);
                numberPad.Click += delegate (object sender, EventArgs e) {
                    if (isInitialPriceChange)
                    {
                        isInitialPriceChange = false;
                        if (numberPad.Text != "OK")
                        {
                            mEtNumpadValue.Text = mPesoSign + "0.00";
                        }
                    }
                    if (numberPad.Text == "Del")
                    {
                        if (mEtNumpadValue.Text.Length <= 5)
                        {
                            string holder = mEtNumpadValue.Text;

                            string firstDigit = holder.Substring(1, 1);
                            string secondDigit = holder.Substring(3, 1);

                            holder = holder.Remove(1, 1).Insert(1, "0"); //first digit always 0 on del
                            holder = holder.Remove(3, 1).Insert(3, firstDigit);
                            holder = holder.Remove(4, 1).Insert(4, secondDigit);
                            mEtNumpadValue.Text = holder;
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text.Remove(mEtNumpadValue.Text.Length - 1, 1);
                        }

                    }
                    else if (numberPad.Text == "OK")
                    {
                        decimal newDiscount = Convert.ToDecimal(mEtNumpadValue.Text.Replace(",", "").Replace("\u20b1", ""));
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
                            if (mEtNumpadValue.Text == mPesoSign + "0.00")
                            {
                                //mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                            else
                            {
                                mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                        }
                    }
                };
            }
        }

        void ChangePriceEvent()
        {
            int childCount = mGridNumpad.ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                TextView numberPad = (TextView)mGridNumpad.GetChildAt(i);
                numberPad.Click += delegate (object sender, EventArgs e) {
                    if (isInitialPriceChange)
                    {
                        isInitialPriceChange = false;
                        if (numberPad.Text != "OK")
                        {
                            mEtNumpadValue.Text = mPesoSign + "0.00";
                        }
                    }
                    if (numberPad.Text == "Del")
                    {
                        if (mEtNumpadValue.Text.Length <= 5)
                        {
                            string holder = mEtNumpadValue.Text;

                            string firstDigit = holder.Substring(1,1);
                            string secondDigit = holder.Substring(3, 1);

                            holder = holder.Remove(1, 1).Insert(1, "0"); //first digit always 0 on del
                            holder = holder.Remove(3, 1).Insert(3, firstDigit);
                            holder = holder.Remove(4, 1).Insert(4, secondDigit);
                            mEtNumpadValue.Text = holder;
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text.Remove(mEtNumpadValue.Text.Length - 1, 1);
                        }

                    }
                    else if (numberPad.Text == "OK")
                    {
                        decimal newPrice = Convert.ToDecimal(mEtNumpadValue.Text.Replace(",", "").Replace("\u20b1", ""));
                        GlobalVariables.mCurrentSelectedItemPriceOnCart = newPrice;
                        foreach (var item in GlobalVariables.globalProductsOnCart)
                        {
                            if (item.productId == GlobalVariables.mCurrentSelectedItemIdOnCart)
                            {
                                item.productPrice = newPrice;
                                item.productSubTotalPrice = (item.productCountOnCart) * item.productPrice;
                                item.productDiscountAmount = 0.00M; //revert discount to 0
                            }
                        }
                        GlobalVariables.mIsFromRemoveItem = false;
                        Finish();
                    }
                    else
                    {
                        if (numberPad.Text == "0")
                        {
                            if (mEtNumpadValue.Text == mPesoSign + "0.00"){
                                //mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                            else {
                                mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                        }
                    }
                };
            }
        }

        void ChangeQtyEvent()
        {
            int childCount = mGridNumpad.ChildCount;

            for (int i = 0; i < childCount; i++)
            {
                TextView numberPad = (TextView)mGridNumpad.GetChildAt(i);
                numberPad.Click += delegate (object sender, EventArgs e) {
                    if (numberPad.Text == "Del")
                    {
                        if (mEtNumpadValue.Text.Length <= 1)
                        {
                            mEtNumpadValue.Text = "";
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text.Remove(mEtNumpadValue.Text.Length - 1, 1);
                        }

                    }
                    else if (numberPad.Text == "OK")
                    {
                        if (mEtNumpadValue.Text == "")
                        {
                            mEtNumpadValue.Text = mEtOldNumpadValue.Text;
                            GlobalVariables.mIsFromRemoveItem = false;
                            Finish();
                        }
                        else
                        {
                            int newQty = Convert.ToInt32(mEtNumpadValue.Text);
                            GlobalVariables.mCurrentSelectedItemQtyOnCart = newQty;
                            foreach (var item in GlobalVariables.globalProductsOnCart)
                            {
                                if (item.productId == GlobalVariables.mCurrentSelectedItemIdOnCart)
                                {
                                    item.productCountOnCart = newQty;
                                    item.productSubTotalPrice = (item.productCountOnCart) * item.productPrice;
                                    item.productDiscountAmount = 0.00M; //revert discount to 0
                                }
                            }
                            GlobalVariables.mIsFromRemoveItem = false;
                            Finish();
                        }
                    }
                    else
                    {
                        if (numberPad.Text == "0")
                        {
                            if (mEtNumpadValue.Text != "")
                            {
                                mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
                            }
                        }
                        else
                        {
                            mEtNumpadValue.Text = mEtNumpadValue.Text + numberPad.Text;
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
            private EditText mEditText;
            private TextView mNumpadOk;
            private Context mContext;
            private bool mIsForPrice;
            private bool mIsInitialLoad;
            public MoneyTextWatcher(string _current, EditText _mEditText, TextView _txtNumpadCheck, Context _context, bool _isForPrice, bool _isInitialLoad)
            {
                mEditText = _mEditText;
                mNumpadOk = _txtNumpadCheck;
                mContext = _context;
                mIsForPrice = _isForPrice;
                mIsInitialLoad = _isInitialLoad;
            }

            public void AfterTextChanged(IEditable s)
            {
                if (mIsInitialLoad)
                {
                    mIsInitialLoad = false;
                    //disable ok
                    int colorInt = mContext.GetColor(Resource.Color.jepoyGray);
                    Color color = new Color(colorInt);
                    mNumpadOk.SetTextColor(color);
                    mNumpadOk.Enabled = false;
                }
               
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {

            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                string value = mEditText.Text.Replace(",", "").Replace("\u20b1", "").Replace(".", "").TrimStart('0');
                if (mIsForPrice & value == "")
                {
                    //disable ok
                    int colorInt = mContext.GetColor(Resource.Color.jepoyGray);
                    Color color = new Color(colorInt);
                    mNumpadOk.SetTextColor(color);
                    mNumpadOk.Enabled = false;
                }
                else {
                    //enable ok
                    int colorInt = mContext.GetColor(Resource.Color.colorAccent);
                    Color color = new Color(colorInt);
                    mNumpadOk.SetTextColor(color);
                    mNumpadOk.Enabled = true;
                }

                decimal ul;
                if (decimal.TryParse(value, out ul))
                {
                    ul /= 100;
                    mEditText.RemoveTextChangedListener(this);
                    mEditText.Text = "\u20b1"+string.Format("{0:n}", ul);
                    mEditText.AddTextChangedListener(this);
                }
            }

        }
    }
}