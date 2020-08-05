﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;
using Android.Support.V7.Widget;
using Product = POS_ANDROID_BACUNA.Data_Classes.Product;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class CheckoutFragment : SupportFragment
    {
        TabLayout mTabs;
        Android.Support.V7.Widget.Toolbar toolbar;
        bool mToolbarVisibiltyStatus = true;
        View inactiveSearchView;
        View activeSearchView;
        SearchView searchViewSearchItems;
        View thisFragmentView;
        RecyclerView mRecyclerViewItemsList;
        RecyclerView.LayoutManager mLayoutManager;
        bool mIsGrid = true; //grid view or listview 
        LayoutInflater mLayoutInflater;
        ViewGroup mViewGroup;
        float mDpVal; //screen density
        public Button mBtnCheckoutButton;
        IMenu mCurrentToolBarMenu = null;
        bool mShowSizes = false;
        string mSelectedProductCategory = "All";
        bool isSearchActive = false;
        int thisFragmentViewOriginalHeight;
        LinearLayout mGridHolder;
        ImageView mSearchCancelButton;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mLayoutInflater = inflater;
            mViewGroup = container;
            thisFragmentView = inflater.Inflate(Resource.Layout.checkout_fragment, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            thisFragmentView.Post(() =>
            {
                thisFragmentViewOriginalHeight = thisFragmentView.Height;
            });
            thisFragmentViewOriginalHeight = thisFragmentView.Height;

            mTabs = thisFragmentView.FindViewById<TabLayout>(Resource.Id.tabs);

            FnSetUpCategories();

            toolbar = thisFragmentView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.checkoutSearchToolbar);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;

            //inflate search to toolbar
            inactiveSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_inactive, toolbar, false);
            activeSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_active, toolbar, false);

            toolbar.AddView(inactiveSearchView);

            //search activate
            var btnSearchActivate = inactiveSearchView.FindViewById<LinearLayout>(Resource.Id.btnSearchActivate);
            btnSearchActivate.Click += btnSearchActivate_Click;

            //search deactivate
            mSearchCancelButton = activeSearchView.FindViewById<ImageView>(Resource.Id.imgSearchCancel);
            mSearchCancelButton.Click += SearchCancelButton_Click;
            searchViewSearchItems = activeSearchView.FindViewById<SearchView>(Resource.Id.txtSearchItems);
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(Context, "CheckoutFragment"));
            searchViewSearchItems.QueryTextSubmit += SearchViewSearchItems_QueryTextSubmit;
            searchViewSearchItems.QueryTextChange += SearchViewSearchItems_QueryTextChange;
            //get screendensity
            Context cont = (MainActivity)this.Activity;
            mDpVal = cont.Resources.DisplayMetrics.Density;

            SetGridLayout(inflater,container, mIsGrid);

            mBtnCheckoutButton = thisFragmentView.FindViewById<Button>(Resource.Id.btnCheckoutTotal);
            mBtnCheckoutButton.Click += MBtnCheckoutButton_Click;

            return thisFragmentView;
        }

        private void SearchViewSearchItems_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower().Trim();
            //refresh grid
            mRecyclerViewItemsList.SetAdapter(new CheckoutRecyclerViewAdapter(mDpVal,
                    SetItemListContainerHeight(mGridHolder, toolbar),
                    PopulateProductData2(queryString),
                    PopulateParentProducts(queryString), mIsGrid, mRecyclerViewItemsList, mBtnCheckoutButton,
                    Context, mShowSizes, this, GlobalVariables.mCurrentSelectedPricingType));
            mRecyclerViewItemsList.Invalidate();
        }

        private void SearchViewSearchItems_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            thisFragmentView.RequestFocus(); //hidekeyboard
        }

        public void FnSetUpCategories()
        {
            var productCategories = new List<ProductCategories>();
            productCategories = GlobalVariables.globalProductCategoryList;
            for (int i = 0; i < productCategories.Count; i++)
            {
                mTabs.AddTab(mTabs.NewTab().SetText(productCategories[i].productCategoryName));
            }

            mTabs.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
            {
                thisFragmentView.RequestFocus();//to hide keyboard on search
                var tab = e.Tab;
                mSelectedProductCategory = tab.Text;
                searchViewSearchItems.SetQuery("", false);
                //redraw grid
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();

                Toast.MakeText((MainActivity)this.Activity, "Showed " + tab.Text + " Products",ToastLength.Long).Show();
            };
        }

        private void MBtnCheckoutButton_Click(object sender, EventArgs e)
        {
            //disable if no items on cart
            if (GlobalVariables.globalProductsOnCart.Count == 0)
            {
                DialogMessageService.MessageBox(Context, "Your cart is empty", "Please add some products to the cart first.");
            }
            else
            {
                GlobalVariables.mIsAllCollapsed = true;
                Intent intent = new Intent(Context, typeof(CheckoutFragmentCartActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivityForResult(intent,2);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 2)
            {
                //call method from main activity that will reset selected customer
                ((MainActivity)this.Activity).SetSelectedCustomerIconAppearance();
                //refresh button
                SetCheckoutButtonTotal(mBtnCheckoutButton, Context);
                //refreshGrid
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();
                //refresh main activity toolbar
                ((MainActivity)this.Activity).RefreshMenu();
            }
            else if (requestCode == 8)
            {
                //refreshGrid on callback
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();

                //remove focus on searchbar
                if (!mToolbarVisibiltyStatus)
                {
                    mSearchCancelButton.PerformClick();
                    thisFragmentView.RequestFocus();
                }
            }
        }

        private int SetItemListContainerHeight(LinearLayout layout, Android.Support.V7.Widget.Toolbar layoutBelow)
        {
            var rlCheckoutButtonContainer = thisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlCheckoutButtonContainer);

            int actionBarHeight = dpToPixel(40);
            int tabLayoutHeight = mTabs.Height;
            int searchToolBarHeight = toolbar.Height;
            int checkoutButtonHeight = rlCheckoutButtonContainer.Height;
            int marginOffset = dpToPixel(10);
            int fragmentContainerHeight = thisFragmentViewOriginalHeight;

            int calculatedHeight = fragmentContainerHeight - (actionBarHeight + tabLayoutHeight + searchToolBarHeight + checkoutButtonHeight + marginOffset);
            int _topMargin = 0;
            int _bottomMargin = 0;
            int _leftMargin = dpToPixel(5);
            int _rightMargin = dpToPixel(5);

            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent,calculatedHeight);
            layoutParams.SetMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            layout.LayoutParameters = layoutParams;
            layout.RequestLayout();

            return calculatedHeight;
        }

        private int dpToPixel(int val)
        {
            return val * (int)Math.Ceiling(mDpVal);
        }

        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if(e.Item.ItemId == Resource.Id.barcode)
            {

            }
            else if (e.Item.ItemId == Resource.Id.toggle_grid_appearance)
            {
                 //change icon based 
                if (mIsGrid)
                {
                    mIsGrid = false;
                }
                else
                {
                    mIsGrid = true;
                }
                SetToogleIcon(mIsGrid);
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();
            }
        }

        public void SetToogleIcon(bool isGrid)
        {
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toggle_grid_appearance);
            if (isGrid)
            {
                item.SetIcon(Resource.Drawable.listview_icon);
            }
            else
            {
                item.SetIcon(Resource.Drawable.gridview_icon);
            }
            
        }

        private void SetGridLayout(LayoutInflater inflater, ViewGroup container, bool isGrid)
        {
             mRecyclerViewItemsList = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.recyclerViewItemsList);
            //Create Layout Manager
            if (isGrid == true)
            {
                mLayoutManager = new GridLayoutManager((MainActivity)this.Activity, 3);
            }
            else 
            {
                mLayoutManager = new LinearLayoutManager((MainActivity)this.Activity);
            }
            
            mRecyclerViewItemsList.SetLayoutManager(mLayoutManager); 
            mRecyclerViewItemsList.HasFixedSize = true;

            mGridHolder = thisFragmentView.FindViewById<LinearLayout>(Resource.Id.recyclerViewItemsListHolder);

            //POST RENDERING
            mGridHolder.Post(() => 
            {
                mRecyclerViewItemsList.SetAdapter(new CheckoutRecyclerViewAdapter(mDpVal, 
                    SetItemListContainerHeight(mGridHolder, toolbar), 
                    PopulateProductData2(""),
                    PopulateParentProducts(""),mIsGrid,mRecyclerViewItemsList,mBtnCheckoutButton, 
                    Context, mShowSizes, this, GlobalVariables.mCurrentSelectedPricingType));
            });
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
                btn.Text = itemCount.ToString() + " Item = \u20b1 " + String.Format("{0:n}",totalPrice);
            }
        }

        private List<ParentProducts> PopulateParentProducts(string _queryString) 
        {
            List<ParentProducts> mParentProducts = new List<ParentProducts>();
            mParentProducts = GlobalVariables.globalParentProductList;

            //filter by product category
            if (mSelectedProductCategory != "All")
            {
                if (_queryString != "")
                {
                    mParentProducts = mParentProducts
                    .Where(o => o.categoryName == mSelectedProductCategory)
                    .Where(o => o.parentProductName.ToLower().Contains(_queryString) || o.productAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mParentProducts = mParentProducts
                    .Where(o => o.categoryName == mSelectedProductCategory)
                    .ToList();
                }
            }
            else
            {
                if (_queryString != "")
                {
                    mParentProducts = mParentProducts
                    .Where(o => o.parentProductName.ToLower().Contains(_queryString) || o.productAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mParentProducts = mParentProducts
                    .ToList();
                }
            }
            return mParentProducts;
        }


        private List<Product> PopulateProductData2(string _queryString)
        {
            List<Product> mProducts = GlobalVariables.globalProductList;

            //filter by product category
            if (mSelectedProductCategory != "All")
            {
                if (_queryString != "")
                {
                    mProducts = mProducts
                    .Where(o => o.productCategory == mSelectedProductCategory)
                    .OrderBy(o => o.parentProductId)
                    .ThenBy(o => o.productSizeId)
                    .Where(o => o.productName.ToLower().Contains(_queryString) || o.productAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mProducts = mProducts
                    .Where(o => o.productCategory == mSelectedProductCategory)
                    .OrderBy(o => o.parentProductId)
                    .ThenBy(o => o.productSizeId)
                    .ToList();
                }
                
            }
            else
            {
                if (_queryString != "")
                {
                    mProducts = mProducts
                    .OrderBy(o => o.parentProductId)
                    .ThenBy(o => o.productSizeId)
                    .Where(o => o.productName.ToLower().Contains(_queryString) || o.productAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mProducts = mProducts
                    .OrderBy(o => o.parentProductId)
                    .ThenBy(o => o.productSizeId)
                    .ToList();
                }
            }

            return mProducts;
        }

        private void SearchCancelButton_Click(object sender, EventArgs e)
        {
            searchViewSearchItems.OnActionViewCollapsed();
            thisFragmentView.SetOnTouchListener(null);
            toolbar.RemoveView(activeSearchView);
            toolbar.AddView(inactiveSearchView);
            mToolbarVisibiltyStatus = true;
            //remove menu items and replace with activated searchbar
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
        }

        private void btnSearchActivate_Click(object sender, EventArgs e)
        {
            toolbar.RemoveView(inactiveSearchView);
            toolbar.AddView(activeSearchView);
            searchViewSearchItems.OnActionViewExpanded();
            mToolbarVisibiltyStatus = false;
            //remove menu items and replace with activated searchbar
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
        }

        private void SetSearchActiveGridHeight(LinearLayout _layout,int _gridheight, int _buttonheight,
            Android.Support.V7.Widget.Toolbar layoutBelow)
        {
           
            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, _gridheight - _buttonheight);
            layoutParams.SetMargins(dpToPixel(5), 0, dpToPixel(5),0);
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            _layout.LayoutParameters = layoutParams;
            _layout.RequestLayout();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //clear toolbar menu to avoid duplicate draw
            toolbar.Menu.Clear();
            toolbar.InflateMenu(Resource.Menu.toolbar_menu_checkout_search_items);
            //set visibility of toolbar bacause it is redrawn
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
            //getMenu
            mCurrentToolBarMenu = toolbar.Menu;
            //setMenuIconBasedOnSavedStateOfTheIcon
            SetToogleIcon(mIsGrid);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        { 
            //subscribe click to menu item show sizes
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.showSize);
            TextView showSizeEvent = (TextView)item.ActionView;
            
            showSizeEvent.Click += delegate (object sender, EventArgs e) { ShowSizeEvent_Click(sender, e, showSizeEvent); };
            //set pre saved state of showsizes toggle
            int colorInt = Context.GetColor(mShowSizes ? Resource.Color.colorAccent : Resource.Color.jepoyGray);
            Color textColor = new Color(colorInt);
            showSizeEvent.SetTextColor(textColor);
            showSizeEvent.SetBackgroundResource(mShowSizes ? 0 : Resource.Drawable.strikethrough_line);

            base.OnPrepareOptionsMenu(menu);
        }

        private void ShowSizeEvent_Click(object sender, EventArgs e, TextView txtShowSize)
        {
            if (mShowSizes)
            {
                //refresh grid
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();

                //put strike to text and color to gray
                int colorInt = Context.GetColor(Resource.Color.jepoyGray);
                Color colorGray = new Color(colorInt);
                txtShowSize.SetTextColor(colorGray);
                txtShowSize.SetBackgroundResource(Resource.Drawable.strikethrough_line);
                mShowSizes = false;
            }
            else
            {
                //refresh grid
                SetGridLayout(mLayoutInflater, mViewGroup, mIsGrid);
                mRecyclerViewItemsList.Invalidate();

                //remove strike, change color to accent color
                int colorInt = Context.GetColor(Resource.Color.colorAccent);
                Color colorAccent = new Color(colorInt);
                txtShowSize.SetTextColor(colorAccent);
                txtShowSize.SetBackgroundResource(0);
                mShowSizes = true;
            }
        }

        public void RefreshPricingType(bool _clearCart)
        {
            //clear on cart items if pricing type is changed
            if (_clearCart)
            {
                GlobalVariables.globalProductsOnCart.Clear();
                SetCheckoutButtonTotal(mBtnCheckoutButton, Context);//refresh total button
            }
            //refresh list
            string queryString = "";
            searchViewSearchItems.SetQuery("", false);
            searchViewSearchItems.ClearFocus();
            mRecyclerViewItemsList.SetAdapter(new CheckoutRecyclerViewAdapter(mDpVal,
                        SetItemListContainerHeight(mGridHolder, toolbar),
                        PopulateProductData2(queryString),
                        PopulateParentProducts(queryString), mIsGrid, mRecyclerViewItemsList, mBtnCheckoutButton,
                        Context, mShowSizes, this, GlobalVariables.mCurrentSelectedPricingType));
            mRecyclerViewItemsList.Invalidate();
        }

        //public class ViewClickListener : Java.Lang.Object, View.IOnTouchListener
        //{
        //    private SearchView mEditText;
        //    private View mThisFragmentView;

        //    public ViewClickListener(SearchView edittext, View thisFragmentView)
        //    {
        //        mEditText = edittext;
        //        mThisFragmentView = thisFragmentView;
        //    }

        //    public bool OnTouch(View v, MotionEvent e)
        //    {
        //        mEditText.ClearFocus();
        //        mThisFragmentView.SetOnTouchListener(null);
        //        return true;
        //    }
        //}

        //public class SearchViewFocusListener : Java.Lang.Object, View.IOnFocusChangeListener
        //{
        //    private SearchView mSearchview;
        //    private View mThisFragmentView;

        //    public SearchViewFocusListener(SearchView searchview, View thisview)
        //    {
        //        mSearchview = searchview;
        //        mThisFragmentView = thisview;
        //    }

        //    public void OnFocusChange(View v, bool hasFocus)
        //    {
        //        mThisFragmentView.SetOnTouchListener(new ViewClickListener(mSearchview, mThisFragmentView));
        //    }
        //}

    }


}