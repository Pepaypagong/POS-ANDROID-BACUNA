using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using Product = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Fragments;
using Android.Graphics.Drawables;

namespace POS_ANDROID_BACUNA.Adapters
{
    class RunnersMultipayRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<TransactionsModel> mTransactions;
        Context mContext;
        string mPesoSign = "\u20b1";
        Activity mActivity;

        public RunnersMultipayRecyclerViewAdapter(Activity _activity, Context _context, List<TransactionsModel> _model)
        {
            mActivity = _activity;
            mContext = _context;
            mTransactions = _model;
        }

        public void RefreshList(List<TransactionsModel> _source)
        {
            mTransactions = _source;
            this.NotifyDataSetChanged();
        }

        public class ContentViewHolder : RecyclerView.ViewHolder
        {
            public ContentViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mTransactionId { get; set; }
            public TextView mTransactionDateTime { get; set; }
            public TextView mSaleAmount { get; set; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.runners_fragment_paybydate_row, parent, false);
            TextView txtTransactionNumber = itemView.FindViewById<TextView>(Resource.Id.txtTransactionNumber);
            TextView txtDateAndTime = itemView.FindViewById<TextView>(Resource.Id.txtDateAndTime);
            TextView txtSaleAmount = itemView.FindViewById<TextView>(Resource.Id.txtSaleAmount);

            ContentViewHolder view = new ContentViewHolder(itemView)
            {
                mTransactionId = txtTransactionNumber,
                mTransactionDateTime = txtDateAndTime,
                mSaleAmount = txtSaleAmount
            };
            return view;
            
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            ContentViewHolder myHolder = viewHolder as ContentViewHolder;

            myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
            myHolder.mMainView.Click += MMainView_Click; //set click event for row

            myHolder.mTransactionId.Text = "# " + mTransactions[position].id.ToString();
            myHolder.mTransactionDateTime.Text = GetDateTime(Convert.ToDateTime(mTransactions[position].TransactionDateTime)); 
            myHolder.mSaleAmount.Text = mPesoSign + string.Format("{0:n}", mTransactions[position].SubTotalAmount);
        }

        private string GetDateTime(DateTime _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("hh:mm tt  d MMM yyyy (ddd)").ToUpper();
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            string id = ((ViewGroup)sender).FindViewById<TextView>(Resource.Id.txtTransactionNumber).Text.Remove(0, 2);
            Intent intent = new Intent(mContext, typeof(TransactionsFragmentTransactionInfoActivity));
            intent.PutExtra("selectedTransactionId", Convert.ToInt32(id));
            mActivity.StartActivityForResult(intent, 43);
        }

        public override int ItemCount
        {
            get
            {
                return mTransactions != null ? mTransactions.Count : 0;
            }
        }

    }
}