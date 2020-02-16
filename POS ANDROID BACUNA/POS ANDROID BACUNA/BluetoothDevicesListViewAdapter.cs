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
    class BluetoothDevicesListViewAdapter : BaseAdapter<ConnectedBluetoothDevice>
    {
        private List<ConnectedBluetoothDevice> mItems;
        private Context mContext;

        public BluetoothDevicesListViewAdapter(Context context, List<ConnectedBluetoothDevice> items)
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

        public override ConnectedBluetoothDevice this[int position]
        {
            get { return mItems[position]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.settings_fragment_bluetooth_list_row, null, false);
            }

            TextView txtFirstName = row.FindViewById<TextView>(Resource.Id.txtBluetoothDeviceName);
            txtFirstName.Text = mItems[position].deviceName;

            return row;
        }
    }
}