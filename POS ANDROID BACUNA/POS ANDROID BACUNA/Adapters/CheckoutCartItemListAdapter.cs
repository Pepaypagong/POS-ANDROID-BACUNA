﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Data_Classes;

namespace POS_ANDROID_BACUNA.Adapters
{
    public class CheckoutCartItemListAdapter : BaseExpandableListAdapter
    {
        private Activity _context;
        private List<ProductsOnCart> _listDataHeader; // header titles
                                              // child data in format of header title, child title
        private Dictionary<string, List<string>> _listDataChild;

        public CheckoutCartItemListAdapter(Activity context, List<ProductsOnCart> listDataHeader, Dictionary<String, List<string>> listChildData)
        {
            _context = context;
            _listDataHeader = listDataHeader;
            _listDataChild = listChildData;
        }
        //for cchild item view
        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return _listDataChild[_listDataHeader[groupPosition].productId.ToString()][childPosition];
        }
        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            string childText = (string)GetChild(groupPosition, childPosition);
            string pesoSign = "\u20b1";
            string itemsOrItem = " items";
            if (_listDataHeader[groupPosition].productCountOnCart == 1)
            {
                itemsOrItem = "item";
            }
            string qtyOnCart = _listDataHeader[groupPosition].productCountOnCart.ToString() + itemsOrItem;
            string itemPrice = pesoSign + String.Format("{0:n}", _listDataHeader[groupPosition].productPrice);

            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.checkout_fragment_cart_list_item_child, parent, false);
            }
            TextView txtQtyOnCart = (TextView)convertView.FindViewById(Resource.Id.txtItemQtyOnCart);
            TextView txtItemPrice = (TextView)convertView.FindViewById(Resource.Id.txtItemPrice);
            TextView txtItemDiscount = (TextView)convertView.FindViewById(Resource.Id.txtItemDiscount);
            LinearLayout llQtyOnCart = (LinearLayout)convertView.FindViewById(Resource.Id.llItemsOnCart);
            LinearLayout llItemPrice = (LinearLayout)convertView.FindViewById(Resource.Id.llItemPriceOnCart);

            txtQtyOnCart.Text = qtyOnCart;
            txtItemPrice.Text = itemPrice;
            llQtyOnCart.Click += delegate (object sender, EventArgs e) {
                Toast.MakeText(_context, "Qty = " + qtyOnCart, ToastLength.Short).Show();
            };
            llItemPrice.Click += delegate (object sender, EventArgs e) {
                Toast.MakeText(_context, "PRICE = " + itemPrice, ToastLength.Short).Show();
            };

            return convertView;
        }
        public override int GetChildrenCount(int groupPosition)
        {
            return _listDataChild[_listDataHeader[groupPosition].productId.ToString()].Count;
        }
        //For header view
        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return _listDataHeader[groupPosition].productId;
        }
        public override int GroupCount
        {
            get
            {
                return _listDataHeader.Count;
            }
        }
        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            string pesoSign = "\u20b1";
            string productName = _listDataHeader[groupPosition].productName; //(string)GetGroup(groupPosition);
            string productQty = _listDataHeader[groupPosition].productCountOnCart.ToString() + " X";
            string productPrice = pesoSign + String.Format("{0:n}",_listDataHeader[groupPosition].productPrice);
            string productSubTotal = pesoSign + String.Format("{0:n}", _listDataHeader[groupPosition].productSubTotalPrice);

            convertView = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.checkout_fragment_cart_list_item_parent, parent, false);

            var txtItemName = (TextView)convertView.FindViewById(Resource.Id.txtItemName);
            var txtPrice = (TextView)convertView.FindViewById(Resource.Id.txtItemPrice);
            var txtQty = (TextView)convertView.FindViewById(Resource.Id.txtQty);
            var txtSubTotal = (TextView)convertView.FindViewById(Resource.Id.txtSubTotal);

            if (convertView != null)
            {
                if (isExpanded)
                {
                    int colorInt = _context.GetColor(Resource.Color.colorPrimary);
                    Color white = new Color(colorInt);
                    convertView.SetBackgroundColor(white);

                    int colorInt2 = _context.GetColor(Resource.Color.colorLightBlack);
                    Color colorLightBlack = new Color(colorInt2);

                    txtItemName.SetTextColor(colorLightBlack);
                    txtPrice.SetTextColor(colorLightBlack);
                    txtSubTotal.SetTextColor(colorLightBlack);
                    txtQty.SetTextColor(colorLightBlack);
                }
                else
                {
                    if (GlobalVariables.mIsAllCollapsed)
                    {
                        int colorInt = _context.GetColor(Resource.Color.colorPrimary);
                        Color white = new Color(colorInt);
                        convertView.SetBackgroundColor(white);

                        int colorInt2 = _context.GetColor(Resource.Color.colorLightBlack);
                        Color colorLightBlack = new Color(colorInt2);
                        txtItemName.SetTextColor(colorLightBlack);
                        txtPrice.SetTextColor(colorLightBlack);
                        txtSubTotal.SetTextColor(colorLightBlack);
                        txtQty.SetTextColor(colorLightBlack);
                    }
                    else
                    {
                        int colorInt = _context.GetColor(Resource.Color.colorBlurredBackground);
                        Color blurBackground = new Color(colorInt);
                        convertView.SetBackgroundColor(blurBackground);

                        int colorInt2 = _context.GetColor(Resource.Color.colorBlurred);
                        Color colorBlurred = new Color(colorInt2);
                        txtItemName.SetTextColor(colorBlurred);
                        txtPrice.SetTextColor(colorBlurred);
                        txtSubTotal.SetTextColor(colorBlurred);
                        txtQty.SetTextColor(colorBlurred);
                    }
                    
                }
            }

            txtItemName.Text = productName;
            txtPrice.Text = productPrice;
            txtQty.Text = productQty;
            txtSubTotal.Text = productSubTotal;

            return convertView;
        }
        public override bool HasStableIds
        {
            get
            {
                return false;
            }
        }
        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        class ViewHolderItem : Java.Lang.Object
        {
        }
    }
}