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
using POS_ANDROID_BACUNA.Fragments;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;
using Java.Util;
using Android.Graphics;
using System.Threading;
using Android.Graphics.Drawables;
using System.Runtime.Hosting;

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "TransactionsFilterActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TransactionsFilterActivity : AppCompatActivity
    {
        private Button mBtnApplyFilter;
        private TextView mTxtStartDate;
        private TextView mTxtEndDate;
        private ImageView mImgCancelStartDate;
        private ImageView mImgCancelEndDate;
        private LinearLayout mLlSaveButtonContainer;
        private ScrollView mSvContents;
        private SupportToolbar toolBar;
        private GridLayout mGlFilters;
        private TextView mTxtLast30Days;
        private CheckBox mCbCustomerSales;
        private CheckBox mCbPayLaterUnpaid;
        private CheckBox mCbPayLaterPaid;
        private CheckBox mCbCash;
        private CheckBox mCbCheck;
        private CheckBox mCbShowCancelSalesOnly;

        Calendar mCalendar = Calendar.GetInstance(Locale.Default);

        //datepicker start
        int selectedStartCalendarYear;
        int selectedStartCalendarMonth;
        int selectedStartCalendarDayOfMonth;

        //datepicker end
        int selectedEndCalendarYear;
        int selectedEndCalendarMonth;
        int selectedEndCalendarDayOfMonth;

        DateTime mCurrentSelectedStartDate = DateTime.Now;
        DateTime mCurrentSelectedEndDate = DateTime.Now;

        bool mIsApplyButtonEnabled = true;
        string mSelectedFilterText = "Total last 30 days";
        bool mIsCustomerSalesFilterChecked = false;
        bool mIsPaylaterUnpaidFilterChecked = false;
        bool mIsPaylaterPaidFilterChecked = false;
        bool mIsCashFilterChecked = false;
        bool mIsCheckFilterChecked = false;
        bool mIsShowCancelledSalesChecked = false;
        bool mIsDateRangeUsed = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.transactions_filter);

            FnGetData();
            FnSetUpControls();
            FnSetUpToolbar();
            FnSetUpEvents();
            Window.DecorView.Post(() =>
            {
                SetScrollViewHeight(mSvContents, toolBar);
            });
        }

        private void SetScrollViewHeight(View contents, View layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int recyclerViewHeight = contents.Height;

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
            contents.LayoutParameters = layoutParams;
            contents.RequestLayout();
        }

        private void FnGetData()
        {
            mSelectedFilterText = Intent.GetStringExtra("selectedFilterDateRange");
 
            mCurrentSelectedStartDate = new DateTime(Intent.GetLongExtra("previousSelectedStartDate", Java.Lang.JavaSystem.CurrentTimeMillis()));
            mCurrentSelectedEndDate = new DateTime(Intent.GetLongExtra("previousSelectedEndDate", Java.Lang.JavaSystem.CurrentTimeMillis()));

            selectedStartCalendarYear = mCurrentSelectedStartDate.Year;
            selectedStartCalendarMonth = mCurrentSelectedStartDate.Month - 1;
            selectedStartCalendarDayOfMonth = mCurrentSelectedStartDate.Day;

            selectedEndCalendarYear = mCurrentSelectedEndDate.Year;
            selectedEndCalendarMonth = mCurrentSelectedEndDate.Month - 1;
            selectedEndCalendarDayOfMonth = mCurrentSelectedEndDate.Day;

            mIsCustomerSalesFilterChecked = Intent.GetBooleanExtra("isCustomerSalesFilterChecked",false);
            mIsPaylaterUnpaidFilterChecked = Intent.GetBooleanExtra("isPaylaterUnpaidFilterChecked", false);
            mIsPaylaterPaidFilterChecked = Intent.GetBooleanExtra("isPaylaterPaidFilterChecked", false);
            mIsCashFilterChecked = Intent.GetBooleanExtra("isCashFilterChecked", false);
            mIsCheckFilterChecked = Intent.GetBooleanExtra("isCheckFilterChecked", false);
            mIsShowCancelledSalesChecked = Intent.GetBooleanExtra("isShowCancelledSalesChecked", false);
        }

        private void FnSetUpEvents()
        {
            mTxtStartDate.Click += MTxtStartDate_Click;
            mTxtEndDate.Click += MTxtEndDate_Click;
            mImgCancelStartDate.Click += MImgCancelStartDate_Click;
            mImgCancelEndDate.Click += MImgCancelEndDate_Click;
            mTxtLast30Days.Click += MTxtLast30Days_Click;
            mBtnApplyFilter.Click += MBtnApplyFilter_Click;
            RegisterDateFilterGridEvent();
        }

        private void MBtnApplyFilter_Click(object sender, EventArgs e)
        {
            if (mIsApplyButtonEnabled)
            {
                var result = new Intent();
                result.PutExtra("isFiltered", IsFiltered());
                result.PutExtra("isCancelled", false);
                result.PutExtra("isCleared", false);
                result.PutExtra("filterText", FilterText());
                result.PutExtra("isDateRangeUsed", mIsDateRangeUsed);
                result.PutExtra("startDate", mCurrentSelectedStartDate.Ticks);
                result.PutExtra("endDate", mCurrentSelectedEndDate.Ticks);
                result.PutExtra("selectedFilterDateRange",mIsDateRangeUsed ? "" : mSelectedFilterText);
                result.PutExtra("isCustomerSalesFilterChecked", mCbCustomerSales.Checked);
                result.PutExtra("isPaylaterUnpaidFilterChecked", mCbPayLaterUnpaid.Checked);
                result.PutExtra("isPaylaterPaidFilterChecked", mCbPayLaterPaid.Checked);
                result.PutExtra("isCashFilterChecked", mCbCash.Checked);
                result.PutExtra("isCheckFilterChecked", mCbCheck.Checked);
                result.PutExtra("isShowCancelledSalesChecked", mCbShowCancelSalesOnly.Checked);
                SetResult(Result.Ok, result);
                Finish();
            }
            else
            {
                DialogMessageService.MessageBox(this,"Incomplete fields","Please set a period first");
            }
        }

        private bool IsFiltered()
        {
            bool retval = false;

            CheckBox[] checkboxes = { mCbCustomerSales,mCbPayLaterPaid,mCbPayLaterUnpaid,mCbCash,mCbCheck,mCbShowCancelSalesOnly};
            if (mTxtStartDate.Text != "Start Date" || mTxtEndDate.Text != "End Date")
            {
                retval = true;
            }
            if (mSelectedFilterText!= "")
            {
                retval = true;
            }
            foreach (var item in checkboxes)
            {
                if (item.Checked)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private string FilterText() 
        {
            string retVal = "";
            if (mTxtStartDate.Text == "Start Date")
            {
                retVal += mSelectedFilterText + " ";
            }
            else
            {
                retVal = mTxtStartDate.Text.Remove(mTxtStartDate.Text.Length - 4);
                retVal += "to " + mTxtEndDate.Text.Remove(mTxtEndDate.Text.Length - 4) + " ";
            }
            if (mCbCustomerSales.Checked)
            {
                retVal += "\nCustomer sales,";
            }
            if (mCbPayLaterUnpaid.Checked)
            {
                retVal += "Pay later (unpaid),";
            }
            if (mCbPayLaterPaid.Checked)
            {
                retVal += "Pay later (paid),";
            }
            if (mCbCash.Checked)
            {
                retVal += "Cash,";
            }
            if (mCbCheck.Checked) 
            {
                retVal += "Check,";
            }

            if (mCbShowCancelSalesOnly.Checked)
            {
                retVal += "Show cancelled sales";
            }
            if (retVal.EndsWith(","))
            {
                retVal = retVal.Substring(0, retVal.Length - 1);
            }
            return retVal;
        }

        public void SetButtonAppearance(bool _isEnabled)
        {
            mIsApplyButtonEnabled = _isEnabled;
            Color white = new Color(ColorHelper.ResourceIdToColor(Resource.Color.colorPrimary, this));
            Color green = new Color(ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));

            mBtnApplyFilter.SetTextColor(_isEnabled ? white : green);
            mBtnApplyFilter.Background = this.GetDrawable(_isEnabled ? Resource.Drawable.buttonCheckoutRoundBorderWithItem
                : Resource.Drawable.buttonCheckoutRoundBorderNoItem);
        }

        private void MImgCancelEndDate_Click(object sender, EventArgs e)
        {
            mImgCancelEndDate.Visibility = ViewStates.Invisible;
            mTxtEndDate.Text = "End Date";
            selectedEndCalendarYear = mCalendar.Get(CalendarField.Year);
            selectedEndCalendarMonth = mCalendar.Get(CalendarField.Month);
            selectedEndCalendarDayOfMonth = mCalendar.Get(CalendarField.DayOfMonth);
            mCurrentSelectedEndDate = DateTime.Now;
            if (mTxtStartDate.Text == "Start Date")
            {
                SetSelectedDateFilter(Resource.Id.txtLast30Days);
                mIsDateRangeUsed = false;
                SetButtonAppearance(true);
            }
            else
            {
                SetButtonAppearance(false);
            }
        }

        private void MImgCancelStartDate_Click(object sender, EventArgs e)
        {
            mImgCancelStartDate.Visibility = ViewStates.Invisible;
            mTxtStartDate.Text = "Start Date";
            selectedStartCalendarYear = mCalendar.Get(CalendarField.Year);
            selectedStartCalendarMonth = mCalendar.Get(CalendarField.Month);
            selectedStartCalendarDayOfMonth = mCalendar.Get(CalendarField.DayOfMonth);
            mCurrentSelectedStartDate = DateTime.Now;
            if (mTxtEndDate.Text == "End Date")
            {
                SetSelectedDateFilter(Resource.Id.txtLast30Days);
                mIsDateRangeUsed = false;
                SetButtonAppearance(true);
            }
            else
            {
                SetButtonAppearance(false);
            }
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
            mImgCancelEndDate.Visibility = ViewStates.Visible;
            selectedEndCalendarYear = e.Year;
            selectedEndCalendarMonth = e.Month;
            selectedEndCalendarDayOfMonth = e.DayOfMonth;
            mTxtEndDate.Text = e.Date.ToString("d MMM yyyy").ToUpper();
            mCurrentSelectedEndDate = e.Date;
            SetSelectedDateFilter(0);
            SetButtonAppearance(mTxtStartDate.Text == "Start Date" ? false : true);
            mIsDateRangeUsed = true;
        }

        private void MTxtStartDate_Click(object sender, EventArgs e)
        {
            DatePickerDialog dpd = new DatePickerDialog(this, StartDateListener, selectedStartCalendarYear, selectedStartCalendarMonth, selectedStartCalendarDayOfMonth);
            dpd.DatePicker.MaxDate = Java.Lang.JavaSystem.CurrentTimeMillis();
            dpd.Show();
        }

        private void StartDateListener(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            mImgCancelStartDate.Visibility = ViewStates.Visible;
            selectedStartCalendarYear = e.Year;
            selectedStartCalendarMonth = e.Month;
            selectedStartCalendarDayOfMonth = e.DayOfMonth;
            mTxtStartDate.Text = e.Date.ToString("d MMM yyyy").ToUpper();
            mCurrentSelectedStartDate = e.Date;
            SetSelectedDateFilter(0);
            SetButtonAppearance(mTxtEndDate.Text == "End Date" ? false : true);
            mIsDateRangeUsed = true;
        }

        private void FnSetUpToolbar()
        {
            SetSupportActionBar(toolBar);
            SupportActionBar ab = SupportActionBar;
            //ab.SetHomeAsUpIndicator(Resource.Drawable.down_arrow_icon);
            ab.SetDisplayShowHomeEnabled(true);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.Title = "Filter";
        }
         
        private void FnSetUpControls()
        {
            toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            mSvContents = FindViewById<ScrollView>(Resource.Id.svContents);
            mTxtStartDate = FindViewById<TextView>(Resource.Id.txtStartDate);
            mTxtEndDate = FindViewById<TextView>(Resource.Id.txtEndDate);
            mImgCancelStartDate = FindViewById<ImageView>(Resource.Id.imgCancelStartDate);
            mImgCancelEndDate = FindViewById<ImageView>(Resource.Id.imgCancelEndDate);
            mGlFilters = FindViewById<GridLayout>(Resource.Id.glFilters);
            mTxtLast30Days = FindViewById<TextView>(Resource.Id.txtLast30Days);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llSaveButtonContainer);
            mBtnApplyFilter = FindViewById<Button>(Resource.Id.btnSave);

            mCbCustomerSales = FindViewById<CheckBox>(Resource.Id.cbCustomerSales);
            mCbPayLaterUnpaid = FindViewById<CheckBox>(Resource.Id.cbPayLaterUnpaid);
            mCbPayLaterPaid = FindViewById<CheckBox>(Resource.Id.cbPayLaterPaid);
            mCbCash = FindViewById<CheckBox>(Resource.Id.cbCash);
            mCbCheck = FindViewById<CheckBox>(Resource.Id.cbCheck);
            mCbShowCancelSalesOnly = FindViewById<CheckBox>(Resource.Id.cbShowCancelSalesOnly);

            mImgCancelStartDate.Visibility = ViewStates.Invisible;
            mImgCancelEndDate.Visibility = ViewStates.Invisible;
            FetchSelectedData();
        }

        private void FetchSelectedData()
        {
            mCbCustomerSales.Checked = mIsCustomerSalesFilterChecked;
            mCbPayLaterUnpaid.Checked = mIsPaylaterUnpaidFilterChecked;
            mCbPayLaterPaid.Checked = mIsPaylaterPaidFilterChecked;
            mCbCash.Checked = mIsCashFilterChecked;
            mCbCheck.Checked = mIsCheckFilterChecked;
            mCbShowCancelSalesOnly.Checked = mIsShowCancelledSalesChecked;

            switch (mSelectedFilterText)
            {
                case "Total last 30 days":
                    SetSelectedDateFilter(Resource.Id.txtLast30Days);
                    break;
                case "Total today":
                    SetSelectedDateFilter(Resource.Id.txtToday);
                    break;
                case "Total yesterday":
                    SetSelectedDateFilter(Resource.Id.txtYesterday);
                    break;
                case "Total this week":
                    SetSelectedDateFilter(Resource.Id.txtThisWeek);
                    break;
                case "Total last week":
                    SetSelectedDateFilter(Resource.Id.txtLastWeek);
                    break;
                case "Total this month":
                    SetSelectedDateFilter(Resource.Id.txtThisMonth);
                    break;
                case "Total last month":
                    SetSelectedDateFilter(Resource.Id.txtLastMonth);
                    break;
                case "Total this year":
                    SetSelectedDateFilter(Resource.Id.txtThisYear);
                    break;
                case "Total last year":
                    SetSelectedDateFilter(Resource.Id.txtLastYear);
                    break;
                default:
                    SetSelectedDateFilter(0);
                    mTxtStartDate.Text = mCurrentSelectedStartDate.ToString("d MMM yyyy").ToUpper();
                    mTxtEndDate.Text = mCurrentSelectedEndDate.ToString("d MMM yyyy").ToUpper();
                    mImgCancelStartDate.Visibility = ViewStates.Visible;
                    mImgCancelEndDate.Visibility = ViewStates.Visible;
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    CancelFilter();
                    Finish();
                    return true;
                case Resource.Id.menuItem_clear:
                    ClearFilter();
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        public override void OnBackPressed()
        {
            CancelFilter();
            base.OnBackPressed();
        }
        public void CancelFilter()
        {
            var result = new Intent();
            result.PutExtra("isFiltered", false); 
            result.PutExtra("isCancelled", true);
            result.PutExtra("isCleared", false);
            SetResult(Result.Ok, result);
        }

        private void ClearFilter()
        {
            mImgCancelStartDate.PerformClick();
            mImgCancelEndDate.PerformClick();
            SetSelectedDateFilter(Resource.Id.txtLast30Days);
            mSelectedFilterText = "Total last 30 days";
            mCbCustomerSales.Checked = false;
            mCbPayLaterUnpaid.Checked = false;
            mCbPayLaterPaid.Checked = false;
            mCbCash.Checked = false;
            mCbCheck.Checked = false;
            mCbShowCancelSalesOnly.Checked = false;
            var result = new Intent();
            result.PutExtra("isFiltered", false);
            result.PutExtra("isCancelled", false);
            result.PutExtra("isCleared", true);
            SetResult(Result.Ok, result);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_transactions_filter, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void RegisterDateFilterGridEvent()
        {
            int childCount = mGlFilters.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View cell = (View)mGlFilters.GetChildAt(i);
                cell.Click += Cell_Click;
            }
        }

        private void MTxtLast30Days_Click(object sender, EventArgs e)
        {
            mImgCancelStartDate.PerformClick();
            mImgCancelEndDate.PerformClick();
            mIsDateRangeUsed = false;
            SetSelectedDateFilter(Resource.Id.txtLast30Days);
            mSelectedFilterText = "Total last 30 days";
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            mImgCancelStartDate.PerformClick();
            mImgCancelEndDate.PerformClick();
            TextView selectedCell = (TextView)sender;
            switch (selectedCell.Id)
            {
                case Resource.Id.txtToday:
                    mSelectedFilterText = "Total today";
                    SetSelectedDateFilter(Resource.Id.txtToday);
                    break;
                case Resource.Id.txtYesterday:
                    mSelectedFilterText = "Total yesterday";
                    SetSelectedDateFilter(Resource.Id.txtYesterday);
                    break;
                case Resource.Id.txtThisWeek:
                    mSelectedFilterText = "Total this week";
                    SetSelectedDateFilter(Resource.Id.txtThisWeek);
                    break;
                case Resource.Id.txtLastWeek:
                    mSelectedFilterText = "Total last week";
                    SetSelectedDateFilter(Resource.Id.txtLastWeek);
                    break;
                case Resource.Id.txtThisMonth:
                    mSelectedFilterText = "Total this month";
                    SetSelectedDateFilter(Resource.Id.txtThisMonth);
                    break;
                case Resource.Id.txtLastMonth:
                    mSelectedFilterText = "Total last month";
                    SetSelectedDateFilter(Resource.Id.txtLastMonth);
                    break;
                case Resource.Id.txtThisYear:
                    mSelectedFilterText = "Total this year";
                    SetSelectedDateFilter(Resource.Id.txtThisYear);
                    break;
                case Resource.Id.txtLastYear:
                    mSelectedFilterText = "Total last year";
                    SetSelectedDateFilter(Resource.Id.txtLastYear);
                    break;
                default:
                    break;
            }
        }

        private void SetSelectedDateFilter(int selectedfilterId)
        {
            Color black = new Color(ColorHelper.ResourceIdToColor(Resource.Color.colorLightBlack, this));
            Color lightGreen = new Color(ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
            Typeface normalTypeFace = FindViewById<TextView>(Resource.Id.txtNormalTypefaceRef).Typeface;
            int childCount = mGlFilters.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                TextView cell = (TextView)mGlFilters.GetChildAt(i);
                cell.SetTextColor(black);
                cell.SetTypeface(normalTypeFace, TypefaceStyle.Normal);
                if (cell.Id == selectedfilterId)
                {
                    cell.SetTextColor(lightGreen);
                    cell.SetTypeface(normalTypeFace, TypefaceStyle.Bold);
                }
            }

            if (selectedfilterId == Resource.Id.txtLast30Days)
            {
                mTxtLast30Days.SetTextColor(lightGreen);
                mTxtLast30Days.SetTypeface(normalTypeFace, TypefaceStyle.Bold);
            }
            else
            {
                mTxtLast30Days.SetTextColor(black);
                mTxtLast30Days.SetTypeface(normalTypeFace, TypefaceStyle.Normal);
            }

        }
    }
}