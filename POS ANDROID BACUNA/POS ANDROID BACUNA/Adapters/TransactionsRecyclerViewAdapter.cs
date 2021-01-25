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
using System.Runtime.InteropServices.WindowsRuntime;

namespace POS_ANDROID_BACUNA.Adapters
{
    class TransactionsRecyclerViewAdapter : RecyclerView.Adapter
    {
        private static int TYPE_HEADER = 0;
        private static int TYPE_ITEM = 1;
        private static string PAYLATER = "PAYLATER";
        private static string PAYMENT = "PAYMENT";
        private static string ORDER = "ORDER";
        private List<TransactionListGroupedByDate> mTransactionGroupedList;
        Context mContext;
        Activity mActivity;
        string mPesoSign = "\u20b1";
        RunnersDataAccess mRunnersDataAccess;
        CustomersDataAccess mCustomersDataAccess;
        public TransactionsRecyclerViewAdapter(Activity _activity, Context _context, List<TransactionListGroupedByDate> _model)
        {
            mActivity = _activity;
            mContext = _context;
            mTransactionGroupedList = _model;
            mRunnersDataAccess = new RunnersDataAccess();
            mCustomersDataAccess = new CustomersDataAccess();
        }

        public void RefreshData(List<TransactionListGroupedByDate> _dataSource)
        {
            mTransactionGroupedList = _dataSource;
            this.NotifyDataSetChanged();
        }

        public class HeaderViewHolder : RecyclerView.ViewHolder
        {
            public HeaderViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mTransactionDate { get; set; }
            public TextView mSaleAmountAndCount { get; set; }
        }

        public class ContentViewHolder : RecyclerView.ViewHolder
        {
            public ContentViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
            public View mMainView { get; set; }
            public TextView mTransactionId { get; set; }
            public TextView mTransactionDate { get; set; }
            public TextView mTransactionTime { get; set; }
            public ImageView mTransactionTypeIcon { get; set; }
            public ImageView mCustomerOrRunnerIcon { get; set; }
            public TextView mSaleAmount { get; set; }
            public TextView mCustomerOrRunnerName { get; set; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView;
            if (viewType == TYPE_HEADER)
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transactions_fragment_list_row_header, parent, false);
                TextView txtTransactionDate = itemView.FindViewById<TextView>(Resource.Id.txtDate);
                TextView txtSaleCountAndAmount = itemView.FindViewById<TextView>(Resource.Id.txtSaleCountAndAmount);
                HeaderViewHolder view = new HeaderViewHolder(itemView)
                {
                    mTransactionDate = txtTransactionDate,
                    mSaleAmountAndCount = txtSaleCountAndAmount
                };
                return view;
            }
            else
            {
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transactions_fragment_list_row, parent, false);
                TextView txtTransactionNumber = itemView.FindViewById<TextView>(Resource.Id.txtTransactionNumber);
                TextView txtTransactionTime = itemView.FindViewById<TextView>(Resource.Id.txtTransactionTime);
                ImageView imgTransactionTypeIcon = itemView.FindViewById<ImageView>(Resource.Id.imgTranstypeIcon);
                ImageView imgCustomerOrRunnerIcon = itemView.FindViewById<ImageView>(Resource.Id.imgCustomerRunnerIcon);
                TextView txtSaleAmount = itemView.FindViewById<TextView>(Resource.Id.txtSaleAmount);
                TextView txtCustomerOrRunnerName = itemView.FindViewById<TextView>(Resource.Id.txtCustomerRunnerName);

