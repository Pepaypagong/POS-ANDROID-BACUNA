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
    class ProductsItemSelectCategoryAdapter : BaseAdapter<ProductCategories>
    {
        private List<ProductCategories> mItems;
        private Context mContext;

        public ProductsItemSelectCategoryAdapter(Context context, List<ProductCategories> items)
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

        public override ProductCategories this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.products_fragment_items_category_select_row, null, false);
            }

            TextView txtCategoryName = row.FindViewById<TextView>(Resource.Id.txtCategoryName);
            txtCategoryName.Text = mItems[position].productCategoryName;

            return row;
        }
    }
}