using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportSearchBar = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes;
using Java.Util;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "SettingsFragmentPrinterListActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsFragmentPrinterListActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        BluetoothAdapter mBluetoothAdapter;
        List<ConnectedBluetoothDevice> mDevices = new List<ConnectedBluetoothDevice>();
        ListView mListViewPrinters;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_fragment_devices_list);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarPrinterList);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            //actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon_thin);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.SetTitle(Resource.String.settings_select_printer);
            mListViewPrinters = FindViewById<ListView>(Resource.Id.lvBluetoothDevicesList);
            mListViewPrinters.ItemClick += MListViewPrinters_ItemClick;
            LoadPrinterList();
        }

        private void MListViewPrinters_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string deviceName = mDevices[e.Position].deviceName;
            string deviceAddress = mDevices[e.Position].deviceAddress;

            //Toast.MakeText(this, deviceName + " Address is : " + deviceAddress, ToastLength.Long).Show();

            BluetoothDevice selectedDevice = (from bd in mBluetoothAdapter.BondedDevices where bd.Address == deviceAddress select bd).FirstOrDefault();

            if (selectedDevice == null) 
                DialogMessageService.MessageBox(this, "Attention", "Please turn on the printer.");

            //show selected printer page
            Intent intent = new Intent(this, typeof(SettingsFragmentSelectedPrinterActivity));
            GlobalVariables.mIsPrinterSet = true;
            GlobalVariables.mSelectedDevice = selectedDevice;
            Finish();
            StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_settings_printer_list, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.printer_list_refresh:
                    LoadPrinterList();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void LoadPrinterList()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (mBluetoothAdapter == null)
            {
                DialogMessageService.MessageBox(this,"Attention", "This device does not support bluetooth");
            }
            else if (mBluetoothAdapter.IsEnabled)
            {
                mDevices.Clear();
                SetUpBluetooth();
            }
            else
            {
                DialogMessageService.MessageBox(this,"Attention", "Please turn on your Bluetooth to connect the printer to your phone.");
            }
        }

        public void SetUpBluetooth()
        {
            if (mBluetoothAdapter == null)
                throw new Exception("No bluetooth Adapter Found");
            if (!mBluetoothAdapter.IsEnabled)
                DialogMessageService.MessageBox(this,"Attention", "Please turn on the bluetooth");

            foreach (var bd in mBluetoothAdapter.BondedDevices)
            {
                mDevices.Add(new ConnectedBluetoothDevice() { deviceName = bd.Name, deviceAddress = bd.Address });
            }

            BluetoothDevicesListViewAdapter adapter = new BluetoothDevicesListViewAdapter(this, mDevices);
            mListViewPrinters.Adapter = adapter;
        }

    }
}