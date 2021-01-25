using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class SettingsFragment : SupportFragment
    {
        RelativeLayout mRlPrinter;
        RelativeLayout mGeneralSettings;
        RelativeLayout mBusinessInformation;
        bool mIsPrinterSet; //set from db
        static bool mDialogShown = false; //prevent double click
        SettingsModel mSelectedSettings;
        SettingsDataAccess mSettingsDataAccess;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            FnGetData();
            View view = inflater.Inflate(Resource.Layout.settings_fragment, container, false);
            mRlPrinter = view.FindViewById<RelativeLayout>(Resource.Id.rlSettingsPrinter);
            mRlPrinter.Click += MRlPrinter_Click;
            return view;
        }

        private void FnGetData()
        {
            mSettingsDataAccess = new SettingsDataAccess();
            mSelectedSettings = mSettingsDataAccess.SelectTable()[0];
        }

        private void MRlPrinter_Click(object sender, EventArgs e)
        {
            if (!mDialogShown)
            {
                using (var mBluetoothAdapter = BluetoothAdapter.DefaultAdapter)
                {
                    if (mBluetoothAdapter == null)
                    {
                        DialogMessageService.MessageBox(Context, "Attention", "This device does not support bluetooth");
                    }
                    else if (mBluetoothAdapter.IsEnabled)
                    {
                        mDialogShown = true;
                        mIsPrinterSet = mSettingsDataAccess.SelectTable()[0].ReceiptPrinterAddress != "";
                        //if there is selected printer, jump to selected printer test print, else jump to printer list
                        if (mIsPrinterSet)
                        {
                            Intent intent = new Intent(Context, typeof(SettingsFragmentSelectedPrinterActivity));
                            StartActivityForResult(intent, 3);
                        }
                        else
                        {
                            Intent intent = new Intent(Context, typeof(SettingsFragmentPrinterListActivity));
                            StartActivityForResult(intent, 3);
                        }
                    }
                    else
                    {
                        DialogMessageService.MessageBox(Context, "Attention", "Please turn on your Bluetooth to connect the printer to your phone.");
                    }
                }
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 3)
            {
                mDialogShown = false; //flag to enable dialog show 
            }
        }

    }
}