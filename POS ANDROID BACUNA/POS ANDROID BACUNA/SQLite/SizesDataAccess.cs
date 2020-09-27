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
    class SizesDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<ProductSizesModel>();
                    connection.CreateTable<ProductSizesModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(ProductSizesModel row)
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
        public List<ProductSizesModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<ProductSizesModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTable(ProductSizesModel ProductSizesModel)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ProductSizesModel>("UPDATE ProductSizesModel set ProductSizeName=? Where Id=?",
                        ProductSizesModel.ProductSizeName, ProductSizesModel.Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool UpdateRank(int id, int rank)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<ProductSizesModel>("UPDATE ProductSizesModel set SizeRank=? Where Id=?",
                        rank, id);
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
                    connection.Delete<ProductSizesModel>(Id);
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
                    var x = connection.Query<ProductSizesModel>("SELECT * FROM ProductSizesModel Where ProductSizeName=?", name);
                    return x.Count == 0 ? false : true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<ProductSizesModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                { 
                    var x = connection.Query<ProductSizesModel>("SELECT * FROM ProductSizesModel Where Id=?", id);
                    return x;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public int GetMaxSizeRank()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<ProductSizesModel>("SELECT * FROM ProductSizesModel ORDER BY SizeRank DESC LIMIT 1");
                    try
                    {
                        return x[0].SizeRank;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }

        public int GetMaxId()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<ProductSizesModel>("SELECT * FROM ProductSizesModel ORDER BY Id DESC LIMIT 1");
                    try
                    {
                        return x[0].Id;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }

        public bool InsertAll(List<ProductSizesModel> sizes)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.InsertAll(sizes);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool DeleteAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.DeleteAll<ProductSizesModel>();
                    return true;
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