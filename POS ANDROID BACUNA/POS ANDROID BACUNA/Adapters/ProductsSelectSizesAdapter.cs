using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Data_Classes;

namespace POS_ANDROID_BACUNA
{
    class ProductsSelectSizesAdapter : BaseAdapter<ProductSizes>
    {
        private List<ProductSizes> mItems;
        private Context mContext;

        public ProductsSelectSizesAdapter(Context context, List<ProductSizes> items)
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

        public override ProductSizes this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.products_fragment_items_size_select_row, null, false);
            }

            TextView txtSize = row.FindViewById<TextView>(Resource.Id.txtSizeName);
            txtSize.Text = mItems[position].productSizeName;

            TextView txtStatus = row.FindViewById<TextView>(Resource.Id.txtSizeStatus);

            bool alreadyExists = GlobalVariables.newProductSizesList.Any(x => x.productSize == mItems[position].productSizeName);

            txtStatus.Text = alreadyExists ? "added" : "not yet added";

            int colorIntGray = mContext.GetColor(Resource.Color.colorLightBlack);
            int colorIntGreen = mContext.GetColor(Resource.Color.colorAccent);

            Color gray = new Color(colorIntGray);
            Color green = new Color(colorIntGreen);

            txtStatus.SetTextColor(alreadyExists ? green : gray);

            return row;
        }
    }
}