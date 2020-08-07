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
        List<MultiSizeAddProductsHolder> mProductHolder;
        RecyclerView mRvSizes;
        RecyclerView.LayoutManager mLayoutManager;
        RecyclerView.Adapter mMultiAddAdapter;
        CheckoutMultiAddListRecyclerViewAdapter _Adapter;
        LinearLayout mllCheckoutCartButtonContainer;
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout_multi_add, menu);
            IMenuItem item = menu.FindItem(Resource.Id.menuItem_pricingType);
            item.SetTitle(GlobalVariables.mCurrentSelectedPricingType);
            return base.OnCreateOptionsMenu(menu);
        }
        private void SetListLayout()
        {
            //Create Layout Manager
            mLayoutManager = new LinearLayoutManager(this);

            mRvSizes.SetLayoutManager(mLayoutManager);
            mRvSizes.HasFixedSize = true;

            _Adapter = new CheckoutMultiAddListRecyclerViewAdapter(InitialDataLoad(), this);
            mMultiAddAdapter = _Adapter;
            mRvSizes.SetAdapter(mMultiAddAdapter);   
        }

        private List<MultiSizeAddProductsHolder> InitialDataLoad()
        {
            int selectedParentProductId = GlobalVariables.mCurrentSelectedItemIdMultiSize;
            List<Product> products = GlobalVariables.globalProductList
                .Where(o => o.parentProductId == selectedParentProductId)
                .OrderBy(o => o.productSizeId) //3x -> S
                .ToList();
            mProductHolder = new List<MultiSizeAddProductsHolder>();
            foreach (var item in products)
            {
                mProductHolder.Add(new MultiSizeAddProductsHolder()
                {
                    productId = item.productId,
                    isSelected = false,
                    productSize = item.productSize,
                    productPrice = GetProductPrice(GlobalVariables.mCurrentSelectedPricingType, products, item.productId),
                    quantity = 0
                });
            }
            return mProductHolder;
        }
        private decimal GetProductPrice(string mCurrentSelectedPricingType, List<Product> products, int productId)
        {
            decimal retval = 0;
            var productsEnum = products.Where(x => x.productId == productId);
            if (mCurrentSelectedPricingType == "RT")
            {
                retval = productsEnum.Select(x=>x.productRetailPrice).FirstOrDefault();
            }
            else if (mCurrentSelectedPricingType == "WS")
            {
                retval = productsEnum.Select(x => x.productWholesalePrice).FirstOrDefault();
            }
            else //runner
            {
                retval = productsEnum.Select(x => x.productRunnerPrice).FirstOrDefault();
            }
            return retval;
        }
        private void FnSetControlClickEvents()
        {
            mCbSelectAll.Click += MCbSelectAll_Click;
            mCbSelectAll.CheckedChange += MCbSelectAll_CheckedChange;
            mTxtAll0.Click += MTxtAll0_Click;
            mTxtAllMinus1.Click += MTxtAllMinus1_Click;
            mTxtAllPlus1.Click += MTxtAllPlus1_Click;
            mTxtAllPlus6.Click += MTxtAllPlus6_Click;
            mTxtAllPlus12.Click += MTxtAllPlus12_Click;

            mBtnCheckoutMultiAddAddToCart.Click += MBtnCheckoutMultiAddAddToCart_Click;
        }

        private void MCbSelectAll_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            mCbSelectAll.Text = e.IsChecked ? "UNSELECT ALL SIZES" : "SELECT ALL SIZES";
        }

        private void MBtnCheckoutMultiAddAddToCart_Click(object sender, EventArgs e)
        {
            mProductHolder = _Adapter.GetUpdatedData();
            if (mProductHolder.Exists(x => x.isSelected == true))
            {
                foreach (var item in mProductHolder.Where(x=>x.isSelected == true))
                {
                    AddItemsToCart(item.productId, item.quantity, item.productPrice);
                }
                //close view then update total button
                Finish();
            }
            else
            {
                DialogMessageService.MessageBox(this,"No item selected","Please select item(s) to add to cart.");
            }
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
                int productCategoryId = 1;
                string productCategory = "";
                int productSizeId = 1;
                string productSize = "";
                int parentProductId = 1;

                //get from db here where item id
                foreach (var item in GlobalVariables.globalProductList)
                {
                    if (item.productId == _itemId)
                    {
                        productName = item.productName;
                        productCategoryId = item.productCategoryId;
                        productCategory = item.productCategory;
                        productSizeId = item.productSizeId;
                        productSize = item.productSize;
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
                    productCategory = productCategory,
                    productSizeId = productSizeId,
                    productSize = productSize,
                    sizeRank = GetSizeRank(productSizeId),
                    productSubTotalPrice = _itemQty * _itemPrice,
                    productDiscountAmount = 0.00M,
                    productDiscountPercentage = 0.00M,
                    parentProductId = parentProductId,
                    parentProductName = GetParentProductName(parentProductId)
                });
            }
        }

        private int GetSizeRank(int _productSizeId)
        {
            return GlobalVariables.globalSizesList
                    .Where(x => x.productSizeId == _productSizeId)
                    .Select(x => x.sizeRank)
                    .FirstOrDefault();
        }

        private string GetParentProductName(int _parentProductId)
        {
            return GlobalVariables.globalParentProductList
                .Where(x => x.parentProductId == _parentProductId)
                .Select(x => x.parentProductName)
                .FirstOrDefault();
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
            if (!mCbSelectAll.Checked)
            {
                mCbSelectAll.Checked = true;
                mTxtAllQuantity.Text = "12";
            }
            _Adapter.ChangeAllItemQuantity(Convert.ToInt32(mTxtAllQuantity.Text));
        }

        private void MTxtAllPlus6_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("6", true);
            if (!mCbSelectAll.Checked)
            {
                mCbSelectAll.Checked = true;
                mTxtAllQuantity.Text = "6";
            }
            _Adapter.ChangeAllItemQuantity(Convert.ToInt32(mTxtAllQuantity.Text));
        }

        private void MTxtAllPlus1_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("1", true);
            if (!mCbSelectAll.Checked)
            {
                mCbSelectAll.Checked = true;
                mTxtAllQuantity.Text = "1";
            }
            _Adapter.ChangeAllItemQuantity(Convert.ToInt32(mTxtAllQuantity.Text));
        }

        private void MTxtAllMinus1_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = ComputeQuantity("1", false);
            if (mTxtAllQuantity.Text == "0")
            {
                mCbSelectAll.Checked = false;
            }
            _Adapter.ChangeAllItemQuantity(Convert.ToInt32(mTxtAllQuantity.Text));
        }

        private void MTxtAll0_Click(object sender, EventArgs e)
        {
            mTxtAllQuantity.Text = "0";
            _Adapter.ChangeAllItemQuantity(0);
            mCbSelectAll.Checked = false;
        }

        private void MCbSelectAll_Click(object sender, EventArgs e)
        {
            _Adapter.CheckAllItems(mCbSelectAll.Checked);
            if (mCbSelectAll.Checked)
            {
                if (mTxtAllQuantity.Text == "0")
                {
                    mTxtAllQuantity.Text = "1";
                }
            }
            else
            {
                mTxtAllQuantity.Text = "0";
            }
            _Adapter.ChangeAllItemQuantity(Convert.ToInt32(mTxtAllQuantity.Text));
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
            //refresh button total
            GlobalVariables.mIsCheckoutFragmentMultiSizeAddOpened = false;
            CheckoutFragment fragment_obj = (CheckoutFragment)GlobalVariables.mCheckoutFragmentCurrentInstance;
            fragment_obj.SetCheckoutButtonTotal(fragment_obj.mBtnCheckoutButton,fragment_obj.Context);
            base.OnDestroy();
        }

    }
}