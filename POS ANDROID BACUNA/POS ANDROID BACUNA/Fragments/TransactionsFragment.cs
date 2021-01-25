using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Adapters;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Data_Classes.Temp;
using POS_ANDROID_BACUNA.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using SearchView = Android.Support.V7.Widget.SearchView;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportSearchBar = Android.Support.V7.Widget.SearchView;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class TransactionsFragment : SupportFragment
    {
        View mThisFragmentView;
        SearchView searchViewSearchTransactions; 
        RecyclerView mRecyclerViewTransactionList;
        RelativeLayout mRlDateFilter;
        TextView mTxtDateFilterText;
        TextView mTxtSaleAmountAndCount;
        ImageView mImgRemoveFilter;
        RecyclerView.LayoutManager mLayoutManager;
        TransactionsDataAccess mTransactionsDataAccess;
        private string mPesoSign = "\u20b1";
        float mDpVal;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            mDpVal = this.Resources.DisplayMetrics.Density;
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FnFetchData();
            FnSetUpMainView(inflater, container);
            FnSetUpControls();
            FnSetUpEvents();
            FnSetTransactionList();
            SetDateFilterTotal(DateTime.Now.AddDays(-30), DateTime.Now, "Total from last 30 days"); //get last 30 days
            return mThisFragmentView;
        }

        public void RedrawView()
        {
            mThisFragmentView.RequestLayout();
            mThisFragmentView.PostDelayed(() =>
            {
                SetContentsLayoutHeight(mRecyclerViewTransactionList, searchViewSearchTransactions);
            },100);
        }

        private void SetContentsLayoutHeight(View layout, SupportSearchBar layoutBelow)
        {
            int searchToolBarHeight = searchViewSearchTransactions.Height;
            int rlDateFilterHeight = mRlDateFilter.Height;
            int marginOffset = searchViewSearchTransactions.Height; //margin top 
            int fragmentContainerHeight = mThisFragmentView.Height;

            int calculatedHeight = fragmentContainerHeight - (searchToolBarHeight + rlDateFilterHeight + marginOffset);
            int _topMargin = dpToPixel(2);
            int _bottomMargin = dpToPixel(0);
            int _leftMargin = dpToPixel(0);
            int _rightMargin = dpToPixel(0);

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

        private void FnFetchData()
        {
            mTransactionsDataAccess = new TransactionsDataAccess();
            mTransactionsDataAccess.CreateTable();
        }

        private void FnSetUpMainView(LayoutInflater inflater, ViewGroup container)
        {
            mThisFragmentView = inflater.Inflate(Resource.Layout.transactions_fragment, container, false);
            mThisFragmentView.Clickable = true;
            mThisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(mThisFragmentView);
        }
        private void FnSetUpControls()
        {
            searchViewSearchTransactions = mThisFragmentView.FindViewById<SearchView>(Resource.Id.searchBar);
            mRecyclerViewTransactionList = mThisFragmentView.FindViewById<RecyclerView>(Resource.Id.recyclerViewTransactions);
            mRlDateFilter = mThisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlDateFilter);
            mTxtDateFilterText = mThisFragmentView.FindViewById<TextView>(Resource.Id.txtDateFilterText);
            mTxtSaleAmountAndCount = mThisFragmentView.FindViewById<TextView>(Resource.Id.txtSaleAmountAndCount);
            mImgRemoveFilter = mThisFragmentView.FindViewById<ImageView>(Resource.Id.imgRemoveFilter);

            searchViewSearchTransactions.OnActionViewExpanded(); //show edit mode of searchview
            searchViewSearchTransactions.ClearFocus(); //clear focus and hide keyboard
            searchViewSearchTransactions.QueryHint = "Search customer, transaction #";
            searchViewSearchTransactions.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(Context, "TransactionsFragment"));
        }
        private void FnSetUpEvents()
        {
            searchViewSearchTransactions.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchViewSearchTransactions); };
            searchViewSearchTransactions.QueryTextChange += SearchBar_QueryTextChange;
        }
        private void SearchBar_Click(object sender, EventArgs e, SupportSearchBar s)
        {
            s.OnActionViewExpanded();
        }
        private void SearchBar_QueryTextChange(object sender, SupportSearchBar.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            //FnPopulateCustomers(queryString);
            //mListView.Invalidate();
        }
        private void FnSetTransactionList()
        {
            mLayoutManager = new LinearLayoutManager((MainActivity)this.Activity);
            mRecyclerViewTransactionList.SetLayoutManager(mLayoutManager);
            mRecyclerViewTransactionList.HasFixedSize = true;
            mRecyclerViewTransactionList.SetAdapter(
                new TransactionsRecyclerViewAdapter(
                    this.Activity,
                    (MainActivity)this.Activity,
                    GetGroupedList())
                );
        }

        private List<TransactionListGroupedByDate> GetGroupedList()
        {
            var Transactions = mTransactionsDataAccess.SelectTable();
            List<TransactionListGroupedByDate> returnList = new List<TransactionListGroupedByDate>();
            var GroupedTransactionDates = Transactions.OrderByDescending(d => StringDateToDateTime(d.TransactionDateTime))
                                      .GroupBy(d => StringDateToDateTime(d.TransactionDateTime))
                                      .Select(d => new { TransactionDateTime = d.Key, Sum = d.Sum(item => item.SubTotalAmount) })
                                      .ToList();
            foreach (var item in GroupedTransactionDates)
            {
                //insert header
                returnList.Add(new TransactionListGroupedByDate()
                {
                    id = 0,
                    TransactionDateTime = StringDateToDateTime(item.TransactionDateTime.ToString()),
                    SubTotalAmount = GetTotalAmount(StringDateToDateTime(item.TransactionDateTime.ToString()), Transactions),
                    TransactionCount = GetTransactionCount(StringDateToDateTime(item.TransactionDateTime.ToString()), Transactions),
                    isHeader = true
                });
                //insert content items
                foreach (var item2 in Transactions
                    .Where(x=> StringDateToDateTime(x.TransactionDateTime) == StringDateToDateTime(item.TransactionDateTime.ToString()))
                    .ToList())
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
                    });
                }
            }

            return returnList;
        }

        private DateTime StringDateToDateTime(string _stringDate)
        {
            var x = Convert.ToDateTime(_stringDate).ToString("yyyy MM dd"); 
            return Convert.ToDateTime(x);
        }

        private int GetTransactionCount(DateTime dateTime, List<TransactionsModel> transactions)
        {
            return transactions.Where(x => StringDateToDateTime(x.TransactionDateTime) == dateTime).Count();
        }

        private decimal GetTotalAmount(DateTime dateTime, List<TransactionsModel> transactions)
        {
            return transactions.Where(x => StringDateToDateTime(x.TransactionDateTime) == dateTime).Sum(x=>x.SubTotalAmount);
        }

        public void SetDateFilterTotal(DateTime _startDate, DateTime _endDate, string _filterLabel)
        {          
            var Transactions = mTransactionsDataAccess.SelectTable();
            var query = Transactions.Where(l => StringDateToDateTime(l.TransactionDateTime) >= _startDate &&
                                                StringDateToDateTime(l.TransactionDateTime) <= _endDate);
            int transactionCount = query.Count();
            decimal totalSaleAmount = query.Sum(x => x.SubTotalAmount);

            mTxtDateFilterText.Text = _filterLabel;
            mTxtSaleAmountAndCount.Text = mPesoSign + string.Format("{0:n}",totalSaleAmount) + " from " + transactionCount.ToString() + " sales.";
        }
    }
}