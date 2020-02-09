using System;
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
        Button mBtnCheckoutButton;
        IMenu mCurrentToolBarMenu = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            mLayoutInflater = inflater;
            mViewGroup = container;
            thisFragmentView = inflater.Inflate(Resource.Layout.checkout_fragment, container, false);
            mTabs = thisFragmentView.FindViewById<TabLayout>(Resource.Id.tabs);
            string[] categories = { "All", "Polo", "Blouse", "Pants", "Shorts", "Palda", "Caps", "Panyo" };
            onCreateTabLayout(categories); //add tabs and texts

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
            var searchCancelButton = activeSearchView.FindViewById<ImageView>(Resource.Id.imgSearchCancel);
            searchCancelButton.Click += SearchCancelButton_Click;
            searchViewSearchItems = activeSearchView.FindViewById<SearchView>(Resource.Id.txtSearchItems);
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(searchViewSearchItems, thisFragmentView));

            //get screendensity
            Context cont = (MainActivity)this.Activity;
            mDpVal = cont.Resources.DisplayMetrics.Density;

            SetGridLayout(inflater,container, mIsGrid);

            mBtnCheckoutButton = thisFragmentView.FindViewById<Button>(Resource.Id.btnCheckoutTotal);

            return thisFragmentView;
        }

        private int SetItemListContainerHeight(LinearLayout layout, Android.Support.V7.Widget.Toolbar layoutBelow)
        {
            var rlCheckoutButtonContainer = thisFragmentView.FindViewById<RelativeLayout>(Resource.Id.rlCheckoutButtonContainer);

            int actionBarHeight = dpToPixel(40);
            int tabLayoutHeight = mTabs.Height;
            int searchToolBarHeight = toolbar.Height;
            int checkoutButtonHeight = rlCheckoutButtonContainer.Height;
            int marginOffset = dpToPixel(10);
            int fragmentContainerHeight = thisFragmentView.Height;

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

            if (e.Item.ItemId == Resource.Id.barcode)
            {
                //clear cart
                GlobalCart.globalProductsCart.Clear();
                SetCheckoutButtonTotal(mBtnCheckoutButton, Context);
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

            var gridHolder = thisFragmentView.FindViewById<LinearLayout>(Resource.Id.recyclerViewItemsListHolder);

            //POST RENDERING
            gridHolder.Post(() => 
            {
                mRecyclerViewItemsList.SetAdapter(new RecyclerViewAdapter(mDpVal, 
                    SetItemListContainerHeight(gridHolder, toolbar), 
                    PopulateProductData(),mIsGrid,mRecyclerViewItemsList,mBtnCheckoutButton, Context));
            });
        }
        
        public void SetCheckoutButtonTotal(Button btn, Context context)
        {
            int itemCount = GlobalCart.globalProductsCart.Count();
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
                foreach (var item in GlobalCart.globalProductsCart)
                {
                    totalPrice += item.productRetailPrice;
                }
                btn.Text = itemCount.ToString() + " Item = \u20b1" + totalPrice.ToString();
            }
        }

        private List<Product> PopulateProductData()
        {
            List<Product> mProducts = new List<Product>();

            int productid = 1;
            int productCount = 20;
            string productName = "PRODUCT ";
            decimal productPrice = Convert.ToDecimal(200.50);
            string productColor = "808080"; //7F00FF violet

            for (int i = 0; i < productCount; i++)
            {
                mProducts.Add(new Product() { 
                    productId = productid,
                    productName = productName + productid.ToString(),
                    productRetailPrice = productPrice + Convert.ToDecimal(productid),
                    productColorBg = productColor
                });
                productid++;
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
            Toast.MakeText((MainActivity)this.Activity, "Search Cancelled", ToastLength.Long).Show();
        }

        private void btnSearchActivate_Click(object sender, EventArgs e)
        {
            toolbar.RemoveView(inactiveSearchView);
            toolbar.AddView(activeSearchView);
            searchViewSearchItems.OnActionViewExpanded();
            thisFragmentView.SetOnTouchListener(new ViewClickListener(searchViewSearchItems, thisFragmentView));
            mToolbarVisibiltyStatus = false;
            //remove menu items and replace with activated searchbar
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
            Toast.MakeText((MainActivity)this.Activity, "Search Activated", ToastLength.Long).Show();
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

        private void onCreateTabLayout(string[] categories)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                mTabs.AddTab(mTabs.NewTab().SetText(categories[i]));
            }

            mTabs.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
            {
                var tab = e.Tab;
                Android.Widget.Toast.MakeText((MainActivity)this.Activity, "Clicked " + tab.Text + " Tab", Android.Widget.ToastLength.Long).Show();
            };
        }

        public class ViewClickListener : Java.Lang.Object, View.IOnTouchListener
        {
            private SearchView mEditText;
            private View mThisFragmentView;

            public ViewClickListener(SearchView edittext, View thisFragmentView)
            {
                mEditText = edittext;
                mThisFragmentView = thisFragmentView;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                mEditText.ClearFocus();
                mThisFragmentView.SetOnTouchListener(null);
                return true;
            }
        }

        public class SearchViewFocusListener : Java.Lang.Object, View.IOnFocusChangeListener
        {
            private SearchView mSearchview;
            private View mThisFragmentView;

            public SearchViewFocusListener(SearchView searchview, View thisview)
            {
                mSearchview = searchview;
                mThisFragmentView = thisview;
            }

            public void OnFocusChange(View v, bool hasFocus)
            {
                mThisFragmentView.SetOnTouchListener(new ViewClickListener(mSearchview, mThisFragmentView));
            }
        }

    }


}