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

namespace POS_ANDROID_BACUNA.Fragments
{
    public class CheckoutFragment : SupportFragment
    {

        TabLayout mTabs;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.checkout_fragment, container, false);
            mTabs = view.FindViewById<TabLayout>(Resource.Id.tabs);
            string[] categories = { "All", "Polo", "Blouse", "Pants", "Shorts", "Palda", "Caps", "Panyo" };
            onCreateTabLayout(categories); //add tabs and texts
            return view;
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

    }
}