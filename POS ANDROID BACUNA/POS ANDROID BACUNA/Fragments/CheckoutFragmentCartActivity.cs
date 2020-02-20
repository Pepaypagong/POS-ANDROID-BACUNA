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
using Android.Graphics;
using POS_ANDROID_BACUNA.Adapters;

namespace POS_ANDROID_BACUNA.Fragments
{
    [Activity(Label = "CheckoutFragmentCartActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CheckoutFragmentCartActivity : AppCompatActivity
    {
        SupportToolbar mToolBar;
        Button mBtnCartTotal;
        ImageButton mBtnClearCart;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.checkout_fragment_cart);
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBarCheckoutCart);
            SetSupportActionBar(mToolBar);
            SupportActionBar actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.left_icon);
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetTitle(Resource.String.checkout_cart_title);

            //get screendensity
            mDpVal = this.Resources.DisplayMetrics.Density;
            lvCartItemList = FindViewById<ExpandableListView>(Resource.Id.lvCheckoutCartItemList);
            rlCheckoutCartButtonContainer = FindViewById<LinearLayout>(Resource.Id.rlCheckoutCartButtonContainer);
            mBtnCartTotal = FindViewById<Button>(Resource.Id.btnCheckoutCartTotal);
            SetCheckoutButtonTotal(mBtnCartTotal,this);
            mBtnClearCart = FindViewById<ImageButton>(Resource.Id.btnCheckoutCartClear);
            mBtnClearCart.Click += MBtnClearCart_Click;

            // Prepare list data
            FnGetListData();

            //Bind list
            listAdapter = new CheckoutCartItemListAdapter(this, mListDataHeader, listDataChild);
            lvCartItemList.SetAdapter(listAdapter);
            FnClickEvents();

            Window.DecorView.Post(() =>
            {
                SetRecyclerViewLayoutHeight(lvCartItemList, mToolBar);
                //scroll to bottom. this causes bugs when item from bottom is clicked when list is long enough to scroll 
                //android:transcriptMode="alwaysScroll" put to expandable listview
                //if (listAdapter.GroupCount > maxNoOfRowsDisplayed())
                //{
                //    lvCartItemList.StackFromBottom = true;
                //}
                //else
                //{
                //    lvCartItemList.StackFromBottom = false;
                //}
                //lvCartItemList.SetSelection(listAdapter.GroupCount - 1); scroll to bottom but only works on click
            });
        }

        public int maxNoOfRowsDisplayed()
        {
            int retVal = 0;
            int listviewHeight = lvCartItemList.Height;
            int listviewRowHeight = dpToPixel(60); //checkout_fragment_cart_list_item_fragment
            retVal = listviewHeight / listviewRowHeight;
            return retVal;
        }

        private void MBtnClearCart_Click(object sender, EventArgs e)
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
                GlobalVariables.globalProductsCart.Clear();
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


            //var xAnonymousGroupData =
            //    from product in mListDataHeader
            //    group product by product.productName into productGroup
            //    select new
            //    {
            //        ProductId = productGroup.Key,
            //        TotalQty = productGroup.Sum(x => x.productId)
            //    };

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

            IMenuItem selectedCustomerItem = menu.FindItem(Resource.Id.menuItem_customer_cart);
            selectedCustomerItem.SetActionView(GetButtonLayout(selectedCustomerItem));
            if (GlobalVariables.mHasSelectedCustomerOnCheckout)
            {
                subscribeCustomLayoutToClick(selectedCustomerItem);
            }
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
                case Resource.Id.menuItem_customer_cart:
                    if (!mDialogShown)
                    {
                        mDialogShown = true;
                        Intent intent = new Intent(this, typeof(CheckoutSelectCustomerActivity));
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
            TextView txtCustomerNameButtonTitle = buttonView.FindViewById<TextView>(Resource.Id.txtCustomerName);
            txtCustomerNameButtonTitle.Text = customerName;
            item.SetActionView(buttonView);
            subscribeCustomLayoutToClick(item);

            //set current selected customer button layout
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = buttonView;
        }

        public void subscribeCustomLayoutToClick(IMenuItem item)
        {
            item.ActionView.Click += (sender, args) => {
                this.OnOptionsItemSelected(item);
            };
        }

        public void SetCheckoutButtonTotal(Button btn, Context context)
        {
            int itemCount = GlobalVariables.globalProductsCart.Count();
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
            OverridePendingTransition(0, 0);
        }
    }
}