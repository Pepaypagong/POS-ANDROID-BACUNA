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
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "SettingsFragmentSelectedPrinterActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsFragmentSelectedPrinterActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        Button mBtnTestPrint;
        TextView mTxtSelectedPrinterName;
        SettingsDataAccess mSettingsDataAccess;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_fragment_selected_printer);
            FnGetData();
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarPrinterSelectedPrinter);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            //actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon_thin);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.SetTitle(Resource.String.settings_select_printer);
            mBtnTestPrint = FindViewById<Button>(Resource.Id.btnTestPrint);
            mBtnTestPrint.Click += new SingleClickListener(MBtnTestPrint_Click).OnClick;
            mTxtSelectedPrinterName = FindViewById<TextView>(Resource.Id.txtPrinterName);
            mTxtSelectedPrinterName.Text = mSettingsDataAccess.SelectTable()[0].ReceiptPrinterName;
        }

        private void FnGetData()
        {
            mSettingsDataAccess = new SettingsDataAccess();
        }

        private void MBtnTestPrint_Click(object sender, EventArgs e)
        {
            using (var mBluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                if (mBluetoothAdapter == null)
                {
                    DialogMessageService.MessageBox(this, "Attention", "This device does not support bluetooth");
                }
                else if (mBluetoothAdapter.IsEnabled)
                {
                    BluetoothDevice printer = mBluetoothAdapter.GetRemoteDevice(mSettingsDataAccess.SelectTable()[0].ReceiptPrinterAddress);
                    PrintReceiptService printReceiptService = new PrintReceiptService();
                    printReceiptService.PrintText(printer, new List<string>() { "PRINTER WORKING" });
                }
                else
                {
                    DialogMessageService.MessageBox(this, "Attention", "Please turn on your Bluetooth to connect the printer to your phone.");
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_settings_printer_selectedPrinter, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.printer_selectedPrinter_delete:
                    removeSelectedPrinter();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void removeSelectedPrinter()
        {
            var currentPrinterSettings = mSettingsDataAccess.SelectTable()[0];
            SettingsModel input = new SettingsModel()
            {
                Id = 1,
                ReceiptAddressLine1 = currentPrinterSettings.ReceiptAddressLine1,
                ReceiptAddressLine2 = currentPrinterSettings.ReceiptAddressLine2,
                ReceiptCompanyName = currentPrinterSettings.ReceiptCompanyName,
                ReceiptContactNumber = currentPrinterSettings.ReceiptContactNumber,
                ReceiptFooterNote = currentPrinterSettings.ReceiptFooterNote,
                ReceiptPrinterAddress = "",
                ReceiptPrinterName = ""
            };
            mSettingsDataAccess.UpdateTable(input);
            Finish();
        }

    }
}
