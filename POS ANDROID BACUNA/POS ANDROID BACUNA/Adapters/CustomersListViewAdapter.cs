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

namespace POS_ANDROID_BACUNA
{
    class CustomersListViewAdapter : BaseAdapter<CustomersModel>
    {
        private List<CustomersModel> mItems;
        private Context mContext;
        private TextView mTxtCustomerName;
        private TextView mTxtAvgPurchaseAmt;
        private TextView mTxtSalesCount;

        public CustomersListViewAdapter(Context context, List<CustomersModel> items)
        {
            mItems = items;
            mContext = context;
        }
        public override int Count
        {
            get { return mItems == null ? 0 : mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override CustomersModel this[int position]
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
            mTxtAvgPurchaseAmt.Text = "Hasn't made a purchase";
            mTxtSalesCount.Text = "3 Sales";

            return row;
        }

        private void FnSetControls(View _view)
        {
            mTxtCustomerName = _view.FindViewById<TextView>(Resource.Id.txtCustomerName);
            mTxtAvgPurchaseAmt = _view.FindViewById<TextView>(Resource.Id.txtAvgPurchaseAmt);
            mTxtSalesCount = _view.FindViewById<TextView>(Resource.Id.txtSalesCount);
        }

        public void RefreshList(List<CustomersModel> updatedList)
        {
            mItems = updatedList;
            NotifyDataSetChanged();
        }
    }
}