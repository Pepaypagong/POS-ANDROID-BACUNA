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
    class TransactionItemsDataAccess 
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<TransactionItemsModel>();
                    connection.CreateTable<TransactionItemsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(TransactionItemsModel model)
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
        public List<TransactionItemsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<TransactionItemsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool DeleteFromTable(int transactionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Delete<TransactionItemsModel>(transactionId);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<TransactionItemsModel> SelectRecord(int transactionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<TransactionItemsModel>("SELECT * FROM TransactionItemsModel Where TransactionId=?", transactionId);
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