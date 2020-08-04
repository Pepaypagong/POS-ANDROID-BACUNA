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


namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentMultiSizeAdd", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentMultiSizeAdd : AppCompatActivity
    {
        SupportToolbar mToolBar;
        RelativeLayout mRlSizesHeader;

        CheckBox mCbSelectAll;
        TextView mTxtAll0;
        TextView mTxtAllMinus1;
        TextView mTxtAllQuantity;
        TextView mTxtAllPlus1;
        TextView mTxtAllPlus6;
        TextView mTxtAllPlus12;

        RecyclerView mRvSizes;
        RecyclerView.LayoutManager mLayoutManager;
        RecyclerView.Adapter mMultiAddAdapter;
        CheckoutMultiAddListRecyclerViewAdapter _checkoutMultiAddListRecyclerViewAdapter;
        LinearLayout mllCheckoutCartButtonContainer;
        ImageButton mBtnCheckoutMultiAddClear;
        Button mBtnCheckoutMultiAddAddToCart;
        float mDpVal;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_multi_size_add);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnSetUpToolBar();
            FnSetControls();
            FnSetControlClickEvents();
            //Window.SetWindowAnimations(Resource.Style.dialog_animation);
            SetListLayout();
            Window.DecorView.Post(() =>
            {
                SetRecyclerViewLayoutHeight(mRvSizes, mRlSizesHeader);
            });
        }

        private void SetListLayout()
        {
            //Create Layout Manager
            mLayoutManager = new LinearLayoutManager(this);

            mRvSizes.SetLayoutManager(mLayoutManager);
            mRvSizes.HasFixedSize = true;

            _checkoutMultiAddListRecyclerViewAdapter = new CheckoutMultiAddListRecyclerViewAdapter(PopulateParentProductSizes(), 
                mRvSizes, mBtnCheckoutMultiAddAddToCart, this, GlobalVariables.mCurrentSelectedPricingType);
            mMultiAddAdapter = _checkoutMultiAddListRecyclerViewAdapter;
            mRvSizes.SetAdapter(mMultiAddAdapter);   
        }

        private List<Product> PopulateParentProductSizes()
        {
            int selectedParentProductId = GlobalVariables.mCurrentSelectedItemIdMultiSize;
            List<Product> mProducts = new List<Product>();

            //get from global product list
            mProducts = GlobalVariables.globalProductList;

            //filter by product id
            mProducts = mProducts.Where(o => o.parentProductId == selectedParentProductId).ToList();

            return mProducts;
        }
        private void FnSetControlClickEvents()
        {
            mCbSelectAll.Click += MCbSelectAll_Click;
            mTxtAll0.Click += MTxtAll0_Click;
            mTxtAllMinus1.Click += MTxtAllMinus1_Click;
            mTxtAllPlus1.Click += MTxtAllPlus1_Click;
            mTxtAllPlus6.Click += MTxtAllPlus6_Click;
            mTxtAllPlus12.Click += MTxtAllPlus12_Click;

            mBtnCheckoutMultiAddAddToCart.Click += MBtnCheckoutMultiAddAddToCart_Click;
        }

        private void ChangeAllQuantityValues(string _newValue)
        {
            //iterate the recyclerview items
            for (int i = 0; i < mRvSizes.GetAdapter().ItemCount; i++)
            {
                View holder = mLayoutManager.FindViewByPosition(i);
                TextView txtQuantity = holder.FindViewById<TextView>(Resource.Id.txtQty);
                txtQuantity.Text = _newValue;
            }
        }

        private void CheckUncheckAllItems(bool _isToCheck)
        {
            //iterate the recyclerview items
            for (int i = 0; i < mRvSizes.GetAdapter().ItemCount; i++)
            {
                View holder = mLayoutManager.FindViewByPosition(i);
                CheckBox checkBox = holder.FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
                checkBox.Checked = _isToCheck ? true : false;
            }
        }

        private void MBtnCheckoutMultiAddAddToCart_Click(object sender, EventArgs e)
        {
            if (IsAnItemChecked() & IsItemTotalQtyGreaterThan0())
            {
                TextView itemId;
                TextView itemSelectedPrice;
                TextView itemQuantity;
                CheckBox itemChecked;

                //iterate the recyclerview items
                for (int i = 0; i < mRvSizes.GetAdapter().ItemCount; i++)
                {
                    View holder = mLayoutManager.FindViewByPosition(i);
                    itemId = holder.FindViewById<TextView>(Resource.Id.txtItemId);
                    itemSelectedPrice = holder.FindViewById<TextView>(Resource.Id.txtItemPrice);
                    itemQuantity = holder.FindViewById<TextView>(Resource.Id.txtQty);
                    itemChecked = holder.FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);

                    if (itemChecked.Checked)
                    {
                        AddItemsToCart(Convert.ToInt32(itemId.Text), Convert.ToInt32(itemQuantity.Text), Convert.ToDecimal(itemSelectedPrice.Text));
                    }
                }

                //close view then update total button
                Finish();
            }
            else
            {
                DialogMessageService.MessageBox(this,"No item selected","Please select item(s) to add to cart.");
            }
        }

        private bool IsAnItemChecked()
        {
            CheckBox itemChecked;
            bool anItemIsChecked = false;

            //iterate the recyclerview items
            for (int i = 0; i < mRvSizes.GetAdapter().ItemCount; i++)
            {
                View holder = mLayoutManager.FindViewByPosition(i);
                itemChecked = holder.FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);

                if (itemChecked.Checked)
                {
                    anItemIsChecked = true;
                }
            }

            return anItemIsChecked;
        }

        private bool IsItemTotalQtyGreaterThan0()
        {
            TextView txtQty;
            bool retVal = false;

            //iterate the recyclerview items
            for (int i = 0; i < mRvSizes.GetAdapter().ItemCount; i++)
            {
                View holder = mLayoutManager.FindViewByPosition(i);
                txtQty = holder.FindViewById<TextView>(Resource.Id.txtQty);

                if (txtQty.Text != "0")
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        private void AddItemsToCart(int _itemId, int _itemQty, decimal _itemPrice)
        {
            bool alreadyExists = GlobalVariables.globalProductsOnCart.Any(x => x.productId == _itemId);

            if (alreadyExists)
            {
                foreach (var item in GlobalVariables.globalProductsOnCart)
                {
                    if (item.productId == _itemId)
                    {
                        item.productPrice = _itemPrice; //update price on click to reset the price on cart
                        item.productDiscountAmount = 0.00M; //reset discount amt to 0 if already on cart
                        item.productDiscountPercentage = 0.00M; //reset discount percentage to 0 if already on cart
                        item.productCountOnCart = item.productCountOnCart + _itemQty;
                        item.productSubTotalPrice = (item.productCountOnCart) * item.productPrice;
                    }
                }
            }
            else
            {
                string productName = "";
                decimal productRetailPrice = 0.00M;
                int productCategoryId = 1;
                int parentProductId = 1;

                //get from db here where item id
                foreach (var item in GlobalVariables.globalProductList)
                {
                    if (item.productId == _itemId)
                    {
                        productName = item.productName;
                        productRetailPrice = item.productRetailPrice;
                        productCategoryId = item.productCategoryId;
                        parentProductId = item.parentProductId;
                    }
                }

                GlobalVariables.globalProductsOnCart.Add(new ProductsOnCart()
                {
                    productId = _itemId,
                    productName = productName,
                    productOrigPrice = _itemPrice,
                    productPrice = _itemPrice,
                    productCountOnCart = _itemQty,
                    productCategoryId = productCategoryId,
                    productSubTotalPrice = _itemQty * _itemPrice,
                    productDiscountAmount = 0.00M,
                    productDiscountPercentage = 0.00M,
                    parentProductId = parentProductId
                });
            }
        }

        public string ComputeQuantity(string _value, bool _isToAdd)
        {
            string retVal = "";
            int maxQty = 999;
            int minQty = 0;
            int currentQty = Convert.ToInt32(mTxtAllQuantity.Text);
            int inputValue = Convert.ToInt32(_value);
            int result = 0;

            if (_isToAdd)
            {
                result = currentQty + inputValue;
                if (result > maxQty)
                {
                    result = maxQty;
                }
            }
            else
            {
                result = currentQty - inputValue;
                if (result < minQty)
                {
                    result = minQty;
                }
            }

            retVal = result.ToString();
            return retVal;
        }

        private void MTxtAllPlus12_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("12", true);
            ChangeAllQuantityValues(mTxtAllQuantity.Text);
        }

        private void MTxtAllPlus6_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("6", true);
            ChangeAllQuantityValues(mTxtAllQuantity.Text);
        }

        private void MTxtAllPlus1_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("1", true);
            ChangeAllQuantityValues(mTxtAllQuantity.Text);
        }

        private void MTxtAllMinus1_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("1", false);
            ChangeAllQuantityValues(mTxtAllQuantity.Text);
        }

        private void MTxtAll0_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = "0";
            CheckUncheckAllItems(false);
            ChangeAllQuantityValues(mTxtAllQuantity.Text);
            mCbSelectAll.Text = "Select All";
            mCbSelectAll.Checked = false;
        }

        private void MCbSelectAll_Click(object sender, EventArgs e)
        {
            //CheckUncheckAllItems(mCbSelectAll.Checked);
            mCbSelectAll.Text =  mCbSelectAll.Checked ? "Unselect All" : "Select All";
            Toast.MakeText(this, mCbSelectAll.Checked ? "Checked All" : "Unchecked All", ToastLength.Long).Show();
            _checkoutMultiAddListRecyclerViewAdapter.CheckAllItems(mCbSelectAll.Checked);
            mMultiAddAdapter.NotifyDataSetChanged();
        }

        private void FnSetControls()
        {
            mRlSizesHeader = FindViewById<RelativeLayout>(Resource.Id.rlSizesHeader);
            mCbSelectAll = FindViewById<CheckBox>(Resource.Id.cbAllSizes);
            mTxtAll0 = FindViewById<TextView>(Resource.Id.txtQtyToZero);
            mTxtAllMinus1 = FindViewById<TextView>(Resource.Id.txtQtyMinusAll1);
            mTxtAllQuantity = FindViewById<TextView>(Resource.Id.txtQuantityAllSize);
            mTxtAllPlus1 = FindViewById<TextView>(Resource.Id.txtQtyPlusAll);
            mTxtAllPlus6 = FindViewById<TextView>(Resource.Id.txtQtyPlusAll6);
            mTxtAllPlus12 = FindViewById<TextView>(Resource.Id.txtQtyPlusAll12);

            mRvSizes = FindViewById<RecyclerView>(Resource.Id.rvSizes);
            mllCheckoutCartButtonContainer = FindViewById<LinearLayout>(Resource.Id.llCheckoutCartButtonContainer);
            mBtnCheckoutMultiAddClear = FindViewById<ImageButton>(Resource.Id.btnCheckoutMultiAddClear);
            mBtnCheckoutMultiAddAddToCart = FindViewById<Button>(Resource.Id.btnCheckoutMultiAddAddToCart);
        }

        private void SetRecyclerViewLayoutHeight(RecyclerView layout, RelativeLayout layoutBelow)
        {
            int checkoutButtonHeight = mllCheckoutCartButtonContainer.Height;
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
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarCheckoutMultiSizeAdd);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = GlobalVariables.mCurrentSelectedItemNameMultiSize;
        }

        protected override void OnPause()
        {
            GlobalVariables.mIsCheckoutFragmentMultiSizeAddOpened = false;
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        protected override void OnDestroy()
        {
            GlobalVariables.mIsCheckoutFragmentMultiSizeAddOpened = false;
            CheckoutFragment fragment_obj = (CheckoutFragment)GlobalVariables.mCheckoutFragmentCurrentInstance;
            fragment_obj.SetCheckoutButtonTotal(fragment_obj.mBtnCheckoutButton,fragment_obj.Context);
            base.OnDestroy();
        }

    }
}