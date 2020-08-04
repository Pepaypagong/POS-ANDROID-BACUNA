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
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Android.Views.InputMethods;
using Android.Bluetooth;
using POS_ANDROID_BACUNA.Data_Classes;
using Java.Util;
using Android.Graphics;
using POS_ANDROID_BACUNA.Adapters;
using System.Linq.Dynamic.Core;
using Android.Graphics.Drawables;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentCartActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentCartActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        Button mBtnCartTotal;
        ImageButton mBtnMoreOptions;
        ExpandableListView lvCartItemList;
        LinearLayout rlCheckoutCartButtonContainer;
        float mDpVal;
        bool mDialogShown = false;
        IMenu mCurrentToolbarMenu;

        //expandable listview
        CheckoutCartItemListAdapter listAdapter;
        List<ProductsOnCart> mListDataHeader;
        Dictionary<string, List<string>> listDataChild;
        int previousGroup = -1;

        bool canscroll = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_cart);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarCheckoutCart);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            //actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon_thin);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetDisplayShowHomeEnabled(true);
            actionBar.SetTitle(Resource.String.checkout_cart_title);

            //get screendensity
            mDpVal = this.Resources.DisplayMetrics.Density;
            lvCartItemList = FindViewById<ExpandableListView>(Resource.Id.lvCheckoutCartItemList);
            rlCheckoutCartButtonContainer = FindViewById<LinearLayout>(Resource.Id.rlCheckoutCartButtonContainer);
            mBtnCartTotal = FindViewById<Button>(Resource.Id.btnCheckoutCartTotal);
            mBtnCartTotal.Click += MBtnCartTotal_Click;
            SetCheckoutButtonTotal(mBtnCartTotal, this);
            mBtnMoreOptions = FindViewById<ImageButton>(Resource.Id.btnCheckoutCartClear);
            mBtnMoreOptions.Click += MBtnMoreOptions_Click;

            // Prepare list data
            FnGetListData();
            FnSetUpListView();
            FnClickEvents();

            Window.DecorView.Post(() =>
            {
                SetRecyclerViewLayoutHeight(lvCartItemList, mToolBar);
            });

        }

        public void SetToolBarMenuTextFromDialogFragment(string _selectedPricingType, bool resetSelectedCustomer)
        {
            if (resetSelectedCustomer)
            {
                GlobalVariables.mHasSelectedCustomerOnCheckout = false;
                GlobalVariables.mCurrentSelectedCustomerOnCheckout = "";
                IMenuItem customerMenuItem = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);
                removeActionLayout(customerMenuItem);
            }
            IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_pricingType);
            item.SetTitle(_selectedPricingType);
            SetToolbarMenuIconTint(_selectedPricingType);
        }
        public void SetToolbarMenuIconTint(string _pricingType)
        {
            IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);
            Drawable drawable = item.Icon;
            if (drawable != null)
            {
                drawable.Mutate();
                drawable.SetColorFilter(_pricingType == "RUNR" ?
                    ColorHelper.ResourceIdToColor(Resource.Color.orange, this) :
                    ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this)
                    , PorterDuff.Mode.SrcAtop);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            Window.DecorView.PostDelayed(() =>
            {
                if (canscroll)
                {
                    lvCartItemList.SmoothScrollToPosition(listAdapter.GroupCount);
                }
            }, 100);
        }

        private void MBtnCartTotal_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Confirm Transaction");
            alert.SetMessage("Print receipt and start a new transaction?");

            alert.SetButton2("CANCEL", (c, ev) =>
            {
                //cancel button
            });

            alert.SetButton("YES", (c, ev) =>
            {
                PrintText(GlobalVariables.mSelectedDevice, toPrint("20200201", "BACUNA RTW", "BEELINE BUILDING, BRGY. BACLARAN", "PARANAQUE CITY, PHILIPPINES", "09174897988",
                "JEFFREY BACUNA", GlobalVariables.mCurrentSelectedCustomerOnCheckout,
                "Please come again, thank you :)"));
                GlobalVariables.globalProductsOnCart.Clear();
                GlobalVariables.mIsAllCollapsed = true;
                Finish();
            });

            alert.Show();

        }

        public int maxNoOfRowsDisplayed()
        {
            int retVal = 0;
            int listviewHeight = lvCartItemList.Height;
            int listviewRowHeight = dpToPixel(60); //checkout_fragment_cart_list_item_fragment
            retVal = listviewHeight / listviewRowHeight;
            return retVal;
        }

        private void MBtnMoreOptions_Click(object sender, EventArgs e)
        {
            ShowMoreOptionsDialogFragment();
        }

        private void ShowMoreOptionsDialogFragment()
        {
            //show dialog here
            SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            MoreOptionsDialogFragment moreOptionsDialog = new MoreOptionsDialogFragment(this);
            moreOptionsDialog.SetStyle((int)DialogFragmentStyle.Normal, Resource.Style.Dialog_FullScreen);
            //moreOptionsDialog.SetStyle(Convert.ToInt32(DialogFragmentStyle.Normal), Resource.Style.Dialog_FullScreen);
            //pass current selected price type t
            var args = new Bundle();
            args.PutString("caller", "CheckoutFragmentCartActivity");
            moreOptionsDialog.Arguments = args;
            moreOptionsDialog.Show(transaction, "moreOptionsDialogFragment");
        }

        internal void PricingTypeDialogFragmentOnActivityResult(bool _clearCart)
        {
            mDialogShown = false; //flag to enable dialog show
            if (_clearCart)
            {
                GlobalVariables.globalProductsOnCart.Clear();
                GlobalVariables.mIsAllCollapsed = true;
                Finish();
            }
        }

        public void ShowSortCartItemsBy()
        {
            SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            SortCartItemsDialogFragment dialogFrag = new SortCartItemsDialogFragment(this);
            dialogFrag.Show(transaction, "sortCartItemsDialogFragment");
        }

        public void FnSetUpListView()
        {
            //Bind list
            listAdapter = new CheckoutCartItemListAdapter(this, mListDataHeader, listDataChild);
            lvCartItemList.SetAdapter(listAdapter);
        }
        public void SortCartItems(string _queryString)
        {
            mListDataHeader = GlobalVariables.globalProductsOnCart.AsQueryable()
                .OrderBy(_queryString).ToList();
            FnSetUpListView();
            listAdapter.NotifyDataSetChanged();
            lvCartItemList.SmoothScrollToPosition(listAdapter.GroupCount);
        }

        public void ClearCart()
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle("Clear Cart");
            alert.SetMessage("Do you want to clear the shopping cart?");

            alert.SetButton2("CANCEL", (c, ev) =>
            {
                //cancel button
            });

            alert.SetButton("YES", (c, ev) =>
            {
                GlobalVariables.globalProductsOnCart.Clear();
                GlobalVariables.mIsAllCollapsed = true;
                Finish();
            });

            alert.Show();
        }

        void FnClickEvents()
        {
            //Listening to child item selection
            //lvCartItemList.ChildClick += delegate (object sender, ExpandableListView.ChildClickEventArgs e) {
            //    Toast.MakeText(this, "child clicked", ToastLength.Short).Show();
            //};

            //Listening to group expand
            //modified so that on selection of one group other opened group has been closed
            lvCartItemList.GroupExpand += delegate (object sender, ExpandableListView.GroupExpandEventArgs e) {

                if (e.GroupPosition != previousGroup)
                {
                    lvCartItemList.CollapseGroup(previousGroup);
                }
                previousGroup = e.GroupPosition;

                GlobalVariables.mIsAllCollapsed = false;
            };

            //Listening to group collapse
            lvCartItemList.GroupCollapse += delegate (object sender, ExpandableListView.GroupCollapseEventArgs e) {
                GlobalVariables.mIsAllCollapsed = true;
            };

        }

        private void LvCartItemList_GroupClick(object sender, ExpandableListView.GroupClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        void FnGetListData()
        {
            mListDataHeader = new List<ProductsOnCart>();
            listDataChild = new Dictionary<string, List<string>>();
            mListDataHeader = GlobalVariables.globalProductsOnCart;

            // Adding child data
            var listCommands = new List<string>();
            listCommands.Add("DummyData");

            for (int i = 0; i < mListDataHeader.Count; i++)
            {
                listDataChild.Add(mListDataHeader[i].productId.ToString(), listCommands);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mCurrentToolbarMenu = menu;
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout_cart, menu);
            IMenuItem item = menu.FindItem(Resource.Id.menuItem_pricingType);
            item.SetTitle(GlobalVariables.mCurrentSelectedPricingType);
            IMenuItem selectedCustomerItem = menu.FindItem(Resource.Id.menuItem_customer_cart);
            selectedCustomerItem.SetActionView(GetButtonLayout(selectedCustomerItem));
            if (GlobalVariables.mHasSelectedCustomerOnCheckout)
            {
                //subscribeCustomLayoutToClick(selectedCustomerItem);
                ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
            }
            SetToolbarMenuIconTint(GlobalVariables.mCurrentSelectedPricingType);
            return base.OnCreateOptionsMenu(menu);
        }

        public View GetButtonLayout(IMenuItem _selectedCustomerItem)
        {
            View buttonView = null;

            if (GlobalVariables.mHasSelectedCustomerOnCheckout == true)
            {
                LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
                buttonView = inflater.Inflate(Resource.Layout.checkout_fragment_customer_name_button, null);
                TextView txtCustomerNameButtonTitle = buttonView.FindViewById<TextView>(Resource.Id.txtCustomerName);
                txtCustomerNameButtonTitle.Text = GlobalVariables.mCurrentSelectedCustomerOnCheckout;
            }

            return buttonView;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menuItem_pricingType:
                    if (!mDialogShown) //avoid double click
                    {
                        mDialogShown = true;
                        //show dialog here
                        SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                        PricingTypeDialogFragment pricingTypeDialog = new PricingTypeDialogFragment();
                        //pass current selected price type t
                        var args = new Bundle();
                        args.PutString("currentPricingType", GlobalVariables.mCurrentSelectedPricingType);
                        args.PutString("callerActivity", "CheckoutFragmentCartActivity");
                        pricingTypeDialog.Arguments = args;
                        pricingTypeDialog.Show(transaction, "pricingTypeDialogFragment");
                    }
                    return true;
                case Resource.Id.menuItem_customer_cart:
                    if (!mDialogShown)
                    {
                        mDialogShown = true;
                        Intent intent = new Intent(this, typeof(CheckoutSelectCustomerActivity));
                        intent.PutExtra("isCustomer", GlobalVariables.mCurrentSelectedPricingType == "RUNR" ? false : true);
                        StartActivityForResult(intent, 4);
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 4)
            {
                canscroll = false;
                mDialogShown = false;
                if (GlobalVariables.mHasSelectedCustomerOnCheckout == true)
                {
                    ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
                }
                else
                {
                    IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);
                    removeActionLayout(item);
                }
            }
            else if (requestCode == 5)
            {
                canscroll = false;
                //refresh grid
                listAdapter.NotifyDataSetChanged();
                SetCheckoutButtonTotal(mBtnCartTotal, this);
                if (GlobalVariables.mIsFromRemoveItem)
                {
                    lvCartItemList.CollapseGroup(previousGroup);
                    //go back to checkout page if cart count = 0
                    if (GlobalVariables.globalProductsOnCart.Count == 0)
                    {
                        Finish();
                    }
                }
            }
            else if (requestCode == 6 | requestCode == 7)
            {
                canscroll = false;
                //refresh grid
                listAdapter.NotifyDataSetChanged();
                SetCheckoutButtonTotal(mBtnCartTotal, this);
            }
            else if (requestCode == 20)//add note
            {

            }
        }

        public void removeActionLayout(IMenuItem item)
        {
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = null;
            item.SetActionView(GlobalVariables.mCurrentSelectedCustomerButtonLayout);
        }

        public void ChangeSelectCustomerIcon(string customerName)
        {
            IMenuItem item = mCurrentToolbarMenu.FindItem(Resource.Id.menuItem_customer_cart);

            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            View buttonView = inflater.Inflate(Resource.Layout.checkout_fragment_customer_name_button, null);
            LinearLayout borderContainer = buttonView.FindViewById<LinearLayout>(Resource.Id.llBorderContainer);
            TextView txtCustomerNameButtonTitle = buttonView.FindViewById<TextView>(Resource.Id.txtCustomerName);
            ImageView imgIcon = buttonView.FindViewById<ImageView>(Resource.Id.imgCustomerIcon);
            txtCustomerNameButtonTitle.Text = customerName;

            SetActionLayoutColor(borderContainer, imgIcon, txtCustomerNameButtonTitle);
            item.SetActionView(buttonView);
            subscribeCustomLayoutToClick(item);

            //set current selected customer button layout
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = buttonView;
        }
        public void SetActionLayoutColor(LinearLayout _borderContainer, ImageView _imgIcon, TextView _txtCustomerNameButtonTitle)
        {
            _txtCustomerNameButtonTitle.SetTextColor(GetColorStateList(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                Resource.Color.orange : Resource.Color.colorAccent));
            _imgIcon.SetColorFilter(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                        ColorHelper.ResourceIdToColor(Resource.Color.orange, this) :
                        ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
            _borderContainer.Background = GetDrawable(GlobalVariables.mCurrentSelectedPricingType == "RUNR" ?
                Resource.Drawable.roundborderOrange : Resource.Drawable.roundborder);
        }

        public void subscribeCustomLayoutToClick(IMenuItem item)
        {
            item.ActionView.Click += (sender, args) => {
                this.OnOptionsItemSelected(item);
            };
        }

        public void SetCheckoutButtonTotal(Button btn, Context context)
        {
            int itemCount = GlobalVariables.globalProductsOnCart.Sum(item => item.productCountOnCart);

            decimal totalPrice = 0;

            if (itemCount == 0)
            {
                int colorInt = context.GetColor(Resource.Color.colorAccent);
                Color colorAccent = new Color(colorInt);

                btn.SetTextColor(colorAccent);
                btn.Background = context.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderNoItem);
                btn.Text = "No Item = " + "\u20b1 " + "0.00";
            }
            else
            {
                btn.SetTextColor(Android.Graphics.Color.White);
                btn.Background = context.GetDrawable(Resource.Drawable.buttonCheckoutRoundBorderWithItem);
                foreach (var item in GlobalVariables.globalProductsOnCart)
                {
                    totalPrice += item.productSubTotalPrice;
                }
                btn.Text = itemCount.ToString() + " Item = \u20b1 " + String.Format("{0:n}", totalPrice);
            }
        }

        private void SetRecyclerViewLayoutHeight(ExpandableListView layout, Android.Support.V7.Widget.Toolbar layoutBelow)
        {

            int searchToolBarHeight = mToolBar.Height;
            int checkoutButtonHeight = rlCheckoutCartButtonContainer.Height;
            int recyclerViewHeight = layout.Height;

            int calculatedHeight = recyclerViewHeight - checkoutButtonHeight;

            int _topMargin = 0;
            int _bottomMargin = 0;
            int _leftMargin = 0;//dpToPixel(5);
            int _rightMargin = 0;//dpToPixel(5);

            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, calculatedHeight);
            layoutParams.SetMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            layout.LayoutParameters = layoutParams;
            layout.RequestLayout();
        }

        private int dpToPixel(int val)
        {
            return val * (int)Math.Ceiling(mDpVal);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(0, 0);//removeanimation
        }

        //PRINT FUNCTIONS
        public List<string> toPrint(string _transNo, string _storeName, string _storeAddress1,
            string _storeAddress2, string _storeContactNo, string _cashierName, string _customerName, string footerNote)
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

            List<ProductsOnCart> cartItems = GlobalVariables.globalProductsOnCart;
            decimal totalPrice = 0;

            //get items from cart
            foreach (var item in cartItems)
            {
                int itemQty = item.productCountOnCart;
                string size = item.productSize;
                totalPrice += itemQty * item.productPrice;

                returnVal.Add(item.productName);//returnVal.Add("("+size+") " + item.productName);
                returnVal.Add(quantityAndPriceLine(itemQty, item.productPrice, maxPaperWidth));
                if (cartItems.IndexOf(item) < cartItems.Count - 1)//last item
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