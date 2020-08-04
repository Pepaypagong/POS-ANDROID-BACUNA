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
using Product = POS_ANDROID_BACUNA.Data_Classes.Product;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Fragments;

namespace POS_ANDROID_BACUNA.Adapters
{
    class CheckoutMultiAddListRecyclerViewAdapter : RecyclerView.Adapter
    {
        RecyclerView mRecyclerView;
        private List<Product> mProducts;

        CheckoutFragment mCheckoutFragment;
        Button mBtnAddToCart;
        Context mCheckoutMultiAddContext;
        bool mIsAllChecked = false;
        string mAllItemsQty = "0";
        string mCurrentPricingType;

        public CheckoutMultiAddListRecyclerViewAdapter(List<Product> products, RecyclerView recyclerView, 
            //Button _minus6, Button _minus1, Button _plus1, Button _plus6, Button _plus12, TextView _txtProductQuantity, 
            Button btnAddToCart, Context checkoutmultiaddcontext, string pricingType)
        {
            mProducts = products;
            mRecyclerView = recyclerView;

            //mMinus6 = _minus6;
            //mMinus1 = _minus1;
            //mPlus1 = _plus1;
            //mPlus6 =_plus6;
            //mPlus12 = _plus12;
            //mProductQuantity = _txtProductQuantity;

            mCheckoutFragment = new CheckoutFragment();
            mBtnAddToCart = btnAddToCart;
            mCheckoutMultiAddContext = checkoutmultiaddcontext;
            mCurrentPricingType = pricingType;
        }

        public class MyViewHolder : RecyclerView.ViewHolder
        {
            public View mMainView { get; set; }
            public TextView mTxtItemId { get; set; }
            public TextView mTxtItemPrice { get; set; }
            public CheckBox mProductSizeAndPrice { get; set; }
            public TextView mTxtQtyToZero { get; set; }
            public TextView mTxtQtyMinus1 { get; set; }
            public TextView mTxtQuantity { get; set; }
            public TextView mTxtQtyPlus1 { get; set; }
            public TextView mTxtQtyPlus6 { get; set; }
            public TextView mTxtQtyPlus12 { get; set; }
            public MyViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.checkout_fragment_multi_size_add_list_item, parent, false);

            TextView mTxtItemId = itemView.FindViewById<TextView>(Resource.Id.txtItemId);
            TextView mTxtItemPrice = itemView.FindViewById<TextView>(Resource.Id.txtItemPrice);
            CheckBox cbProductSizeAndPrice = itemView.FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView mTxtQtyToZero = itemView.FindViewById<TextView>(Resource.Id.txtQtyToZero);
            TextView mTxtQtyMinus1 = itemView.FindViewById<TextView>(Resource.Id.txtQtyMinus1); 
            TextView mTxtQuantity = itemView.FindViewById<TextView>(Resource.Id.txtQty);
            TextView mTxtQtyPlus1 = itemView.FindViewById<TextView>(Resource.Id.txtQtyPlus1); 
            TextView mTxtQtyPlus6 = itemView.FindViewById<TextView>(Resource.Id.txtQtyPlus6);
            TextView mTxtQtyPlus12 = itemView.FindViewById<TextView>(Resource.Id.txtQtyPlus12);

            MyViewHolder view = new MyViewHolder(itemView)
            {
                mTxtItemId = mTxtItemId,
                mTxtItemPrice = mTxtItemPrice,
                mProductSizeAndPrice = cbProductSizeAndPrice,
                mTxtQtyToZero = mTxtQtyToZero,
                mTxtQtyMinus1 = mTxtQtyMinus1,
                mTxtQuantity = mTxtQuantity, 
                mTxtQtyPlus1 = mTxtQtyPlus1, 
                mTxtQtyPlus6 = mTxtQtyPlus6, 
                mTxtQtyPlus12 = mTxtQtyPlus12
            };

            return view;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            MyViewHolder myHolder = viewHolder as MyViewHolder;

            subscribeEvents(myHolder);

            myHolder.mTxtItemId.Text = mProducts[position].productId.ToString();
            myHolder.mTxtItemPrice.Text = GetProductPrice(position).ToString();
            myHolder.mProductSizeAndPrice.Text = "Size " + mProducts[position].productSize.ToString() + "\n \u20b1 " + String.Format("{0:n}", GetProductPrice(position));

            //set if item is checked
            myHolder.mProductSizeAndPrice.Checked = mIsAllChecked ? true : false;
        }
        private decimal GetProductPrice(int position)
        {
            decimal retval = 0;
            if (mCurrentPricingType == "RT")
            {
                retval = mProducts[position].productRetailPrice;
            }
            else if (mCurrentPricingType == "WS")
            {
                retval = mProducts[position].productWholesalePrice;
            }
            else //runner
            {
                retval = mProducts[position].productRunnerPrice;
            }

            return retval;
        }

