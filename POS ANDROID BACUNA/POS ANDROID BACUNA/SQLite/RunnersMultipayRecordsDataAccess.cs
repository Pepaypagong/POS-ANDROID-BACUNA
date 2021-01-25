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
using SQLite;

namespace POS_ANDROID_BACUNA.SQLite
{
    class RunnersMultipayRecordsDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<RunnersMultipayRecordsModel>();
                    connection.CreateTable<RunnersMultipayRecordsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(RunnersMultipayRecordsModel model)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Insert(model);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<RunnersMultipayRecordsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<RunnersMultipayRecordsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public List<RunnersMultipayRecordsModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<RunnersMultipayRecordsModel>("SELECT * FROM RunnersMultipayRecordsModel Where Id=?", id);
                    return x;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

    }
}