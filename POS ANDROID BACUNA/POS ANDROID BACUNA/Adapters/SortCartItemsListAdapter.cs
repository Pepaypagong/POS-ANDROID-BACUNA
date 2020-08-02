using System;
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

namespace POS_ANDROID_BACUNA
{
    class SortCartItemsListAdapter : BaseAdapter<OrderFields>
    {
        private List<OrderFields> mItems;
        private Context mContext;
        private ListView mListView;

        public SortCartItemsListAdapter(Context context, List<OrderFields> items, ListView listview)
        {
            mItems = items;
            mContext = context;
            mListView = listview;
        }
        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override OrderFields this[int position]
        {
            get { return mItems[position]; }
        }

        public void RefreshList(List<OrderFields> orderFields)
        {
            this.mItems.Clear();
            this.mItems.AddRange(orderFields);
            NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.sortCartItemsDialogFragmentRow, null, false);
            }

            var cbFieldName = row.FindViewById<CheckBox>(Resource.Id.cbFieldName);
            var radioBtnAscending = row.FindViewById<RadioButton>(Resource.Id.radioBtnAscending);
            var radioBtnDescending = row.FindViewById<RadioButton>(Resource.Id.radioBtnDescending);
            var txtFieldId = row.FindViewById<TextView>(Resource.Id.txtFieldId);
            txtFieldId.Text = mItems[position].FieldId.ToString();
            cbFieldName.Checked = mItems[position].IsChecked ? true : false;
            cbFieldName.Text = mItems[position].FieldName;
            radioBtnAscending.Checked = mItems[position].IsDesc ? false : true;
            radioBtnDescending.Checked = mItems[position].IsDesc ? true : false;
            cbFieldName.Click -= CbFieldName_Click;
            cbFieldName.Click += CbFieldName_Click;
            radioBtnAscending.Click -= RadioBtnAscending_Click;
            radioBtnAscending.Click += RadioBtnAscending_Click;
            radioBtnDescending.Click -= RadioBtnDescending_Click;
            radioBtnDescending.Click += RadioBtnDescending_Click;
            SetRowColors(mItems[position].IsChecked ? true : false, row, cbFieldName, radioBtnAscending, radioBtnDescending);
            return row;
        }

        private void RadioBtnDescending_Click(object sender, EventArgs e)
        {
            FindFieldId(sender, true);
        }

        private void RadioBtnAscending_Click(object sender, EventArgs e)
        {
            FindFieldId(sender, false);
        }

        private void FindFieldId(object _sender, bool _isDesc)
        {
            View parentView = (View)((RadioButton)_sender).Parent.Parent;
            var txtFieldId = parentView.FindViewById<TextView>(Resource.Id.txtFieldId);
            UpdateOrderBy(Convert.ToInt32(txtFieldId.Text),_isDesc);
        }

        private void UpdateOrderBy(int _fieldId, bool _isDesc)
        {
            foreach (var item in GlobalVariables.globalSortingFieldList.Where(x => x.FieldId == _fieldId))
            {
                item.IsDesc = _isDesc;
            }
        }

        private void SetRowColors(bool isEnabled, View _rowContainerView, CheckBox _cbFieldName, 
            RadioButton _radioBtnAscending, RadioButton _radioBtnDescending)
        {
            _rowContainerView.SetBackgroundColor(isEnabled?
                ColorHelper.ResourceIdToColor(Resource.Color.colorPrimary, mContext):
                ColorHelper.ResourceIdToColor(Resource.Color.colorBlurredBackground, mContext));
            _cbFieldName.SetTextColor(isEnabled ?
                ColorHelper.ResourceIdToColor(Resource.Color.colorTextEnabled, mContext) :
                ColorHelper.ResourceIdToColor(Resource.Color.colorBlurred, mContext));
            _radioBtnAscending.SetTextColor(isEnabled ?
                ColorHelper.ResourceIdToColor(Resource.Color.colorTextEnabled, mContext) :
                ColorHelper.ResourceIdToColor(Resource.Color.colorBlurred, mContext));
            _radioBtnDescending.SetTextColor(isEnabled ?
                ColorHelper.ResourceIdToColor(Resource.Color.colorTextEnabled, mContext) :
                ColorHelper.ResourceIdToColor(Resource.Color.colorBlurred, mContext));
            _cbFieldName.ButtonTintList = mContext.GetColorStateList(isEnabled ? Resource.Color.colorAccent : Resource.Color.colorBlurred);
            _radioBtnAscending.ButtonTintList = mContext.GetColorStateList(isEnabled ? Resource.Color.colorAccent : Resource.Color.colorBlurred);
            _radioBtnDescending.ButtonTintList = mContext.GetColorStateList(isEnabled ? Resource.Color.colorAccent : Resource.Color.colorBlurred);
        }

        private void CbFieldName_Click(object sender, EventArgs e)
        {
            View parentView = (View)((CheckBox)sender).Parent;
            var txtFieldId = parentView.FindViewById<TextView>(Resource.Id.txtFieldId);
            bool isCbChecked = ((CheckBox)sender).Checked;
            int fieldId = Convert.ToInt32(txtFieldId.Text);
            if (isCbChecked) //if checked
            {
                UpRank(fieldId);
            }
            else//unchecked
            {
                DownRank(fieldId);
            }
            RefreshList(GlobalVariables.globalSortingFieldList.OrderByDescending(x => x.FieldRank).ToList());
        }

        private void UpRank(int _fieldId)
        {
            int fieldCount = GlobalVariables.globalSortingFieldList.Count;
            foreach (var item in GlobalVariables.globalSortingFieldList.Where(x=>x.FieldId == _fieldId))
            {
                item.FieldRank = GetUpdatedRankCount(fieldCount);
                item.IsChecked = true;
            }
        }

        private void DownRank(int _fieldId)
        {
            foreach (var item in GlobalVariables.globalSortingFieldList.Where(x => x.FieldId == _fieldId))
            {
                item.FieldRank = 0;
                item.IsChecked = false;
            }
        }

        private int GetUpdatedRankCount(int _fieldCount)
        {
            int retVal;
            int lowestRank = GlobalVariables.globalSortingFieldList
                            .Where(x => x.IsChecked == true)
                            .OrderBy(x => x.FieldRank)
                            .Select(x => x.FieldRank)
                            .FirstOrDefault();
            if (lowestRank == 0)
            {
                retVal = _fieldCount;
            }
            else
            {
                retVal = lowestRank - 1;
            }
            return retVal;
        }
    }
}