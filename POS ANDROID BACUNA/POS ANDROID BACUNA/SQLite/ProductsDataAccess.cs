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
    class ProductsDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<ProductsModel>();
                    connection.CreateTable<ProductsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(ProductsModel row)
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
        public List<ProductsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<ProductsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTable(ProductsModel ProductsModel)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ProductsModel>("UPDATE ProductsModel set ProductName=?, ProductCategoryId=?, " +
                        "ProductCategory=?, ProductCost=?, ProductRetailPrice=?, ProductWholesalePrice=?, ProductRunnerPrice=?, ProductColorBg=?," +
                        "ProductImage=?, ProductAlias=?, ProductCode=?, DateModified=? Where Id=?",
                        ProductsModel.ProductName, ProductsModel.ProductCategoryId, ProductsModel.ProductCategory,
                        ProductsModel.ProductCost, 
                        ProductsModel.ProductRetailPrice, ProductsModel.ProductWholesalePrice, ProductsModel.ProductRunnerPrice,
                        ProductsModel.ProductColorBg, ProductsModel.ProductImage, ProductsModel.ProductAlias, ProductsModel.ProductCode,
                        ProductsModel.DateModified, ProductsModel.Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool DeleteFromTable(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Delete<ProductsModel>(id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool DeleteFromTableUsingParentProductId(int _parentProductId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ProductsModel>("DELETE FROM ProductsModel WHERE ParentProductId = ?", _parentProductId);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<ProductsModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                { 
                    var x = connection.Query<ProductsModel>("SELECT * FROM ProductsModel Where Id=?", id);
                    return x;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool ProductCodeExists(string productCode)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<ProductsModel>("SELECT * FROM ProductsModel Where ProductCode=?", productCode);
                    return x.Count == 0 ? false : true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

    }
}