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
    class SettingsDataAccess
    {
        string connectionString = SQLiteConnetionString.LoadConnectionString();

        public SettingsDataAccess()
        {
            CreateTable();
            if (SelectTable().Count < 1)
            {
                var model = new SettingsModel
                {
                    Id = 1,
                    ReceiptCompanyName = "Bacuna RTW",
                    ReceiptAddressLine1 = "BEELING BUILDING, BRGY. BACLARAN",
                    ReceiptAddressLine2 = "PARANAQUE CITY, PHILIPPINES",
                    ReceiptContactNumber = "09174897988",
                    ReceiptFooterNote = "Please come again, thank you :)",
                    ReceiptPrinterAddress = "",
                    ReceiptPrinterName = ""
                };
                InsertIntoTable(model);
            }
        }

        public bool InsertIntoTable(SettingsModel row)
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

        public bool CreateTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    //connection.DeleteAll<SettingsModel>();
                    connection.CreateTable<SettingsModel>(); //automatic create table if not exists
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<SettingsModel> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    return connection.Table<SettingsModel>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public bool UpdateTable(SettingsModel model)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Query<SettingsModel>("UPDATE SettingsModel set ReceiptCompanyName=?, ReceiptAddressLine1=?, ReceiptAddressLine2=?, " +
                        "ReceiptContactNumber=?, ReceiptFooterNote=?, ReceiptPrinterAddress=?, ReceiptPrinterName=? Where Id=?",
                        model.ReceiptCompanyName, model.ReceiptAddressLine1, model.ReceiptAddressLine2,
                        model.ReceiptContactNumber, model.ReceiptFooterNote, model.ReceiptPrinterAddress, model.ReceiptPrinterName, model.Id);
                    connection.Update(model);
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