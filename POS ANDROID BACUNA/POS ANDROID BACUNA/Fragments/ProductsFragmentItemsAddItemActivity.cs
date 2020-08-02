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
    [Activity(Label = "ProductsFragmentItemsAddItemActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentItemsAddItemActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        RelativeLayout mRlContents;
        CardView mCardviewProductAppearance;
        View mViewColorSelector;
        ImageView mImgViewPictureSelector;
        TextView mTxtProductName;
        EditText mEditTextProductAlias;
        EditText mEditTextProductName;
        TextInputLayout mTxtInputLayputProductName;
        RelativeLayout mRlProductCat;
        TextView mlblCategory;
        int mCategoryId = 0;
        TextView mTxtProductCat;
        RelativeLayout mRlProductDescription;
        TextView mlblDescription;
        TextView mTxtProductDescription;
        ImageView mImgBtnAddSize;
        ListView mLvProductSizes;
        ProductsItemSizesListViewAdapter mAdapter;
        LinearLayout mLlCreateProductButtonContainer;

        Button mBtnCreateProduct;
        float mDpVal;

        bool isEdit = false;
        int editParentProductId;
        List<ParentProducts> mSelectedParentProductRow;
        //List<NewProduct> mProducts;

        int PRODUCT_ALIAS_MAX_LENGTH = 7;
        int PRODUCT_NAME_MAX_LENGTH = 25;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_items_new_product);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            if (isEdit){FnShowSelectedData(editParentProductId);}
            FnSetListViewAdapter();
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mRlContents, mToolBar);
            });
        }

        private void FnShowSelectedData(int _parentProductId)
        {
            mSelectedParentProductRow = GlobalVariables.globalParentProductList.Where(x => x.parentProductId == _parentProductId).ToList();

            mEditTextProductName.Text = mSelectedParentProductRow[0].parentProductName;
            mCategoryId = mSelectedParentProductRow[0].categoryId;
            mTxtProductCat.Text = mSelectedParentProductRow[0].categoryName;
            mTxtProductCat.SetTextColor(ResourceIdToColor(Resource.Color.colorTextEnabled));
            mlblCategory.Visibility = ViewStates.Visible;
            mTxtProductDescription.Text = mSelectedParentProductRow[0].productDescription == null? "Description" : mSelectedParentProductRow[0].productDescription;
            mTxtProductDescription.SetTextColor(ResourceIdToColor(mSelectedParentProductRow[0].productDescription == null ? 
                Resource.Color.colorTextDisabled : 
                Resource.Color.colorTextEnabled));
            mlblDescription.Visibility = mSelectedParentProductRow[0].productDescription == null ? ViewStates.Invisible : ViewStates.Visible;
            mViewColorSelector.SetBackgroundColor(Android.Graphics.Color.ParseColor("#"+mSelectedParentProductRow[0].productColorBg));
            mCardviewProductAppearance.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#"+mSelectedParentProductRow[0].productColorBg));
            mEditTextProductAlias.Text = mSelectedParentProductRow[0].productAlias;

            //product list
            GlobalVariables.newProductSizesList.Clear();
            var productToCopy = GlobalVariables.globalProductList
                .OrderBy(x => x.productSizeId)
                .Where(x => x.parentProductId == _parentProductId)
                .ToList();
            GlobalVariables.newProductSizesList = DataClassHelper.ToNewProduct(productToCopy);
            //mProducts = GlobalVariables.newProductSizesList;
        }

        private void FnGetData()
        {
            isEdit = Intent.GetBooleanExtra("isEdit", false);
            editParentProductId = Intent.GetIntExtra("editParentProductId",0); 
        }

        private void FnSetListViewAdapter()
        {
            mAdapter = new ProductsItemSizesListViewAdapter(this, GlobalVariables.newProductSizesList.OrderBy(x => x.productSizeId).ToList());
            mLvProductSizes.Adapter = mAdapter;
        }

        private void FnSetEvents()
        {
            mToolBar.MenuItemClick += MToolBar_MenuItemClick;
            mViewColorSelector.Click += MViewColorSelector_Click;
            mImgViewPictureSelector.Click += MImgViewPictureSelector_Click;
            mRlProductCat.Click += MRlProductCat_Click;
            mRlProductDescription.Click += MRlProductDescription_Click;
            mImgBtnAddSize.Click += MImgBtnAddSize_Click;
            mLvProductSizes.ItemClick += MLvProductSizes_ItemClick;
            mBtnCreateProduct.Click += MBtnCreateProduct_Click;
            mEditTextProductName.TextChanged += MEditTextProductName_TextChanged;
        }

        private void MToolBar_MenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menuItem_copy)
            {
                //copy here
                GlobalVariables.parentProductCopyHolder.Clear();
                GlobalVariables.newProductCopyHolder.Clear();
                CopyData();
                Toast.MakeText(this, "Product copied!", ToastLength.Long).Show();
            }
            else if (e.Item.ItemId == Resource.Id.menuItem_paste)
            {
                if (GlobalVariables.parentProductCopyHolder.Count > 0)
                {
                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = builder.Create();
                    alert.SetTitle("Confirm action");
                    alert.SetMessage("Paste copied product?");

                    alert.SetButton2("CANCEL", (c, ev) =>
                    {
                        //cancel button
                    });

                    alert.SetButton("YES", (c, ev) =>
                    {
                        PasteData(GlobalVariables.parentProductCopyHolder, GlobalVariables.newProductCopyHolder);
                        Toast.MakeText(this, "Successfully pasted!", ToastLength.Long).Show();
                    });

                    alert.Show();
                }
                else
                {
                    Toast.MakeText(this, "Clipboard is empty!", ToastLength.Long).Show();
                }
                
            }
            else if (e.Item.ItemId == Resource.Id.menuItem_delete)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = builder.Create();
                alert.SetTitle("Delete product");
                alert.SetMessage("Are you sure you want to delete this product? This action cannot be undone.");

                alert.SetButton2("CANCEL", (c, ev) =>
                {
                    //cancel button
                });

                alert.SetButton("YES", (c, ev) =>
                {
                    //delete code here
                    DeleteProduct(editParentProductId);
                    Finish();
                });

                alert.Show();
            }
        }

        private void PasteData(List<ParentProductCopyHolder> _sourceParentProduct, List<NewProductCopyHolder> _sourceProduct)
        {
            mEditTextProductName.Text = _sourceParentProduct[0].parentProductName + " (Copy)";
            mCategoryId = _sourceParentProduct[0].categoryId;
            mTxtProductCat.Text = mCategoryId == 0 ? "Category" : _sourceParentProduct[0].categoryName;
            mTxtProductCat.SetTextColor(ResourceIdToColor(Resource.Color.colorTextEnabled));
            mlblCategory.Visibility = mCategoryId == 0 ? ViewStates.Invisible : ViewStates.Visible;
            mTxtProductDescription.Text = _sourceParentProduct[0].productDescription == null ? "Description" : _sourceParentProduct[0].productDescription;
            mTxtProductDescription.SetTextColor(ResourceIdToColor(_sourceParentProduct[0].productDescription == null ? 
                Resource.Color.colorTextDisabled : Resource.Color.colorTextEnabled));
            mlblDescription.Visibility = _sourceParentProduct[0].productDescription == null ? ViewStates.Invisible : ViewStates.Visible;
            mViewColorSelector.SetBackgroundColor(Android.Graphics.Color.ParseColor("#" + _sourceParentProduct[0].productColorBg));
            mCardviewProductAppearance.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#" + _sourceParentProduct[0].productColorBg));
            mEditTextProductAlias.Text = _sourceParentProduct[0].productAlias;

            //product list
            GlobalVariables.newProductSizesList.Clear();
            GlobalVariables.newProductSizesList = DataClassHelper.CopyToNewProduct(GlobalVariables.newProductCopyHolder);
            FnSetListViewAdapter(); //refresh new product list
            mLvProductSizes.Invalidate();
        }

        private void CopyData()
        {
            var _categoryId = mCategoryId;
            var _categoryText = mTxtProductCat.Text;
            var _productName = mEditTextProductName.Text.Trim() == "" ? null : mEditTextProductName.Text.Trim();
            var _productAlias = mEditTextProductAlias.Text == "" ? null : mEditTextProductAlias.Text;
            CopyParentProduct(_productName,
                _categoryId,
                _categoryText,
                _productAlias,
                GetBackgroundColor(),
                null,
                GetDescription());
            var productSizesList = GlobalVariables.newProductSizesList.OrderBy(x => x.productSizeId).ToList();
            for (int i = 0; i < productSizesList.Count; i++)
            {
                CopyProduct(0, //product id 0 to flag as new product 
                     "(" + productSizesList[i].productSize + ") " + _productName,
                     0, //product id 0 to flag as new parent product 
                     mCategoryId,
                     _categoryText,
                     productSizesList[i].productSizeId,
                     productSizesList[i].productSize,
                     productSizesList[i].productCost,
                     productSizesList[i].productRetailPrice,
                     productSizesList[i].productWholesalePrice,
                     productSizesList[i].productRunnerPrice,
                     GetBackgroundColor(),
                     null,
                     _productAlias,
                     productSizesList[i].productCode
                    );
            }
        }

        private void CopyParentProduct(string _parentProductName, int _categoryId, string _categoryName,
            string _productAlias, string _productColorBg, System.Drawing.Image _productImage, string _productDescription)
        {
            GlobalVariables.parentProductCopyHolder.Add(new ParentProductCopyHolder()
            {
                parentProductName = _parentProductName,
                categoryId = _categoryId,
                categoryName = _categoryName,
                productAlias = _productAlias,
                productColorBg = _productColorBg,
                productImage = _productImage,
                productDescription = _productDescription
            });
        }

        private void CopyProduct(int _productId, string _productName, int _parentProductId, int _productCategoryId,
            string _productCategory, int _productSizeId, string _productSize, decimal _productCost, decimal _productRetailPrice,
            decimal _productWholesalePrice, decimal _productRunnerPrice, string _productColorBg, System.Drawing.Image _productImage,
            string _productAlias, string _productCode)
        {
            GlobalVariables.newProductCopyHolder.Add(new NewProductCopyHolder()
            {
                productId = _productId,
                productName = _productName,
                parentProductId = _parentProductId,
                productCategoryId = _productCategoryId,
                productCategory = _productCategory,
                productSizeId = _productSizeId,
                productSize = _productSize,
                productCost = _productCost,
                productRetailPrice = _productRetailPrice,
                productWholesalePrice = _productWholesalePrice,
                productRunnerPrice = _productRunnerPrice,
                productColorBg = _productColorBg,
                productImage = _productImage,
                productAlias = _productAlias,
                productCode = _productCode
            });
        }

        private void DeleteProduct(int _parentProductId)
        {
            //delete from parent table
            GlobalVariables.globalParentProductList.RemoveAll(x => x.parentProductId == _parentProductId);
            //delete from child table
            GlobalVariables.globalProductList.RemoveAll(x => x.parentProductId == _parentProductId);
        }

        private void MLvProductSizes_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int sizeId = GlobalVariables.newProductSizesList.OrderBy(x => x.productSizeId).ToList()[e.Position].productSizeId;
            if (!mDialogShown)
            {
                mDialogShown = true;
                Intent intent = new Intent(this, typeof(ProductsFragmentItemsAddSizeActivity));
                intent.PutExtra("productSizeId", sizeId);
                intent.PutExtra("isEdit", true);
                StartActivityForResult(intent, 17);
            }
            
        }

        private void MImgViewPictureSelector_Click(object sender, EventArgs e)
        {
            mLvProductSizes.SmoothScrollToPosition(mAdapter.Count);
        }

        private void MEditTextProductName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            mTxtProductName.Text = mEditTextProductName.Text;
            mEditTextProductAlias.Text = mEditTextProductName.Text;
            if (mEditTextProductName.Text == "")
            {
                mTxtProductName.Text = "Product name";
                mEditTextProductAlias.Text = "";
            }
        }

        private void MRlProductDescription_Click(object sender, EventArgs e)
        {
            if (!mDialogShown)
            {
                mDialogShown = true;
                bool isBlank = mlblDescription.Visibility == ViewStates.Invisible;
                Intent intent = new Intent(this, typeof(ProductsFragmentItemsDescriptionActivity));
                intent.PutExtra("productDescriptionInit", mTxtProductDescription.Text);
                intent.PutExtra("productDescriptionStatus", isBlank);
                StartActivityForResult(intent, 14);
            }
        }

        private void MRlProductCat_Click(object sender, EventArgs e)
        {
            if (!mDialogShown)
            {
                mDialogShown = true;
                Intent intent = new Intent(this, typeof(ProductsFragmentItemsSelectCategoryActivity));
                intent.PutExtra("productCatNameInit", mTxtProductCat.Text);
                StartActivityForResult(intent, 13);
            }
        }

        private void MBtnCreateProduct_Click(object sender, EventArgs e)
        {
            if (!hasErrors())
            {
                if (!SizesListEmpty())
                {
                    if (isEdit)
                    {
                        FnEditData(editParentProductId);
                    }
                    else
                    {
                        FnSaveData();
                    }
                    Finish();
                }
            }
        }

        private void FnSaveData() 
        {
            //save parent product to "database"
            int parentProductId = GenerateParentProductId();
            SaveParentProduct(parentProductId,
                mEditTextProductName.Text.Trim(),
                mCategoryId,
                mTxtProductCat.Text,
                mEditTextProductAlias.Text,
                GetBackgroundColor(),
                null,
                GetDescription());
            //save product sizes to "database"
            var productSizesList = GlobalVariables.newProductSizesList.OrderBy(x => x.productSizeId).ToList();
            for (int i = 0; i < productSizesList.Count; i++)
            {
                SaveProduct(GenerateProductId(),
                     "(" + GlobalVariables.newProductSizesList[i].productSize + ") " + mEditTextProductName.Text.Trim(),
                     parentProductId,
                     mCategoryId,
                     mTxtProductCat.Text,
                     GlobalVariables.newProductSizesList[i].productSizeId,
                     GlobalVariables.newProductSizesList[i].productSize,
                     GlobalVariables.newProductSizesList[i].productCost,
                     GlobalVariables.newProductSizesList[i].productRetailPrice,
                     GlobalVariables.newProductSizesList[i].productWholesalePrice,
                     GlobalVariables.newProductSizesList[i].productRunnerPrice,
                     GetBackgroundColor(),
                     null,
                     mEditTextProductAlias.Text,
                     GlobalVariables.newProductSizesList[i].productCode
                    );
            }
        }

        private void FnEditData(int _parentProductId)
        {
            EditParentProduct(_parentProductId,
                mEditTextProductName.Text.Trim(),
                mCategoryId,
                mTxtProductCat.Text,
                mEditTextProductAlias.Text,
                GetBackgroundColor(),
                null,
                GetDescription()
                );

            if (GlobalVariables.productsToDeleteOnEditMode.Count > 0)
            {
                foreach (var productsizeid in GlobalVariables.productsToDeleteOnEditMode)
                {
                    int productId = GlobalVariables.globalProductList
                        .Where(x => x.parentProductId == _parentProductId && x.productSizeId == productsizeid)
                        .Select(x => x.productId)
                        .FirstOrDefault();

                    GlobalVariables.globalProductList.RemoveAll(x => x.productId == productId);
                }
            }
            
            foreach (var item in GlobalVariables.newProductSizesList.OrderBy(x => x.productSizeId))
            {
                //save new record for new sizes
                if (item.productId == 0)
                {
                    SaveProduct(GenerateProductId(),
                     "(" + item.productSize + ") " + mEditTextProductName.Text.Trim(),
                     _parentProductId,
                     mCategoryId,
                     mTxtProductCat.Text,
                     item.productSizeId,
                     item.productSize,
                     item.productCost,
                     item.productRetailPrice,
                     item.productWholesalePrice,
                     item.productRunnerPrice,
                     GetBackgroundColor(),
                     null,
                     mEditTextProductAlias.Text,
                     item.productCode
                    );
                }
                else //edit for existing sizes
                {
                    EditProduct(item.productId,
                     "(" + item.productSize + ") " + mEditTextProductName.Text.Trim(),
                     mCategoryId,
                     mTxtProductCat.Text,
                     item.productCost,
                     item.productRetailPrice,
                     item.productWholesalePrice,
                     item.productRunnerPrice,
                     GetBackgroundColor(),
                     null,
                     mEditTextProductAlias.Text,
                     item.productCode
                    );
                }
                
            }
        }
        private void EditParentProduct(int _parentProductId, string _parentProductName, int _categoryId, string _categoryName,
            string _productAlias, string _productColorBg, System.Drawing.Image _productImage, string _productDescription)
        {
            foreach (var item in GlobalVariables.globalParentProductList.Where(x => x.parentProductId == _parentProductId))
            {
                item.parentProductName = _parentProductName;
                item.categoryId = _categoryId;
                item.categoryName = _categoryName;
                item.productAlias = _productAlias;
                item.productColorBg = _productColorBg;
                item.productImage = _productImage;
                item.productDescription = _productDescription;
            }
        }

        private void EditProduct(int _productId, string _productName, int _productCategoryId,
            string _productCategory, decimal _productCost, decimal _productRetailPrice,
            decimal _productWholesalePrice, decimal _productRunnerPrice, string _productColorBg, System.Drawing.Image _productImage,
            string _productAlias, string _productCode)
        {
            foreach (var item in GlobalVariables.globalProductList.Where(x => x.productId == _productId))
            {
                item.productName = _productName;
                item.productCategoryId = _productCategoryId;
                item.productCategory = _productCategory;
                item.productCost = _productCost;
                item.productRetailPrice = _productRetailPrice;
                item.productWholesalePrice = _productWholesalePrice;
                item.productRunnerPrice = _productRunnerPrice;
                item.productColorBg = _productColorBg;
                item.productImage = _productImage;
                item.productAlias = _productAlias;
                item.productCode = _productCode;
            }
        }

        private bool SizesListEmpty()
        {
            bool retVal;
            if (GlobalVariables.newProductSizesList.Count != 0)
            {
                retVal = false;
            }
            else
            {
                retVal = true;
                DialogMessageService.MessageBox(this, "Incomplete fields", "Please input sizes for this product.");
            }
            return retVal;
        }

        private string GetDescription()
        {
            return mlblDescription.Visibility == ViewStates.Visible ? mTxtProductDescription.Text : null;
        }

        private string GetBackgroundColor()
        {
            ColorDrawable viewColor = (ColorDrawable)mViewColorSelector.Background;
            int colorId = viewColor.Color;

            return Java.Lang.Integer.ToHexString(colorId).Substring(2).ToUpper();
        }

        private void SaveProduct(int _productId, string _productName, int _parentProductId, int _productCategoryId, 
            string _productCategory, int _productSizeId, string _productSize, decimal _productCost, decimal _productRetailPrice,
            decimal _productWholesalePrice, decimal _productRunnerPrice, string _productColorBg, System.Drawing.Image _productImage,
            string _productAlias, string _productCode)
        {
            GlobalVariables.globalProductList.Add(new Product() { 
                productId = _productId,
                productName = _productName,
                parentProductId = _parentProductId,
                productCategoryId = _productCategoryId,
                productCategory = _productCategory,
                productSizeId = _productSizeId,
                productSize = _productSize,
                productCost = _productCost,
                productRetailPrice = _productRetailPrice,
                productWholesalePrice = _productWholesalePrice,
                productRunnerPrice = _productRunnerPrice,
                productColorBg = _productColorBg,
                productImage = _productImage,
                productAlias = _productAlias,
                productCode = _productCode
            });
        }

        private void SaveParentProduct(int _parentProductId, string _parentProductName, int _categoryId, string _categoryName,
            string _productAlias, string _productColorBg, System.Drawing.Image _productImage, string _productDescription)
        {
            GlobalVariables.globalParentProductList.Add(new ParentProducts() { 
                parentProductId = _parentProductId,
                parentProductName = _parentProductName,
                categoryId = _categoryId,
                categoryName =_categoryName,
                productAlias = _productAlias,
                productColorBg = _productColorBg,
                productImage = _productImage,
                productDescription = _productDescription
            });
        }

        private int GenerateProductId()
        {
            int retVal;
            try
            {
                retVal = GlobalVariables.globalProductList.Max(x => x.productId) + 1;
            }
            catch (Exception)
            {
                retVal = 1;
            }

            return retVal;
        }

        private int GenerateParentProductId()
        {
            int retVal;
            try
            {
                retVal = GlobalVariables.globalParentProductList.Max(x => x.parentProductId) + 1;
            }
            catch (Exception)
            {
                retVal = 1;
            }

            return retVal;
        }

        private bool hasErrors()
        {
            var retVal = false;
            string productName = mTxtInputLayputProductName.EditText.Text.Trim();
            if (productName == "")
            {
                retVal = true;
                mTxtInputLayputProductName.Error = "Product name cannot be blank";
            }
            else if (ProductNameExists(productName))
            {
                if (isEdit)
                {
                    if (mEditTextProductName.Text.Trim() == mSelectedParentProductRow[0].parentProductName)
                    {
                        retVal = false;
                    }
                    else
                    {
                        retVal = true;
                        mTxtInputLayputProductName.Error = "Product name already exists";
                    }
                }
                else
                {
                    retVal = true;
                    mTxtInputLayputProductName.Error = "Product name already exists";
                }
            }
            else if (mlblCategory.Visibility == ViewStates.Invisible)
            {
                retVal = true;
                mTxtInputLayputProductName.ErrorEnabled = false;
                DialogMessageService.MessageBox(this, "Incomplete fields", "Please select a category.");
            }
            else
            { 
                mTxtInputLayputProductName.ErrorEnabled = false;
                retVal = false;
            }
            return retVal;
        }

        private bool ProductNameExists(string _parentProductName)
        {
            return GlobalVariables.globalParentProductList.Exists(x => x.parentProductName == _parentProductName);
        }

        private void MImgBtnAddSize_Click(object sender, EventArgs e)
        {
            if (!hasErrors())
            {
                if (!mDialogShown)
                {
                    mDialogShown = true;
                    Intent intent = new Intent(this, typeof(ProductsFragmentItemsSelectSizeActivity));
                    intent.PutExtra("productName", mEditTextProductName.Text);
                    StartActivityForResult(intent, 10);
                }
            }     
        }

        private void MViewColorSelector_Click(object sender, EventArgs e)
        {
            if (!mDialogShown)
            {
                mDialogShown = true;
                SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                ColorSelectorDialogFragment colorSelectorDialog = new ColorSelectorDialogFragment();
                colorSelectorDialog.Show(transaction, "colorSelectorDialogFragment");
            }
        }

        private void FnSetControls()
        {
            mRlContents = FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mViewColorSelector = FindViewById<View>(Resource.Id.viewColorSelector);
            mImgViewPictureSelector = FindViewById<ImageView>(Resource.Id.imgPictureSelector);
            mCardviewProductAppearance = FindViewById<CardView>(Resource.Id.cardviewProductAppearance);
            mLlCreateProductButtonContainer = FindViewById<LinearLayout>(Resource.Id.llCreateProductButtonContainer);
            mTxtProductName = FindViewById<TextView>(Resource.Id.txtProductName); 
            mEditTextProductAlias = FindViewById<EditText>(Resource.Id.etItemAlias);
            mEditTextProductName = FindViewById<EditText>(Resource.Id.etProductName);
            mTxtInputLayputProductName = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutProductName);
            mRlProductCat = FindViewById<RelativeLayout>(Resource.Id.rlProductCat);
            mlblCategory = FindViewById<TextView>(Resource.Id.lblCategory);
            mTxtProductCat = FindViewById<TextView>(Resource.Id.txtProductCat);
            mRlProductDescription = FindViewById<RelativeLayout>(Resource.Id.rlProductDescription);
            mlblDescription = FindViewById<TextView>(Resource.Id.lblDescription);
            mTxtProductDescription = FindViewById<TextView>(Resource.Id.txtProductDescription);
            mImgBtnAddSize = FindViewById<ImageView>(Resource.Id.imgBtnAddSize);
            mLvProductSizes = FindViewById<ListView>(Resource.Id.lvProductSizes); 
            mBtnCreateProduct = FindViewById<Button>(Resource.Id.btnCreateProduct);
            mBtnCreateProduct.Text = isEdit ? "SAVE" : "CREATE PRODUCT";
            mEditTextProductAlias.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(PRODUCT_ALIAS_MAX_LENGTH) });
            mEditTextProductName.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(PRODUCT_NAME_MAX_LENGTH) });
        }

        private void SetContentsLayoutHeight(RelativeLayout layout, SupportToolbar layoutBelow)
        {
            int checkoutButtonHeight = mLlCreateProductButtonContainer.Height;
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

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = isEdit ? "EDIT PRODUCT":"NEW PRODUCT";
        }

        protected override void OnPause()
        {
            GlobalVariables.mIsProductsFragmentEditItemOpened = false;
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        public void SetLabelColor(string hexColor)
        {
            mCardviewProductAppearance.SetCardBackgroundColor(Android.Graphics.Color.ParseColor(hexColor));
            mViewColorSelector.SetBackgroundColor(Android.Graphics.Color.ParseColor(hexColor));
        }

        public void ColorSelectorDialogFragmentOnActivityResult()
        {
            mDialogShown = false; //flag to enable dialog show 
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //clear toolbar menu to avoid duplicate draw
            mToolBar.Menu.Clear();
            if (isEdit)
            {
                mToolBar.InflateMenu(Resource.Menu.toolbar_menu_products_items_edit_product);
            }
            else
            {
                mToolBar.InflateMenu(Resource.Menu.toolbar_menu_products_items_new_product);
            }
            return base.OnCreateOptionsMenu(menu);
        }

        private Color ResourceIdToColor(int _colorInt)
        {
            int colorInt = this.GetColor(_colorInt);
            Color color = new Color(colorInt);
            return color;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //mAdapter.NotifyDataSetChanged();
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 10)
            {
                mDialogShown = false;
                FnSetListViewAdapter();
                mLvProductSizes.Invalidate();
                Window.DecorView.PostDelayed(() =>
                {
                    mLvProductSizes.SmoothScrollToPosition(mAdapter.Count);
                },100);
            }
            if (requestCode == 13)
            {
                mDialogShown = false;
                string returnDataCategoryName = data.GetStringExtra("productCategoryName");
                int returnDataCategoryId = data.GetIntExtra("productCategoryId", 0);
                mTxtProductCat.Text = returnDataCategoryName;
                mTxtProductCat.SetTextColor(returnDataCategoryName == "Category" ? 
                    ResourceIdToColor(Resource.Color.colorTextDisabled): 
                    ResourceIdToColor(Resource.Color.colorTextEnabled));
                mCategoryId = returnDataCategoryId;
                mlblCategory.Visibility = returnDataCategoryName == "Category" ? ViewStates.Invisible : ViewStates.Visible;
            }
            if (requestCode == 14)
            {
                mDialogShown = false;
                string returnData = data.GetStringExtra("productDescription");
                mTxtProductDescription.Text = returnData == "" ? "Description" : returnData;
                mTxtProductDescription.SetTextColor(returnData == "" ?
                    ResourceIdToColor(Resource.Color.colorTextDisabled) :
                    ResourceIdToColor(Resource.Color.colorTextEnabled));
                mlblDescription.Visibility = returnData == "" ? ViewStates.Invisible : ViewStates.Visible;
            }
            if (requestCode == 17)
            {
                mDialogShown = false;
                FnSetListViewAdapter();
                mLvProductSizes.Invalidate();
            }
        }

        protected override void OnDestroy()
        {
            GlobalVariables.productsToDeleteOnEditMode.Clear();
            GlobalVariables.newProductSizesList.Clear();
            base.OnDestroy();
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