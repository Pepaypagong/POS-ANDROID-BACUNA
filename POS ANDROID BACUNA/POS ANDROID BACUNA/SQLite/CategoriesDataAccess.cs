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
    class CategoriesDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();
        public CategoriesDataAccess()
        {
            //insert "All" Category as Id 1
            if (!NameExists("All"))
            {
                var Category = new ProductCategoriesModel {
                    ProductCategoryName = "All"
                };
                InsertIntoTable(Category);
            }
        }

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<ProductCategoriesModel>();
                    connection.CreateTable<ProductCategoriesModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(ProductCategoriesModel row)
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
        public List<ProductCategoriesModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<ProductCategoriesModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTable(ProductCategoriesModel ProductCategoriesModel)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ProductCategoriesModel>("UPDATE ProductCategoriesModel set ProductCategoryName=? Where Id=?",
                        ProductCategoriesModel.ProductCategoryName, ProductCategoriesModel.Id);
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
                    connection.Delete<ProductCategoriesModel>(Id);
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
                    var x = connection.Query<ProductCategoriesModel>("SELECT * FROM ProductCategoriesModel Where ProductCategoryName=?", name);
                    return x.Count == 0 ? false : true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<ProductCategoriesModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                { 
                    var x = connection.Query<ProductCategoriesModel>("SELECT * FROM ProductCategoriesModel Where Id=?", id);
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