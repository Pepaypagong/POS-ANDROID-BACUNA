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
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Fragments;

namespace POS_ANDROID_BACUNA
{
    class PaymentMethodsListViewAdapter : BaseAdapter<SplitPayments>
    {
        private List<SplitPayments> mItems;
        private Context mContext;
        private TextView mPaymentId;
        private ImageView mImgRemove;
        private TextView mTxtPaymentType;
        private TextView mTxtPaymentAmount;
        private string mPesoSign = "\u20b1";
        private decimal mTotalSaleAmount;
        private ListView mLvSplitPayments;
        private bool mIsInitialAddFooter = true;
        private View footerView;
        private TextView txtBalanceOrChange;
        public PaymentMethodsListViewAdapter(Context context, List<SplitPayments> items, decimal totalSaleAmount, ListView lvSplitPayments)
        {
            mItems = items;
            mContext = context;
            mTotalSaleAmount = totalSaleAmount;
            mLvSplitPayments = lvSplitPayments;

            footerView = LayoutInflater.From(mContext).Inflate(Resource.Layout.checkout_fragment_payment_methods_footer, null, false);
            txtBalanceOrChange = footerView.FindViewById<TextView>(Resource.Id.txtBalanceOrChange);
            mLvSplitPayments.AddFooterView(footerView);
        }
        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override SplitPayments this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.checkout_fragment_payment_methods_item, null, false);
            }

            FnSetControls(row);
            FnSetupEvents();
            mPaymentId.Text = mItems[position].id.ToString();
            mTxtPaymentType.Text = mItems[position].paymentType;
            mTxtPaymentAmount.Text = mPesoSign + string.Format("{0:n}", mItems[position].amount);

            return row;
        }

        private void FnSetupEvents()
        {
            mImgRemove.Click -= MImgRemove_Click;
            mImgRemove.Click += MImgRemove_Click;
        }

        private void MImgRemove_Click(object sender, EventArgs e)
        {
            var img = (View)sender;
            ViewGroup Xparent = (ViewGroup)img.Parent;
            TextView txtPaymentId = Xparent.FindViewById<TextView>(Resource.Id.txtPaymentId);
            int paymentId = Convert.ToInt32(txtPaymentId.Text);
            mItems.RemoveAll(x => x.id == paymentId);
            SetupFooterListView();
            this.NotifyDataSetChanged();
            //refresh add payment button
            ((CheckoutFragmentPaymentMethodsActivity)mContext).SetButtonAppearance(mItems.Sum(o=>o.amount), mTotalSaleAmount);
            ((CheckoutFragmentPaymentMethodsActivity)mContext).SetSplitPaymentLabelVisibility();
        }

        public void RefreshListView()
        {
            SetupFooterListView();
            this.NotifyDataSetChanged();
        }
        public List<SplitPayments> GetUpdatedData()
        {
            return mItems;
        }
        private void FnSetControls(View _view)
        {
            mPaymentId = _view.FindViewById<TextView>(Resource.Id.txtPaymentId);
            mImgRemove = _view.FindViewById<ImageView>(Resource.Id.imgRemove);
            mTxtPaymentType = _view.FindViewById<TextView>(Resource.Id.txtPaymentType);
            mTxtPaymentAmount = _view.FindViewById<TextView>(Resource.Id.txtPaymentAmount);
        }
        public void SetupFooterListView()
        {
            decimal sumOfSplitPayments = mItems.Sum(x => x.amount);
            decimal totalSaleAmount = mTotalSaleAmount;
            
            txtBalanceOrChange.Text = GenerateBalanceOrChange(sumOfSplitPayments, totalSaleAmount);
            if (sumOfSplitPayments != 0 && (sumOfSplitPayments != totalSaleAmount))
            {
                if (mLvSplitPayments.FooterViewsCount == 0)
                {
                    mLvSplitPayments.AddFooterView(footerView);
                }
            }
            else if (sumOfSplitPayments == totalSaleAmount || sumOfSplitPayments == 0)
            {
                if (mLvSplitPayments.FooterViewsCount > 0)
                {
                    mLvSplitPayments.RemoveFooterView(footerView);
                }
            }
        }
        private string GenerateBalanceOrChange(decimal _sumOfSplitPayments, decimal _totalSaleAmount)
        {
            string retval = "";

            if ((_totalSaleAmount == _sumOfSplitPayments) || _sumOfSplitPayments == 0)
            {
                retval = "";
            }
            else if (_totalSaleAmount > _sumOfSplitPayments)
            {
                retval = "Balance: " + mPesoSign + string.Format("{0:n}", _totalSaleAmount - _sumOfSplitPayments);
            }
            else if (_totalSaleAmount < _sumOfSplitPayments)
            {
                retval = "Change: " + mPesoSign + string.Format("{0:n}", _sumOfSplitPayments - _totalSaleAmount);
            }

            return retval;
        }
    }
}