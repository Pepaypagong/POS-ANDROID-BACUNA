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
    [Activity(Label = "SettingsFragmentSelectedPrinterActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsFragmentSelectedPrinterActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        Button mBtnTestPrint;
        TextView mTxtSelectedPrinterName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings_fragment_selected_printer);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarPrinterSelectedPrinter);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetTitle(Resource.String.settings_select_printer);
            mBtnTestPrint = FindViewById<Button>(Resource.Id.btnTestPrint);
            mBtnTestPrint.Click += new SingleClickListener(MBtnTestPrint_Click).OnClick;
            mTxtSelectedPrinterName = FindViewById<TextView>(Resource.Id.txtPrinterName);
            mTxtSelectedPrinterName.Text = GlobalCart.mSelectedDevice.Name;
        }

        private void MBtnTestPrint_Click(object sender, EventArgs e)
        {
            PrintText(GlobalCart.mSelectedDevice, toPrint("20200201","BACUNA RTW","BEELINE BUILDING, BRGY. BACLARAN","PARANAQUE CITY, PHILIPPINES","09174897988", 
                "JEFFREY BACUNA"," ",
                "Please come again, thank you :)"));
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
            GlobalCart.mSelectedDevice = null;
            GlobalCart.mIsPrinterSet = false;
            Finish();
        }

        public List<string> toPrint(string _transNo, string _storeName, string _storeAddress1, 
            string _storeAddress2, string _storeContactNo, string _cashierName, string _customerName,string footerNote)
        {
            int maxPaperWidth = 32;
            string wideCharsStart = "\u001b!\u0010";
            string wideCharsEnd = "\u001b!\u0000";
            string boldStart = "\u001b!\u0008";
            string boldEnd = "\u001b!\u0000";
            string returnSpace = "\r\n";
            string returnSpaceSmall = "";
            _cashierName = "Cashier:" + boldStart + _cashierName + boldEnd;

            string line = "";
            for (int i = 0; i < 32; i++)
            {
                if (i == 0 | i == 32)
                {
                    returnSpaceSmall += "-";
                }
                else
                {
                    returnSpaceSmall += " ";
                }
                line += "-";
            }

            List<string> returnVal = new List<string>() {
                boldStart +wideCharsStart+ centeredString(maxPaperWidth, _storeName)+wideCharsEnd + boldEnd, //STORENAME
                centeredString(maxPaperWidth, _storeAddress1), //STORE ADDRESS 1
                centeredString(maxPaperWidth, _storeAddress2), //STORE ADDRESS 2
                centeredString(maxPaperWidth, _storeContactNo),returnSpace, //STORE ADDRESS 2
                "Transaction #:" + boldStart + _transNo + boldEnd, //trans no
                "Date:" + boldStart + DateTime.Now.ToString("MMM d, yyyy h:mm tt")  + boldEnd, //date now String.Format("MMM d, yyyy h:mm tt",DateTime.Now)
                _cashierName, //CASHIER NAME
                "Customer:" + boldStart + _customerName + boldEnd, //CustomerName
                line //line divider
            };

            List<Product> cartItems = GlobalCart.globalProductsCart;
            decimal totalPrice = 0;

            //get items from cart
            foreach (var item in cartItems)
            {
                int itemQty = 12;
                string size = "S";
                totalPrice += itemQty * item.productRetailPrice;

                returnVal.Add("("+size+ ") " + item.productName);
                returnVal.Add(quantityAndPriceLine(itemQty, item.productRetailPrice, maxPaperWidth));
                if (cartItems.IndexOf(item) < cartItems.Count-1)//last item
                {
                    returnVal.Add(returnSpaceSmall);
                }
            }

            //Footer
            returnVal.Add(line);
            returnVal.Add(TotalLine(totalPrice, wideCharsStart, wideCharsEnd, maxPaperWidth));
            returnVal.Add(line);
            returnVal.Add(centeredString(maxPaperWidth, footerNote));
            return returnVal;
        }

        string TotalLine(decimal _totalPrice, string _boldStart, string _boldEnd, int _maxPaperWidth)
        {
            string retVal = "";
            string totalLabel = "TOTAL:";
            string pesoSign = "P ";

            int totalLabelLength = totalLabel.Length;
            int totalAmountLength = (pesoSign+String.Format("{0:n}", _totalPrice)).ToString().Length;

            int spacingCount = _maxPaperWidth - (totalLabelLength + totalAmountLength);
            string spaceBetween = "";

            for (int i = 0; i < spacingCount; i++)
            {
                spaceBetween += " ";
            }

            retVal = _boldStart + totalLabel + spaceBetween  + pesoSign + String.Format("{0:n}", _totalPrice) + _boldEnd;

            return retVal;
        }

        string quantityAndPriceLine(int _qty, decimal _retailPrice, int _maxPaperWidth)
        {
            string retVal = "";
            string pesoSign = "P ";
            string qtyAndPrice = (_qty + " pcs * " + String.Format("{0:n}", _retailPrice)).ToString();
            string subTotalPrice = (pesoSign + String.Format("{0:n}", (_retailPrice * _qty))).ToString();

            int qtyAndPriceLength = qtyAndPrice.Length;
            int subTotalPriceLength = subTotalPrice.Length;

            int spacingCount = _maxPaperWidth - (qtyAndPriceLength + subTotalPriceLength);
            string spaceBetween = "";

            for (int i = 0; i < spacingCount; i++)
            {
                spaceBetween += " ";
            }

            retVal = qtyAndPrice + spaceBetween + subTotalPrice;
            return retVal;
        }

        public string centeredString(int _paperWidth, string _stringToAdjust)
        {
            string retVal = _stringToAdjust;
            int inputLength = _stringToAdjust.Length;

            if (inputLength > _paperWidth)
            {
                //do nothing because max input on address line is 32.
            }
            else
            {
                //compute spacing here
                int leftSpacingCount = (_paperWidth - inputLength)/2;
                string space = "";
                for (int i = 0; i < leftSpacingCount; i++)
                {
                    space += " ";
                }
                retVal = space + _stringToAdjust;
            }

            return retVal;
        }

        public async void PrintText(BluetoothDevice selectedDevice, List<string> input)
        {
            try
            {
                BluetoothSocket _socket = selectedDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                await _socket.ConnectAsync();
                string footerSpace = "\r\n" + "\r\n" + "\r\n" + "\r\n" + "\r\n";

                for (int i = 0; i < input.Count; i++)
                {
                    string toPrint = input[i] + "\r\n";
                    if (i == input.Count - 1)
                    {
                        toPrint = input[i] + footerSpace;
                    }
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(toPrint);
                    await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                _socket.Close();
            }
            catch (Exception exp)
            {
                //BluetoothMessage("Attention", "Please turn on the printer.");
                //throw exp;
            }
        }

    }
}
