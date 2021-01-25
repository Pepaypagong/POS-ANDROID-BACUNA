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
using Android.Support.V4.View;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "TransactionsFragmentTransactionInfoActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TransactionsFragmentTransactionInfoActivity : AppCompatActivity
    {
        string mPesoSign = "\u20b1 ";
        TabLayout mTabs;
        SupportToolbar mToolbar;
        ViewPager mViewPager;

        TextView mTxtSaleAmount;
        ImageView mImgTranstypeIcon;
        TextView mTxtTransactionStatus;
        TextView mTxtTransactionNumber;
        TextView mTxtCashierName;
        LinearLayout mLlNote;
        LinearLayout mLlSaveButtonContainer;
        Button mSaveButton;
        ImageButton mImgMoreOptions;

        TransactionsDataAccess mTransactionsDataAccess;
        int mSelectedTransactionId;
        TransactionsModel mSelectedTransaction;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.transactions_fragment_transaction_info);
            FnGetData();
            FnSetUpToolbar();
            FnSetControls();
            FnSetEvents();
            Window.DecorView.Post(() =>
            {
                SetViewPagerHeight(mViewPager, mTabs);
            });
        }

        private void FnSetUpToolbar()
        {
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolbar);
            SupportActionBar actionBar = SupportActionBar;

            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = mSelectedTransaction.TransactionDateTime;
        }

        private void FnGetData()
        {
            mTransactionsDataAccess = new TransactionsDataAccess();
            mSelectedTransactionId = Intent.GetIntExtra("selectedTransactionId", 0);
            mSelectedTransaction = mTransactionsDataAccess.SelectRecord(mSelectedTransactionId)[0];
        }

        public int DataToFragments()
        {
            return mSelectedTransactionId;
        }

        private void FnSetControls()
        {
            mTxtSaleAmount = FindViewById<TextView>(Resource.Id.txtSaleAmount);
            mImgTranstypeIcon = FindViewById<ImageView>(Resource.Id.imgTranstypeIcon);
            mTxtTransactionStatus = FindViewById<TextView>(Resource.Id.txtTransactionStatus);
            mTxtTransactionNumber = FindViewById<TextView>(Resource.Id.txtTransactionNumber);
            mTxtCashierName = FindViewById<TextView>(Resource.Id.txtCashierName);
            mLlNote = FindViewById<LinearLayout>(Resource.Id.llNote);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mSaveButton = FindViewById<Button>(Resource.Id.btnSave);
            mImgMoreOptions = FindViewById<ImageButton>(Resource.Id.imgMoreOptions);

            mTabs = FindViewById<TabLayout>(Resource.Id.tabsMenu);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            SetUpViewPager(mViewPager);
            mTabs.SetupWithViewPager(mViewPager);

            mTxtSaleAmount.Text = mPesoSign + string.Format("{0:n}", mSelectedTransaction.SubTotalAmount);
            mTxtTransactionStatus.Text = "[TRANSACTION STATUS]";
            mTxtTransactionNumber.Text = "# " + mSelectedTransaction.id.ToString();
            mTxtCashierName.Text = mSelectedTransaction.CashierName;
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

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabFragmentAdapter adapter = new TabFragmentAdapter(SupportFragmentManager);
            adapter.AddFragment(new TransactionsFragmentTransactionItemsFragment(), "ITEMS");
            adapter.AddFragment(new TransactionsFragmentTransactionDetailsFragment(), "DETAILS");
            viewPager.Adapter = adapter;
        }
        private void FnSetEvents()
        {
            mTabs.TabSelected += MTabs_TabSelected;
            mImgMoreOptions.Click += MImgMoreOptions_Click;
        }

        private void MImgMoreOptions_Click(object sender, EventArgs e)
        {
            ShowMoreOptionsDialogFragment();
        }

        private void MTabs_TabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {

        }

        private void SetViewPagerHeight(ViewPager layout, View layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int recyclerViewHeight = layout.Height;

            int _topMargin = 0;
            int _bottomMargin = 0;
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

        private void ShowMoreOptionsDialogFragment()
        {
            //show dialog here
            CreateOptions();
            SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            MoreOptionsDialogFragment moreOptionsDialog = new MoreOptionsDialogFragment(this);
            moreOptionsDialog.SetStyle((int)DialogFragmentStyle.Normal, Resource.Style.Dialog_FullScreen);
            //moreOptionsDialog.SetStyle(Convert.ToInt32(DialogFragmentStyle.Normal), Resource.Style.Dialog_FullScreen);
            //pass current selected price type t
            var args = new Bundle();
            args.PutString("caller", "TransactionsFragmentTransactionInfoActivity");
            args.PutInt("selectedTransactionId", mSelectedTransactionId);
            moreOptionsDialog.Arguments = args;
            moreOptionsDialog.Show(transaction, "moreOptionsDialogFragment");
        }

        private void CreateOptions()
        {
            GlobalVariables.globalOptionList.Clear();
            GlobalVariables.globalOptionList
                .Add(new Options()
                {
                    OptionId = 1,
                    OptionText = "Show Receipt",
                    TextColorResourceId = Resource.Color.colorLightBlack,
                    ShowArrow = true,
                    CallerClassName = "TransactionsFragmentTransactionInfoActivity",
                    Action = "Modify",
                    TargetActivity = "TransactionsReceiptPreview",
                    RequestCode = 45,
                    IsDialog = false
                });
            GlobalVariables.globalOptionList
                .Add(new Options()
                {
                    OptionId = 2,
                    OptionText = "Cancel Transaction",
                    TextColorResourceId = Resource.Color.colorLightBlack,
                    ShowArrow = true,
                    CallerClassName = "TransactionsFragmentTransactionInfoActivity",
                    Action = "Delete",
                    TargetActivity = "TransactionsReceiptPreview",
                    RequestCode = 46,
                    IsDialog = false
                });
        }

        public void CancelTransaction()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Cancel Transaction");
            alert.SetMessage("Do you want to cancel this transaction?");

            alert.SetButton2("NO", (c, ev) =>
            {
                //cancel button
            });

            alert.SetButton("YES", (c, ev) =>
            {
                Finish();
            });

            alert.Show();
        }
    }
}