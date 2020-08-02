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

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class OrderFields
    {
        public int FieldId { get; set; }
        public int FieldRank { get; set; }
        public bool IsChecked { get; set; }
        public string FieldName { get; set; }
        public bool IsDesc { get; set; }
        public string fieldNameOnDb { get; set; }
    }
}