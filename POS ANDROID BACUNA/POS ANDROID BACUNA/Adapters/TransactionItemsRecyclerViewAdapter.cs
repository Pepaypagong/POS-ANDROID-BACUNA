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

namespace POS_ANDROID_BACUNA.Adapters
{
    class TransactionItemsRecyclerViewAdapter : RecyclerView.Adapter
    {
        private List<TransactionItemsModel> mTransactionItems;
        Context mContext;
        Activity mActivity;
        string mPesoSign = "\u20b1";
        bool mIsReceipt = false;

        public TransactionItemsRecyclerViewAdapter(Activity _activity, Context _context, List<TransactionItemsModel> _model, bool _isReceipt)
        {
            mActivity = _activity;
            mContext = _context;
            mTransactionItems = _model;
            mIsReceipt = _isReceipt;
        }

        public class DetailsViewHolder : RecyclerView.ViewHolder
        {
            public DetailsViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mProductName { get; set; }
            public TextView mProductPrice { get; set; }
            public TextView mQuantityOnCart { get; set; }
            public TextView mSubtotalAmount { get; set; }
        }

        public class ReceiptViewHolder : RecyclerView.ViewHolder
        {
            public ReceiptViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mProductSizeAndName { get; set; }
            public TextView mQuantityAndPrice { get; set; }
            public TextView mOrigAmount { get; set; }
            public TextView mDiscountedAmount { get; set; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            int resourceLayout = mIsReceipt ? Resource.Layout.transactions_fragment_receipt_preview_child
                                            : Resource.Layout.transactions_fragment_items_fragment_row;
            View itemView = LayoutInflater.From(parent.Context).Inflate(resourceLayout, parent, false);

            if (mIsReceipt)
            {
                TextView txtItemName = itemView.FindViewById<TextView>(Resource.Id.txtItemName);
                TextView txtQtyAndPrice = itemView.FindViewById<TextView>(Resource.Id.txtQtyAndPrice);
                TextView txtOrigAmount = itemView.FindViewById<TextView>(Resource.Id.txtOrigAmount);
                TextView txtDiscountedAmount = itemView.FindViewById<TextView>(Resource.Id.txtDiscountedAmount);
                ReceiptViewHolder view = new ReceiptViewHolder(itemView)
                {
                    mProductSizeAndName = txtItemName,
                    mQuantityAndPrice = txtQtyAndPrice,
                    mOrigAmount = txtOrigAmount,
                    mDiscountedAmount = txtDiscountedAmount
                };
                return view;
            }
            else
            {
                TextView txtProductName = itemView.FindViewById<TextView>(Resource.Id.txtItemName);
                TextView txtProductPrice = itemView.FindViewById<TextView>(Resource.Id.txtItemPrice);
                TextView txtQuantityOnCart = itemView.FindViewById<TextView>(Resource.Id.txtQty);
                TextView txtSubTotalAmount = itemView.FindViewById<TextView>(Resource.Id.txtSubTotal);
                DetailsViewHolder view = new DetailsViewHolder(itemView)
                {
                    mProductName = txtProductName,
                    mProductPrice = txtProductPrice,
                    mQuantityOnCart = txtQuantityOnCart,
                    mSubtotalAmount = txtSubTotalAmount
                };
                return view;
            }
            
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            if (mIsReceipt)
            {
                ReceiptViewHolder receiptHolder = viewHolder as ReceiptViewHolder;
                receiptHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
                receiptHolder.mMainView.Click += MMainView_Click; //set click event for row

                receiptHolder.mProductSizeAndName.Text = mTransactionItems[position].ProductName;

                string pieces = mTransactionItems[position].ProductCountOnCart > 1 ? " pcs * " : " pc * ";
                receiptHolder.mQuantityAndPrice.Text = mTransactionItems[position].ProductCountOnCart + pieces
                   + string.Format("{0:n}", mTransactionItems[position].ProductPrice);
                receiptHolder.mOrigAmount.Visibility = mTransactionItems[position].ProductDiscountAmount > 0 ? ViewStates.Visible : ViewStates.Invisible;
                receiptHolder.mOrigAmount.Text = mPesoSign + string.Format("{0:n}", mTransactionItems[position].ProductOrigPrice);
                receiptHolder.mDiscountedAmount.Text = mPesoSign + string.Format("{0:n}", mTransactionItems[position].ProductSubTotalPrice);
            }
            else
            {
                DetailsViewHolder myHolder = viewHolder as DetailsViewHolder;
                myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
                myHolder.mMainView.Click += MMainView_Click; //set click event for row

                myHolder.mProductName.Text = mTransactionItems[position].ProductName;
                myHolder.mProductPrice.Text = mPesoSign + string.Format("{0:n}", mTransactionItems[position].ProductPrice);
                myHolder.mQuantityOnCart.Text = mTransactionItems[position].ProductCountOnCart.ToString() + " X";
                myHolder.mSubtotalAmount.Text = mPesoSign + string.Format("{0:n}", mTransactionItems[position].ProductSubTotalPrice);
            }
        }

        private void MMainView_Click(object sender, EventArgs e)
        {

        }

        public override int ItemCount
        {
            get
            {
                return mTransactionItems != null ? mTransactionItems.Count : 0;
            }
        }

    }
}