using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V4.Widget;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Support.Design.Widget;
using System;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Calligraphy;
using POS_ANDROID_BACUNA.Fragments;
using System.Collections.Generic;
using Android.Support.V4.App;
using Newtonsoft.Json;

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout mDrawerLayout;
        private SupportFragment mCurrentFragment;
        private CheckoutFragment mCheckoutFragment;
        private ProductsFragment mProductsFragment;
        private CustomersFragment mCustomersFragment;
        private TransactionsFragment mTransactionsFragment;
        private SettingsFragment mSettingsFragment;
        private Stack<SupportFragment> mStackFragment;
        private IMenu mCurrentToolBarMenu;
        private string mCurrentSelectedPriceType = "Retail";
        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            mCheckoutFragment = new CheckoutFragment();
            mProductsFragment = new ProductsFragment();
            mCustomersFragment = new CustomersFragment();
            mTransactionsFragment = new TransactionsFragment();
            mSettingsFragment = new SettingsFragment();
            mStackFragment = new Stack<SupportFragment>();

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.main_toolbar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.menu_icon_small);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetTitle(Resource.String.checkout_title);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Add(Resource.Id.fragmentContainer, mSettingsFragment, "SettingsFragment");
            trans.Hide(mSettingsFragment);
            trans.Add(Resource.Id.fragmentContainer, mTransactionsFragment, "TransactionsFragment");
            trans.Hide(mTransactionsFragment);
            trans.Add(Resource.Id.fragmentContainer, mCustomersFragment, "CustomersFragment");
            trans.Hide(mCustomersFragment);
            trans.Add(Resource.Id.fragmentContainer, mProductsFragment, "ProductsFragment");
            trans.Hide(mProductsFragment);

            trans.Add(Resource.Id.fragmentContainer, mCheckoutFragment, "CheckoutFragment");

            trans.Commit();

            mCurrentFragment = mCheckoutFragment;
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                InvalidateOptionsMenu();//redraw toolbar Menu

                e.MenuItem.SetChecked(true);
                SupportActionBar ab = SupportActionBar;

                switch (e.MenuItem.ItemId)
                {
                    case (Resource.Id.nav_checkout):
                        ShowFragment(mCheckoutFragment);
                        ab.SetTitle(Resource.String.checkout_title);
                        break;
                    case (Resource.Id.nav_products):
                        ShowFragment(mProductsFragment);
                        ab.SetTitle(Resource.String.products_title);
                        break;
                    case (Resource.Id.nav_customers):
                        ShowFragment(mCustomersFragment);
                        ab.SetTitle(Resource.String.customers_title);
                        break;
                    case (Resource.Id.nav_transactions):
                        ShowFragment(mTransactionsFragment);
                        ab.SetTitle(Resource.String.transactions_title);
                        break;
                    case (Resource.Id.nav_settings):
                        ShowFragment(mSettingsFragment);
                        ab.SetTitle(Resource.String.settings_title);
                        break;
                }

                mDrawerLayout.CloseDrawers();
            };
        }

        private void ShowFragment(SupportFragment fragment)
        {
            var trans = SupportFragmentManager.BeginTransaction();

            trans.Hide(mCurrentFragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();

            mStackFragment.Push(mCurrentFragment);
            mCurrentFragment = fragment;
        }

        public override void OnBackPressed()
        {
            InvalidateOptionsMenu();//redraw toolbar Menu

            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
                mCurrentFragment = mStackFragment.Pop();

                //corrects the highlighted navigation menu item
                NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                navigationView.SetCheckedItem(CurrentSelectedNavigation(mCurrentFragment));
                mDrawerLayout.CloseDrawers();//close drawer on back press
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private int CurrentSelectedNavigation(SupportFragment currentFragment)
        {
            SupportActionBar ab = SupportActionBar;//instantiate actionbar
            int returnValue = 0;

            if (currentFragment == mCheckoutFragment)
            {
                ab.SetTitle(Resource.String.checkout_title);//set title of actionbar
                returnValue = Resource.Id.nav_checkout;
            }
            else if (currentFragment == mProductsFragment)
            {
                ab.SetTitle(Resource.String.products_title);
                returnValue = Resource.Id.nav_products;
            }
            else if (currentFragment == mCustomersFragment)
            {
                ab.SetTitle(Resource.String.customers_title);
                returnValue = Resource.Id.nav_customers;
            }
            else if (currentFragment == mTransactionsFragment)
            {
                ab.SetTitle(Resource.String.transactions_title);
                returnValue = Resource.Id.nav_transactions;
            }
            else if (currentFragment == mSettingsFragment)
            {
                ab.SetTitle(Resource.String.settings_title);
                returnValue = Resource.Id.nav_settings;
            }

            return returnValue;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                case Resource.Id.toolbarMenu_pricingType:
                    //show dialog here
                    SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                    PricingTypeDialogFragment pricingTypeDialog = new PricingTypeDialogFragment();
                    //pass current selected price type t
                    var args = new Bundle();
                    args.PutString("currentPricingType", mCurrentSelectedPriceType);
                    pricingTypeDialog.Arguments = args;

                    pricingTypeDialog.Show(transaction, "pricingTypeDialogFragment");
                    return true;
                case Resource.Id.toolbarMenu_customer:
                    Android.Widget.Toast.MakeText(this, "Clicked Customer Icon!", Android.Widget.ToastLength.Long).Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mCurrentToolBarMenu = menu;

            if (mCurrentFragment == mCheckoutFragment)
            {
                MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout, menu);
                //code here to set the selected price type
                IMenuItem item = menu.FindItem(Resource.Id.toolbarMenu_pricingType);
                item.SetTitle(mCurrentSelectedPriceType);
            }
            else if (mCurrentFragment == mProductsFragment)
            {
                MenuInflater.Inflate(Resource.Menu.toolbar_menu_products, menu);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public void SetToolBarMenuTextFromFragment(string menuText)
        {
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_pricingType);
            item.SetTitle(menuText);
            mCurrentSelectedPriceType = menuText;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}