        void subscribeEvents(MyViewHolder _myHolder)
        {
            _myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
            _myHolder.mMainView.Click += MMainView_Click; //set click event for row

            _myHolder.mProductSizeAndPrice.Click -= MProductSizeAndPrice_Click;
            _myHolder.mProductSizeAndPrice.Click += MProductSizeAndPrice_Click;

            _myHolder.mTxtQtyToZero.Click -= MTxtQtyToZero_Click;
            _myHolder.mTxtQtyToZero.Click += MTxtQtyToZero_Click;
            _myHolder.mTxtQtyMinus1.Click -= MTxtQtyMinus1_Click;
            _myHolder.mTxtQtyMinus1.Click += MTxtQtyMinus1_Click;
            _myHolder.mTxtQtyPlus1.Click -= MTxtQtyPlus1_Click;
            _myHolder.mTxtQtyPlus1.Click += MTxtQtyPlus1_Click;
            _myHolder.mTxtQtyPlus6.Click -= MTxtQtyPlus6_Click;
            _myHolder.mTxtQtyPlus6.Click += MTxtQtyPlus6_Click;
            _myHolder.mTxtQtyPlus12.Click -= MTxtQtyPlus12_Click;
            _myHolder.mTxtQtyPlus12.Click += MTxtQtyPlus12_Click;
        }

        public ViewGroup GetMainParent(TextView _senderTextView)
        {
            TextView x = _senderTextView;
            ViewGroup parent;
            return parent = (ViewGroup)x.Parent.Parent;
        }

        public string ComputeQuantity(string _value, bool _isToAdd, TextView _currentQty)
        {
            string retVal = "";
            int maxQty = 9999;
            int minQty = 0;
            int currentQty = Convert.ToInt32(_currentQty.Text);
            int inputValue = Convert.ToInt32(_value);
            int result = 0;

            if (_isToAdd)
            {
                result = currentQty + inputValue;
                if (result > maxQty)
                {
                    result = maxQty;
                }
            }
            else
            {
                result = currentQty - inputValue;
                if (result < minQty)
                {
                    result = minQty;
                }
            }

            retVal = result.ToString();
            return retVal;
        }

        private void MTxtQtyPlus12_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = GetMainParent((TextView)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = GetMainParent((TextView)sender).FindViewById<TextView>(Resource.Id.txtQty);

            checkbox.Checked = true;
            txtQuantity.Text = ComputeQuantity("12",true, txtQuantity);
        }

        private void MTxtQtyPlus6_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = GetMainParent((TextView)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = GetMainParent((TextView)sender).FindViewById<TextView>(Resource.Id.txtQty);

            checkbox.Checked = true;
            txtQuantity.Text = ComputeQuantity("6", true, txtQuantity);
        }

        private void MTxtQtyPlus1_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = GetMainParent((TextView)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = GetMainParent((TextView)sender).FindViewById<TextView>(Resource.Id.txtQty);

            checkbox.Checked = true;
            txtQuantity.Text = ComputeQuantity("1", true, txtQuantity);
        }

        private void MTxtQtyMinus1_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = GetMainParent((TextView)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = GetMainParent((TextView)sender).FindViewById<TextView>(Resource.Id.txtQty);

            if (txtQuantity.Text == "1")
            {
                checkbox.Checked = false;
                txtQuantity.Text = ComputeQuantity("1", false, txtQuantity);
            }
            else if(txtQuantity.Text != "0")
            {
                checkbox.Checked = true;
                txtQuantity.Text = ComputeQuantity("1", false, txtQuantity);
            }
            
        }

        private void MTxtQtyToZero_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = GetMainParent((TextView)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = GetMainParent((TextView)sender).FindViewById<TextView>(Resource.Id.txtQty);

            checkbox.Checked = false;
            txtQuantity.Text = "0";
        }

        private void MProductSizeAndPrice_Click(object sender, EventArgs e)
        {
            CheckBox x = (CheckBox)sender;
            ViewGroup parent = (ViewGroup)x.Parent;

            CheckBox checkbox = parent.FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = parent.FindViewById<TextView>(Resource.Id.txtQty);

            if (checkbox.Checked)
            {
                if (txtQuantity.Text == "0")
                {
                    txtQuantity.Text = "1";
                }
            }
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            CheckBox checkbox = ((ViewGroup)sender).FindViewById<CheckBox>(Resource.Id.cbSizeAndPrice);
            TextView txtQuantity = ((ViewGroup)sender).FindViewById<TextView>(Resource.Id.txtQty);

            if (!checkbox.Checked)
            {
                checkbox.Checked = true;
                if (txtQuantity.Text == "0")
                {
                    txtQuantity.Text = "1";
                }    
            }
            else
            {
                checkbox.Checked = false;
            }
        }

        public void CheckAllItems(bool _isChecked)
        {
            mIsAllChecked = _isChecked ? true : false;
        }

        public void ChangeAllItemQty()
        { 
            
        }

        public override int ItemCount
        {
            get
            {
                return mProducts.Count;
            }
        }

    }
}