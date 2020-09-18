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
using SQLite;

namespace POS_ANDROID_BACUNA.Data_Classes
{
    public class ParentProductsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ParentProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductColorBg { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductAlias { get; set; }
        public string ProductDescription { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
    }
}