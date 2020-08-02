
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;
using System;
using POS_ANDROID_BACUNA.Data_Classes;
using System.Linq;
using System.Collections.Generic;
using POS_ANDROID_BACUNA.Fragments;
using System.Reflection;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace POS_ANDROID_BACUNA
{

    public class SortCartItemsDialogFragment : DialogFragment
    {

        private ImageView mImgClose;
        private TextView mTxtTitle;
        private ListView mLvSortingFields;
        Context mCallerContext;
        private List<OrderFields> mOrderFields;
        private SortCartItemsListAdapter mAdapter;
        private Button mBtnSort;

        public SortCartItemsDialogFragment(Context _callerContext)
        {
            mCallerContext = _callerContext;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //PositionDialogAtBottom();
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.sortCartItemsDialogFragment, container, false);

            FnSetupControls(view);
            FnSetUpEvents(view);
            FnSetupData();
            FnSetupListView();
            return view;
        }

        private void FnSetupData()
        {
            mOrderFields = new List<OrderFields>();
            mOrderFields.Add(new OrderFields() { FieldId = 1, FieldRank = 0, FieldName = "Product name", 
                fieldNameOnDb = "productId", IsChecked = false, IsDesc = true});
            mOrderFields.Add(new OrderFields() { FieldId = 2, FieldRank = 0, FieldName = "Size", 
                fieldNameOnDb = "productSizeId", IsChecked = false, IsDesc = true });
            mOrderFields.Add(new OrderFields() { FieldId = 3, FieldRank = 0, FieldName = "Subtotal Amount", 
                fieldNameOnDb = "productSubTotalPrice", IsChecked = false, IsDesc = true });
            mOrderFields.Add(new OrderFields() { FieldId = 4, FieldRank = 0, FieldName = "Quantity", 
                fieldNameOnDb = "productCountOnCart", IsChecked = false, IsDesc = true });
            mOrderFields = mOrderFields.OrderByDescending(x => x.FieldRank).ToList();
            GlobalVariables.globalSortingFieldList = mOrderFields.OrderByDescending(x => x.FieldRank).ToList();
        }

        private void FnSetUpEvents(View _parentView)
        {
            mImgClose.Click += MImgClose_Click;
            mBtnSort.Click += MBtnSort_Click;
            mLvSortingFields.ItemClick += MLvSortingFields_ItemClick;
        }

        private void MLvSortingFields_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var checkbox = ParentView(sender).GetChildAt(e.Position).FindViewById<CheckBox>(Resource.Id.cbFieldName);
            checkbox.PerformClick();
        }

        public ViewGroup ParentView(object _sender)
        {
            View x = (View)_sender;
            ViewGroup g = (ViewGroup)x;
            return g;
        }

        private void MBtnSort_Click(object sender, EventArgs e)
        {
            if (EnableSort())
            {
                ((CheckoutFragmentCartActivity)this.Activity).SortCartItems(SortQueryString());
            }
            this.Dismiss();
        }

        private string SortQueryString()
        {
            string retval = "";
            foreach (var item in GlobalVariables.globalSortingFieldList.OrderByDescending(x=>x.FieldRank))
            {
                if (item.IsChecked)
                {
                    string isDesc = item.IsDesc ? " DESC," : " ASC,";
                    retval += item.fieldNameOnDb + isDesc;
                }
            }
            retval = retval.Remove(retval.Length - 1,1);
            return retval;
        }

        private bool EnableSort()
        {
            return GlobalVariables.globalSortingFieldList.Exists(x => x.IsChecked == true);
        }

        private void MImgClose_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void FnSetupControls(View _parentView)
        {
            mImgClose = _parentView.FindViewById<ImageView>(Resource.Id.imgClose);
            mTxtTitle = _parentView.FindViewById<TextView>(Resource.Id.txtTitle);
            mLvSortingFields = _parentView.FindViewById<ListView>(Resource.Id.lvFields);
            mBtnSort = _parentView.FindViewById<Button>(Resource.Id.btnSort);
        }

        private void FnSetupListView()
        {
            mAdapter = new SortCartItemsListAdapter(mCallerContext, 
                                                    mOrderFields.OrderByDescending(x => x.FieldRank)
                                                    .ToList(),
                                                    mLvSortingFields);
            mLvSortingFields.Adapter = mAdapter;
        }

        public override void OnResume()
        {
            Window window = Dialog.Window;
            window.SetGravity(GravityFlags.Center);
            ViewGroup.LayoutParams paramss = window.Attributes;
            paramss.Width = WindowManagerLayoutParams.MatchParent;
            paramss.Height = WindowManagerLayoutParams.WrapContent;
            window.Attributes = (WindowManagerLayoutParams)paramss;
            base.OnResume();
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);//Set title bar to invisible
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;//Set the animation
            base.OnActivityCreated(savedInstanceState);
        }
    }
}