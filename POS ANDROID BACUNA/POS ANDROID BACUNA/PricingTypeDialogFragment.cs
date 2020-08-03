
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

    public class PricingTypeDialogFragment : DialogFragment
    {
        private View mMainView;
        private ImageButton mCloseButton;
        private Button mSaveButton;
        private RadioButton mRadioRetail;
        private RadioButton mRadioWholesale;
        private RadioButton mRadioRunner;
        private RadioButton mCheckedRadioButton;
        private RadioGroup mPricingTypeRadioGrp;
        private string mCallerActivity;
        private string mCurrentPricingType;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            mMainView = inflater.Inflate(Resource.Layout.pricingType_dialogFragment, container, false);

            FnGetData();
            FnSetupControls(mMainView);
            FnSetupEvents();

            //check the radio button that is currently selected from main activity
            setCurrentCheckedPricingType(mCurrentPricingType);

            return mMainView;
        }

        private void MSaveButton_Click(object sender, EventArgs e)
        {
            mCheckedRadioButton = mMainView.FindViewById<RadioButton>(mPricingTypeRadioGrp.CheckedRadioButtonId);
            //set text on main act
            if (mCallerActivity == "MainActivity")
            {
                ((MainActivity)this.Activity).SetToolBarMenuTextFromFragment(SelectedPrice(mCheckedRadioButton.Text), ResetSelectedCustomer());
            }
            else if (mCallerActivity == "CheckoutFragmentCartActivity")
            {
                ((CheckoutFragmentCartActivity)this.Activity).
                    SetToolBarMenuTextFromDialogFragment(SelectedPrice(mCheckedRadioButton.Text), ResetSelectedCustomer());
            }
            GlobalVariables.mCurrentSelectedPricingType = SelectedPrice(mCheckedRadioButton.Text);
            this.Dismiss();
        }

        private void FnSetupEvents()
        {
            mCloseButton.Click += MCloseButton_Click;
            mSaveButton.Click += MSaveButton_Click;
        }

        private void FnSetupControls(View view)
        {
            mCloseButton = view.FindViewById<ImageButton>(Resource.Id.btnPricingTypeClose);
            mSaveButton = view.FindViewById<Button>(Resource.Id.btnPricingTypeSave);
            mRadioRetail = view.FindViewById<RadioButton>(Resource.Id.radioRetail);
            mRadioWholesale = view.FindViewById<RadioButton>(Resource.Id.radioWholesale);
            mRadioRunner = view.FindViewById<RadioButton>(Resource.Id.radioRunner);
            mPricingTypeRadioGrp = view.FindViewById<RadioGroup>(Resource.Id.radioGrpPricingType);

            mRadioRetail.Text = "RT - Retail";
            mRadioWholesale.Text = "WS - Wholesale";
            mRadioRunner.Text = "RUNR - Runner";
            mSaveButton.Text = "SELECT";
        }

        private void FnGetData()
        {
            mCurrentPricingType = Arguments.GetString("currentPricingType");
            mCallerActivity = Arguments.GetString("callerActivity");
        }

        private bool ResetSelectedCustomer()
        {
            bool retval = true;
            string previous = Arguments.GetString("currentPricingType");
            string current = SelectedPrice(mCheckedRadioButton.Text);

            if (previous == current)
            {
                retval = false;
            }
            else if(previous != current)
            {
                if ((previous == "RT" && current == "WS")||(previous == "WS" && current == "RT"))
                {
                    retval = false; 
                }
                else
                {
                    retval = true;
                }
            }

            return retval;
        }

        private string SelectedPrice(string _radioText)
        {
            string retval = "X";

            if (_radioText == "RT - Retail")
            {
                retval = "RT";
            }
            else if (_radioText == "WS - Wholesale")
            {
                retval = "WS";
            }
            else if (_radioText == "RUNR - Runner")
            {
                retval = "RUNR";
            }

            return retval;
        }

        private void setCurrentCheckedPricingType(string currentSelectedPriceType)
        {
            //set current selected pricing type
            if (currentSelectedPriceType == "RT")
            {
                //set checked to retail
                mPricingTypeRadioGrp.Check(Resource.Id.radioRetail);
            }
            else if (currentSelectedPriceType == "WS")
            {
                //set checked to wholesale
                mPricingTypeRadioGrp.Check(Resource.Id.radioWholesale);
            }
            else if (currentSelectedPriceType == "RUNR")
            {
                //set  checked to runner
                mPricingTypeRadioGrp.Check(Resource.Id.radioRunner);
            }
        }

        private void MCloseButton_Click(object sender, System.EventArgs e)
        {
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);//Set title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;//Set the animation
        }

        public override void OnDestroy()
        {
            if (mCallerActivity == "MainActivity")
            {
                ((MainActivity)this.Activity).PricingTypeDialogFragmentOnActivityResult();
            }
            else if (mCallerActivity == "CheckoutFragmentCartActivity")
            {
                ((CheckoutFragmentCartActivity)this.Activity).PricingTypeDialogFragmentOnActivityResult();
            }
            
            base.OnDestroy();
        }

    }
}