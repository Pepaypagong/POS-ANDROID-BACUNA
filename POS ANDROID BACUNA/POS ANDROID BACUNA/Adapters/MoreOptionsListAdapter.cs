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
    class MoreOptionsListAdapter : BaseAdapter<Options>
    {
        private List<Options> mItems;
        private Context mContext;

        public MoreOptionsListAdapter(Context context, List<Options> items)
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

        public override Options this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.moreOptionsDialogFragmentRow, null, false);
            }

            TextView txtOptionText = row.FindViewById<TextView>(Resource.Id.txtOptionText);
            ImageView imgArrow = row.FindViewById<ImageView>(Resource.Id.imgArrow);

            int textColorInt = mContext.GetColor(mItems[position].TextColorResourceId);
            Color textColor = new Color(textColorInt);

            txtOptionText.Text = mItems[position].OptionText;
            txtOptionText.SetTextColor(textColor);
            imgArrow.Visibility = mItems[position].ShowArrow ? ViewStates.Visible : ViewStates.Invisible;

            return row;
        }
    }
}