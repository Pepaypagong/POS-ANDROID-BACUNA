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
using POS_ANDROID_BACUNA.Data_Classes;

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
        bool mShowSizes = false;
        string mSelectedProductCategory = "All";

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
            var searchCancelButton = activeSearchView.FindViewById<ImageView>(Resource.Id.imgSearchCancel);
            searchCancelButton.Click += SearchCancelButton_Click;
            searchViewSearchItems = activeSearchView.FindViewById<SearchView>(Resource.Id.txtSearchItems);
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(searchViewSearchItems, thisFragmentView));

            //get screendensity
            Context cont = (MainActivity)this.Activity;
            mDpVal = cont.Resources.DisplayMetrics.Density;

            SetGridLayout(inflater,container, mIsGrid);

            mBtnCheckoutButton = thisFragmentView.FindViewById<Button>(Resource.Id.btnCheckoutTotal);
            mBtnCheckoutButton.Click += MBtnCheckoutButton_Click;

            return thisFragmentView;
        }

        public void FnSetUpCategories()
        {
            List<ProductCategories> productCategories = new List<ProductCategories>();
            productCategories.Add(new ProductCategories() { productCategoryId = 1, productCategoryName = "All" });
            productCategories.Add(new ProductCategories() { productCategoryId = 2, productCategoryName = "Polo" });
            productCategories.Add(new ProductCategories() { productCategoryId = 3, productCategoryName = "Blouse" });
            productCategories.Add(new ProductCategories() { productCategoryId = 4, productCategoryName = "Pants" });
            productCategories.Add(new ProductCategories() { productCategoryId = 5, productCategoryName = "Palda" });
            productCategories.Add(new ProductCategories() { productCategoryId = 6, productCategoryName = "Panyo" });

            for (int i = 0; i < productCategories.Count; i++)
            {
                mTabs.AddTab(mTabs.NewTab().SetText(productCategories[i].productCategoryName));
            }

            mTabs.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
            {
                var tab = e.Tab;

                mSelectedProductCategory = tab.Text;
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

            var gridHolder = thisFragmentView.FindViewById<LinearLayout>(Resource.Id.recyclerViewItemsListHolder);

            //POST RENDERING
            gridHolder.Post(() => 
            {
                mRecyclerViewItemsList.SetAdapter(new CheckoutRecyclerViewAdapter(mDpVal, 
                    SetItemListContainerHeight(gridHolder, toolbar), 
                    PopulateProductData2(mSelectedProductCategory,mShowSizes),PopulateParentProducts(),mIsGrid,mRecyclerViewItemsList,mBtnCheckoutButton, Context, mShowSizes));
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

        private List<ProductSizes> PopulateSizesData()
        {
            List<ProductSizes> mSizes = new List<ProductSizes>();
            mSizes.Add(new ProductSizes() { productSizeId = 1, productSizeName = "0" });
            mSizes.Add(new ProductSizes() { productSizeId = 2, productSizeName = "2" });
            mSizes.Add(new ProductSizes() { productSizeId = 3, productSizeName = "4" });
            mSizes.Add(new ProductSizes() { productSizeId = 4, productSizeName = "6" });
            mSizes.Add(new ProductSizes() { productSizeId = 5, productSizeName = "8" });
            mSizes.Add(new ProductSizes() { productSizeId = 6, productSizeName = "10" });
            mSizes.Add(new ProductSizes() { productSizeId = 7, productSizeName = "12" });
            mSizes.Add(new ProductSizes() { productSizeId = 8, productSizeName = "14" });
            mSizes.Add(new ProductSizes() { productSizeId = 9, productSizeName = "16" });
            mSizes.Add(new ProductSizes() { productSizeId = 10, productSizeName = "18" });
            mSizes.Add(new ProductSizes() { productSizeId = 11, productSizeName = "20" });
            mSizes.Add(new ProductSizes() { productSizeId = 12, productSizeName = "24" });
            mSizes.Add(new ProductSizes() { productSizeId = 13, productSizeName = "S" });
            mSizes.Add(new ProductSizes() { productSizeId = 14, productSizeName = "M" });
            mSizes.Add(new ProductSizes() { productSizeId = 15, productSizeName = "L" });
            mSizes.Add(new ProductSizes() { productSizeId = 16, productSizeName = "XL" });
            mSizes.Add(new ProductSizes() { productSizeId = 17, productSizeName = "2X" });
            mSizes.Add(new ProductSizes() { productSizeId = 18, productSizeName = "3X" });
            return mSizes;
        }

        private List<Product> PopulateProductData()
        {
            List<Product> mProducts = new List<Product>();
            List<ProductSizes> mSizes = PopulateSizesData();

            int productid = 1;
            int productCount = 10;
            string productName = "Product ";
            decimal productPrice = Convert.ToDecimal(100);

            for (int i = 0; i < productCount; i++)
            {
                productName = i == 0 ? "Product " : "Product ";

                for (int x = 0; x < mSizes.Count; x++)
                {
                    string sizeName = "(" + mSizes[x].productSizeName + ") "; //fetch from list of sizes
                    mProducts.Add(new Product()
                    {
                        productId = productid,
                        productName = sizeName + productName + productid.ToString(),
                        productSize = sizeName,
                        productRetailPrice = productPrice + (Convert.ToDecimal(productid) * 2),
                        productColorBg = Guid.NewGuid().ToString().Substring(0, 6) //random color
                    });
                }
                productid++;
            }

            return mProducts;
        }

        private List<ParentProducts> PopulateParentProducts() 
        {
            List<ParentProducts> mParentProducts = new List<ParentProducts>();
            mParentProducts.Add(new ParentProducts() { parentProductId = 1, parentProductName = "Pants Black", categoryName = "Pants" }); //use category id
            mParentProducts.Add(new ParentProducts() { parentProductId = 2, parentProductName = "Polo Jacket Kat", categoryName = "Polo" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 3, parentProductName = "Polo Barong", categoryName = "Polo" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 4, parentProductName = "Baby Collar Kat", categoryName = "Blouse" });
            mParentProducts.Add(new ParentProducts() { parentProductId = 5, parentProductName = "Marine Collar Kat", categoryName = "Blouse" });
            //filter by product category
            if (mSelectedProductCategory != "All")
            {
                mParentProducts = mParentProducts.Where(o => o.categoryName == mSelectedProductCategory).ToList();
            }
            return mParentProducts;
        }


        private List<Product> PopulateProductData2(string _productCategory, bool _showSize)
        {
            List<Product> mProducts = new List<Product>();
            //

            //pants
            mProducts.Add(new Product() { productId = 1, productName = "(24) Pants Black", productCategory = "Pants", productSize = "24", productRetailPrice = 150.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 2, productName = "(S) Pants Black", productCategory = "Pants", productSize = "S", productRetailPrice = 160.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 3, productName = "(M) Pants Black", productCategory = "Pants", productSize = "M", productRetailPrice = 170.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 4, productName = "(L) Pants Black", productCategory = "Pants", productSize = "L", productRetailPrice = 180.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            //polo
            mProducts.Add(new Product() { productId = 5, productName = "(24) Polo Jacket Kat", productCategory = "Polo", productSize = "24", productRetailPrice = 110.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 6, productName = "(S) Polo Jacket Kat", productCategory = "Polo", productSize = "S", productRetailPrice = 120.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 7, productName = "(M) Polo Jacket Kat", productCategory = "Polo", productSize = "M", productRetailPrice = 130.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 8, productName = "(L) Polo Jacket Kat", productCategory = "Polo", productSize = "L", productRetailPrice = 140.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 9, productName = "(24) Polo Barong", productCategory = "Polo", productSize = "24", productRetailPrice = 160.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 10, productName = "(S) Polo Barong", productCategory = "Polo", productSize = "S", productRetailPrice = 170.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 11, productName = "(M) Polo Barong", productCategory = "Polo", productSize = "M", productRetailPrice = 180.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 12, productName = "(L) Polo Barong", productCategory = "Polo", productSize = "L", productRetailPrice = 190.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            //blouse
            mProducts.Add(new Product() { productId = 13, productName = "(S) Baby Collar Kat", productCategory = "Blouse", productSize = "S", productRetailPrice = 120.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 14, productName = "(M) Baby Collar Kat", productCategory = "Blouse", productSize = "M", productRetailPrice = 130.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 15, productName = "(L) Baby Collar Kat", productCategory = "Blouse", productSize = "L", productRetailPrice = 140.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 16, productName = "(XL) Baby Collar Kat", productCategory = "Blouse", productSize = "XL", productRetailPrice = 150.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 17, productName = "(S) Marine Collar Kat", productCategory = "Blouse", productSize = "S", productRetailPrice = 100.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 18, productName = "(M) Marine Collar Kat", productCategory = "Blouse", productSize = "M", productRetailPrice = 110.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 19, productName = "(L) Marine Collar Kat", productCategory = "Blouse", productSize = "L", productRetailPrice = 120.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });
            mProducts.Add(new Product() { productId = 20, productName = "(XL) Marine Collar Kat", productCategory = "Blouse", productSize = "XL", productRetailPrice = 130.00M, productColorBg = Guid.NewGuid().ToString().Substring(0, 6) });

            //filter by product category
            if (mSelectedProductCategory != "All")
            {
                mProducts = mProducts.Where(o => o.productCategory == mSelectedProductCategory).ToList();
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