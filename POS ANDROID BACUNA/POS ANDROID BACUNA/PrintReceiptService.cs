using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes;
using Java.Util;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA
{
    public class PrintReceiptService
    {
        public List<string> toPrint(string _transNo, string _transDateTime, string _storeName, string _storeAddress1,
               string _storeAddress2, string _storeContactNo, string _cashierName, string _customerOrRunner, 
               string _customerName, string footerNote, List<TransactionItemsModel> _transactionItems)
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
                "Transaction#:" + boldStart + _transNo + boldEnd, //trans no
                "Date:" + boldStart + _transDateTime + boldEnd, //date now String.Format("MMM d, yyyy h:mm tt",DateTime.Now)
                _cashierName, //CASHIER NAME
                _customerOrRunner + boldStart + _customerName + boldEnd, //CustomerName
                line, //line divider
                "[(Size) Item name]              ",
                "[Qty]*[Price]    [Subtotal Amt.]",
                line
            };

            decimal totalPrice = 0;

            //get items from cart
            foreach (var item in _transactionItems)
            {
                int itemQty = item.ProductCountOnCart;
                string size = item.ProductSize;
                totalPrice += itemQty * item.ProductPrice;

                returnVal.Add(item.ProductName);//returnVal.Add("("+size+") " + item.productName);
                returnVal.Add(quantityAndPriceLine(itemQty, item.ProductPrice, maxPaperWidth));
                if (_transactionItems.IndexOf(item) < _transactionItems.Count - 1)//last item
                {
                    returnVal.Add(returnSpaceSmall);
                }
            }

            //Footer
            returnVal.Add(line);
            returnVal.Add(TotalLine("TOTAL:",totalPrice, wideCharsStart, wideCharsEnd, maxPaperWidth));
            returnVal.Add(line);
            returnVal.Add(centeredString(maxPaperWidth, footerNote));
            returnVal.Add(centeredString(maxPaperWidth, DateTime.Now.ToString("MMM d, yyyy - hh:mm tt").ToUpper()));
            return returnVal;
        }

        string TotalLine(string _label, decimal _totalPrice, string _boldStart, string _boldEnd, int _maxPaperWidth)
        {
            string retVal = "";
            string totalLabel = _label;
            string pesoSign = "P";

            int totalLabelLength = totalLabel.Length;
            int totalAmountLength = (pesoSign + String.Format("{0:n}", _totalPrice)).ToString().Length;

            int spacingCount = _maxPaperWidth - (totalLabelLength + totalAmountLength);
            string spaceBetween = "";

            for (int i = 0; i < spacingCount; i++)
            {
                spaceBetween += " ";
            }

            retVal = _boldStart + totalLabel + spaceBetween + pesoSign + String.Format("{0:n}", _totalPrice) + _boldEnd;

            return retVal;
        }

        string quantityAndPriceLine(int _qty, decimal _retailPrice, int _maxPaperWidth)
        {
            string retVal = "";
            string pesoSign = "P";
            string pcOrPcs = _qty > 1 ? " pcs * " : " pc * ";
            string qtyAndPrice = (_qty + pcOrPcs + String.Format("{0:n}", _retailPrice)).ToString();
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
                int leftSpacingCount = (_paperWidth - inputLength) / 2;
                string space = "";
                for (int i = 0; i < leftSpacingCount; i++)
                {
                    space += " ";
                }
                retVal = space + _stringToAdjust;
            }

            return retVal;
        }

        public List<string> GetMultipayStringPrintList(string _storeName, string _storeAddress1,
               string _storeAddress2, string _storeContactNo, string footerNote, 
               List<TransactionItemsModel> _transactionItems, List<TransactionsModel> _transactionHeader, RunnersMultipayRecordsModel _multipayRecord,
               RunnersDataAccess _runnersDataAccess)
        {
            int maxPaperWidth = 32;
            string wideCharsStart = "\u001b!\u0010";
            string wideCharsEnd = "\u001b!\u0000";
            string boldStart = "\u001b!\u0008";
            string boldEnd = "\u001b!\u0000";
            string returnSpace = "\r\n";
            string returnSpaceSmall = "";

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
                centeredString(maxPaperWidth, _storeContactNo),returnSpace
            };

            foreach (var _header in _transactionHeader)
            {
                returnVal.Add("Transaction#:" + boldStart + _header.id + boldEnd);
                returnVal.Add("Date:" + boldStart + GetFormattedDateTimeString(_header.TransactionDateTime) + boldEnd); //format
                returnVal.Add("Cashier:" + boldStart + _header.CashierName + boldEnd);
                returnVal.Add("Runner:" + boldStart + GetRunnerName(_runnersDataAccess,_header.CustomerOrRunnerId) + boldEnd); //get runner name
                returnVal.Add(line);
                returnVal.Add("[(Size) Item name]              ");
                returnVal.Add("[Qty]*[Price]    [Subtotal Amt.]");
                returnVal.Add(line);

                decimal totalPrice = 0;

                //get items from cart
                foreach (var _items in _transactionItems.Where(x=>x.TransactionId == _header.id))
                {
                    int itemQty = _items.ProductCountOnCart;
                    string size = _items.ProductSize;
                    totalPrice += itemQty * _items.ProductPrice;

                    returnVal.Add(_items.ProductName);
                    returnVal.Add(quantityAndPriceLine(itemQty, _items.ProductPrice, maxPaperWidth));

                    var currentTransactionItems = _transactionItems.Where(x => x.TransactionId == _header.id).ToList();
                    if (currentTransactionItems.IndexOf(_items) < currentTransactionItems.Count - 1)//last item
                    {
                        returnVal.Add(returnSpaceSmall);
                    }
                }

                //Child Footer
                returnVal.Add(line);
                returnVal.Add(TotalLine("TOTAL:",totalPrice, wideCharsStart, wideCharsEnd, maxPaperWidth));
                returnVal.Add(line);
            }

            returnVal.Add(line);
            returnVal.Add(TotalLine("GRAND TOTAL:", _multipayRecord.SubTotalAmount, wideCharsStart, wideCharsEnd, maxPaperWidth));
            if (_multipayRecord.PaymentCashAmount > 0)
            {
                returnVal.Add(TotalLine("Cash:", _multipayRecord.PaymentCashAmount, "", "", maxPaperWidth));
            }
            if (_multipayRecord.PaymentCheckAmount > 0)
            {
                returnVal.Add(TotalLine("Check:", _multipayRecord.PaymentCheckAmount, "", "", maxPaperWidth));
            }
            returnVal.Add(TotalLine("Change:",
                (_multipayRecord.PaymentCheckAmount + _multipayRecord.PaymentCashAmount) - _multipayRecord.SubTotalAmount, "", "", maxPaperWidth));
            returnVal.Add(line);
            returnVal.Add(centeredString(maxPaperWidth, footerNote));
            returnVal.Add(centeredString(maxPaperWidth, DateTime.Now.ToString("MMM d, yyyy - hh:mm tt").ToUpper()));
            return returnVal;
        }

        private string GetRunnerName(RunnersDataAccess dataAccess, int id)
        {
            return dataAccess.SelectRecord(id)[0].FullName;
        }
        private string GetFormattedDateTimeString(string _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("MMM d, yyyy - hh:mm tt").ToUpper();
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