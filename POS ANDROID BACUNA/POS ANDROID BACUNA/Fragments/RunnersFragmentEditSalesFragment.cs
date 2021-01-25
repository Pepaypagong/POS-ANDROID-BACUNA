using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Support.V7.Widget;
using Product = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;
using Android.Support.Design.Widget;
using Android.Text;
using POS_ANDROID_BACUNA.SQLite;
using POS_ANDROID_BACUNA.Data_Classes.Temp;
using Android.Graphics.Drawables;
using System.Runtime.CompilerServices;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class RunnersFragmentEditSalesFragment : SupportFragment
    {
        View thisFragmentView;
        SupportToolbar mToolBar;
        bool mDialogShown = false;
        RecyclerView mRecyclerViewTransactionList;
        RelativeLayout mRlDateFilter;
        TextView mTxtDateFilterText;
        TextView mTxtSaleAmountAndCount;
        ImageView mImgRemoveFilter;
        RecyclerView.LayoutManager mLayoutManager;
        TransactionsDataAccess mTransactionsDataAccess;
        TransactionsRecyclerViewAdapter mAdapter;
        private string mPesoSign = "\u20b1";
        float mDpVal;
        List<RunnersModel> mSelectedRunnerRow;
        bool mIsFiltered = false;
        string mFilterText = "Total last 30 days";
        string mFilterDateRange = "Total last 30 days";
        DateTime mFilterStartDate = DateTime.Now;
        DateTime mFilterEndDate = DateTime.Now;
        bool mIsCustomerSalesFilterChecked = false;
        bool mIsPaylaterUnpaidFilterChecked = false;
        bool mIsPaylaterPaidFilterChecked = false;
        bool mIsCashFilterChecked = false;
        bool mIsCheckFilterChecked = false;
        bool mIsShowCancelledSalesChecked = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.customers_fragment_customer_sales, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            FnGetData();
            FnSetUpControls();
            FnSetUpEvents();
            FnSetTransactionList();
            SetUnpaidAmount();
            thisFragmentView.Post(() =>
            {
                SetRecyclerViewLayoutHeight(mRecyclerViewTransactionList, mToolBar);
            });
            return thisFragmentView;
        }

        private void SetRecyclerViewLayoutHeight(RecyclerView layout, View layoutBelow)
        {
            int checkoutButtonHeight = mRlDateFilter.Height;
            int recyclerViewHeight = layout.Height;

            int _topMargin = dpToPixel(0);
            int _bottomMargin = dpToPixel(0);
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

        private void FnSetTransactionList()
        {
            mLayoutManager = new LinearLayoutManager((RunnersFragmentEditRunnerActivity)this.Activity);
            mRecyclerViewTransactionList.SetLayoutManager(mLayoutManager);
            mRecyclerViewTransactionList.HasFixedSize = true;
            mAdapter = new TransactionsRecyclerViewAdapter(this.Activity,(RunnersFragmentEditRunnerActivity)this.Activity,
                                                            GetGroupedList(mSelectedRunnerRow[0].Id, mFilterStartDate,mFilterEndDate,mFilterDateRange));
            mRecyclerViewTransactionList.SetAdapter(mAdapter);
        }
        
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            mToolBar.Menu.Clear();
            mToolBar.InflateMenu(Resource.Menu.toolbar_menu_runners_sales);

            IMenuItem item = mToolBar.Menu.FindItem(Resource.Id.menuItemFilter);
            subscribeCustomLayoutToClick(item);
            SetFilterIconTint(mIsFiltered);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public void subscribeCustomLayoutToClick(IMenuItem item)
        {
            item.ActionView.Click += (sender, args) => {
                this.OnOptionsItemSelected(item);
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuItemFilter:
                    Intent intent = new Intent(this.Context, typeof(TransactionsFilterActivity));
                    intent.PutExtra("previousSelectedStartDate", mFilterStartDate.Ticks);
                    intent.PutExtra("previousSelectedEndDate", mFilterEndDate.Ticks);
                    intent.PutExtra("selectedFilterDateRange", mFilterDateRange);
                    intent.PutExtra("isCustomerSalesFilterChecked", mIsCustomerSalesFilterChecked);
                    intent.PutExtra("isPaylaterPaidFilterChecked", mIsPaylaterPaidFilterChecked);
                    intent.PutExtra("isPaylaterUnpaidFilterChecked", mIsPaylaterUnpaidFilterChecked);
                    intent.PutExtra("isCashFilterChecked", mIsCashFilterChecked);
                    intent.PutExtra("isCheckFilterChecked", mIsCheckFilterChecked);
                    intent.PutExtra("isShowCancelledSalesChecked", mIsShowCancelledSalesChecked);
                    StartActivityForResult(intent, 42);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void SetFilterIconTint(bool _hasFilter)
        {
            ImageView imgIcon = mToolBar.FindViewById<ImageView>(Resource.Id.imgIcon);
            imgIcon.SetColorFilter(_hasFilter ?
                    ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this.Context) :
                    ColorHelper.ResourceIdToColor(Resource.Color.colorPrimaryDark, this.Context));
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 42)
            {
                bool isCleared = data.GetBooleanExtra("isCleared", false);
                bool isFiltered = data.GetBooleanExtra("isFiltered", false);
                mIsFiltered = isFiltered;
                if (isCleared || isFiltered)
                {
                    mFilterText = isCleared ? "Total last 30 days" : data.GetStringExtra("filterText");
                    mFilterStartDate = isCleared ? DateTime.Now : new DateTime(data.GetLongExtra("startDate", DateTime.Now.Ticks));
                    mFilterEndDate = isCleared ? DateTime.Now : new DateTime(data.GetLongExtra("endDate", DateTime.Now.Ticks));
                    mFilterDateRange = isCleared ? "Total last 30 days" : data.GetStringExtra("selectedFilterDateRange");
                    mIsCustomerSalesFilterChecked = isCleared ? false : data.GetBooleanExtra("isCustomerSalesFilterChecked", false);
                    mIsPaylaterUnpaidFilterChecked = isCleared ? false : data.GetBooleanExtra("isPaylaterUnpaidFilterChecked", false);
                    mIsPaylaterPaidFilterChecked = isCleared ? false : data.GetBooleanExtra("isPaylaterPaidFilterChecked", false);
                    mIsCashFilterChecked = isCleared ? false : data.GetBooleanExtra("isCashFilterChecked", false);
                    mIsCheckFilterChecked = isCleared ? false : data.GetBooleanExtra("isCheckFilterChecked", false);
                    mIsShowCancelledSalesChecked = isCleared ? false : data.GetBooleanExtra("isShowCancelledSalesChecked", false);
                    SetFilterIconTint(isFiltered);
                    mTxtDateFilterText.Text = isCleared ? "Total last 30 days" : mFilterText;
                    mImgRemoveFilter.Visibility = isCleared ? ViewStates.Invisible : ViewStates.Visible;

                    //refresh list here
                    mAdapter.RefreshData(GetGroupedList(mSelectedRunnerRow[0].Id,mFilterStartDate,mFilterEndDate,mFilterDateRange));
                }
                else if (data.GetBooleanExtra("isCancelled", false))
                {
                    //retain previous filter
                }
            }
        }
         
        private void MToolBar_MenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.menuItemMultipay)
            {
                if (HasUnpaidSales())
                {
                    Intent intent = new Intent(this.Context, typeof(RunnersFragmentMultiplePaymentRecords));
                    intent.PutExtra("selectedRunnerId", mSelectedRunnerRow[0].Id);
                    StartActivityForResult(intent, 41);
                }
            }
        }

        private bool HasUnpaidSales()
        {
            bool retval = false;

            if (mTransactionsDataAccess.SelectTable().Exists(x => x.CustomerOrRunnerId == mSelectedRunnerRow[0].Id && !x.IsPaid))
            {
                retval = true;
            }
            else
            {
                retval = false;
                DialogMessageService.MessageBox(this.Context, "", "No unpaid transactions");
            }
            return retval;
        }
         
        private void FnGetData()
        {
            mTransactionsDataAccess = new TransactionsDataAccess();
            mTransactionsDataAccess.CreateTable();
            mSelectedRunnerRow = ((RunnersFragmentEditRunnerActivity)this.Activity).DataToFragments();
        }

        private void FnSetUpEvents()
        {
            mImgRemoveFilter.Click += MImgRemoveFilter_Click;
            mToolBar.MenuItemClick += MToolBar_MenuItemClick;
        }

        private void MImgRemoveFilter_Click(object sender, EventArgs e)
        {
            mFilterText = "";
            mIsFiltered = false;
            mFilterDateRange = "Total last 30 days";
            mIsCustomerSalesFilterChecked = false;
            mIsPaylaterUnpaidFilterChecked = false;
            mIsPaylaterPaidFilterChecked = false;
            mIsCashFilterChecked = false;
            mIsCheckFilterChecked = false;
            mIsShowCancelledSalesChecked = false;
            SetFilterIconTint(mIsFiltered);
            mTxtDateFilterText.Text = "Total last 30 days";
            mImgRemoveFilter.Visibility = ViewStates.Invisible;

            mAdapter.RefreshData(GetGroupedList(mSelectedRunnerRow[0].Id, mFilterStartDate, mFilterEndDate, mFilterDateRange));
        }

        private void FnSetUpControls()
        {
            mToolBar = thisFragmentView.FindViewById<SupportToolbar>(Resource.Id.toolbarFilter);
            mRecyclerViewTransactionList = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.rvSalesList);
            mRlDateFilter = thisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlDateFilter);
            mTxtDateFilterText = thisFragmentView.FindViewById<TextView>(Resource.Id.txtDateFilterText);
            mTxtSaleAmountAndCount = thisFragmentView.FindViewById<TextView>(Resource.Id.txtSaleAmountAndCount);
            mImgRemoveFilter = thisFragmentView.FindViewById<ImageView>(Resource.Id.imgRemoveFilter);

            mToolBar.Title = "P 1,000.00 Unpaid";
            mTxtDateFilterText.Text = mFilterText;
        }

        public void SetUnpaidAmount()
        {
            decimal unpaidAmount = mTransactionsDataAccess.SelectTable()
                                    .Where(x => x.CustomerOrRunnerId == mSelectedRunnerRow[0].Id && !x.IsPaid)
                                    .Sum(x => x.SubTotalAmount);
            mToolBar.Title = mPesoSign + " " + string.Format("{0:n}",unpaidAmount) + " unpaid";
        }
        public void SetTotalSalesAmount(DateTime filterStartDate, DateTime filterEndDate)
        {
            decimal amount = mTransactionsDataAccess.SelectTable()
                                    .Where(x => x.CustomerOrRunnerId == mSelectedRunnerRow[0].Id && 
                                    StringDateToDate(x.TransactionDateTime) >= filterStartDate && 
                                    StringDateToDate(x.TransactionDateTime) <= filterEndDate)
                                    .Sum(x => x.SubTotalAmount);
            int salesCount = mTransactionsDataAccess.SelectTable()
                                    .Where(x => x.CustomerOrRunnerId == mSelectedRunnerRow[0].Id &&
                                    StringDateToDate(x.TransactionDateTime) >= filterStartDate &&
                                    StringDateToDate(x.TransactionDateTime) <= filterEndDate)
                                    .Count();
            mTxtSaleAmountAndCount.Text = mPesoSign + " " + string.Format("{0:n}", amount) + " from " + salesCount + " sale(s)";
        }
        private List<TransactionListGroupedByDate> GetGroupedList(int selectedRunnerId, DateTime filterStartDate, DateTime filterEndDate, string filterString)
        {
            var Transactions = mTransactionsDataAccess.SelectTable().Where(x => x.CustomerOrRunnerId == selectedRunnerId)
                   .ToList();

            switch (filterString)
            {
                case "Total last 30 days":
                    filterStartDate = DateTime.Now.AddDays(-30);
                    filterEndDate = DateTime.Now;
                    break;
                case "Total today":
                    filterStartDate = DateTime.Now;
                    filterEndDate = DateTime.Now;
                    break;
                case "Total yesterday":
                    filterStartDate = DateTime.Now.AddDays(-1);
                    filterEndDate = DateTime.Now.AddDays(-1);
                    break;
                case "Total this week":
                    filterStartDate = filterStartDate.AddDays(-(int)filterStartDate.DayOfWeek);
                    filterEndDate = filterStartDate.AddDays(6);
                    break;
                case "Total last week":
                    filterStartDate = filterStartDate.AddDays(-(int)filterStartDate.DayOfWeek + (-7));
                    filterEndDate = filterStartDate.AddDays(6);
                    break;
                case "Total this month":
                    filterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    filterEndDate = filterStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "Total last month":
                    filterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1);
                    filterEndDate = filterStartDate.AddMonths(1).AddDays(-1);
                    break;
                case "Total this year":
                    filterStartDate = new DateTime(DateTime.Now.Year,1 ,1);
                    filterEndDate = filterStartDate.AddMonths(12).AddDays(-1);
                    break;
                case "Total last year":
                    filterStartDate = new DateTime(DateTime.Now.Year-1, 1, 1);
                    filterEndDate = filterStartDate.AddMonths(12).AddDays(-1);
                    break;
                default:
                    break;
            }

            filterStartDate = new DateTime(filterStartDate.Year, filterStartDate.Month, filterStartDate.Day, 0, 0, 0);
            filterEndDate = new DateTime(filterEndDate.Year, filterEndDate.Month, filterEndDate.Day, 0, 0, 0);

            SetTotalSalesAmount(filterStartDate, filterEndDate);

            Transactions = Transactions.Where(x => StringDateToDate(x.TransactionDateTime) >= filterStartDate 
                && StringDateToDate(x.TransactionDateTime) <= filterEndDate).ToList();

            List<TransactionListGroupedByDate> returnList = new List<TransactionListGroupedByDate>();
            var GroupedTransactionDates = Transactions.OrderByDescending(d => StringDateToDate(d.TransactionDateTime))
                                      .GroupBy(d => StringDateToDate(d.TransactionDateTime))
                                      .Select(d => new { TransactionDateTime = d.Key, Sum = d.Sum(item => item.SubTotalAmount) })
                                      .ToList();
            foreach (var item in GroupedTransactionDates)
            {
                //insert header
                returnList.Add(new TransactionListGroupedByDate()
                {
                    id = 0,
                    TransactionDateTime = StringDateToDate(item.TransactionDateTime.ToString()),
                    SubTotalAmount = GetTotalAmount(StringDateToDate(item.TransactionDateTime.ToString()), Transactions),
                    TransactionCount = GetTransactionCount(StringDateToDate(item.TransactionDateTime.ToString()), Transactions),
                    isHeader = true
                });
                //insert content items

                var Transactions2 = Transactions
                    .Where(x => StringDateToDate(x.TransactionDateTime) == StringDateToDate(item.TransactionDateTime.ToString()))
                    .OrderByDescending(x => StringDateToDateTime(x.TransactionDateTime))
                    .ThenByDescending(x => x.id)
                    .ToList();
                foreach (var item2 in Transactions2)
                {
                    returnList.Add(new TransactionListGroupedByDate()
                    {
                        id = item2.id,
                        DateCreated = Convert.ToDateTime(item2.DateCreated),
                        DateModified = Convert.ToDateTime(item2.DateModified),
                        ticketNumber = item2.id,
                        TransactionDateTime = Convert.ToDateTime(item2.TransactionDateTime),
                        CustomerOrRunnerId = item2.CustomerOrRunnerId,
                        SubTotalAmount = item2.SubTotalAmount,
                        DiscountAmount = item2.DiscountAmount,
                        TransactionType = item2.TransactionType,
                        isHeader = false,
                        isPaid = item2.IsPaid
                    });
                }
            }

            return returnList;
        }

        private DateTime StringDateToDate(string _stringDate)
        {
            var x = Convert.ToDateTime(_stringDate).ToString("yyyy MM dd");
            return Convert.ToDateTime(x);
        }

        private DateTime StringDateToDateTime(string _stringDate)
        {
            var x = Convert.ToDateTime(_stringDate).ToString("yyyy MM dd HH:mm");
            return Convert.ToDateTime(x);
        }

        private int GetTransactionCount(DateTime dateTime, List<TransactionsModel> transactions)
        {
            return transactions.Where(x => StringDateToDate(x.TransactionDateTime) == dateTime).Count();
        }

        private decimal GetTotalAmount(DateTime dateTime, List<TransactionsModel> transactions)
        {
            return transactions.Where(x => StringDateToDate(x.TransactionDateTime) == dateTime).Sum(x => x.SubTotalAmount);
        }

    }
}