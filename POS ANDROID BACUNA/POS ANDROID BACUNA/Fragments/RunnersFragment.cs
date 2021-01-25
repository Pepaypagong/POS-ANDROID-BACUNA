using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SearchView = Android.Support.V7.Widget.SearchView;
using POS_ANDROID_BACUNA.Adapters;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class RunnersFragment : SupportFragment
    {
        View mThisFragmentView;
        public SearchView searchViewSearchRunners;
        ListView mLvRunners;
        TextView mTxtTotalAmount;
        RunnersDataAccess mRunnersDataAccess;
        TransactionsDataAccess mTransactionsDataAccess;
        List<RunnersModel> mRunnersList;
        RunnersListViewAdapter mAdapter;
        bool mDialogShown;
        string mPesoSign = "\u20b1";
        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FnSetUpMainView(inflater, container);
            FnSetUpData();
            FnSetUpControls();
            FnSetUpEvents();
            FnSetRunnersList();
            searchViewSearchRunners.OnActionViewExpanded(); //show edit mode of searchview
            searchViewSearchRunners.ClearFocus(); //clear focus and hide keyboard
            ShowTotalUnpaid();
            return mThisFragmentView;
        }

        public void ShowTotalUnpaid()
        {
            decimal totalBalance = mTransactionsDataAccess.SelectTable().Where(x => x.IsPaid == false).Sum(x => x.SubTotalAmount);
            mTxtTotalAmount.Text = mPesoSign + " " + string.Format("{0:n}", totalBalance);
        }

        private void FnSetUpData()
        {
            mRunnersDataAccess = new RunnersDataAccess();
            mRunnersDataAccess.CreateTable();
            mRunnersList = mRunnersDataAccess.SelectTable();
            mTransactionsDataAccess = new TransactionsDataAccess();
        }

        private void FnSetUpMainView(LayoutInflater inflater, ViewGroup container)
        {
            mThisFragmentView = inflater.Inflate(Resource.Layout.runners_fragment, container, false);
            mThisFragmentView.Clickable = true;
            mThisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(mThisFragmentView);
        }
        private void FnSetUpControls()
        {
            searchViewSearchRunners = mThisFragmentView.FindViewById<SearchView>(Resource.Id.searchBar);
            mLvRunners = mThisFragmentView.FindViewById<ListView>(Resource.Id.lvRunners);
            mTxtTotalAmount = mThisFragmentView.FindViewById<TextView>(Resource.Id.txtTotalAmount);
            searchViewSearchRunners.QueryHint = "Search Runners";
        }
        private void FnSetUpEvents()
        {
            mLvRunners.ItemClick += MLvRunners_ItemClick;
            searchViewSearchRunners.Click += delegate (object sender, EventArgs e) { SearchBar_Click(sender, e, searchViewSearchRunners); };
            searchViewSearchRunners.QueryTextChange += SearchBar_QueryTextChange;
        }
        private void SearchBar_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            FnGetListData(queryString);
            mAdapter.RefreshList(mRunnersList);
            mLvRunners.Invalidate();
        }
        private void SearchBar_Click(object sender, EventArgs e, SearchView s)
        {
            s.OnActionViewExpanded();
        }
        private void MLvRunners_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!mDialogShown) //avoid double click
            {
                mDialogShown = true;
                int RunnerId = mRunnersList[e.Position].Id;
                Intent intent = new Intent(Context, typeof(RunnersFragmentEditRunnerActivity));
                intent.PutExtra("RunnerId", RunnerId);
                StartActivityForResult(intent, 39);
            }
        }

        public void FnGetListData(string _queryString)
        {
            mRunnersList = mRunnersDataAccess.SelectTable();
            var mItems = mRunnersList;  //GlobalVariables.globalRunnersList;
            if (_queryString != "")
            {
                mItems = mItems
                    .Where(x => x.FullName.ToLower().Contains(_queryString))
                    .ToList();
                mRunnersList = mItems;
            }
        }

        public void FnSetRunnersList()
        {
            FnGetListData("");
            mAdapter = new RunnersListViewAdapter((MainActivity)this.Activity, mRunnersList, mTransactionsDataAccess.SelectTable());
            mLvRunners.Adapter = mAdapter;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 39)
            {
                mDialogShown = false;
                FnGetListData(searchViewSearchRunners.Query);
                mAdapter.RefreshList(mRunnersList);
                mLvRunners.Invalidate();
                searchViewSearchRunners.SetQuery("", false);
            }
        }
    }
}