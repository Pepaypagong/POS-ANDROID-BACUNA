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
    public class CustomersFragmentEditSalesFragment : SupportFragment
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
        CustomersDataAccess mCustomersDataAccess;
        private string mPesoSign = "\u20b1";
        float mDpVal;

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
            return thisFragmentView;
        }

        private void FnGetData()
        {

        }

        private void FnSetUpEvents()
        {
            mImgRemoveFilter.Click += MImgRemoveFilter_Click;
            mToolBar.MenuItemClick += MToolBar_MenuItemClick;
        }

        private void MImgRemoveFilter_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FnSetUpControls()
        {
            mToolBar = thisFragmentView.FindViewById<SupportToolbar>(Resource.Id.toolbarFilter);
            mRecyclerViewTransactionList = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.rvSalesList);
            mTxtDateFilterText = thisFragmentView.FindViewById<TextView>(Resource.Id.txtDateFilterText);
            mTxtSaleAmountAndCount = thisFragmentView.FindViewById<TextView>(Resource.Id.txtSaleAmountAndCount);
            mImgRemoveFilter = thisFragmentView.FindViewById<ImageView>(Resource.Id.imgRemoveFilter);
        }

        private void MToolBar_MenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs e)
        {

        }
    }
}