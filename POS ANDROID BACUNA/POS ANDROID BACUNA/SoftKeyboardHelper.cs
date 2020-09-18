using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace POS_ANDROID_BACUNA
{
    public class SoftKeyboardHelper
    {
        public static void HideKeyboard(View _view, Context _context)
        {
            InputMethodManager inputManager = (InputMethodManager)_context.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(_view.WindowToken, 0);
        }

        //makes the child of parent view clickable and focusable
        public static void SetUpFocusAndClickUI(View view) 
        {
            for (int i = 0; i < ((ViewGroup)view).ChildCount; i++)
            {
                View innerView = ((ViewGroup)view).GetChildAt(i);
                innerView.Clickable = true;
                innerView.FocusableInTouchMode = true;
            }
        }
    }

    public class SearchViewFocusListener : Java.Lang.Object, View.IOnFocusChangeListener
    {
        private Context mContext;
        private String mSenderFragmentName;
        float mDpVal;
        private int origGridHeight;
        LinearLayout gridHolder;
        RecyclerView recyclerView;
        RelativeLayout button;
        Toolbar toolbar;
        SearchView searchView;

        public SearchViewFocusListener(Context _context, String _senderFragmentName)
        {
            mContext = _context;
            mSenderFragmentName = _senderFragmentName;
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (mSenderFragmentName == "CheckoutFragment")
            {
                if (hasFocus)
                {
                    //keyboard opened here
                    //get the current gridview holder 
                    ViewGroup parentView = (ViewGroup)v.Parent.Parent.Parent;
                    gridHolder = parentView.FindViewById<LinearLayout>(Resource.Id.recyclerViewItemsListHolder);
                    button = parentView.FindViewById<RelativeLayout>(Resource.Id.rlCheckoutButtonContainer);
                    toolbar = parentView.FindViewById<Toolbar>(Resource.Id.checkoutSearchToolbar);

                    origGridHeight = gridHolder.Height;
                    gridHolder.PostDelayed(() => {
                        SetGridHeight(gridHolder, gridHolder.Height, button.Height, toolbar, false);
                    },100);
                }
                else
                {
                    SoftKeyboardHelper.HideKeyboard(v, mContext);
                    SetGridHeight(gridHolder, 0, 0, toolbar, true);
                }
            }
            else if(mSenderFragmentName == "TransactionsFragment")
            {
                if (hasFocus)
                {
                    //keyboard opened here
                    //get the current recyclerview
                    ViewGroup parentView = (ViewGroup)v.Parent.Parent;
                    recyclerView = parentView.FindViewById<RecyclerView>(Resource.Id.recyclerViewTransactions);
                    button = parentView.FindViewById<RelativeLayout>(Resource.Id.rlDateFilter); //date filter footer holder
                    searchView = parentView.FindViewById<SearchView>(Resource.Id.searchBar);

                    origGridHeight = recyclerView.Height;
                    recyclerView.PostDelayed(() => {
                        SetGridHeight(recyclerView, recyclerView.Height, button.Height, searchView, false);
                    }, 100);
                }
                else
                {
                    SoftKeyboardHelper.HideKeyboard(v, mContext);
                    SetGridHeight(recyclerView, 0, 0, searchView, true);
                }
            }
            else
            {
                if (!hasFocus)
                {
                    SoftKeyboardHelper.HideKeyboard(v, mContext);
                }
            }
        }

        private void SetGridHeight(View gridHolder, int gridheight, int buttonheight, View layoutBelow, bool isKeyboardHidden)
        {
            mDpVal = mContext.Resources.DisplayMetrics.Density;
            RelativeLayout.LayoutParams layoutParams =
            new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, isKeyboardHidden ? origGridHeight : gridheight - buttonheight);
            if (mSenderFragmentName == "TransactionsFragment")
            {
                layoutParams.SetMargins(dpToPixel(0), dpToPixel(2), dpToPixel(0), dpToPixel(0));
            }
            else
            {
                layoutParams.SetMargins(dpToPixel(5), dpToPixel(0), dpToPixel(5), dpToPixel(0));
            }
            layoutParams.AddRule(LayoutRules.Below, layoutBelow.Id);
            gridHolder.LayoutParameters = layoutParams;
            gridHolder.RequestLayout();
        }
        private int dpToPixel(int val)
        {
            return val * (int)Math.Ceiling(mDpVal);
        }
    }
}