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
using POS_ANDROID_BACUNA.Interfaces;
using Android.Support.V7.Widget.Helper;
using POS_ANDROID_BACUNA.SQLite;

namespace POS_ANDROID_BACUNA.Fragments
{
    public class ProductsFragmentSizesFragment : SupportFragment, IOnStartDragListener
    {
        View thisFragmentView;
        //toolbar
        Android.Support.V7.Widget.Toolbar toolbar;
        bool mToolbarVisibiltyStatus = true;
        View inactiveSearchView;
        View activeSearchView;
        ImageView searchCancelButton;
        SearchView searchViewSearchItems;

        static bool mDialogShown = false;  // flag for stopping double click

        private ItemTouchHelper _mItemTouchHelper;
        public static List<ProductSizesModel> mProductSizesList;
        private RecyclerView rvSizes;
        private SizesDataAccess mSizesDataAccess;
        ProductsItemSizesReorderableAdapter mResourceAdapter;
        private bool disableReordering = false;

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.products_fragment_categories_fragment, container, false);
            FnSetUpSearchToolbar(inflater);
            FnSetUpData();
            FnSetUpControls();
            FnSetUpList();

            return thisFragmentView;
        }

        private void FnSetUpList()
        {
            FnGetListData("");
            mResourceAdapter = new ProductsItemSizesReorderableAdapter(mProductSizesList, this, this);
            rvSizes.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false));
            rvSizes.SetAdapter(mResourceAdapter);
            rvSizes.HasFixedSize = true;

            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(mResourceAdapter);
            _mItemTouchHelper = new ItemTouchHelper(callback);
            _mItemTouchHelper.AttachToRecyclerView(rvSizes);
        }

        private void FnGetListData(string _queryString)
        {
            mProductSizesList = mSizesDataAccess.SelectTable();
            mProductSizesList = mProductSizesList != null ?
                                mProductSizesList.OrderBy(x => x.SizeRank).Where(x=>x.ProductSizeName.Contains(_queryString)).ToList() : 
                                mProductSizesList;
        }

        private void FnSetUpControls()
        {
            rvSizes = thisFragmentView.FindViewById<RecyclerView>(Resource.Id.rvList);
        }

        private void FnSetUpData()
        {
            mSizesDataAccess = new SizesDataAccess();
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
            searchViewSearchItems.SetOnQueryTextFocusChangeListener(new SearchViewFocusListener(Context, "ProductsFragment"));
            searchViewSearchItems.QueryHint = "Search sizes";
            searchViewSearchItems.QueryTextChange += SearchViewSearchItems_QueryTextChange;
        }

        private void SearchViewSearchItems_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            string queryString = e.NewText.ToLower();
            FnGetListData(queryString);
            mResourceAdapter.RefreshList(mProductSizesList);

            _mItemTouchHelper.AttachToRecyclerView(queryString == "" ? rvSizes : null);
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
                Intent intent = new Intent(Context, typeof(ProductsFragmentAddCategoryOrSizeActivity));//reused the add category activity
                intent.PutExtra("isSize", true);
                intent.PutExtra("isEdit", false);
                StartActivityForResult(intent, 18);
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 18)
            {
                mDialogShown = false;
                FnGetListData(searchViewSearchItems.Query);
                mResourceAdapter.RefreshList(mProductSizesList);
            }
            else if (requestCode == 37)
            {
                FnGetListData(searchViewSearchItems.Query);
                mResourceAdapter.RefreshList(mProductSizesList);
            }
        }

        public void OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            if (!disableReordering)
            {
                _mItemTouchHelper.StartDrag(viewHolder);
            }
        }

        //public void GetCollection()
        //{
        //    ResourceList = new ObservableCollection<string>();
        //    ResourceList.Add("OnPause()");
        //    ResourceList.Add("OnStart()");
        //    ResourceList.Add("OnCreate()");
        //}
    }
}