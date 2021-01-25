using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Javax.Security.Auth;
using POS_ANDROID_BACUNA.Data_Classes;

namespace POS_ANDROID_BACUNA
{
    class RunnersListViewAdapter : BaseAdapter<RunnersModel>
    {
        private List<RunnersModel> mItems;
        private List<TransactionsModel> mTransactions;
        private Context mContext;
        private TextView mTxtCustomerName;
        private TextView mTxtAvgPurchaseAmt;
        private TextView mTxtSalesCount;
        string mPesoSign = "\u20b1";

        public RunnersListViewAdapter(Context context, List<RunnersModel> items, List<TransactionsModel> transactions)
        {
            mItems = items;
            mTransactions = transactions;
            mContext = context;
        }
        public override int Count
        {
            get { return mItems != null ? mItems.Count : 0; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override RunnersModel this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.customers_listview_row, null, false);
            }

            FnSetControls(row);

            mTxtCustomerName.Text = mItems[position].FullName;
            mTxtAvgPurchaseAmt.Text = GetCurrentBalance(mItems[position].Id) == 0 ? "No Balance" :
                "Current Balance: " + mPesoSign + string.Format("{0:n}", GetCurrentBalance(mItems[position].Id));
            mTxtSalesCount.Text = mTransactions.Where(x => x.CustomerOrRunnerId == mItems[position].Id).Count().ToString() + " sales";

            return row;
        }
        private decimal GetCurrentBalance(int runnerId)
        {
            return mTransactions.Where(x => x.CustomerOrRunnerId == runnerId && !x.IsPaid).Sum(x => x.SubTotalAmount);
        }

        //private Decimal GetAveragePurchaseAmount(int runnerId)
        //{
        //    decimal retVal;
        //    decimal totalPurchaseAmount = mTransactions.Where(x => x.CustomerOrRunnerId == runnerId).Sum(x => x.SubTotalAmount);
        //    int purchaseCount = mTransactions.Where(x => x.CustomerOrRunnerId == runnerId).Count();

        //    try
        //    {
        //        retVal = totalPurchaseAmount / purchaseCount;
        //    }
        //    catch (Exception)
        //    {
        //        retVal = 0;
        //    }
        //    return retVal;
        //}

        private void FnSetControls(View _view)
        {
            mTxtCustomerName = _view.FindViewById<TextView>(Resource.Id.txtCustomerName);
            mTxtAvgPurchaseAmt = _view.FindViewById<TextView>(Resource.Id.txtAvgPurchaseAmt);
            mTxtSalesCount = _view.FindViewById<TextView>(Resource.Id.txtSalesCount);
        }

        public void RefreshList(List<RunnersModel> updatedList)
        {
            mItems = updatedList;
            NotifyDataSetChanged();
        }
    }
}