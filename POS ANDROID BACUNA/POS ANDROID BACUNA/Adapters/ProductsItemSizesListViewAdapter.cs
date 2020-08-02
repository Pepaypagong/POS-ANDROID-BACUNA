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

namespace POS_ANDROID_BACUNA
{
    class ProductsItemSizesListViewAdapter : BaseAdapter<NewProduct>
    {
        private List<NewProduct> mItems;
        private Context mContext;
        string pesoSign = "\u20b1 ";
        public ProductsItemSizesListViewAdapter(Context context, List<NewProduct> items)
        {
            mItems = items;
            mContext = context;
        }
        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override NewProduct this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.products_fragment_items_new_product_sizes_row, null, false);
            }

            TextView txtSize = row.FindViewById<TextView>(Resource.Id.txtSize);
            txtSize.Text = mItems[position].productSize;

            TextView txtWholesale = row.FindViewById<TextView>(Resource.Id.txtWholesale);
            txtWholesale.Text = pesoSign + String.Format("{0:n}", mItems[position].productWholesalePrice);

            TextView txtRetail = row.FindViewById<TextView>(Resource.Id.txtRetail);
            txtRetail.Text = pesoSign + String.Format("{0:n}", mItems[position].productRetailPrice);

            TextView txtRunner = row.FindViewById<TextView>(Resource.Id.txtRunner);
            txtRunner.Text = pesoSign + String.Format("{0:n}", mItems[position].productRunnerPrice);

            return row;
        }
    }
}