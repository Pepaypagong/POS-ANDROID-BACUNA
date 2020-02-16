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
    public class DialogMessageService
    {
        public static Android.App.AlertDialog.Builder MessageBox(Context context, string title, string message)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
            Android.App.AlertDialog alert = builder.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton("OK", (c, ev) =>
            {
                //ok button
            });
            alert.Show();

            return builder;
        }
    }
}