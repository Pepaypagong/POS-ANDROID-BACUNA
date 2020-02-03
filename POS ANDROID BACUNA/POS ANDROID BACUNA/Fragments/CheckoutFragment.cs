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
using static Android.Views.ViewGroup;
using SupportFragment = Android.Support.V4.App.Fragment;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Views.InputMethods;

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

        public override void OnCreate(Bundle savedInstanceState)
        {
            HasOptionsMenu = true; //enable on menu on fragment
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            thisFragmentView = inflater.Inflate(Resource.Layout.checkout_fragment, container, false);
            mTabs = thisFragmentView.FindViewById<TabLayout>(Resource.Id.tabs);
            string[] categories = { "All", "Polo", "Blouse", "Pants", "Shorts", "Palda", "Caps", "Panyo" };
            onCreateTabLayout(categories); //add tabs and texts

            toolbar = thisFragmentView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.checkoutSearchToolbar);
            
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
            return thisFragmentView;
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