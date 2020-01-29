using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace POS_ANDROID_BACUNA
{
    class SingleClickListener
    {
        public SingleClickListener(Action<object, EventArgs> setOnClick)
        {
            _setOnClick = setOnClick;
        }
        private bool hasClicked;

        private Action<object, EventArgs> _setOnClick;

        public void OnClick(object v, EventArgs e)
        {
            if (!hasClicked)
            {
                _setOnClick(v, e);
                hasClicked = true;
            }
            reset();
        }

        private void reset()
        {
            Android.OS.Handler mHandler = new Android.OS.Handler();
            mHandler.PostDelayed(new Action(() => { hasClicked = false; }), 500);
        }
    }
}