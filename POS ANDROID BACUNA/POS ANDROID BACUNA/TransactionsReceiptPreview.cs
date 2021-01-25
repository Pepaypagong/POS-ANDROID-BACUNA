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
using POS_ANDROID_BACUNA.Fragments;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.SQLite;
using Java.Util;
using Android.Graphics;
using System.Threading;
using Android.Graphics.Drawables;
using System.Runtime.Hosting;
using Newtonsoft.Json.Linq;
using POS_ANDROID_BACUNA.Adapters;
using Android.Text;
using Android.Text.Style;
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes.Temp;

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "TransactionsReceiptPreview", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TransactionsReceiptPreview : AppCompatActivity
    {
        private string mPesoSign = "\u20b1";
        private Button mBtnApplyFilter;
        private LinearLayout mLlSaveButtonContainer;
        private ScrollView mSvContents;
        private SupportToolbar toolBar;
        private TextView mTxtCompanyName;
        private TextView mTxtAddressLine1;
        private TextView mTxtAddressLine2;
        private TextView mTxtContactNumber;
        private TextView mTxtTransactionNumber;
        private TextView mTxtTransactionDate;
        private TextView mTxtCashier;
        private TextView mTxtRunnerName;
        private LinearLayout mLltransactionHeader;
        private RecyclerView mRvReceipt;
        private TextView mTxtGrandTotal;
        private TextView mTxtCashPaid, mLblCash;
        private TextView mTxtCheckPaid, mLblCheck;
        private TextView mTxtChange, mLblChange;
        private TextView mTxtFooterLine1;
        private TextView mTxtDatePrinted;
        private TextView mTxtPdf, mTxtPrint, mTxtEmail, mTxtShare;
        private ImageView mImgPdf, mImgPrint, mImgEmail, mImgShare;
        private SettingsModel mReceiptSettings;
        private SettingsDataAccess mSettingsDataAccess;
        private TransactionsDataAccess mTransactionsDataAccess;
        private TransactionItemsDataAccess mTransactionItemsDataAccess;
        private RunnersDataAccess mRunnersDataAccess;
        private CustomersDataAccess mCustomersDataAccess;
        private int mSelectedTransactionId;
        private bool mIsMultipay;
        private bool mIsMultipayPreview;
        private string mMultipayPreviewIdList;
        private TransactionsModel mSelectedTransaction;
        private RunnersMultipayRecordsModel mSelectedMultipayRecord;
        private RunnersMultipayRecordsDataAccess mRunnersMultipayRecordsDataAccess;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.transactions_fragment_receipt_preview);
            FnGetData();
            FnSetUpControls();
            FnSetUpToolbar();
            FnSetUpEvents();
            FnSetUpList();
            Window.DecorView.Post(() =>
            {
                SetScrollViewHeight(mSvContents, toolBar);
            });
        }

        private void FnSetUpList()
        {
            var mLayoutManager = new LinearLayoutManager(this);
            mRvReceipt.SetLayoutManager(mLayoutManager);
            mRvReceipt.HasFixedSize = true;
            if (mIsMultipay)
            { 
                mRvReceipt.SetAdapter(
                                new RunnersMultipayReceiptRecyclerViewAdapter(this,GetMultipayReceiptList(mSelectedTransactionId)));
            }
            else
            {
                mRvReceipt.SetAdapter(
                new TransactionItemsRecyclerViewAdapter(
                    this,
                    this,
                    mTransactionItemsDataAccess.SelectRecord(mSelectedTransactionId), true)
                );
            }
        }

        private List<RunnersMultipayReceiptGroupedByDate> GetMultipayReceiptList(int _selectedRecordId)
        {
            List<RunnersMultipayReceiptGroupedByDate> retval = new List<RunnersMultipayReceiptGroupedByDate>();
            //string idString = mIsMultipayPreview ? mMultipayPreviewIdList : mRunnersMultipayRecordsDataAccess.SelectRecord(_selectedRecordId)[0].TransactionIds; //#10, #11, #12, #13
            string idString = mIsMultipayPreview ? mMultipayPreviewIdList : "#47, #46";
            int[] transactionIds = idString.Replace("#", "").Replace(" ", "").Split(',').Select(int.Parse).ToArray();

            var selectedTransactions = mTransactionsDataAccess.SelectTable().OrderByDescending(x => x.id)
                                                                            .Where(x => transactionIds.Contains(x.id)).ToList();
            foreach (var item in selectedTransactions)
            {
                retval.Add(new RunnersMultipayReceiptGroupedByDate()
                {
                    Id = item.id,
                    TransactionDateTime = item.TransactionDateTime,
                    RunnerId = item.CustomerOrRunnerId,
                    SubTotalAmount = item.SubTotalAmount,
                    DiscountAmount = item.DiscountAmount,
                    CashierName = item.CashierName,
                    ViewType = 0
                });

                List<TransactionItemsModel> transactionItems = mTransactionItemsDataAccess.SelectRecord(item.id);
                foreach (var item2 in transactionItems)
                {
                    retval.Add(new RunnersMultipayReceiptGroupedByDate()
                    {
                        ProductId = item2.ProductId,
                        ProductName = item2.ProductName,
                        ProductSize = item2.ProductSize,
                        ProductPrice = item2.ProductPrice,
                        ProductOrigPrice = item2.ProductOrigPrice,
                        ProductDiscountAmount = item2.ProductDiscountAmount,
                        ProductDiscountPercentage = item2.ProductDiscountPercentage,
                        ProductSubTotalPrice = item2.ProductSubTotalPrice,
                        ProductCountOnCart = item2.ProductCountOnCart,
                        ViewType = 1
                    });
                }

                retval.Add(new RunnersMultipayReceiptGroupedByDate()
                {
                    FooterTotalAmount = item.SubTotalAmount,
                    ViewType = 2
                });
            }

            return retval;
        }

        private void SetScrollViewHeight(View contents, View layoutBelow)
        {
            int checkoutButtonHeight = mLlSaveButtonContainer.Height;
            int recyclerViewHeight = contents.Height;

            int _topMargin = 0;
            int _bottomMargin = 0;
            int _leftMargin = 0;
            int _rightMargin = 0;

            int calculatedHeight = recyclerViewHeight - checkoutButtonHeight;
            calculatedHeight = calculatedHeight - _bottomMargin;

            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, calculatedHeight);
            layoutParams.SetMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            contents.LayoutParameters = layoutParams;
            contents.RequestLayout();
        }

        private void FnGetData()
        {
            mSelectedTransactionId = Intent.GetIntExtra("selectedTransactionId", 0);
            mIsMultipay = Intent.GetBooleanExtra("isMultipay", false);
            mIsMultipayPreview = Intent.GetBooleanExtra("isMultipayPreview",false);
            mMultipayPreviewIdList = Intent.GetStringExtra("multipayPreviewIdList");
            mSettingsDataAccess = new SettingsDataAccess();
            mTransactionsDataAccess = new TransactionsDataAccess();
            mTransactionItemsDataAccess = new TransactionItemsDataAccess();
            mRunnersDataAccess = new RunnersDataAccess();
            mCustomersDataAccess = new CustomersDataAccess();
            mRunnersMultipayRecordsDataAccess = new RunnersMultipayRecordsDataAccess();
            mReceiptSettings = mSettingsDataAccess.SelectTable()[0];
            mSelectedTransaction = mIsMultipay ? null : mTransactionsDataAccess.SelectRecord(mSelectedTransactionId)[0];
            //mSelectedMultipayRecord = mIsMultipay ? mRunnersMultipayRecordsDataAccess.SelectRecord(mSelectedTransactionId)[0] : null;
            mSelectedMultipayRecord = new RunnersMultipayRecordsModel() {
                Id = 1,
                CashierName = "Jepoy",
                TransactionDateTime = "2020-12-14 6:45",
                RunnerId = 1,
                SubTotalAmount = 45884,
                DiscountAmount = 0,
                PaymentCashAmount = 45884,
                PaymentCheckAmount = 0,
                TransactionIds = "#47, #46",
                StartDate = "2020-12-11 6:45",
                EndDate = "2020-12-14 12:45"
            };
        }

        private void FnSetUpEvents()
        {
            mImgPdf.Click += MImgPdf_Click;
            mImgEmail.Click += MImgEmail_Click;
            mImgPrint.Click += MImgPrint_Click;
            mImgShare.Click += MImgShare_Click;
            mTxtPdf.Click += MImgPdf_Click;
            mTxtEmail.Click += MImgEmail_Click;
            mTxtPrint.Click += MImgPrint_Click;
            mTxtShare.Click += MImgShare_Click;
        }

        private void MImgShare_Click(object sender, EventArgs e)
        {
            
        }

        private void MImgPrint_Click(object sender, EventArgs e)
        {
            using (var mBluetoothAdapter = BluetoothAdapter.DefaultAdapter)
            {
                if (mBluetoothAdapter == null)
                {
                    DialogMessageService.MessageBox(this, "Attention", "This device does not support bluetooth");
                }
                else if (mBluetoothAdapter.IsEnabled)
                {
                    BluetoothDevice printer = mBluetoothAdapter.GetRemoteDevice(mReceiptSettings.ReceiptPrinterAddress);
                    PrintReceiptService printReceiptService = new PrintReceiptService();
                    if (mIsMultipay)
                    {
                        string idString = "#47, #46";
                        int[] transactionIds = idString.Replace("#", "").Replace(" ", "").Split(',').Select(int.Parse).ToArray();
                        var receiptList = printReceiptService.GetMultipayStringPrintList(
                            mReceiptSettings.ReceiptCompanyName,
                            mReceiptSettings.ReceiptAddressLine1,
                            mReceiptSettings.ReceiptAddressLine2,
                            mReceiptSettings.ReceiptContactNumber,
                            mReceiptSettings.ReceiptFooterNote,
                            mTransactionItemsDataAccess.SelectTable()
                                                       .OrderBy(x => x.Id).Where(x => transactionIds.Contains(x.TransactionId)).ToList(),
                            mTransactionsDataAccess.SelectTable()
                                                       .OrderByDescending(x => x.id).Where(x => transactionIds.Contains(x.id)).ToList(),
                            mSelectedMultipayRecord,
                            mRunnersDataAccess);
                        printReceiptService.PrintText(printer, receiptList);
                    }
                    else
                    {
                        printReceiptService.PrintText(printer,
                        printReceiptService.toPrint(
                            mSelectedTransaction.id.ToString(),
                            GetFormattedDateTimeString(mSelectedTransaction.TransactionDateTime),
                            mReceiptSettings.ReceiptCompanyName,
                            mReceiptSettings.ReceiptAddressLine1,
                            mReceiptSettings.ReceiptAddressLine2,
                            mReceiptSettings.ReceiptContactNumber,
                            mSelectedTransaction.CashierName,
                            mSelectedTransaction.TransactionType == "PAYLATER" ? "Runner:" : "Customer:",
                            GetCustomerOrRunnerName(mSelectedTransaction.TransactionType == "PAYLATER", mSelectedTransaction.CustomerOrRunnerId),
                            mReceiptSettings.ReceiptFooterNote,
                            mTransactionItemsDataAccess.SelectRecord(mSelectedTransactionId))
                            );
                    }
                }
                else
                {
                    DialogMessageService.MessageBox(this, "Attention", "Please turn on your Bluetooth to connect the printer to your phone.");
                }
            }
        }

        private void MImgEmail_Click(object sender, EventArgs e)
        {
            
        }

        private void MImgPdf_Click(object sender, EventArgs e)
        {
            
        }

        private void FnSetUpToolbar()
        {
            SetSupportActionBar(toolBar);
            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.x_icon);
            ab.SetDisplayShowHomeEnabled(true);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.Title = "";
        }
         
        private void FnSetUpControls()
        {
            toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbarTitle);
            mSvContents = FindViewById<ScrollView>(Resource.Id.svContents);
            mLlSaveButtonContainer = FindViewById<LinearLayout>(Resource.Id.llReceiptOptions);
            mBtnApplyFilter = FindViewById<Button>(Resource.Id.btnSave);
            mTxtCompanyName = FindViewById<TextView>(Resource.Id.txtCompanyName);
            mTxtAddressLine1 = FindViewById<TextView>(Resource.Id.txtAddressLine1);
            mTxtAddressLine2 = FindViewById<TextView>(Resource.Id.txtAddressLine2);
            mTxtContactNumber = FindViewById<TextView>(Resource.Id.txtContactNumber);
            mTxtTransactionNumber = FindViewById<TextView>(Resource.Id.txtTransactionNumber);
            mTxtTransactionDate = FindViewById<TextView>(Resource.Id.txtDate);
            mTxtCashier = FindViewById<TextView>(Resource.Id.txtCashier);
            mTxtRunnerName = FindViewById<TextView>(Resource.Id.txtRunnerName);
            mLltransactionHeader = FindViewById<LinearLayout>(Resource.Id.lltransactionHeader);
            mRvReceipt = FindViewById<RecyclerView>(Resource.Id.rvReceipt);
            mTxtGrandTotal = FindViewById<TextView>(Resource.Id.txtGrandTotal);
            mTxtCashPaid = FindViewById<TextView>(Resource.Id.txtCash);
            mTxtCheckPaid = FindViewById<TextView>(Resource.Id.txtCheck);
            mTxtChange = FindViewById<TextView>(Resource.Id.txtChange);
            mLblCash = FindViewById<TextView>(Resource.Id.lblCash);
            mLblCheck = FindViewById<TextView>(Resource.Id.lblCheck);
            mLblChange = FindViewById<TextView>(Resource.Id.lblChange);
            mTxtFooterLine1 = FindViewById<TextView>(Resource.Id.txtFooterLine1);
            mTxtDatePrinted = FindViewById<TextView>(Resource.Id.txtDatePrinted);
            mTxtPdf = FindViewById<TextView>(Resource.Id.txtPdf);
            mTxtPrint = FindViewById<TextView>(Resource.Id.txtPrint);
            mTxtEmail = FindViewById<TextView>(Resource.Id.txtEmail);
            mTxtShare = FindViewById<TextView>(Resource.Id.txtShare);
            mImgPdf = FindViewById<ImageView>(Resource.Id.imgPdf);
            mImgPrint = FindViewById<ImageView>(Resource.Id.imgPrint);
            mImgEmail = FindViewById<ImageView>(Resource.Id.imgEmail);
            mImgShare = FindViewById<ImageView>(Resource.Id.imgShare);
            mRvReceipt.NestedScrollingEnabled = false;
            mLltransactionHeader.Visibility = mIsMultipay ? ViewStates.Gone : ViewStates.Visible;
            ShowData();
        }

        private void ShowData()
        {  
            mTxtCompanyName.Text = mReceiptSettings.ReceiptCompanyName.ToUpper();
            mTxtAddressLine1.Text = mReceiptSettings.ReceiptAddressLine1;
            mTxtAddressLine2.Text = mReceiptSettings.ReceiptAddressLine2; 
            mTxtContactNumber.Text = mReceiptSettings.ReceiptContactNumber;
            mTxtFooterLine1.Text = mReceiptSettings.ReceiptFooterNote;
            mTxtDatePrinted.Text = DateTime.Now.ToString("MMM d, yyyy - hh:mm tt").ToUpper();
            if (!mIsMultipay)
            {
                SetSpannableString(mTxtTransactionNumber, "Transaction #: ", mSelectedTransaction.id.ToString());
                SetSpannableString(mTxtTransactionDate, "Date: ", GetFormattedDateTimeString(mSelectedTransaction.TransactionDateTime));
                SetSpannableString(mTxtCashier, "Cashier: ", mSelectedTransaction.CashierName.ToString());
                SetSpannableString(mTxtRunnerName, mSelectedTransaction.TransactionType == "PAYLATER" ? "Runner: " : "Customer: ",
                                                   GetCustomerOrRunnerName(mSelectedTransaction.TransactionType == "PAYLATER",
                                                   mSelectedTransaction.CustomerOrRunnerId));
            }
            mTxtGrandTotal.Text = mPesoSign + string.Format("{0:n}", mIsMultipay ? 
                                  mSelectedMultipayRecord.SubTotalAmount : mSelectedTransaction.SubTotalAmount);
            mTxtCashPaid.Text = mPesoSign + string.Format("{0:n}", mIsMultipay ?
                                  mSelectedMultipayRecord.PaymentCashAmount : mSelectedTransaction.PaymentCashAmount);
            mTxtCheckPaid.Text = mPesoSign + string.Format("{0:n}", mIsMultipay ?
                                  mSelectedMultipayRecord.PaymentCheckAmount : mSelectedTransaction.PaymentCheckAmount);

            decimal amountPaid = mIsMultipay ? mSelectedMultipayRecord.PaymentCashAmount + mSelectedMultipayRecord.PaymentCheckAmount : 
                                 mSelectedTransaction.PaymentCashAmount + mSelectedTransaction.PaymentCheckAmount;
            if (amountPaid > (mIsMultipay ? mSelectedMultipayRecord.SubTotalAmount : mSelectedTransaction.SubTotalAmount))
            {
                mTxtChange.Text = mPesoSign + 
                    string.Format("{0:n}", amountPaid - (mIsMultipay ? mSelectedMultipayRecord.SubTotalAmount : mSelectedTransaction.SubTotalAmount));
            }
            else
            {
                mTxtChange.Text = mPesoSign + "0.00";
            }
            if (mIsMultipay)
            {
                SetFooterVisibilityMultipay();
            }
            else
            {
                SetFooterVisibility(mSelectedTransaction.TransactionType, mSelectedTransaction.IsPaid, amountPaid);
            }
        }

        private void SetFooterVisibilityMultipay()
        {
            mLblCash.Visibility = mSelectedMultipayRecord.PaymentCashAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mTxtCashPaid.Visibility = mSelectedMultipayRecord.PaymentCashAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mLblCheck.Visibility = mSelectedMultipayRecord.PaymentCheckAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mTxtCheckPaid.Visibility = mSelectedMultipayRecord.PaymentCheckAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
        }

        private void SetFooterVisibility(string _transactionType, bool _isPaylaterPaid, decimal _amountPaid)
        {
            mLblCash.Visibility = mSelectedTransaction.PaymentCashAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mTxtCashPaid.Visibility = mSelectedTransaction.PaymentCashAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mLblCheck.Visibility = mSelectedTransaction.PaymentCheckAmount > 0 ? ViewStates.Visible : ViewStates.Gone;
            mTxtCheckPaid.Visibility = mSelectedTransaction.PaymentCheckAmount > 0 ? ViewStates.Visible : ViewStates.Gone;

            if (_transactionType == "PAYLATER" || _transactionType == "ORDER")
            {
                if (!_isPaylaterPaid || _transactionType == "ORDER")
                {
                    mLblCash.Visibility = ViewStates.Gone;
                    mTxtCashPaid.Visibility = ViewStates.Gone;
                    mLblCheck.Visibility = ViewStates.Gone;
                    mTxtCheckPaid.Visibility = ViewStates.Gone;
                    mLblChange.Visibility = ViewStates.Gone;
                    mTxtChange.Visibility = ViewStates.Gone;
                }
            }
        }

        private void SetSpannableString(TextView _textview, string _label, string _boldText)
        {
            SpannableStringBuilder str = new SpannableStringBuilder(_label + _boldText);
            str.SetSpan(new StyleSpan(TypefaceStyle.Bold),_label.Length,_label.Length+_boldText.Length,SpanTypes.ExclusiveExclusive);
            _textview.TextFormatted = str;
        }

        private string GetFormattedDateTimeString(string _datetime)
        {
            return Convert.ToDateTime(_datetime).ToString("MMM d, yyyy - hh:mm tt").ToUpper();
        }

        private string GetCustomerOrRunnerName(bool isRunner, int id)
        {
            if (isRunner)
            {
                return mRunnersDataAccess.SelectRecord(id)[0].FullName;
            }
            else
            {

               return mCustomersDataAccess.SelectRecord(id)[0].FullName;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.toolbar_menu_transactions_filter, menu);
            return base.OnCreateOptionsMenu(menu);
        }

    }
}