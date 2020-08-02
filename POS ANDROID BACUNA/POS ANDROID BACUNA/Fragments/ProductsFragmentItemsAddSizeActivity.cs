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


namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "ProductsFragmentItemsAddSizeActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentItemsAddSizeActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        TabLayout mTabs;
        TextInputLayout mTextInputLayoutProductCode;
        
        private int mProductId;
        private string mProductName;
        private int mParentProductId;
        private int mProductCategoryId;
        private string mProductCategory;
        private int mProductSizeId;
        private string mProductSize;
        private decimal mProductCost;
        private decimal mProductRetailPrice;
        private decimal mProductWholesalePrice;
        private decimal mProductRunnerPrice;

        string mPesoSign = "\u20b1";
        EditText mEtProductCode;
        EditText mEtCost;
        EditText mEtWholesalePrice;
        EditText mEtRetailPrice;
        EditText mEtRunnerPrice;

        private bool isEdit;
        List<NewProduct> mSelectedProductRow;

        Button mBtnCreateSize;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_items_new_size);
            FnGetData();
            FnSetControls();
            if (isEdit)
            {
                FnShowSelectedData(mProductSizeId);
            }
            FnSetUpToolBar();
            FnSetTabs();
            FnSetEvents();
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            Window.DecorView.Post(() =>
            {
                keyboardState("VISIBLE", mEtProductCode);
                mEtProductCode.RequestFocus();
                mEtProductCode.SetSelection(mEtProductCode.Length());
            });
        }

        void keyboardState(string _state, EditText _edittext)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            if (_state == "HIDDEN")
            {
                imm.HideSoftInputFromWindow(_edittext.WindowToken, 0);
            }
            else
            {
                imm.ShowSoftInput(_edittext, 0);
            }
        }

        private void FnShowSelectedData(int _productSizeId)
        {
            mSelectedProductRow = GlobalVariables.newProductSizesList.Where(x => x.productSizeId == _productSizeId).ToList();

            mProductId = mSelectedProductRow[0].productId;
            mProductName = mSelectedProductRow[0].productName;
            mParentProductId = mSelectedProductRow[0].parentProductId;
            mProductCategoryId = mSelectedProductRow[0].productCategoryId;
            mProductCategory = mSelectedProductRow[0].productCategory;
            mProductSizeId = _productSizeId;
            mProductSize = mSelectedProductRow[0].productSize;
            mEtProductCode.Text = mSelectedProductRow[0].productCode != null ? mSelectedProductRow[0].productCode : "";
            mProductCost = mSelectedProductRow[0].productCost;
            mEtCost.Text = mPesoSign + " " + string.Format("{0:n}", mProductCost);
            mProductRetailPrice = mSelectedProductRow[0].productRetailPrice;
            mEtRetailPrice.Text = mPesoSign + " " + string.Format("{0:n}", mProductRetailPrice); 
            mProductWholesalePrice = mSelectedProductRow[0].productWholesalePrice;
            mEtWholesalePrice.Text = mPesoSign + " " + string.Format("{0:n}", mProductWholesalePrice);
            mProductRunnerPrice = mSelectedProductRow[0].productRunnerPrice;
            mEtRunnerPrice.Text = mPesoSign + " " + string.Format("{0:n}", mProductRunnerPrice);
        }

        private void FnSetTabs()
        {
            mTabs.AddTab(mTabs.NewTab().SetText("PRODUCT INFO."));
            mTabs.AddTab(mTabs.NewTab().SetText("STOCK"));
        }

        private void FnGetData()
        {
            mProductId = 0; //for marking new products on edit of sizes
            mProductSizeId = Intent.GetIntExtra("productSizeId", 0);
            isEdit = Intent.GetBooleanExtra("isEdit", false);
            mProductSize = Intent.GetStringExtra("productSizeName");
            mProductName = Intent.GetStringExtra("productName");
        }

        private void FnSetEvents()
        {
            mToolBar.MenuItemClick += MToolBar_MenuItemClick;
            mBtnCreateSize.Click += MBtnCreateSize_Click;
            if (!isEdit)
            {
                mEtCost.Text = mPesoSign + " 0.00";
                mEtWholesalePrice.Text = mPesoSign + " 0.00";
                mEtRetailPrice.Text = mPesoSign + " 0.00";
                mEtRunnerPrice.Text = mPesoSign + " 0.00";
            }
            mEtCost.AddTextChangedListener(new CurrencyTextWatcher(mEtCost,this, mPesoSign));
            mEtWholesalePrice.AddTextChangedListener(new CurrencyTextWatcher(mEtWholesalePrice, this, mPesoSign));
            mEtRetailPrice.AddTextChangedListener(new CurrencyTextWatcher(mEtRetailPrice, this, mPesoSign));
            mEtRunnerPrice.AddTextChangedListener(new CurrencyTextWatcher(mEtRunnerPrice, this, mPesoSign));
            mEtCost.FocusChange += delegate (object sender, View.FocusChangeEventArgs e) 
            { EdittextFocusChangedEvent(sender, e, mEtCost); };
            mEtWholesalePrice.FocusChange += delegate (object sender, View.FocusChangeEventArgs e) 
            { EdittextFocusChangedEvent(sender, e, mEtWholesalePrice); };
            mEtRetailPrice.FocusChange += delegate (object sender, View.FocusChangeEventArgs e)
            { EdittextFocusChangedEvent(sender, e, mEtRetailPrice); };
            mEtRunnerPrice.FocusChange += delegate (object sender, View.FocusChangeEventArgs e)
            { EdittextFocusChangedEvent(sender, e, mEtRunnerPrice); };
        }

        private void MToolBar_MenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menuItem_delete)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = builder.Create();
                alert.SetTitle("Attention!");
                alert.SetMessage("Are you sure you want to delete?");

                alert.SetButton2("CANCEL", (c, ev) =>
                {
                    //cancel button
                });

                alert.SetButton("YES", (c, ev) =>
                {
                    DeleteProduct(mProductSizeId);
                    MarkProductAsToDelete(mProductSizeId);
                    Finish();
                });

                alert.Show();
            }
        }

        private void MarkProductAsToDelete(int _productSizeId)
        {
            GlobalVariables.productsToDeleteOnEditMode.Add(_productSizeId);
        }

        private void DeleteProduct(int _productSizeId)
        {
            GlobalVariables.newProductSizesList.RemoveAll(x => x.productSizeId == _productSizeId);
        }

        private void EdittextFocusChangedEvent(object sender, View.FocusChangeEventArgs e, EditText _edittext)
        {
            _edittext.SetSelection(_edittext.Text.Length);
        }

        private void FnSetControls()
        {
            mTabs = FindViewById<TabLayout>(Resource.Id.tabs);
            mTextInputLayoutProductCode = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutProductCode);
            mEtProductCode = FindViewById<EditText>(Resource.Id.etProductCode);
            mEtCost = FindViewById<EditText>(Resource.Id.etProductCost); 
            mEtWholesalePrice = FindViewById<EditText>(Resource.Id.etWholesalePrice);
            mEtRetailPrice = FindViewById<EditText>(Resource.Id.etRetailPrice);
            mEtRunnerPrice = FindViewById<EditText>(Resource.Id.etRunnerPrice);
            mBtnCreateSize = FindViewById<Button>(Resource.Id.btnCreateProduct);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = builder.Create();
                    alert.SetTitle("Cancel warning");
                    alert.SetMessage("Are you sure you want to cancel?");

                    alert.SetButton2("NO", (c, ev) =>
                    {
                        //cancel button
                    });

                    alert.SetButton("YES", (c, ev) =>
                    {
                        Finish();
                    });

                    alert.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mToolBar.Menu.Clear();
            if (isEdit)
            {
                mToolBar.InflateMenu(Resource.Menu.toolbar_menu_products_items_edit_size);
            }
            return base.OnCreateOptionsMenu(menu);
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = isEdit ? mProductName : "(" + mProductSize + ") " + mProductName;
        }

        private void MBtnCreateSize_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                ProcessUserInputs();
                if (isEdit)
                {
                    EditProductSize(mProductSizeId,mEtProductCode.Text.Trim(), mProductCost, mProductRetailPrice, mProductWholesalePrice, mProductRunnerPrice);
                }
                else
                {
                    //close this activity and the select size activity;
                    GlobalVariables.mIsCreateProductSizeClicked = true;
                    SaveNewProductSize(mProductId, "(" + mProductSize + ") " + mProductName, mParentProductId, mProductCategoryId, mProductCategory, mProductSizeId, mProductSize,
                                        mEtProductCode.Text.Trim(), mProductCost, mProductRetailPrice, mProductWholesalePrice, mProductRunnerPrice);
                }
                Finish();
            }
        }

        private void EditProductSize(int productSizeId,
            string productCode, decimal productCost, decimal retailPrice,
            decimal wholesalePrice, decimal runnerPrice)
        {
            foreach (var item in GlobalVariables.newProductSizesList.Where(x => x.productSizeId == productSizeId))
            {
                item.productCode = productCode;
                item.productCost = productCost;
                item.productRetailPrice = retailPrice;
                item.productWholesalePrice = wholesalePrice;
                item.productRunnerPrice = runnerPrice;
            }
        }

        private void SaveNewProductSize(int productId, string productName, int parentProductId, 
            int productCategoryId, string productCategory, int productSizeId, string productSize, 
            string productCode, decimal productCost, decimal retailPrice,
            decimal wholesalePrice, decimal runnerPrice)
        {
            GlobalVariables.newProductSizesList.Add(new NewProduct { 
                productId = productId,
                productName = productName,
                parentProductId = parentProductId,
                productCategoryId = productCategoryId,
                productCategory = productCategory,
                productSizeId = productSizeId,
                productSize = productSize,
                productCode = productCode,
                productCost = productCost,
                productRetailPrice = retailPrice,
                productWholesalePrice = wholesalePrice,
                productRunnerPrice = runnerPrice
            });
        }

        private bool HasErrors()
        {
            bool retVal;
            if (productCodeExists(mEtProductCode.Text.Trim()))
            {
                if (isEdit)
                {
                    if (mEtProductCode.Text.Trim() == mSelectedProductRow[0].productCode)
                    {
                        retVal = false;
                    }
                    else
                    {
                        retVal = true;
                        mTextInputLayoutProductCode.Error = "Product code already exists.";
                    }
                }
                else
                {
                    retVal = true;
                    mTextInputLayoutProductCode.Error = "Product code already exists.";
                }
            }
            else
            {
                retVal = false;
                mTextInputLayoutProductCode.ErrorEnabled = false;
            }
            return retVal;
        }

        private bool productCodeExists(string _productCode)
        {
            bool retVal = false;
            if (_productCode == "")
            {
                retVal = false;
            }
            else if (GlobalVariables.globalProductList.Exists(x => x.productCode == _productCode))
            {
                retVal = true;
            }
            else if (GlobalVariables.newProductSizesList.Exists(x => x.productCode == _productCode))
            {
                retVal = true;
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        private void ProcessUserInputs()
        {
            mParentProductId = 1;
            mProductCategoryId = 1;
            mProductCategory = "Test Category";
        
            mProductCost = Convert.ToDecimal(mEtCost.Text.Substring(2, mEtCost.Text.Length-2));
            mProductRetailPrice = Convert.ToDecimal(mEtRetailPrice.Text.Substring(2, mEtRetailPrice.Text.Length - 2));
            mProductWholesalePrice = Convert.ToDecimal(mEtWholesalePrice.Text.Substring(2, mEtWholesalePrice.Text.Length - 2));
            mProductRunnerPrice = Convert.ToDecimal(mEtRunnerPrice.Text.Substring(2, mEtRunnerPrice.Text.Length - 2));
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        public override void OnBackPressed()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Cancel warning");
            alert.SetMessage("Are you sure you want to cancel?");

            alert.SetButton2("NO", (c, ev) =>
            {
                //cancel button
            });

            alert.SetButton("YES", (c, ev) =>
            {
                base.OnBackPressed();
            });

            alert.Show();
        }

    }
}