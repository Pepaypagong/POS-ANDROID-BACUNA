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
using ProductsModel = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;
using static POS_ANDROID_BACUNA.Fragments.CheckoutFragment;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class ProductsFragmentItemsFragment : SupportFragment
    {
        View thisFragmentView;
        TabLayout mTabs;

        //data
        ProductsDataAccess mProductsDataAccess;
        ParentProductsDataAccess mParentProductsDataAccess;

        //toolbar
        Android.Support.V7.Widget.Toolbar toolbar;
        bool mToolbarVisibiltyStatus = true;
        View inactiveSearchView;
        View activeSearchView;
        ImageView searchCancelButton;
        SearchView searchViewSearchItems;

        //listview 
        string mSelectedProductCategory = "All";
        ExpandableListView lvCartItemList;
        ProductsItemListAdapter mListAdapter;
        List<ParentProductsModel> mListDataHeader;
        Dictionary<int, List<ProductsModel>> mListDataChild;
        List<ProductsModel> mListChildProducts;
        int previousGroup = -1;

        static bool mDialogShown = false;  // flag for stopping double click

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.products_fragment_items_fragment, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            FnSetUpData();
            FnSetUpControls(inflater);
            FnSetUpSearchToolbar(inflater);
            FnSetUpListView("");
            FnClickEvents();
            return thisFragmentView;
        }

        private void FnSetUpData()
        {
            mProductsDataAccess = new ProductsDataAccess();
            mParentProductsDataAccess = new ParentProductsDataAccess();
            mProductsDataAccess.CreateTable();
            mParentProductsDataAccess.CreateTable();
        }

        private void FnSetUpControls(LayoutInflater inflater)
        {
            toolbar = thisFragmentView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarSearchProducts);
            //inflate search to toolbar
            inactiveSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_inactive, toolbar, false);
            activeSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_active, toolbar, false);
            lvCartItemList = thisFragmentView.FindViewById<ExpandableListView>(Resource.Id.lvProductList);
        }

        private void FnSetUpListView(string _queryString)
        {            
            FnGetListData(_queryString);
            //Bind list
            mListAdapter = new ProductsItemListAdapter(this, mListDataHeader, mListDataChild, lvCartItemList);
            lvCartItemList.SetAdapter(mListAdapter);
        }
          
        void FnClickEvents()
        {
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            //Listening to child item selection
            lvCartItemList.ChildClick += delegate (object sender, ExpandableListView.ChildClickEventArgs e) {
                searchViewSearchItems.ClearFocus();
                Toast.MakeText((MainActivity)this.Activity, "child clicked", ToastLength.Short).Show();
            };

            //Listening to group expand
            //modified so that on selection of one group other opened group has been closed
            lvCartItemList.GroupExpand += delegate (object sender, ExpandableListView.GroupExpandEventArgs e) {
                searchViewSearchItems.ClearFocus();
                if (e.GroupPosition != previousGroup)
                {
                    //lvCartItemList.CollapseGroup(previousGroup);
                    //previousGroup = e.GroupPosition;
                }
            };

            //Listening to group collapse
            lvCartItemList.GroupCollapse += delegate (object sender, ExpandableListView.GroupCollapseEventArgs e) {
                searchViewSearchItems.ClearFocus();
                Toast.MakeText((MainActivity)this.Activity, "group collapsed", ToastLength.Short).Show();
            };

        }

        void FnGetListData(string _queryString)
        {
            mListDataHeader = new List<ParentProductsModel>();
            mListChildProducts = new List<ProductsModel>();
            mListDataChild = new Dictionary<int, List<ProductsModel>>();

            // Adding child data
            mListDataHeader = mParentProductsDataAccess.SelectTable();

            if (mSelectedProductCategory != "All")
            {
                if (_queryString != "")
                {
                    mListDataHeader = mListDataHeader
                    .Where(o => o.CategoryName == mSelectedProductCategory)
                    .Where(o => o.ParentProductName.ToLower().Contains(_queryString) || o.ProductAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mListDataHeader = mListDataHeader
                    .Where(o => o.CategoryName == mSelectedProductCategory)
                    .ToList();
                }
            }
            else
            {
                if (_queryString != "")
                {
                    mListDataHeader = mListDataHeader
                    .Where(o => o.ParentProductName.ToLower().Contains(_queryString) || o.ProductAlias.ToLower().Contains(_queryString))
                    .ToList();
                }
                else
                {
                    mListDataHeader = mListDataHeader
                    .ToList();
                }
            }

            mListChildProducts = mProductsDataAccess.SelectTable();

            for (int i = 0; i < mListDataHeader.Count; i++)
            {
                var childProductList = mListChildProducts.Where(o => o.ParentProductId == mListDataHeader[i].Id).ToList();
                mListDataChild.Add(mListDataHeader[i].Id, childProductList.OrderBy(x => x.ProductSizeId).ToList());
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //clear toolbar menu to avoid duplicate draw
            toolbar.Menu.Clear();
            toolbar.InflateMenu(Resource.Menu.toolbar_menu_products_items_search_items);
            SetSelectedToolbarMenuCategory(1, mSelectedProductCategory);
            //set visibility of toolbar bacause it is redrawn
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
            //make searchview inactive
            deactivateSearch();
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public void deactivateSearch()
        {
            if (!mToolbarVisibiltyStatus)
            {
                toolbar.RemoveView(inactiveSearchView);
                searchCancelButton.PerformClick();
            }
        }

        private void FnSetUpSearchToolbar(LayoutInflater inflater)
        {
            toolbar.AddView(inactiveSearchView);

            //search activate
            var btnSearchActivate = inactiveSearchView.FindViewById<LinearLayout>(Resource.Id.btnSearchActivate);
            btnSearchActivate.Click += btnSearchActivate_Click;

            //search deactivate
            searchCancelButton = activeSearchView.FindViewById<ImageView>(Resource.Id.imgSearchCancel);
            searchCancelButton.Click += SearchCancelButton_Click;

            searchViewSearchItems = activeSearchView.FindViewById<SearchView>(Resource.Id.txtSearchItems);
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(this.Context, "ProductsFragment"));
            searchViewSearchItems.QueryTextChange += SearchViewSearchItems_QueryTextChange;
        }

        private void SearchViewSearchItems_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower();
            FnSetUpListView(queryString);
            lvCartItemList.Invalidate();
        }

        private void SearchCancelButton_Click(object sender, EventArgs e)
        {
            searchViewSearchItems.OnActionViewCollapsed();
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
            mToolbarVisibiltyStatus = false;
            //remove menu items and replace with activated searchbar
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
            Toast.MakeText((MainActivity)this.Activity, "Search Activated", ToastLength.Long).Show();
        }

        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.add_new_item)
            {
                Toast.MakeText((MainActivity)this.Activity, "Pressed Add New Item", ToastLength.Long).Show();
                if (!mDialogShown)
                {
                    mDialogShown = true;
                    Intent intent = new Intent(Context, typeof(ProductsFragmentItemsAddItemActivity));
                    StartActivityForResult(intent, 9);
                }
            }
            else if (e.Item.ItemId == Resource.Id.menuItem_category)
            {
                if (!mDialogShown)
                {
                    mDialogShown = true;
                    IMenuItem item = toolbar.Menu.FindItem(Resource.Id.menuItem_category);
                    Intent intent = new Intent(Context, typeof(ProductsFragmentItemsSelectCategoryActivity));
                    //ActivityOptions options = ActivityOptions.MakeCustomAnimation(Context, 0, 0);
                    intent.PutExtra("caller", "ProductsFragmentItemsSelectCategoryActivity");
                    intent.PutExtra("productCatNameInit", item.TitleFormatted);
                    //StartActivityForResult(intent, 15, options.ToBundle());
                    StartActivityForResult(intent, 15);
                }
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 9)
            {
                mDialogShown = false;
                FnSetUpListView("");
                lvCartItemList.Invalidate();
                //refresh count on main activty toolbar title
                ((MainActivity)this.Activity).SetProductCountTitle();
            }
            else if (requestCode == 15)
            {
                mDialogShown = false;
                SetSelectedToolbarMenuCategory(data.GetIntExtra("productCategoryId", 0), data.GetStringExtra("productCategoryName"));
            }
            else if (requestCode == 16)
            {
                FnSetUpListView("");
                lvCartItemList.Invalidate();
                //refresh count on main activty toolbar title
                ((MainActivity)this.Activity).SetProductCountTitle();
                //clear search
                searchViewSearchItems.SetQuery("", false);
            }
        }

        private void SetSelectedToolbarMenuCategory(int _categoryId, string _categoryName)
        {
            IMenuItem item = toolbar.Menu.FindItem(Resource.Id.menuItem_category);
            item.SetTitle(_categoryName);
            mSelectedProductCategory = _categoryName;
            FnSetUpListView("");
            lvCartItemList.Invalidate();
        }

        public override void OnPause()
        {
            base.OnPause();
        }
        //private void FnSetUpCategories()
        //{
        //    var productCategories = GlobalVariables.globalProductCategoryList;

        //    for (int i = 0; i < productCategories.Count; i++)
        //    {
        //        mTabs.AddTab(mTabs.NewTab().SetText(productCategories[i].productCategoryName));
        //    }

        //    mTabs.TabSelected += (object sender, TabLayout.TabSelectedEventArgs e) =>
        //    {
        //        var tab = e.Tab;
        //        mSelectedProductCategory = tab.Text;
        //        FnSetUpListView();
        //        lvCartItemList.Invalidate();
        //        //Toast.MakeText((MainActivity)this.Activity, "Showed " + tab.Text + " Products", ToastLength.Long).Show();
        //    };
        //}
    }
}