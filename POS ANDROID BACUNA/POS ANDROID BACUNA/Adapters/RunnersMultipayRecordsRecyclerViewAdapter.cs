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
using Android.Text;
using Android.Text.Style;
using Android.Graphics;

namespace POS_ANDROID_BACUNA.Adapters
{
    class RunnersMultipayRecordsRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<RunnersMultipayRecordsModel> mTransactions;
        Context mContext;
        string mPesoSign = "\u20b1";
        Activity mActivity;

        public RunnersMultipayRecordsRecyclerViewAdapter(Activity _activity, Context _context, List<RunnersMultipayRecordsModel> _model)
        {
            mActivity = _activity;
            mContext = _context;
            mTransactions = _model;
        }

        public void RefreshList(List<RunnersMultipayRecordsModel> _source)
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
            public TextView mId { get; set; }
            public TextView mTransactionNumbers { get; set; }
            public TextView mDatePaid { get; set; }
            public TextView mDateRange { get; set; }
            public TextView mReceiptAmount { get; set; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.runners_fragment_multipay_records_row, parent, false);
            TextView txtId = itemView.FindViewById<TextView>(Resource.Id.txtId);
            TextView txtTransactionNumbers = itemView.FindViewById<TextView>(Resource.Id.txtTransactions);
            TextView txtDatePaid = itemView.FindViewById<TextView>(Resource.Id.txtDatePaid);
            TextView txtDateRange = itemView.FindViewById<TextView>(Resource.Id.txtDateRange);
            TextView txtReceiptAmount = itemView.FindViewById<TextView>(Resource.Id.txtReceiptAmount);

            ContentViewHolder view = new ContentViewHolder(itemView)
            {
                mId = txtId,
                mTransactionNumbers = txtTransactionNumbers,
                mDatePaid = txtDatePaid,
                mDateRange = txtDateRange,
                mReceiptAmount = txtReceiptAmount
            };
            return view;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            ContentViewHolder myHolder = viewHolder as ContentViewHolder;

            myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
            myHolder.mMainView.Click += MMainView_Click; //set click event for row

            myHolder.mId.Text = mTransactions[position].Id.ToString();

            SetSpannableString(myHolder.mDatePaid, "Date paid: ", GetDateTime(Convert.ToDateTime(mTransactions[position].TransactionDateTime)));
            SetSpannableString(myHolder.mDateRange, "Date covered: ",
                GetDateRangeString(mTransactions[position].StartDate, mTransactions[position].EndDate).ToUpper());
            SetSpannableString(myHolder.mTransactionNumbers, "Transactions: ", mTransactions[position].TransactionIds);
            //myHolder.mDatePaid.Text = "Date paid: " + GetDateTime(Convert.ToDateTime(mTransactions[position].TransactionDateTime));
            //myHolder.mDateRange.Text = "Date range: " + GetDateRangeString(mTransactions[position].StartDate, mTransactions[position].EndDate);
            //myHolder.mTransactionNumbers.Text = "Transactions: " + mTransactions[position].TransactionIds;
            myHolder.mReceiptAmount.Text = mPesoSign + string.Format("{0:n}", mTransactions[position].SubTotalAmount);
        }

        private void SetSpannableString(TextView _textview, string _label, string _boldText)
        {
            SpannableStringBuilder str = new SpannableStringBuilder(_label + _boldText);
            str.SetSpan(new StyleSpan(TypefaceStyle.Bold), _label.Length, _label.Length + _boldText.Length, SpanTypes.ExclusiveExclusive);
            _textview.TextFormatted = str;
        }

        private string GetDateRangeString(string _startDate, string _endDate)
        {
            DateTime startDate = Convert.ToDateTime(_startDate);
            DateTime endDate = Convert.ToDateTime(_endDate);

            if (startDate.Year != endDate.Year)
            {
                return startDate.ToString("MMM d, yyyy") + " - " + endDate.ToString("MMM d, yyyy");
            }
            else
            {
                return startDate.ToString("MMM d") + " - " + endDate.ToString("MMM d, yyyy");
            }
        }

        private string GetDateTime(DateTime _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("MMM d, yyyy hh:mm tt").ToUpper();
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            string id = ((ViewGroup)sender).FindViewById<TextView>(Resource.Id.txtId).Text;
            Intent intent = new Intent(mContext, typeof(TransactionsReceiptPreview));
            intent.PutExtra("selectedTransactionId", Convert.ToInt32(id));
            intent.PutExtra("isMultipay", true);
            mActivity.StartActivityForResult(intent, 45);
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