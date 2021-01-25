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
    class TransactionsDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<TransactionsModel>();
                    connection.CreateTable<TransactionsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(TransactionsModel trasactionsModel)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Insert(trasactionsModel);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<TransactionsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<TransactionsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTransactionStatusToPaid(TransactionsModel _model)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<TransactionsModel>("UPDATE TransactionsModel set DateModified=?, IsPaid=?, PaymentCashAmount=?, " +
                        "PaymentCheckAmount=? Where id=?",
                        _model.DateModified, _model.IsPaid, _model.PaymentCashAmount, _model.PaymentCheckAmount, _model.id);
                    connection.Update(_model);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool DeleteFromTable(int transactionId)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Delete<TransactionsModel>(transactionId);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool RecordExists(int Id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<TransactionsModel>("SELECT * FROM TransactionsModel Where Id=?", Id);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<TransactionsModel> SelectRecord(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<TransactionsModel>("SELECT * FROM TransactionsModel Where Id=?", id);
                    return x;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public int GetLatestTransactionId()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    var x = connection.Query<TransactionsModel>("SELECT * FROM TransactionsModel ORDER BY id DESC LIMIT 1");
                    return x[0].id;
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