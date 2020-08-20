
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;
using System;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Fragments;

namespace POS_ANDROID_BACUNA
{

    public class CheckoutFragmentPaymentAddPaymentDialogFragment : DialogFragment
    {
        private Button mBtnCancel;
        private TextView mTxtTitle;
        Context mCallerContext;

        RelativeLayout mRlCash1, mRlCash2;
        ImageView mImgCash;
        TextView mTxtCash;

        RelativeLayout mRlCheck1, mRlCheck2;
        ImageView mImgCheck;
        TextView mTxtCheck;

        private double mTotalSaleAmount;
        private double mTotalSplitPaymentsAmount;
        private string mSelectedPaymentMethod;
        public CheckoutFragmentPaymentAddPaymentDialogFragment(Context _callerContext)
        {
            mCallerContext = _callerContext;
            mSelectedPaymentMethod = "";
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //PositionDialogAtBottom();
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.checkout_fragment_payment_add_dialogFragment, container, false);

            FnGetData();
            FnSetupControls(view);
            FnSetUpEvents(view);
            return view;
        }

        private void FnGetData()
        {
            mTotalSaleAmount = Arguments.GetDouble("totalSaleAmount");
            mTotalSplitPaymentsAmount = Arguments.GetDouble("totalSplitPaymentsAmount");
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

        private void FnSetUpEvents(View _parentView)
        {
            mBtnCancel.Click += MImgClose_Click;
            mRlCash1.Click += MRlCash1_Click;
            mRlCash2.Click += MRlCash1_Click;
            mImgCash.Click += MRlCash1_Click;
            mTxtCash.Click += MRlCash1_Click;
            mRlCheck1.Click += MRlCheck1_Click;
            mRlCheck2.Click += MRlCheck1_Click;
            mImgCheck.Click += MRlCheck1_Click;
            mTxtCheck.Click += MRlCheck1_Click;
        }

        private void MRlCheck1_Click(object sender, EventArgs e)
        {
            mSelectedPaymentMethod = "Check";
            this.Dismiss();
        }

        private void MRlCash1_Click(object sender, EventArgs e)
        {
            mSelectedPaymentMethod = "Cash";
            this.Dismiss();
        }

        private void MImgClose_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void FnSetupControls(View _parentView)
        {
            mRlCash1 = _parentView.FindViewById<RelativeLayout>(Resource.Id.rlCash1);
            mRlCash2 = _parentView.FindViewById<RelativeLayout>(Resource.Id.rlCash2);
            mImgCash = _parentView.FindViewById<ImageView>(Resource.Id.imgCash);
            mTxtCash = _parentView.FindViewById<TextView>(Resource.Id.txtCash);

            mRlCheck1 = _parentView.FindViewById<RelativeLayout>(Resource.Id.rlCheck1);
            mRlCheck2 = _parentView.FindViewById<RelativeLayout>(Resource.Id.rlCheck2);
            mImgCheck = _parentView.FindViewById<ImageView>(Resource.Id.imgCheck);
            mTxtCheck = _parentView.FindViewById<TextView>(Resource.Id.txtCheck);

            mBtnCancel = _parentView.FindViewById<Button>(Resource.Id.btnBottomCancel);
            mTxtTitle = _parentView.FindViewById<TextView>(Resource.Id.txtTitle);
            mTxtTitle.Text = "Payment method";
        }
        private void OpenPaymentMethodNumpad(string _paymentMethod)
        {
            if (_paymentMethod == "Cash" || _paymentMethod == "Check")
            {
                Intent intent = new Intent(mCallerContext, typeof(CheckoutFragmentPaymentNumpadActivity));
                intent.PutExtra("isInitialCall", false);
                intent.PutExtra("totalSaleAmount", mTotalSaleAmount);
                intent.PutExtra("totalSplitPaymentsAmount", mTotalSplitPaymentsAmount);
                intent.PutExtra("isPaymentModeCash", _paymentMethod == "Cash" ? true : false);
                ((CheckoutFragmentPaymentMethodsActivity)this.Activity).StartActivityForResult(intent, 28);
            }
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);//Set title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;//Set the animation
        }
        public override void OnDestroy()
        {
            ((CheckoutFragmentPaymentMethodsActivity)this.Activity).AddPaymentDialogFragmentActivityResult();
            base.OnDestroy();
        }
        public override void OnPause()
        {
            OpenPaymentMethodNumpad(mSelectedPaymentMethod);
            base.OnPause();
        }
        public override void OnResume()
        {
            PositionDialogAtBottom();
            base.OnResume();
        }

    }
}