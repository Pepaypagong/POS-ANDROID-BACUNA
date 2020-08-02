
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

    public class MoreOptionsDialogFragment : DialogFragment
    {

        private ImageView mImgClose;
        private TextView mTxtTitle;
        private ListView mLvOptions;
        private List<Options> mSelectedOptions;
        int listviewClickedPos = -1;
        Context mCallerContext;

        public MoreOptionsDialogFragment(Context _callerContext)
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
            var view = inflater.Inflate(Resource.Layout.moreOptionsDialogFragment, container, false);
            
            FnSetupControls(view);
            FnSetUpEvents(view);

            string caller = Arguments.GetString("caller");
            FnSetUpListView(caller);

            return view;
        }

        private void PositionDialogAtBottom()
        {
            Window window = Dialog.Window;
            window.SetGravity(GravityFlags.Bottom);
            ViewGroup.LayoutParams paramss = window.Attributes;
            paramss.Width = WindowManagerLayoutParams.MatchParent;
            paramss.Height = WindowManagerLayoutParams.WrapContent;
            window.Attributes = (WindowManagerLayoutParams)paramss;
        }

        private void FnSetUpListView(string _caller)
        {
            mSelectedOptions = GlobalVariables.globalOptionList.Where(x => x.CallerClassName == _caller).ToList();
            MoreOptionsListAdapter adapter = new MoreOptionsListAdapter(this.Context, mSelectedOptions);
            mLvOptions.Adapter = adapter;
        }

        private void FnSetUpEvents(View _parentView)
        {
            mImgClose.Click += MImgClose_Click;
            mLvOptions.ItemClick += MLvOptions_ItemClick;
        }

        private void MLvOptions_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                listviewClickedPos = e.Position;
                this.Dismiss();
            }
            catch (Exception){}
        }

        private void MImgClose_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void FnSetupControls(View _parentView)
        {
            mImgClose = _parentView.FindViewById<ImageView>(Resource.Id.imgClose);
            mTxtTitle = _parentView.FindViewById<TextView>(Resource.Id.txtTitle);
            mLvOptions = _parentView.FindViewById<ListView>(Resource.Id.lvOptions);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);//Set title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;//Set the animation
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnResume()
        {
            PositionDialogAtBottom();
            base.OnResume();
        }

        public override void OnPause()
        {
            if (listviewClickedPos >= 0)
            {
                int optionId = mSelectedOptions[listviewClickedPos].OptionId;
                string action = mSelectedOptions[listviewClickedPos].Action;
                bool isDialog = mSelectedOptions[listviewClickedPos].IsDialog;
                string callerClassName = mSelectedOptions[listviewClickedPos].CallerClassName;
                string targetClassName = mSelectedOptions[listviewClickedPos].TargetActivity;
                int requestCode = mSelectedOptions[listviewClickedPos].RequestCode;
                if (action == "Modify")
                {
                    if (isDialog)
                    {
                        if (optionId == 3)
                        {
                            ((CheckoutFragmentCartActivity)this.Activity).ShowSortCartItemsBy();
                        }
                    }
                    else
                    {
                        Intent intent = new Intent(mCallerContext, Type.GetType("POS_ANDROID_BACUNA.Fragments." + targetClassName));
                        ((Android.App.Activity)mCallerContext).StartActivityForResult(intent, requestCode);
                    }
                }
                else if (action == "Delete")
                {
                    if (optionId == 4) //clearcart
                    {
                        ((CheckoutFragmentCartActivity)this.Activity).ClearCart();
                        //Type type = Type.GetType("POS_ANDROID_BACUNA.Fragments." + callerClassName);
                        //MethodInfo method = type.GetMethod("ClearCart");
                        //method.Invoke(((CheckoutFragmentCartActivity)this.Activity), null);
                    }
                }
            }

            base.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

    }
}