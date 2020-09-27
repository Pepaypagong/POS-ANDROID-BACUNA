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
using Product = POS_ANDROID_BACUNA.Data_Classes.ProductsModel;
using Android.Graphics;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Adapters;
using static POS_ANDROID_BACUNA.Fragments.CheckoutFragment;
using Android.Support.V7.Widget.Helper;
using POS_ANDROID_BACUNA.Interfaces;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class ProductsFragmentCategoriesFragment : SupportFragment, IOnStartDragListener
    {
        View thisFragmentView;
        //toolbar
        Android.Support.V7.Widget.Toolbar toolbar;
        bool mToolbarVisibiltyStatus = true;
        View inactiveSearchView;
        View activeSearchView;
        ImageView searchCancelButton;
        SearchView searchViewSearchItems;

        private ItemTouchHelper _mItemTouchHelper;
        public static List<ProductCategoriesModel> mCategoriesList;
        private RecyclerView rvCategories;
        private CategoriesDataAccess mCategoriesDataAccess;
        ProductsItemCategoryReorderableAdapter mResourceAdapter;

        static bool mDialogShown = false;  // flag for stopping double click
        private bool disableReordering = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.products_fragment_categories_fragment, container, false);
            thisFragmentView.Clickable = true;
            thisFragmentView.FocusableInTouchMode = true;
            SoftKeyboardHelper.SetUpFocusAndClickUI(thisFragmentView);
            FnSetUpSearchToolbar(inflater);
            FnSetUpData();
            FnSetUpControls();
            FnSetUpList();

            return thisFragmentView;
        }

        private void FnSetUpList()
        {
            FnGetListData("");
            mResourceAdapter = new ProductsItemCategoryReorderableAdapter(mCategoriesList, this, this);
            rvCategories.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false));
            rvCategories.SetAdapter(mResourceAdapter);
            rvCategories.HasFixedSize = true;

            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(mResourceAdapter);
            _mItemTouchHelper = new ItemTouchHelper(callback);
            _mItemTouchHelper.AttachToRecyclerView(rvCategories);
        }

        private void FnGetListData(string queryString)
        {
            mCategoriesList = mCategoriesDataAccess.SelectTable().Where(x => x.Id != 1).ToList();
            //mCategoriesList = mCategoriesDataAccess.SelectTable().ToList();
            mCategoriesList = mCategoriesDataAccess != null ? 
                              mCategoriesList.OrderBy(x => x.Rank)
                                             .Where(x => x.ProductCategoryName.ToUpper().Contains(queryString.ToUpper())).ToList() : 
                              mCategoriesList;
        }

        private void FnSetUpData()
        {
            mCategoriesDataAccess = new CategoriesDataAccess();
        }

        private void FnSetUpControls()
        {
            rvCategories = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.rvList);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            //clear toolbar menu to avoid duplicate draw
            toolbar.Menu.Clear();
            toolbar.InflateMenu(Resource.Menu.toolbar_menu_products_categories_search);
            //set visibility of toolbar bacause it is redrawn
            mToolbarVisibiltyStatus = true;
            toolbar.Menu.SetGroupVisible(Resource.Id.menugroup_search, mToolbarVisibiltyStatus);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public void DeactivateSearch()
        {
            try
            {
                toolbar.RemoveView(inactiveSearchView);
                searchCancelButton.PerformClick();
            }
            catch (Exception)
            {

            }
        }

        private void FnSetUpSearchToolbar(LayoutInflater inflater)
        {
            toolbar = thisFragmentView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarSearchCategories);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;

            //inflate search to toolbar
            inactiveSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_inactive, toolbar, false);
            activeSearchView = inflater.Inflate(Resource.Layout.checkout_fragment_search_active, toolbar, false);

            toolbar.AddView(inactiveSearchView);

            //search activate
            var btnSearchActivate = inactiveSearchView.FindViewById<LinearLayout>(Resource.Id.btnSearchActivate);
            btnSearchActivate.Click += btnSearchActivate_Click;

            //search deactivate
            searchCancelButton = activeSearchView.FindViewById<ImageView>(Resource.Id.imgSearchCancel);
            searchCancelButton.Click += SearchCancelButton_Click;
            searchViewSearchItems = activeSearchView.FindViewById<SearchView>(Resource.Id.txtSearchItems);
            //reused from CheckoutFragment
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(this.Context,"ProductsFragment"));
            searchViewSearchItems.QueryHint = "Search categories";
            searchViewSearchItems.QueryTextChange += SearchViewSearchItems_QueryTextChange;
        }

        private void SearchViewSearchItems_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToUpper();
            FnGetListData(queryString);
            mResourceAdapter.RefreshList(mCategoriesList);

            _mItemTouchHelper.AttachToRecyclerView(queryString == "" ? rvCategories : null);
            disableReordering = queryString == "" ? false : true;
            mResourceAdapter.ShowReorderableIcon(!disableReordering);
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

        private void Toolbar_MenuItemClick(object sender, Android.Support.V7.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.add_new_item)
            {
                Toast.MakeText((MainActivity)this.Activity, "Pressed Add New Item", ToastLength.Long).Show();
            }
            if (!mDialogShown)
            {
                mDialogShown = true;
                Intent intent = new Intent(Context, typeof(ProductsFragmentAddCategoryOrSizeActivity));
                intent.PutExtra("isSize", false);
                StartActivityForResult(intent, 12);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 12)
            {
                mDialogShown = false;
                FnGetListData(searchViewSearchItems.Query);
                mResourceAdapter.RefreshList(mCategoriesList);
                ((MainActivity)this.Activity).RefreshCategoryTabsOnCheckout(); //refresh categories on checkout
            }
            else if (requestCode == 38)
            {
                FnGetListData(searchViewSearchItems.Query);
                mResourceAdapter.RefreshList(mCategoriesList);
                ((MainActivity)this.Activity).RefreshCategoryTabsOnCheckout(); //refresh categories on checkout
            }
        }

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            if (!disableReordering)
            {
                _mItemTouchHelper.StartDrag(viewHolder);
            }
        }
    }
}