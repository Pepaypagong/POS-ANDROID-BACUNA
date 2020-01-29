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
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
namespace POS_ANDROID_BACUNA.Fragments
{
    public class CustomersFragment : SupportFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //test button
            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.customers_fragment, container, false);

            Button testButton = view.FindViewById<Button>(Resource.Id.BtnTest);

            testButton.Click += new SingleClickListener(TestButton_Click).OnClick;
            return view;
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            SupportFragmentTransaction transaction = FragmentManager.BeginTransaction();
            PricingTypeDialogFragment pricingTypeDialog = new PricingTypeDialogFragment();
            var args = new Bundle();
            args.PutString("currentPricingType", "Retail");
            pricingTypeDialog.Arguments = args;
            pricingTypeDialog.Show(transaction, "pricingTypeDialogFragment");
        }
    }
}