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
using SupportActivity = Android.Support.V7.App.AppCompatActivity;
namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class Options
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public int TextColorResourceId { get; set; }
        public bool ShowArrow { get; set; }
        public string CallerClassName { get; set; }
        public string Action { get; set; }
        public string TargetActivity { get; set; }
        public int RequestCode { get; set; }
        public bool IsDialog { get; set; }
    }
}