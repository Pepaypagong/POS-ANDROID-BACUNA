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
using POS_ANDROID_BACUNA.Data_Classes.Temp;
using POS_ANDROID_BACUNA.SQLite;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;

namespace POS_ANDROID_BACUNA.Adapters
{
    class RunnersMultipayReceiptRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<RunnersMultipayReceiptGroupedByDate> mItems;
        Context mContext;
        string mPesoSign = "\u20b1";
        RunnersDataAccess mRunnersDataAccess;
        private static int TYPE_HEADER = 0;
        private static int TYPE_ITEM = 1;
        private static int TYPE_FOOTER = 2;

        public RunnersMultipayReceiptRecyclerViewAdapter(Context _context, List<RunnersMultipayReceiptGroupedByDate> _model)
        {
            mContext = _context;
            mItems = _model;
            mRunnersDataAccess = new RunnersDataAccess();
        }
        public class HeaderViewHolder : RecyclerView.ViewHolder
        {
            public HeaderViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mTransactionNumber { get; set; }
            public TextView mTransactionDate { get; set; }
            public TextView mCashier { get; set; }
            public TextView mRunner { get; set; }
        }


        public class DetailsViewHolder : RecyclerView.ViewHolder
        {
            public DetailsViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mProductSizeAndName { get; set; }
            public TextView mQuantityAndPrice { get; set; }
            public TextView mOrigAmount { get; set; }
            public TextView mDiscountedAmount { get; set; }
        }

        public class FooterViewHolder : RecyclerView.ViewHolder
        {
            public FooterViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mTotalAmount { get; set; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;
            if (viewType == TYPE_HEADER)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transactions_fragment_receipt_preview_header, parent, false);
                TextView txtTransactionNumber = itemView.FindViewById<TextView>(Resource.Id.txtTransactionNumber);
                TextView txtTransactionDate = itemView.FindViewById<TextView>(Resource.Id.txtDate);
                TextView txtCashier = itemView.FindViewById<TextView>(Resource.Id.txtCashier);
                TextView txtRunner = itemView.FindViewById<TextView>(Resource.Id.txtRunnerName);
                HeaderViewHolder view = new HeaderViewHolder(itemView)
                {
                    mTransactionNumber = txtTransactionNumber,
                    mTransactionDate = txtTransactionDate,
                    mCashier = txtCashier,
                    mRunner = txtRunner
                };
                return view;
            }
            else if (viewType == TYPE_ITEM)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transactions_fragment_receipt_preview_child, parent, false);
                TextView txtItemName = itemView.FindViewById<TextView>(Resource.Id.txtItemName);
                TextView txtQtyAndPrice = itemView.FindViewById<TextView>(Resource.Id.txtQtyAndPrice);
                TextView txtOrigAmount = itemView.FindViewById<TextView>(Resource.Id.txtOrigAmount);
                TextView txtDiscountedAmount = itemView.FindViewById<TextView>(Resource.Id.txtDiscountedAmount);
                DetailsViewHolder view = new DetailsViewHolder(itemView)
                {
                    mProductSizeAndName = txtItemName,
                    mQuantityAndPrice = txtQtyAndPrice,
                    mOrigAmount = txtOrigAmount,
                    mDiscountedAmount = txtDiscountedAmount
                };
                return view;
            }
            else //footer
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transactions_fragment_receipt_preview_childfooter, parent, false);
                TextView txtTotalAmount = itemView.FindViewById<TextView>(Resource.Id.txtTotal);
                FooterViewHolder view = new FooterViewHolder(itemView)
                {
                    mTotalAmount = txtTotalAmount
                };
                return view;
            }
        }
        private void SetSpannableString(TextView _textview, string _label, string _boldText)
        {
            SpannableStringBuilder str = new SpannableStringBuilder(_label + _boldText);
            str.SetSpan(new StyleSpan(TypefaceStyle.Bold), _label.Length, _label.Length + _boldText.Length, SpanTypes.ExclusiveExclusive);
            _textview.TextFormatted = str;
        }
        private string GetFormattedDateTimeString(string _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("MMM d, yyyy - hh:mm tt").ToUpper();
        }

        private string GetRunnerName(int id)
        {
            return mRunnersDataAccess.SelectRecord(id)[0].FullName;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            int viewType = GetItemViewType(position);
            if (viewType == TYPE_HEADER)
            {
                HeaderViewHolder headerViewHolder = viewHolder as HeaderViewHolder;
                SetSpannableString(headerViewHolder.mTransactionNumber, "Transaction #: ", mItems[position].Id. ToString());
                SetSpannableString(headerViewHolder.mTransactionDate, "Date: ", GetFormattedDateTimeString(mItems[position].TransactionDateTime));
                SetSpannableString(headerViewHolder.mCashier, "Cashier: ", mItems[position].CashierName);
                SetSpannableString(headerViewHolder.mRunner, "Runner: " , GetRunnerName(mItems[position].RunnerId));

            }
            else if (viewType == TYPE_ITEM)
            {
                DetailsViewHolder detailsHolder = viewHolder as DetailsViewHolder;
                detailsHolder.mProductSizeAndName.Text = mItems[position].ProductName;
                string pieces = mItems[position].ProductCountOnCart > 1 ? " pcs * " : " pc * ";
                detailsHolder.mQuantityAndPrice.Text = mItems[position].ProductCountOnCart + pieces
                   + string.Format("{0:n}", mItems[position].ProductPrice);
                detailsHolder.mOrigAmount.Visibility = mItems[position].ProductDiscountAmount > 0 ? ViewStates.Visible : ViewStates.Invisible;
                detailsHolder.mOrigAmount.Text = mPesoSign + string.Format("{0:n}", mItems[position].ProductOrigPrice);
                detailsHolder.mDiscountedAmount.Text = mPesoSign + string.Format("{0:n}", mItems[position].ProductSubTotalPrice);
            }
            else //footer
            {
                FooterViewHolder footerViewHolder = viewHolder as FooterViewHolder;
                footerViewHolder.mTotalAmount.Text = mPesoSign + string.Format("{0:n}", mItems[position].FooterTotalAmount);
            }

        }

        public override int GetItemViewType(int position)
        {
            int view_type = mItems[position].ViewType;
            switch (view_type)
            {
                case 0:
                    return TYPE_HEADER;
                    break;
                case 1:
                    return TYPE_ITEM;
                    break;
                default:
                    return TYPE_FOOTER;
                    break;
            }
        }

        public override int ItemCount
        {
            get
            {
                return mItems != null ? mItems.Count : 0;
            }
        }

    }
}