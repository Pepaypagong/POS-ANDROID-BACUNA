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
    [Activity(Label = "ProductsFragmentItemsDescriptionActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ProductsFragmentItemsDescriptionActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        RelativeLayout mRlContents;
        LinearLayout mLlSaveButtonContainer;
        EditText mEtDescription;
        Button mBtnSave;
        float mDpVal;
        string selectedDescription;
        bool isBlank;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.products_fragment_items_new_product_description);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            FnGetData();
            //Window.SetWindowAnimations(Resource.Style.dialog_animation);
            Window.SetSoftInputMode(SoftInput.AdjustResize); //make button appear on top of keyboard
            Window.DecorView.Post(() =>
            {
                SetContentsLayoutHeight(mRlContents, mToolBar);
                keyboardState("VISIBLE", mEtDescription);
                mEtDescription.RequestFocus();
                mEtDescription.SetSelection(mEtDescription.Length());
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

        private void SetContentsLayoutHeight(RelativeLayout layout, SupportToolbar layoutBelow)
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
            mToolBar.InflateMenu(Resource.Menu.toolbar_menu_products_items_description);
            return base.OnCreateOptionsMenu(menu);
        }

        private void FnGetData()
        {
            selectedDescription = Intent.GetStringExtra("productDescriptionInit");
            isBlank = Intent.GetBooleanExtra("productDescriptionStatus", false);

            mEtDescription.Text = isBlank ? "" : selectedDescription;
        }

        private void FnSetEvents()
        {
            mToolBar.MenuItemClick += MToolBar_MenuItemClick;
            mBtnSave.Click += MBtnSave_Click;
        }

        private void MToolBar_MenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menuItemClear)
            {
                mEtDescription.Text = "";
            }
        }

        private void MBtnSave_Click(object sender, EventArgs e)
        {
            var result = new Intent();
            result.PutExtra("productDescription", mEtDescription.Text);
            SetResult(Result.Ok, result);

            Finish();
        }

        private void FnSetControls()
        {
            mRlContents = FindViewById<RelativeLayout>(Resource.Id.rlContents);
            mEtDescription = FindViewById<EditText>(Resource.Id.etDescription);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnSave = FindViewById<Button>(Resource.Id.btnSave);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    var result = new Intent();
                    result.PutExtra("productDescription", isBlank ? "" : selectedDescription);
                    SetResult(Result.Ok, result);
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
            actionBar.Title = "Description";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        public override void OnBackPressed()
        {
            var result = new Intent();
            result.PutExtra("productDescription", isBlank ? "" : selectedDescription);
            SetResult(Result.Ok, result);
            base.OnBackPressed();
        }

    }
}