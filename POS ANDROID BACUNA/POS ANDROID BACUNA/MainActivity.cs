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
using Android.Views.InputMethods;
using POS_ANDROID_BACUNA.Data_Classes;
using Android.Graphics.Drawables;

namespace POS_ANDROID_BACUNA
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
        private string mCurrentSelectedPriceType = "RT";
        static bool mDialogShown = false;  // flag for stopping double click

        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base)); //custom font
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

            //adjust spacing of logo from title of actionbar

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }
            mDrawerLayout.DrawerOpened += MDrawerLayout_DrawerOpened;


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

            //pass to global variable the instance of the checkout fragment
            GlobalVariables.mCheckoutFragmentCurrentInstance = mCheckoutFragment;
            PopulateDatabase.PopulateAll();
        }

        private void MDrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
        {
            hideKeyboard();
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
                        ab.Title = "PRODUCTS (" + GlobalVariables.globalProductList.Count.ToString() + ")";
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
                    if (!mDialogShown) //avoid double click
                    {
                        mDialogShown = true;
                        //show dialog here
                        SupportFragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                        PricingTypeDialogFragment pricingTypeDialog = new PricingTypeDialogFragment();
                        //pass current selected price type t
                        var args = new Bundle();
                        args.PutString("currentPricingType", mCurrentSelectedPriceType);
                        args.PutString("callerActivity", "MainActivity");
                        pricingTypeDialog.Arguments = args;
                        pricingTypeDialog.Show(transaction, "pricingTypeDialogFragment");
                    }
                    return true;
                case Resource.Id.toolbarMenu_customer:
                    if (!mDialogShown)
                    {
                        mDialogShown = true;
                        Intent intent = new Intent(this, typeof(CheckoutSelectCustomerActivity));
                        intent.PutExtra("isCustomer", mCurrentSelectedPriceType == "RUNR" ? false : true);
                        StartActivityForResult(intent, 1);
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void PricingTypeDialogFragmentOnActivityResult(bool _clearCart)
        {
            mDialogShown = false; //flag to enable dialog show 
            CheckoutFragment fragment = (CheckoutFragment)SupportFragmentManager.FindFragmentByTag("CheckoutFragment");
            fragment.RefreshPricingType(_clearCart);
        }

        public void RefreshMenu()
        {
            InvalidateOptionsMenu();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            mCurrentToolBarMenu = menu;

            if (mCurrentFragment == mCheckoutFragment)
            {
                mCurrentSelectedPriceType = GlobalVariables.mCurrentSelectedPricingType;
                MenuInflater.Inflate(Resource.Menu.toolbar_menu_checkout, menu);
                //code here to set the selected price type
                IMenuItem item = menu.FindItem(Resource.Id.toolbarMenu_pricingType);
                item.SetTitle(mCurrentSelectedPriceType);
                //set the action layout for the selected customer
                IMenuItem selectedCustomerItem = menu.FindItem(Resource.Id.toolbarMenu_customer);
                selectedCustomerItem.SetActionView(GlobalVariables.mCurrentSelectedCustomerButtonLayout);
                if (GlobalVariables.mHasSelectedCustomerOnCheckout)
                {
                    //subscribeCustomLayoutToClick(selectedCustomerItem);
                    ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
                }
                SetToolbarMenuIconTint(mCurrentSelectedPriceType);
            }
            else if (mCurrentFragment == mProductsFragment)
            {
                MenuInflater.Inflate(Resource.Menu.toolbar_menu_products, menu);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public void SetToolBarMenuTextFromFragment(string menuText, bool resetSelectedCustomer)
        {
            if (resetSelectedCustomer)
            {
                GlobalVariables.mHasSelectedCustomerOnCheckout = false;
                GlobalVariables.mCurrentSelectedCustomerOnCheckout = "";
                IMenuItem customerMenuItem = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_customer);
                removeActionLayout(customerMenuItem);
            }
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_pricingType);
            item.SetTitle(menuText);
            mCurrentSelectedPriceType = menuText;
            SetToolbarMenuIconTint(menuText);
        }

        public void SetToolbarMenuIconTint(string _pricingType)
        {
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_customer);
            Drawable drawable = item.Icon;
            if (drawable != null)
            {
                drawable.Mutate();
                drawable.SetColorFilter(_pricingType == "RUNR"?
                    ColorHelper.ResourceIdToColor(Resource.Color.orange, this):
                    ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this)
                    , PorterDuff.Mode.SrcAtop);
            }
            
        }

        public void ChangeSelectCustomerIcon(string customerName)
        {
            IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_customer);

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
            _txtCustomerNameButtonTitle.SetTextColor(GetColorStateList(mCurrentSelectedPriceType == "RUNR" ? 
                Resource.Color.orange : Resource.Color.colorAccent));
            _imgIcon.SetColorFilter(mCurrentSelectedPriceType == "RUNR" ?
                        ColorHelper.ResourceIdToColor(Resource.Color.orange, this) :
                        ColorHelper.ResourceIdToColor(Resource.Color.colorAccent, this));
            _borderContainer.Background = GetDrawable(mCurrentSelectedPriceType == "RUNR" ? 
                Resource.Drawable.roundborderOrange : Resource.Drawable.roundborder);
        }

        public void subscribeCustomLayoutToClick(IMenuItem item)
        {
            item.ActionView.Click += (sender, args) => {
                this.OnOptionsItemSelected(item);
            };
        }

        public void removeActionLayout(IMenuItem item)
        {
            GlobalVariables.mCurrentSelectedCustomerButtonLayout = null;
            item.SetActionView(GlobalVariables.mCurrentSelectedCustomerButtonLayout);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                mDialogShown = false;
                SetSelectedCustomerIconAppearance();
            }
        }

        public void SetSelectedCustomerIconAppearance()
        {
            if (GlobalVariables.mHasSelectedCustomerOnCheckout == true)
            {
                ChangeSelectCustomerIcon(GlobalVariables.mCurrentSelectedCustomerOnCheckout);
            }
            else
            {
                IMenuItem item = mCurrentToolBarMenu.FindItem(Resource.Id.toolbarMenu_customer);
                removeActionLayout(item);
            }
        }

        void hideKeyboard()
        {
            var im = ((InputMethodManager)GetSystemService(Android.Content.Context.InputMethodService));
            if (CurrentFocus != null)
            {
                im.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void SetProductCountTitle()
        {
            SupportActionBar supportActionBar = SupportActionBar;
            supportActionBar.Title = "PRODUCTS (" + GlobalVariables.globalProductList.Count.ToString() + ")";
        }

    }
}