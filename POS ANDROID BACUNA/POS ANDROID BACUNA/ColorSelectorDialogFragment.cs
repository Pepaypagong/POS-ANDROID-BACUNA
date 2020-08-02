
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;
using Android.Graphics.Drawables;
using System;
using System.Drawing;
using Java.Lang;
using POS_ANDROID_BACUNA.Fragments;

namespace POS_ANDROID_BACUNA
{

    public class ColorSelectorDialogFragment : DialogFragment
    {

        private ImageButton mCloseButton;
        private GridLayout mColorGrid;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.colorSelector_dialogFragment, container, false);

            mCloseButton = view.FindViewById<ImageButton>(Resource.Id.btnPricingTypeClose);
            mColorGrid = view.FindViewById<GridLayout>(Resource.Id.glNumPad);
            mCloseButton.Click += MCloseButton_Click;

            RegisterColorGridClickEvent();

            return view;
        }

        public void RegisterColorGridClickEvent()
        {
            int childCount = mColorGrid.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View colorView = (View)mColorGrid.GetChildAt(i);
                colorView.Click += ColorView_Click;
            }
        }

        private void ColorView_Click(object sender, System.EventArgs e)
        {
            View colorView = (View)sender;
            ColorDrawable viewColor = (ColorDrawable)colorView.Background;
            int colorId = viewColor.Color;
            
            string selectedColor = "#" + Integer.ToHexString(colorId).Substring(2).ToUpper();
            ((ProductsFragmentItemsAddItemActivity)this.Activity).SetLabelColor(selectedColor);
            this.Dismiss();
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
            ((ProductsFragmentItemsAddItemActivity)this.Activity).ColorSelectorDialogFragmentOnActivityResult();
            base.OnDestroy();
        }

    }
}