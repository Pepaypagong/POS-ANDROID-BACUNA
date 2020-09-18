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
    class ParentProductsDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<ParentProductsModel>();
                    connection.CreateTable<ParentProductsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(ParentProductsModel row)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Insert(row);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<ParentProductsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<ParentProductsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTable(ParentProductsModel ParentProductsModel)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ParentProductsModel>("UPDATE ParentProductsModel set ParentProductName=?, CategoryId=?, " +
                        "CategoryName=?, ProductColorBg=?, ProductImage=?, ProductAlias=?, " +
                        "ProductDescription=?, DateModified=? Where Id=?",
                        ParentProductsModel.ParentProductName, ParentProductsModel.CategoryId, ParentProductsModel.CategoryName,
                        ParentProductsModel.ProductColorBg, ParentProductsModel.ProductImage, ParentProductsModel.ProductAlias,
                        ParentProductsModel.ProductDescription, ParentProductsModel.DateModified,
                        ParentProductsModel.Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool DeleteFromTable(int Id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Delete<ParentProductsModel>(Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool NameExists(string name)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<ParentProductsModel>("SELECT * FROM ParentProductsModel Where ParentProductName=?", name);
                    return x.Count == 0 ? false : true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<ParentProductsModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                { 
                    var x = connection.Query<ParentProductsModel>("SELECT * FROM ParentProductsModel Where Id=?", id);
                    return x;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public int GetLatestParentProductId()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<ParentProductsModel>("SELECT * FROM ParentProductsModel ORDER BY Id DESC LIMIT 1");
                    return x[0].Id;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }
    }
}