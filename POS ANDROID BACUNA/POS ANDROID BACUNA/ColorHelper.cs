using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace POS_ANDROID_BACUNA
{
    public class ColorHelper
    {
        public static Color ResourceIdToColor(int _colorInt, Context _context)
        {
            int colorInt = _context.GetColor(_colorInt);
            Color color = new Color(colorInt);
            return color;
        }
    }
}