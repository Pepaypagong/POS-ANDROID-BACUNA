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
    [Activity(Label = "RunnersFragmentPaybydateActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RunnersFragmentPaybydateActivity : AppCompatActivity
    {
        bool mDialogShown = false;
        SupportToolbar mToolBar;
        TextView mTxtRunnerName;
        TextView mTxtStartDate;
        TextView mTxtEndDate;
        RecyclerView mRvList;
        Button mBtnPay;
        float mDpVal;
        private string mPesoSign = "\u20b1";

        RecyclerView.LayoutManager mLayoutManager;
        RunnersMultipayRecyclerViewAdapter mRecyclerViewAdapter;
        TransactionsDataAccess mTransactionsDataAccess;
        RunnersDataAccess mRunnersDataAccess;
        int mSelectedRunnerId;
        RunnersModel mSelectedRunner;

        Calendar mCalendar = Calendar.GetInstance(Locale.Default);

        //datepicker start
        int selectedStartCalendarYear;
        int selectedStartCalendarMonth;
        int selectedStartCalendarDayOfMonth;

        //datepicker end
        int selectedEndCalendarYear;
        int selectedEndCalendarMonth;
        int selectedEndCalendarDayOfMonth;

        DateTime mSelectedStartDate;
        DateTime mSelectedEndDate;

        decimal mTotalReceiptAmount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.runners_fragment_paybydate);
            mDpVal = this.Resources.DisplayMetrics.Density;
            FnSetUpData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
            FnSetTransactionList();
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            Window.DecorView.Post(() =>
            {
                SetRecyclerViewLayoutHeight(mRvList, mTxtEndDate);
                mRvList.SmoothScrollToPosition(mRecyclerViewAdapter.ItemCount);
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_runners_new_multipay, menu);
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
                    Intent intent = new Intent(this, typeof(TransactionsReceiptPreview));
                    intent.PutExtra("selectedTransactionId", 0);
                    intent.PutExtra("isMultipay", true);
                    intent.PutExtra("isMultipayPreview", true);
                    intent.PutExtra("multipayPreviewIdList",GetSelectedIdList());
                    StartActivityForResult(intent, 46);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private string GetSelectedIdList()
        {
            string retval = "";
            foreach (var item in GetTransactions(mSelectedStartDate, mSelectedEndDate))
            {
                retval += "#" + item.id.ToString() + ", ";
            }
            retval = retval.Remove(retval.Length - 2);
            return retval;
        }

        private void FnSetUpData()
        {
            mSelectedRunnerId = Intent.GetIntExtra("selectedRunnerId", 0);
            mTransactionsDataAccess = new TransactionsDataAccess();
            mRunnersDataAccess = new RunnersDataAccess();
            mSelectedRunner = mRunnersDataAccess.SelectRecord(mSelectedRunnerId)[0];

            //get startdate and end date based from unpaid records of the selected runner
            var holder = mTransactionsDataAccess.SelectTable()
                                                .Where(x => x.CustomerOrRunnerId == mSelectedRunnerId && !x.IsPaid).ToList();
            holder = holder.OrderBy(x => StringDateToDate(x.TransactionDateTime)).ToList();

            if (holder.Count != 0)
            {
                mSelectedStartDate = StringDateToDate(holder[0].TransactionDateTime);
                mSelectedEndDate = StringDateToDate(holder[holder.Count() - 1].TransactionDateTime);
            }

            selectedStartCalendarYear = mSelectedStartDate.Year;
            selectedStartCalendarMonth = mSelectedStartDate.Month - 1;
            selectedStartCalendarDayOfMonth = mSelectedStartDate.Day;

            selectedEndCalendarYear = mSelectedEndDate.Year;
            selectedEndCalendarMonth = mSelectedEndDate.Month - 1;
            selectedEndCalendarDayOfMonth = mSelectedEndDate.Day;
        }

        private void FnSetTransactionList()
        {
            mLayoutManager = new LinearLayoutManager(this);
            mRvList.SetLayoutManager(mLayoutManager);
            mRvList.HasFixedSize = true;
            mRecyclerViewAdapter = new RunnersMultipayRecyclerViewAdapter(this, this, GetTransactions(mSelectedStartDate,mSelectedEndDate));
            mRvList.SetAdapter(mRecyclerViewAdapter);
        }
        private List<TransactionsModel> GetTransactions(DateTime _startDate, DateTime _endDate)
        {
            var transactions = mTransactionsDataAccess.SelectTable().Where(x=>x.CustomerOrRunnerId == mSelectedRunnerId && !x.IsPaid);
            List<TransactionsModel> returnList = new List<TransactionsModel>();
            returnList = transactions.Where(d=> StringDateToDate(d.TransactionDateTime) >= ShortDateFormat(_startDate) &&
                                                StringDateToDate(d.TransactionDateTime) <= ShortDateFormat(_endDate))
                                     .OrderByDescending(d => StringDateToDateTime(d.TransactionDateTime))
                                     .ToList();
            ShowTotalAmount(returnList);
            return returnList;
        }

        private void ShowTotalAmount(List<TransactionsModel> transactions)
        {
            mTotalReceiptAmount = transactions.Sum(x => x.SubTotalAmount);
            int transactionCount = transactions.Count();
            mBtnPay.Text = "Pay " + mPesoSign + string.Format("{0:n}", mTotalReceiptAmount) + " from " + transactionCount + " sale(s)";
        }

        private DateTime ShortDateFormat(DateTime _date)
        {
            return new DateTime(_date.Year, _date.Month, _date.Day, 0, 0, 0);
        }

        private DateTime StringDateToDateTime(string _stringDate)
        {
            var x = Convert.ToDateTime(_stringDate).ToString("yyyy MM dd hh:mm tt");
            return Convert.ToDateTime(x);
        }

        private DateTime StringDateToDate(string _stringDate)
        {
            var x = Convert.ToDateTime(_stringDate).ToString("yyyy MM dd");
            return Convert.ToDateTime(x);
        }

        private void FnSetControls()
        {
            mTxtRunnerName = FindViewById<TextView>(Resource.Id.txtRunnerName);
            mTxtStartDate = FindViewById<TextView>(Resource.Id.txtStartDate);
            mTxtEndDate = FindViewById<TextView>(Resource.Id.txtEndDate);
            mRvList = FindViewById<RecyclerView>(Resource.Id.rvList);
            mBtnPay = FindViewById<Button>(Resource.Id.btnPay);

            mTxtRunnerName.Text = mSelectedRunner.FullName;
            mTxtStartDate.Text = mSelectedStartDate.ToString("d MMM yyyy").ToUpper();
            mTxtEndDate.Text = mSelectedEndDate.ToString("d MMM yyyy").ToUpper();
        }
        private void FnSetEvents()
        {
            mBtnPay.Click += MBtnPay_Click;
            mTxtStartDate.Click += MTxtStartDate_Click;
            mTxtEndDate.Click += MTxtEndDate_Click;
        }

        private void MTxtEndDate_Click(object sender, EventArgs e)
        {
            DatePickerDialog dpd = new DatePickerDialog(this, EndDateListener, selectedEndCalendarYear, selectedEndCalendarMonth, selectedEndCalendarDayOfMonth);
            if (mTxtStartDate.Text == "Start Date")
            {
                dpd.DatePicker.MinDate = Java.Lang.JavaSystem.CurrentTimeMillis();
            }
            else
            {
                Calendar calendar = Calendar.GetInstance(Locale.Default);
                calendar.Set(selectedStartCalendarYear, selectedStartCalendarMonth, selectedStartCalendarDayOfMonth);
                dpd.DatePicker.MinDate = calendar.TimeInMillis;
            }
            dpd.DatePicker.MaxDate = Java.Lang.JavaSystem.CurrentTimeMillis();
            dpd.Show();
        }

        private void EndDateListener(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            selectedEndCalendarYear = e.Year;
            selectedEndCalendarMonth = e.Month;
            selectedEndCalendarDayOfMonth = e.DayOfMonth;
            mTxtEndDate.Text = e.Date.ToString("d MMM yyyy").ToUpper();
            mSelectedEndDate = e.Date;
            //refresh list here
            mRecyclerViewAdapter.RefreshList(GetTransactions(mSelectedStartDate, mSelectedEndDate));
        }

        private void MTxtStartDate_Click(object sender, EventArgs e)
        {
            DatePickerDialog dpd = new DatePickerDialog(this, StartDateListener, selectedStartCalendarYear, selectedStartCalendarMonth, selectedStartCalendarDayOfMonth);
            dpd.DatePicker.MaxDate = Java.Lang.JavaSystem.CurrentTimeMillis();
            dpd.Show();
        }

        private void StartDateListener(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            selectedStartCalendarYear = e.Year;
            selectedStartCalendarMonth = e.Month;
            selectedStartCalendarDayOfMonth = e.DayOfMonth;
            mTxtStartDate.Text = e.Date.ToString("d MMM yyyy").ToUpper();
            mSelectedStartDate = e.Date;
            //refresh list here
            mRecyclerViewAdapter.RefreshList(GetTransactions(mSelectedStartDate, mSelectedEndDate));
        }

        private void MBtnPay_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RunnersFragmentPaymentActivity));
            double totalSaleAmount = Convert.ToDouble(mTotalReceiptAmount);
            intent.PutExtra("TotalSaleAmount", totalSaleAmount);
            intent.PutExtra("IsFromRunnerMultipay", true);
            intent.PutExtra("SelectedRunnerIdOnMultipay", mSelectedRunner.Id);
            intent.PutExtra("TransactionIds", GetSelectedIdList());
            intent.PutExtra("StartDate",mSelectedStartDate.ToString(GlobalVariables.DATABASE_TIME_FORMAT));
            intent.PutExtra("EndDate",mSelectedEndDate.ToString(GlobalVariables.DATABASE_TIME_FORMAT));
            StartActivityForResult(intent, 47);
        }

        private void SetRecyclerViewLayoutHeight(RecyclerView layout, View layoutBelow)
        {
            int checkoutButtonHeight = mBtnPay.Height;
            int recyclerViewHeight = layout.Height;

            int _topMargin = dpToPixel(15);
            int _bottomMargin = dpToPixel(22);
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

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;

            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = "MULTIPLE PAYMENTS";
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 47)
            {
                mRecyclerViewAdapter.RefreshList(GetTransactions(mSelectedStartDate, mSelectedEndDate));
                bool isStartNewTransaction = data.GetBooleanExtra("isStartNewTransaction", false);
                if (isStartNewTransaction)
                {
                    var result = new Intent();
                    result.PutExtra("isStartNewTransaction", isStartNewTransaction);
                    SetResult(Result.Ok, result);
                    Finish();
                }
            }
        }
    }
}