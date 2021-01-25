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
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "RunnersFragmentMultiplePaymentRecords", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RunnersFragmentMultiplePaymentRecords : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        TextView mTxtRunnerName;
        RecyclerView mRvList;

        RecyclerView.LayoutManager mLayoutManager;
        RunnersMultipayRecordsRecyclerViewAdapter mRecyclerViewAdapter;
        RunnersMultipayRecordsDataAccess mRunnersMultipayRecordsDataAccess;
        RunnersDataAccess mRunnersDataAccess;
        int mSelectedRunnerId;
        RunnersModel mSelectedRunner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.runners_fragment_multipay_records);
            FnSetUpData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            FnSetTransactionList();
            Window.SetSoftInputMode(SoftInput.AdjustResize);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_runners_paybydate, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menuItemReceipt:
                    Intent intent = new Intent(this, typeof(RunnersFragmentPaybydateActivity));
                    intent.PutExtra("selectedRunnerId", mSelectedRunnerId);
                    StartActivityForResult(intent, 44);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void FnSetUpData()
        {
            mSelectedRunnerId = Intent.GetIntExtra("selectedRunnerId", 0);
            mRunnersMultipayRecordsDataAccess = new RunnersMultipayRecordsDataAccess();
            mRunnersDataAccess = new RunnersDataAccess();
            mSelectedRunner = mRunnersDataAccess.SelectRecord(mSelectedRunnerId)[0];
        }

        private void FnSetTransactionList()
        {
            mLayoutManager = new LinearLayoutManager(this);
            mRvList.SetLayoutManager(mLayoutManager);
            mRvList.HasFixedSize = true;
            mRecyclerViewAdapter = new RunnersMultipayRecordsRecyclerViewAdapter(this, this, GetRecords());
            mRvList.SetAdapter(mRecyclerViewAdapter);
        }
        private List<RunnersMultipayRecordsModel> GetRecords()
        {
            //var list = new List<RunnersMultipayRecordsModel>();
            //list.Add(new RunnersMultipayRecordsModel() 
            //    {
            //        Id = 1,
            //        CashierName = "Jepoy",
            //        TransactionDateTime = "2020-12-14 6:45",
            //        RunnerId = 1,
            //        SubTotalAmount = 45884,
            //        DiscountAmount = 0,
            //        PaymentCashAmount = 46000,
            //        PaymentCheckAmount = 0,
            //        TransactionIds = "#47, #46",
            //        StartDate = "2020-12-11 6:45",
            //        EndDate = "2020-12-14 12:45"
            //    });
            //return list;
            return mRunnersMultipayRecordsDataAccess.SelectTable().Where(x => x.RunnerId == mSelectedRunnerId).ToList();
        }

        private void FnSetControls()
        {
            mTxtRunnerName = FindViewById<TextView>(Resource.Id.txtRunnerName);
            mRvList = FindViewById<RecyclerView>(Resource.Id.rvList);

            mTxtRunnerName.Text = mSelectedRunner.FullName;
        }
        private void FnSetEvents()
        {
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;

            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = "MULTIPLE PAYMENT RECORDS";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 44)
            {
                mRecyclerViewAdapter.RefreshList(GetRecords());
            }
        }
    }
}