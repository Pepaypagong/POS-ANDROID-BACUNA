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
using Android.Text;
using POS_ANDROID_BACUNA.SQLite;
using Android.InputMethodServices;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "ProductsFragmentAddCategoryOrSizeActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentAddCategoryOrSizeActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        RelativeLayout mRlContents;
        EditText mEtCategory;
        TextView mTxtMaxCharsLabel;
        LinearLayout mLlCreateProductButtonContainer;
        Button mBtnCreate;
        float mDpVal;
        bool isSize;
        bool isEdit;
        int SIZE_MAX_LENGTH = 5;
        int CATEGORY_MAX_LENGTH = 20;
        SizesDataAccess mSizesDataAccess;
        CategoriesDataAccess mCategoriesDataAccess;
        int selectedRecordId;
        ProductSizesModel selectedSize;
        ProductCategoriesModel selectedCategory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_categories_new_category);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetUpEvents();
            //Window.SetWindowAnimations(Resource.Style.dialog_animation);
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            Window.DecorView.Post(() =>
            {
                keyboardState("VISIBLE", mEtCategory);
                mEtCategory.RequestFocus();
                mEtCategory.SetSelection(mEtCategory.Length());
                //SetContentsLayoutHeight(mRlContents, mToolBar);
            });
        }

        private void FnSetUpEvents()
        {
            mBtnCreate.Click += MBtnCreate_Click;
        }

        private void MBtnCreate_Click(object sender, EventArgs e)
        {
            if (!HasErrors())
            {
                if (isSize) //add size mode
                {
                    if (isEdit)
                    {
                        UpdateSize();
                    }
                    else
                    {
                        SaveSize();
                    }
                }
                else //add category mode
                {
                    if (isEdit)
                    {
                        UpdateCategory();
                    }
                    else
                    {
                        SaveCategory();
                    }
                }
                Finish();
            }
        }

        private void UpdateCategory()
        {
            var category = new ProductCategoriesModel
            {
                ProductCategoryName = mEtCategory.Text.ToUpper().Trim(),
                Id = selectedCategory.Id
            };
            mCategoriesDataAccess.UpdateTable(category);
        }

        private void UpdateSize()
        {
            var size = new ProductSizesModel
            {
                ProductSizeName = mEtCategory.Text.Trim(),
                Id = selectedSize.Id
            };
            mSizesDataAccess.UpdateTable(size);
        }

        private bool HasErrors()
        {
            bool retVal = false;
            string name = mEtCategory.Text.Trim();
            string label = isSize ? "size" : "category";
            string selectedName = "";

            if (isEdit)
            {
                selectedName = isSize ? selectedSize.ProductSizeName : selectedCategory.ProductCategoryName;
            }

            if (name == "")
            {
                DialogMessageService.MessageBox(this, "Blank fields", "Please input a "+ label +" name.");
                retVal = true;
            }
            else if(!isSize && (mCategoriesDataAccess.NameExists(name.ToUpper())|| name.ToLower() == "All"))
            {
                if (isEdit && name.ToUpper() == selectedName.ToUpper())
                {
                    retVal = false;
                }
                else
                {
                    DialogMessageService.MessageBox(this, "Error", "This " + label + " name already exists.");
                    retVal = true;
                }
            }
            else if (isSize && mSizesDataAccess.NameExists(name))
            {
                if (isEdit && name == selectedName)
                {
                    retVal = false;
                }
                else
                {
                    DialogMessageService.MessageBox(this, "Error", "This " + label + " already exists.");
                    retVal = true;
                }
            }
            else
            {
                retVal = false;
            }
            return retVal;
        }

        private void SaveCategory()
        {
            var category = new ProductCategoriesModel {
                ProductCategoryName = mEtCategory.Text.ToUpper().Trim(),
                Rank = mCategoriesDataAccess.GetMaxRank() + 1
            };
            mCategoriesDataAccess.InsertIntoTable(category);
        }

        private void SaveSize()
        {
            var size = new ProductSizesModel {
                ProductSizeName = mEtCategory.Text.Trim(),
                SizeRank = mSizesDataAccess.GetMaxSizeRank() + 1
            };
            mSizesDataAccess.InsertIntoTable(size);
        }

        private void FnGetData()
        {
            isSize = Intent.GetBooleanExtra("isSize", false); 
            isEdit = Intent.GetBooleanExtra("isEdit", false);
            selectedRecordId = Intent.GetIntExtra("selectedRecordId", 0);
            mSizesDataAccess = new SizesDataAccess();
            mCategoriesDataAccess = new CategoriesDataAccess();
            if (isEdit)
            {
                if (isSize)
                {
                    selectedSize = mSizesDataAccess.SelectRecord(selectedRecordId)[0];
                }
                else
                {
                    selectedCategory = mCategoriesDataAccess.SelectRecord(selectedRecordId)[0];
                }
            }
        }

        private void FnShowEditFields()
        {
            if (isSize)
            {
                mEtCategory.Text = selectedSize.ProductSizeName;
            }
            else
            {
                mEtCategory.Text = selectedCategory.ProductCategoryName;
            }
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

        private void FnSetControls()
        {
            mRlContents = FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mEtCategory = FindViewById<EditText>(Resource.Id.etCategory);
            mTxtMaxCharsLabel = FindViewById<TextView>(Resource.Id.txtMaxCharLabel);
            mTxtMaxCharsLabel.Text = isSize ? "Max. 5 characters" : "Max. 20 characters";
            if (isSize)
            {
                mEtCategory.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(SIZE_MAX_LENGTH) });
            }
            else
            {
                mEtCategory.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(CATEGORY_MAX_LENGTH) });
            }
            mLlCreateProductButtonContainer = FindViewById<LinearLayout>(Resource.Id.llCreateProductButtonContainer);
            mBtnCreate = FindViewById<Button>(Resource.Id.btnCreateProduct);
            if (isEdit)
            {
                FnShowEditFields();
            }
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (isEdit)
            {
                MenuInflater.Inflate(Resource.Menu.toolbar_menu_customers_edit, menu);
            }
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menuItem_delete:
                    if (isSize)
                    {
                        DeleteSize();
                    }
                    else
                    {
                        DeleteCategory();
                    }
                    
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void DeleteCategory()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Delete Category");
            alert.SetMessage("When you delete this category, the products with this category will also be deleted. Continue?"); //indicate here if the size has fo 

            alert.SetButton2("CANCEL", (c, ev) =>
            {
                //cancel button
            });
            alert.SetButton("YES", (c, ev) =>
            {
                mCategoriesDataAccess.DeleteFromTable(selectedRecordId);
                Finish();
            });
            alert.Show();
        }

        private void DeleteSize()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Delete Size");
            alert.SetMessage("When you delete this size, the products with this size will also be deleted. Continue?"); //indicate here if the size has fo 

            alert.SetButton2("CANCEL", (c, ev) =>
            {
                //cancel button
            });
            alert.SetButton("YES", (c, ev) =>
            {
                mSizesDataAccess.DeleteFromTable(selectedRecordId);
                Finish();
            });
            alert.Show();
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = isSize ? isEdit ? "EDIT SIZE" : "NEW SIZE" 
                                     : isEdit ? "EDIT CATEGORY" : "NEW CATEGORY";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}