                ContentViewHolder view = new ContentViewHolder(itemView)
                {
                    mTransactionId = txtTransactionNumber,
                    mTransactionTime = txtTransactionTime,
                    mTransactionTypeIcon = imgTransactionTypeIcon,
                    mCustomerOrRunnerIcon = imgCustomerOrRunnerIcon,
                    mSaleAmount = txtSaleAmount,
                    mCustomerOrRunnerName = txtCustomerOrRunnerName,
                };
                return view;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            int type = GetItemViewType(position);
            if (type == TYPE_HEADER)
            {
                HeaderViewHolder myHolder = viewHolder as HeaderViewHolder;
                myHolder.mTransactionDate.Text = GetDate(mTransactionGroupedList[position].TransactionDateTime);
                myHolder.mSaleAmountAndCount.Text = GenerateTotalSaleAmountAndCount(position);
            }
            else if (type == TYPE_ITEM)
            {
                ContentViewHolder myHolder = viewHolder as ContentViewHolder;

                myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
                myHolder.mMainView.Click += MMainView_Click; //set click event for row

                myHolder.mTransactionId.Text = "# " + mTransactionGroupedList[position].ticketNumber.ToString();
                myHolder.mTransactionTime.Text = GetTime(mTransactionGroupedList[position].TransactionDateTime);
                myHolder.mSaleAmount.Text = mPesoSign + string.Format("{0:n}", mTransactionGroupedList[position].SubTotalAmount);
                myHolder.mCustomerOrRunnerName.Text = GetCustomerOrRunnerName(mTransactionGroupedList[position].TransactionType,
                                                                              mTransactionGroupedList[position].CustomerOrRunnerId);
                myHolder.mTransactionTypeIcon.SetImageResource(GetTranstypeIconResource(mTransactionGroupedList[position].TransactionType, mTransactionGroupedList[position].isPaid));
                myHolder.mTransactionTypeIcon.SetColorFilter(GetTranstypeIconColorFilter(mTransactionGroupedList[position].TransactionType, mTransactionGroupedList[position].isPaid));
                myHolder.mCustomerOrRunnerIcon.SetImageResource(GetCustomerOrRunnerIcon(mTransactionGroupedList[position].TransactionType));
                myHolder.mCustomerOrRunnerIcon.SetColorFilter(GetIconColorFilter(mTransactionGroupedList[position].TransactionType));
            }
        }

        private Android.Graphics.Color GetTranstypeIconColorFilter(string _transactionType, bool _isPaid)
        {
            Android.Graphics.Color retVal = ColorHelper.ResourceIdToColor(Resource.Color.colorPrimaryDark, mContext);
            if (_transactionType == PAYLATER && !_isPaid)
            {
                retVal = ColorHelper.ResourceIdToColor(Resource.Color.colorRed, mContext);
            }
            else
            {
                retVal = ColorHelper.ResourceIdToColor(Resource.Color.colorPrimaryDark, mContext);
            }

            return retVal;
        }

        private int GetCustomerOrRunnerIcon(string _transactionType)
        {
            int retval = 0;
            if (_transactionType == PAYMENT || _transactionType == ORDER)
            {
                retval = Resource.Drawable.customer_icon;
            }
            else if (_transactionType == PAYLATER)
            {
                retval = Resource.Drawable.runner_icon;
            }
            return retval;
        }

        private Android.Graphics.Color GetIconColorFilter(string _transactionType)
        {
            return _transactionType == PAYLATER ?
                   ColorHelper.ResourceIdToColor(Resource.Color.orange, mContext) :
                   ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, mContext);
        }

        private string GetCustomerOrRunnerName(string _transactionType, int _id)
        {
            string retVal = "";
            if (_transactionType == PAYMENT || _transactionType == ORDER)
            {
                retVal = mCustomersDataAccess.SelectRecord(_id)[0].FullName;
            }
            else if (_transactionType == PAYLATER)
            {
                retVal = mRunnersDataAccess.SelectRecord(_id)[0].FullName;
            }
            else
            {
                retVal = "[null]";
            }

            return retVal;
        }

        private string GenerateTotalSaleAmountAndCount(int position)
        {
            return mTransactionGroupedList[position].TransactionCount + " sales, " + 
                   mPesoSign + string.Format("{0:n}", mTransactionGroupedList[position].SubTotalAmount);
        }

        public override int GetItemViewType(int position)
        {
            if (isPositionHeader(position))
                return TYPE_HEADER;
            return TYPE_ITEM;
        }

        private bool isPositionHeader(int position)
        {
            return mTransactionGroupedList[position].isHeader;
        }

        private string GetDate(DateTime _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("dddd, d MMM yyyy");
        }
        private string GetTime(DateTime _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("hh:mm tt").ToUpper();
        }

        private int GetTranstypeIconResource(string _transactionType, bool _isPaid)
        {
            int retval = 0;
            if (_transactionType == PAYMENT)
            {
                retval = Resource.Drawable.money_icon;
            }
            else if (_transactionType == PAYLATER)
            {
                retval = _isPaid ? Resource.Drawable.paid_icon : Resource.Drawable.down_arrow_icon;
            }
            return retval;
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            string id = ((ViewGroup)sender).FindViewById<TextView>(Resource.Id.txtTransactionNumber).Text.Remove(0,2);
            Intent intent = new Intent(mContext, typeof(TransactionsFragmentTransactionInfoActivity));
            intent.PutExtra("selectedTransactionId", Convert.ToInt32(id));
            mActivity.StartActivityForResult(intent, 41);
        }

        public override int ItemCount
        {
            get
            {
                return mTransactionGroupedList != null ? mTransactionGroupedList.Count : 0;
            }
        }

    }
}