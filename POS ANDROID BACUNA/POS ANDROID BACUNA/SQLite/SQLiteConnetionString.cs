using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Data_Classes;
using Android.Util;
using Dapper;
using SQLite;

namespace POS_ANDROID_BACUNA
{
    public class SQLiteConnetionString
    {
        public static string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public static string LoadConnectionString()
        {
            return System.IO.Path.Combine(folder, "Database.db");
        }
    }
}