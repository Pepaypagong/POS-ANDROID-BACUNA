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
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentPaymentPayLaterActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentPaymentPayLaterActivity : AppCompatActivity
    {
        bool mDialogShown = false;

        SupportToolbar mToolBar;
        TextView mTxtRunnerAlias;
        TextView mTxtRunnerName;
        TextView mTxtTransactionDate;
        TextView mTxtTransactionTime;
        ImageView mImgGetCurrentDatetime;
        TextView mTxtCurrentAccountBalance;
        TextView mTxtPayLaterAmount;
        TextView mTxtNewAccountBalance;
        Button mBtnGetPaidLater;

        double mTotalSaleAmount;
        string mCurrentSelectedRunner;
        int mCurrentSelectedRunnerId;
        DateTime mSelectedTransactionDatetime = DateTime.Now;
        double mCurrentAccountBalance;

        int selectedCalendarYear;
        int selectedCalendarMonth;
        int selectedCalendarDayOfMonth;
        int selectedHourOfDay;
        int selectedMinute;

        TransactionsModel mTransaction;
        string mPesoSign = "\u20b1";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_payment_paylater);
            FnGetData();
            FnSetUpToolBar();
            FnSetControls();
            FnSetEvents();
        }
        private void FnGetData()
        {
            mTotalSaleAmount = Intent.GetDoubleExtra("TotalSaleAmount", 0);
            mCurrentSelectedRunner = GlobalVariables.mCurrentSelectedCustomerOnCheckout; //runner
            mCurrentSelectedRunnerId = 0; //not yet implemented
            mSelectedTransactionDatetime = DateTime.Now;
            mCurrentAccountBalance = 0; //not yet implemented

            SetSelectedDateNow();
        }

        private void SetSelectedDateNow()
        {
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            selectedCalendarYear = calendar.Get(CalendarField.Year);
            selectedCalendarMonth = calendar.Get(CalendarField.Month);
            selectedCalendarDayOfMonth = calendar.Get(CalendarField.DayOfMonth);
            mSelectedTransactionDatetime = DateTime.Now;
        }

        private void FnSetUpToolBar()
        {
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.Title = "Confirm Transaction";
        }
        private void FnSetEvents()
        {
            mTxtTransactionDate.Click += MEtTransactionDate_Click;
            mTxtTransactionTime.Click += MTxtTransactionTime_Click;
            mBtnGetPaidLater.Click += MBtnGetPaidLater_Click;
            mImgGetCurrentDatetime.Click += MImgGetCurrentDatetime_Click;
        }

        private void MImgGetCurrentDatetime_Click(object sender, EventArgs e)
        {
            SetSelectedDateNow();
            mTxtTransactionDate.Text = DateTime.Now.ToString("d/MMM/yyyy").ToUpper();
            mTxtTransactionTime.Text = DateTime.Now.ToString("h:mm tt").ToUpper();
            SetTransactionDateTimeAppearance(true);
        }
        private void MTxtTransactionTime_Click(object sender, EventArgs e)
        {
            selectedHourOfDay = Convert.ToInt32(GetSelectedTime(mTxtTransactionTime.Text, DateTime.Now.ToString("d/MMM/yyyy")).ToString("HH"));
            selectedMinute = Convert.ToInt32(GetSelectedTime(mTxtTransactionTime.Text, DateTime.Now.ToString("d/MMM/yyyy")).ToString("mm"));
            TimePickerDialog tpd = new TimePickerDialog(this, TimeListener, selectedHourOfDay, selectedMinute, false);
            tpd.Show();
        }
        
        private DateTime GetSelectedTime(string _timeString, string _dateString)
        { 
            return Convert.ToDateTime(_dateString + " " + _timeString.ToLower());
        }

        private void MEtTransactionDate_Click(object sender, EventArgs e)
        {
            DatePickerDialog dpd = new DatePickerDialog(this, DateListener, selectedCalendarYear, selectedCalendarMonth, selectedCalendarDayOfMonth);
            dpd.Show();
        }

        private void DateListener(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            if (e.Date > DateTime.Now)
            {
                DialogMessageService.MessageBox(this, "Invalid date", "Selected date must not be later than today");
            }
            else
            {
                selectedCalendarYear = e.Year;
                selectedCalendarMonth = e.Month;
                selectedCalendarDayOfMonth = e.DayOfMonth;
                mTxtTransactionDate.Text = e.Date.ToString("d/MMM/yyyy").ToUpper();
                mSelectedTransactionDatetime = e.Date;
                SetTransactionDateTimeAppearance(false);
            }
        }

        private void TimeListener(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            if (GetSelectedTime(HourAndMinuteToDateTime(e.HourOfDay, e.Minute).ToString("h:mm tt").ToLower(), mTxtTransactionDate.Text) > DateTime.Now)
            {
                DialogMessageService.MessageBox(this, "Invalid time", "Selected time must not be later than the current time");
            }
            else
            {
                selectedHourOfDay = e.HourOfDay;
                selectedMinute = e.Minute;
                mTxtTransactionTime.Text = HourAndMinuteToDateTime(e.HourOfDay, e.Minute).ToString("h:mm tt").ToUpper();
                SetTransactionDateTimeAppearance(false);
            }
        }

        private DateTime HourAndMinuteToDateTime(int _hourOfDay, int _minute)
        {
            return Convert.ToDateTime(DateTime.Now.ToString("d/MMM/yyyy") + " " + _hourOfDay.ToString() + ":" + _minute.ToString());
        }
        private void MBtnGetPaidLater_Click(object sender, EventArgs e)
        {
            if (!mDialogShown)
            {
                SaveTransactionToDatabase();
                Intent intent = new Intent(this, typeof(CheckoutFragmentPaymentDoneActivity));
                intent.PutExtra("TotalSaleAmount", Convert.ToDouble(mTotalSaleAmount));
                intent.PutExtra("ChangeAmount", 0);
                intent.PutExtra("TransactionType", "PAYLATER");
                StartActivityForResult(intent, 32);
            }
        }

        private void SaveTransactionToDatabase()
        {
            TransactionsDataAccess transactionsDataAccess = new TransactionsDataAccess();
            mTransaction = new TransactionsModel();
            mTransaction.TransactionDateTime = GetSelectedDateTime();
            mTransaction.DateCreated = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT);
            mTransaction.DateModified = DateTime.Now.ToString(GlobalVariables.DATABASE_TIME_FORMAT);
            mTransaction.CustomerOrRunnerId = 1; //to change
            mTransaction.SubTotalAmount = Convert.ToDecimal(mTotalSaleAmount);
            mTransaction.DiscountAmount = 0;
            mTransaction.TransactionType = "PAYLATER"; //ORDER, PAYMENT, PAYLATER
            transactionsDataAccess.InsertIntoTable(mTransaction);
        }

        private string GetSelectedDateTime()
        {
            string transactionDateFormatted = Convert.ToDateTime(mTxtTransactionDate.Text.ToLower()).ToString("yyyy-MM-dd");
            string transactionTime = Convert.ToDateTime(mTxtTransactionTime.Text.ToLower()).ToString("HH:mm");
            string transactionDateTimeFormatted = transactionDateFormatted + ' ' + transactionTime;
            return transactionDateTimeFormatted;
        }

        private void FnSetControls()
        {
            mTxtRunnerAlias = FindViewById<TextView>(Resource.Id.TxtRunnerAlias);
            mTxtRunnerName = FindViewById<TextView>(Resource.Id.txtRunnerName);
            mTxtTransactionDate = FindViewById<TextView>(Resource.Id.txtTransactionDate);
            mTxtTransactionTime = FindViewById<TextView>(Resource.Id.txtTransactionTime);
            mImgGetCurrentDatetime = FindViewById<ImageView>(Resource.Id.imgGetCurrentDateTime);
            mTxtCurrentAccountBalance = FindViewById<TextView>(Resource.Id.txtCurrentAccountBalance);
            mTxtPayLaterAmount = FindViewById<TextView>(Resource.Id.txtPayLaterAmount);
            mTxtNewAccountBalance = FindViewById<TextView>(Resource.Id.txtNewAccountBalance);
            mBtnGetPaidLater = FindViewById<Button>(Resource.Id.btnSave);

            mTxtRunnerAlias.Text = GetRunnerAlias();
            mTxtRunnerName.Text = mCurrentSelectedRunner;
            mTxtTransactionDate.Text = mSelectedTransactionDatetime.ToString("d/MMM/yyyy").ToUpper();
            mTxtTransactionTime.Text = mSelectedTransactionDatetime.ToString("h:mm tt").ToUpper();
            mTxtCurrentAccountBalance.Text = mPesoSign + string.Format("{0:n}", mCurrentAccountBalance);
            mTxtPayLaterAmount.Text = mPesoSign + string.Format("{0:n}", mTotalSaleAmount);
            mTxtNewAccountBalance.Text = mPesoSign + string.Format("{0:n}", GetNewAccountBalance());
            mBtnGetPaidLater.Text = "Get paid later " + mPesoSign + string.Format("{0:n}", mTotalSaleAmount);

            SetTransactionDateTimeAppearance(true);
        }

        private void SetTransactionDateTimeAppearance(bool _isCurrentTime)
        {
            mImgGetCurrentDatetime.Visibility = _isCurrentTime ? ViewStates.Invisible : ViewStates.Visible;
            mTxtTransactionDate.SetTextColor(ColorHelper.ResourceIdToColor(_isCurrentTime ? 
                Resource.Color.colorBlurred : Resource.Color.colorLightBlack, this));
            mTxtTransactionTime.SetTextColor(ColorHelper.ResourceIdToColor(_isCurrentTime ?
                Resource.Color.colorBlurred : Resource.Color.colorLightBlack, this));
        }

        private double GetNewAccountBalance()
        {
            return mCurrentAccountBalance + mTotalSaleAmount;
        }

        private string GetRunnerAlias()
        {
            if (mCurrentSelectedRunner.Length > 1)
            {
                return mCurrentSelectedRunner.Substring(0, 2).ToUpper();
            }
            else
            {
                return mCurrentSelectedRunner.ToUpper();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 32)
            {
                mDialogShown = false;
                try
                {
                    bool isStartNewTransaction = data.GetBooleanExtra("isStartNewTransaction", false);
                    if (isStartNewTransaction)
                    {
                        var result = new Intent();
                        result.PutExtra("isStartNewTransaction", isStartNewTransaction);
                        SetResult(Result.Ok, result);
                        Finish();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
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

    }
}