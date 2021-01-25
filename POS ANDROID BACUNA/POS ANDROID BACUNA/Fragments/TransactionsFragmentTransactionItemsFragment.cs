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

namespace POS_ANDROID_BACUNA.Fragments
{
    public class TransactionsFragmentTransactionItemsFragment : SupportFragment
    {
        View thisFragmentView;
        RecyclerView mRvList;

        TransactionItemsDataAccess mTransactionItemsDataAccess;
        int mSelectedTransactionId;
        List<TransactionItemsModel> mTransactionItemsList;
        RecyclerView.LayoutManager mLayoutManager;
        bool mDialogShown = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.transactions_fragment_items_fragment, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            FnGetData();
            FnSetUpControls();
            FnSetUpList();
            return thisFragmentView;
        }

        private void FnSetUpList()
        {
            mLayoutManager = new LinearLayoutManager((TransactionsFragmentTransactionInfoActivity)this.Activity);
            mRvList.SetLayoutManager(mLayoutManager);
            mRvList.HasFixedSize = true;
            mRvList.SetAdapter(
                new TransactionItemsRecyclerViewAdapter(
                    this.Activity,
                    (TransactionsFragmentTransactionInfoActivity)this.Activity,
                    mTransactionItemsList,false)
                );
        }

        private void FnGetData()
        {
            mTransactionItemsDataAccess = new TransactionItemsDataAccess();
            mSelectedTransactionId = ((TransactionsFragmentTransactionInfoActivity)this.Activity).DataToFragments();
            mTransactionItemsList = mTransactionItemsDataAccess.SelectRecord(mSelectedTransactionId);
        }

        private void FnSetUpControls()
        {
            mRvList = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.rvList);
            DividerItemDecoration x = new DividerItemDecoration((TransactionsFragmentTransactionInfoActivity)this.Activity, 
                DividerItemDecoration.Vertical);
            mRvList.AddItemDecoration(x);
        }
    }
}