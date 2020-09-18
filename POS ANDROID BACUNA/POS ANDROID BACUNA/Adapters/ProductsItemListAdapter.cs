using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using POS_ANDROID_BACUNA.Data_Classes;
using POS_ANDROID_BACUNA.Fragments;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace POS_ANDROID_BACUNA.Adapters
{
    public class ProductsItemListAdapter : BaseExpandableListAdapter
    {
        private SupportFragment _supportFragment;
        private List<ParentProductsModel> _listDataHeader; // header titles
                                              // child data in format of header title, child title
        private Dictionary<int, List<ProductsModel>> _listDataChild;
        private ExpandableListView _expandableListView;

        string pesoSign = "\u20b1 ";

        public ProductsItemListAdapter(SupportFragment supportFragment, List<ParentProductsModel> listDataHeader, 
            Dictionary<int,List<ProductsModel>> listChildData, ExpandableListView expadableListview)
        {
            _supportFragment = supportFragment;
            _listDataHeader = listDataHeader;
            _listDataChild = listChildData;
            _expandableListView = expadableListview;
        }
        //for cchild item view
        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            throw new NotImplementedException();
        }

        protected ProductsModel GetProductChild(int groupPosition, int childPosition)
        {
            return _listDataChild[_listDataHeader[groupPosition].Id][childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            ProductsModel child = GetProductChild(groupPosition, childPosition);

            if (convertView == null)
            {
                convertView = _supportFragment.LayoutInflater.Inflate(Resource.Layout.products_fragment_items_list_item_child, parent, false);
            }
            TextView txtListChild = (TextView)convertView.FindViewById(Resource.Id.lblListItem);
            TextView txtWholesalePrice = (TextView)convertView.FindViewById(Resource.Id.txtWsPrice);
            TextView txtRetailPrice = (TextView)convertView.FindViewById(Resource.Id.txtRetailPrice);
            TextView txtRunnerPrice = (TextView)convertView.FindViewById(Resource.Id.txtRunnerPrice);
            txtListChild.Text = child.ProductName;
            txtWholesalePrice.Text = pesoSign + string.Format("{0:n}", child.ProductWholesalePrice);
            txtRetailPrice.Text = pesoSign + string.Format("{0:n}", child.ProductRetailPrice);
            txtRunnerPrice.Text = pesoSign + string.Format("{0:n}", child.ProductRunnerPrice);
            return convertView;
        } 

        public override int GetChildrenCount(int groupPosition)
        {
            return _listDataChild[_listDataHeader[groupPosition].Id].Count;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            throw new NotImplementedException();
        }

        protected ParentProductsModel GetProductGroup(int groupPosition)
        {
            return _listDataHeader[groupPosition];
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
            //string headerTitle = (string)GetGroup(groupPosition);
            ParentProductsModel group = GetProductGroup(groupPosition);
            string headerTitle = group.ParentProductName;

            convertView = convertView ?? _supportFragment.LayoutInflater.Inflate(Resource.Layout.products_fragment_items_list_item_parent, parent, false);
            var lblListHeader = (TextView)convertView.FindViewById(Resource.Id.lblListHeader);
            var cardViewItemImageHolder = (CardView)convertView.FindViewById(Resource.Id.cardViewItemImageHolder);
            var txtItemAlias = convertView.FindViewById<TextView>(Resource.Id.txtItemAlias);
            var txtParentProductId = (TextView)convertView.FindViewById(Resource.Id.txtParentProductId);
            var rlEdit = (RelativeLayout)convertView.FindViewById(Resource.Id.rlEdit);
            var imgEdit = (ImageView)convertView.FindViewById(Resource.Id.btnEditParentItem);

            lblListHeader.Text = headerTitle;
            txtParentProductId.Text = group.Id.ToString();
            cardViewItemImageHolder.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#" + group.ProductColorBg));
            txtItemAlias.Text = group.ProductAlias;

            rlEdit.Click -= RlEdit_Click;
            rlEdit.Click += RlEdit_Click;
            imgEdit.Click -= ImgEdit_Click;
            imgEdit.Click += ImgEdit_Click;

            return convertView;
        }

        public ViewGroup GetImgBtnEditParent(ImageView _senderImgView)
        {
            ImageView x = _senderImgView;
            ViewGroup parent;
            return parent = (ViewGroup)x.Parent.Parent;
        }
        public ViewGroup GetRlEditParent(RelativeLayout _senderImgView)
        {
            RelativeLayout x = _senderImgView;
            ViewGroup parent;
            return parent = (ViewGroup)x.Parent;
        }
        private void RlEdit_Click(object sender, EventArgs e)
        {
            if (!GlobalVariables.mIsProductsFragmentEditItemOpened)
            {
                GlobalVariables.mIsProductsFragmentEditItemOpened = true;
                TextView txtParentProductId = GetRlEditParent((RelativeLayout)sender).FindViewById<TextView>(Resource.Id.txtParentProductId);
                int parentProductId = Convert.ToInt32(txtParentProductId.Text);
                Intent intent = new Intent(_supportFragment.Context, typeof(ProductsFragmentItemsAddItemActivity));
                intent.PutExtra("editParentProductId", parentProductId);
                intent.PutExtra("isEdit", true);
                _supportFragment.StartActivityForResult(intent, 16);
            }
        }

        private void ImgEdit_Click(object sender, EventArgs e)
        {
            if (!GlobalVariables.mIsProductsFragmentEditItemOpened)
            {
                GlobalVariables.mIsProductsFragmentEditItemOpened = true;
                //position = _expandableListView.GetPositionForView((View)sender); //get position of view on listview
                TextView txtParentProductId = GetImgBtnEditParent((ImageView)sender).FindViewById<TextView>(Resource.Id.txtParentProductId);
                int parentProductId = Convert.ToInt32(txtParentProductId.Text);

                //Toast.MakeText(_supportFragment.Context, "Edit Clicked for ID: " + parentProductId.ToString() +
                //    " Item Name: " + parentProductName, ToastLength.Long).Show();

                //show edit activity
                Intent intent = new Intent(_supportFragment.Context, typeof(ProductsFragmentItemsAddItemActivity));
                intent.PutExtra("editParentProductId", parentProductId);
                intent.PutExtra("isEdit", true);
                _supportFragment.StartActivityForResult(intent, 16);
            }
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