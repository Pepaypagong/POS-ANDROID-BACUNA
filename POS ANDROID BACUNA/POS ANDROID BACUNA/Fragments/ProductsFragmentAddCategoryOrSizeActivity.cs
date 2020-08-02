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
        int SIZE_MAX_LENGTH = 5;
        int CATEGORY_MAX_LENGTH = 20;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_categories_new_category);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
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

        private void FnGetData()
        {
            isSize = Intent.GetBooleanExtra("isSize", false);
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
                    Finish();
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
            actionBar.Title = isSize ? "NEW SIZE" : "NEW CATEGORY";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

    }
}