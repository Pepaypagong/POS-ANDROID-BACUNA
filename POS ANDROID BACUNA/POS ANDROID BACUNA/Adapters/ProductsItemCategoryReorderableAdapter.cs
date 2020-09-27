using Android.Views;
using System.Collections.ObjectModel;
using Android.Support.V7.Widget;
using Android.Widget;
using static Android.Views.View;
using POS_ANDROID_BACUNA.Interfaces;
using POS_ANDROID_BACUNA.Data_Classes;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using POS_ANDROID_BACUNA.SQLite;
using System.Linq;
using Android.Content;
using SupportFragment = Android.Support.V4.App.Fragment;
using System;
using Java.Util;
using System.Xml.Serialization;


namespace POS_ANDROID_BACUNA.Fragments
{
    public class ProductsItemCategoryReorderableAdapter : RecyclerView.Adapter, ITemTouchHelperAdapter
    {
        private SupportFragment mSupportFragment;
        private List<ProductCategoriesModel> mItems;
        private readonly IOnStartDragListener mDragStartListener;
        private CategoriesDataAccess mCategoriesDataAccess;
        private bool mShowReorderableIcon;

        public ProductsItemCategoryReorderableAdapter(List<ProductCategoriesModel> list, IOnStartDragListener dragStartListener,
            SupportFragment supportFragment)
        {
            mItems = list;
            mDragStartListener = dragStartListener;
            mCategoriesDataAccess = new CategoriesDataAccess();
            mSupportFragment = supportFragment;
            mShowReorderableIcon = true;
        }

        public override int ItemCount => mItems.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ProductsCategoryViewHolder;
            if (viewHolder == null) return;
            viewHolder.RecordId.Text = mItems[position].Id.ToString();
            viewHolder.ResourceName.Text = (mItems[position].ProductCategoryName);
            viewHolder.ItemView.LongClick += delegate(object o, LongClickEventArgs e) {
                mDragStartListener.OnStartDrag(holder);
            };
            if (!viewHolder.ItemView.HasOnClickListeners)
            {
                viewHolder.ItemView.Click += ItemView_Click;
            }
            viewHolder.ReorderIcon.Visibility = mShowReorderableIcon ? ViewStates.Visible : ViewStates.Invisible;
        }

        private void ItemView_Click(object sender, System.EventArgs e)
        {
            string id = ((ViewGroup)sender).FindViewById<TextView>(Resource.Id.txtRecordId).Text;
            Intent intent = new Intent(mSupportFragment.Context, typeof(ProductsFragmentAddCategoryOrSizeActivity));
            intent.PutExtra("isSize", false);
            intent.PutExtra("isEdit", true);
            intent.PutExtra("selectedRecordId", Convert.ToInt32(id));
            mSupportFragment.StartActivityForResult(intent, 38);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var itemView = inflater.Inflate(Resource.Layout.products_fragment_categories_list_row_item, parent, false);
            var itemViewHolder = new ProductsCategoryViewHolder(itemView);
            return itemViewHolder;
        }

        public void OnItemDismiss(int position)
        {
            var item = mItems[position];
            mItems.Remove(item);
            NotifyItemRemoved(position);
        }

        public bool OnItemDrop(int fromPosition, int toPosition)
        {
            UpdateSizeRank();
            mItems = mCategoriesDataAccess.SelectTable().Where(x=>x.Id != 1).OrderBy(x => x.Rank).ToList();
            ProductsFragmentCategoriesFragment.mCategoriesList = mItems;
            ((MainActivity)mSupportFragment.Activity).RefreshCategoryTabsOnCheckout();
            return true;
        }
        public void UpdateSizeRank()
        {
            int counter = 2;
            foreach (var item in mItems.Where(x=>x.Id != 1))
            {
                mCategoriesDataAccess.UpdateRank(item.Id, counter);
                counter += 1;
            }
        }

        public bool OnItemMove(int fromPosition, int toPosition)
        {
            SwapItems(fromPosition, toPosition);
            NotifyItemMoved(fromPosition, toPosition);
            return true;
        }

        private void SwapItems(int fromPosition, int toPosition)
        {
            if (fromPosition < toPosition)
            {
                for (int i = fromPosition; i < toPosition; i++)
                {
                    SwapOnData(i, i + 1);
                }
            }
            else
            {
                for (int i = fromPosition; i > toPosition; i--)
                {
                    SwapOnData(i, i - 1);
                }
            }
        }

        public void SwapOnData(int from, int to)
        {
            var tempPlanResource = mItems[from];
            mItems[from] = mItems[to];
            mItems[to] = tempPlanResource;
        }

        public void RefreshList(List<ProductCategoriesModel> updatedList)
        {
            mItems = updatedList;
            NotifyDataSetChanged();
        }

        public void ShowReorderableIcon(bool show)
        {
            mShowReorderableIcon = show;
            NotifyDataSetChanged();
        }

    }

    public class ProductsCategoryViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout ReorderView;
        public ImageView ReorderIcon;
        public TextView ResourceName;
        public TextView RecordId;

        public ProductsCategoryViewHolder(View view) : base(view)
        {
            ResourceName = view.FindViewById<TextView>(Resource.Id.mTVResourceName);
            ReorderView = view.FindViewById<LinearLayout>(Resource.Id.ReorderView);
            ReorderIcon = view.FindViewById<ImageView>(Resource.Id.mTvReorderIcon);
            RecordId = view.FindViewById<TextView>(Resource.Id.txtRecordId);
        }
    }
}