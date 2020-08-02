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
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.customers_fragment, container, false);
            return view;
        }
    }
}