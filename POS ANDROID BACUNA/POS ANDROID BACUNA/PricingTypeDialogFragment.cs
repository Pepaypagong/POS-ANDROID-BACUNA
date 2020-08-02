
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;

namespace POS_ANDROID_BACUNA
{

    public class PricingTypeDialogFragment : DialogFragment
    {

        private ImageButton mCloseButton;
        private Button mSaveButton;
        private RadioButton mRadioRetail;
        private RadioButton mRadioWholesale;
        private RadioButton mRadioRunner;
        private RadioButton mCheckedRadioButton;
        private RadioGroup mPricingTypeRadioGrp;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.pricingType_dialogFragment, container, false);

            //get the ids of the radio buttons
            mRadioRetail = view.FindViewById<RadioButton>(Resource.Id.radioRetail);
            mRadioWholesale = view.FindViewById<RadioButton>(Resource.Id.radioWholesale);
            mRadioRunner = view.FindViewById<RadioButton>(Resource.Id.radioRunner);

            mRadioRetail.Text = "RT - Retail";
            mRadioWholesale.Text = "WS - Wholesale";
            mRadioRunner.Text = "RUNR - Runner";

            mCloseButton = view.FindViewById<ImageButton>(Resource.Id.btnPricingTypeClose);
            mSaveButton = view.FindViewById<Button>(Resource.Id.btnPricingTypeSave);
            mSaveButton.Text = "SELECT";
            mCloseButton.Click += MCloseButton_Click;

            mPricingTypeRadioGrp = view.FindViewById<RadioGroup>(Resource.Id.radioGrpPricingType);

            mSaveButton.Click += (object sender, System.EventArgs e) =>
            {
                //on click select the checked radio button
                mCheckedRadioButton = view.FindViewById<RadioButton>(mPricingTypeRadioGrp.CheckedRadioButtonId);

                //set text on main act
                ((MainActivity)this.Activity).SetToolBarMenuTextFromFragment(SelectedPrice(mCheckedRadioButton.Text));
                this.Dismiss();
            };

            //check the radio button that is currently selected from main activity
            setCurrentCheckedPricingType(Arguments.GetString("currentPricingType"));

            return view;
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
            ((MainActivity)this.Activity).PricingTypeDialogFragmentOnActivityResult();
            base.OnDestroy();
        }

    }
}