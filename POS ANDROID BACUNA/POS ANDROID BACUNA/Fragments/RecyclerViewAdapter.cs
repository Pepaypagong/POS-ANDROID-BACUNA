using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using Product = POS_ANDROID_BACUNA.Data_Classes.Product;
using POS_ANDROID_BACUNA.Data_Classes;

namespace POS_ANDROID_BACUNA.Fragments
{
    class RecyclerViewAdapter : RecyclerView.Adapter
    {
        RecyclerView mRecyclerView;
        CardView mCardViewRowItem;
        CardView mCardViewItemHolder;
        LinearLayout mllItemHolder;
        int mGridHeight;
        float mDpVal;
        private List<Product> mProducts;
        bool mIsGrid;
        CheckoutFragment mCheckoutFragment;
        Button mBtnCheckoutButton;
        Context mCheckoutContext;

        public RecyclerViewAdapter(float dpVal, int gridHeight, List<Product> products, bool isGrid, RecyclerView recyclerView, Button btnCheckoutButton, Context checkoutContext)
        {
            mGridHeight = gridHeight;
            mDpVal = dpVal;
            mProducts = products;
            mIsGrid = isGrid;
            mRecyclerView = recyclerView;
            mCheckoutFragment = new CheckoutFragment();
            mBtnCheckoutButton = btnCheckoutButton;
            mCheckoutContext = checkoutContext;
        }

        public class MyViewHolder : RecyclerView.ViewHolder
        {
            public View mMainView { get; set; }
            public TextView mProductName { get; set; }
            public TextView mProductPrice { get; set; }
            public LinearLayout mItemBackgroudHolderGrid { get; set; }
            public CardView mItemBackgroudHolderListView { get; set; }
            public ImageView mItemImage { get; set; }

            public MyViewHolder(View view) : base(view)
            {
                mMainView = view;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = mIsGrid ? LayoutInflater.From(parent.Context).Inflate(Resource.Layout.checkout_fragment_list_item_grid, parent, false) :
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.checkout_fragment_list_item_listview, parent, false);

            if (mIsGrid)
            {
                mCardViewRowItem = itemView.FindViewById<CardView>(Resource.Id.recyclerViewItem);
                mllItemHolder = itemView.FindViewById<LinearLayout>(Resource.Id.llItemHolder);
            }
            else
            {
                mCardViewItemHolder = itemView.FindViewById<CardView>(Resource.Id.CardViewItemHolderListView);
            }

            ImageView imgItemImage = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            TextView txtProductName = itemView.FindViewById<TextView>(Resource.Id.txtProductName);
            TextView txtProductPrice = itemView.FindViewById<TextView>(Resource.Id.txtProductPrice);
            

            MyViewHolder view = new MyViewHolder(itemView)
            {
                mProductName = txtProductName,
                mProductPrice = txtProductPrice,
                mItemBackgroudHolderGrid = mllItemHolder,
                mItemBackgroudHolderListView = mCardViewItemHolder,
                mItemImage = imgItemImage
            };

            int _topMargin = DpToPixel(2);
            int _bottomMargin = DpToPixel(5);
            int _leftMargin = DpToPixel(5);
            int _rightMargin = DpToPixel(5);

            int _rowCount = mIsGrid ? 4 : 7; //itenerary if isgrid=true rows=4 else 7 //unused

            int _heightBalance = (_topMargin + _bottomMargin)*_rowCount;

            if (mIsGrid)
            {
                CardView.LayoutParams layoutParams = new CardView.LayoutParams(CardView.LayoutParams.MatchParent, (mGridHeight - _heightBalance) / _rowCount);
                layoutParams.SetMargins(_leftMargin, _topMargin, _rightMargin, _bottomMargin);
                mCardViewRowItem.LayoutParameters = layoutParams;
                mCardViewRowItem.RequestLayout();
            }

            return view;
        }

        private int DpToPixel(int val)
        { 
            return val * (int)Math.Ceiling(mDpVal);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            MyViewHolder myHolder = viewHolder as MyViewHolder;
            myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
            myHolder.mMainView.Click += MMainView_Click; //set click event for row
            myHolder.mProductName.Text = mProducts[position].productName;
            //condition here to display price based on transaction pricing type
            myHolder.mProductPrice.Text = "\u20b1 "+String.Format("{0:n}",mProducts[position].productRetailPrice);
            if (mIsGrid)
            {
                myHolder.mItemBackgroudHolderGrid.SetBackgroundColor(Android.Graphics.Color.ParseColor("#" + (mProducts[position].productColorBg)));
            }
            else
            {
                myHolder.mItemBackgroudHolderListView.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#" + (mProducts[position].productColorBg)));
            }

            if (mProducts[position].productImage != null)
            {
                //set item image here
            }
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            int position = mRecyclerView.GetChildAdapterPosition((View)sender);

            //add to cart
            GlobalVariables.globalProductsCart.Add(new Product()
            {
                productId = mProducts[position].productId,
                productName = mProducts[position].productName,
                productRetailPrice = Convert.ToDecimal(mProducts[position].productRetailPrice),
                productColorBg = mProducts[position].productColorBg
            });

            bool alreadyExists = GlobalVariables.globalProductsOnCart.Any(x => x.productId == mProducts[position].productId);

            if (alreadyExists)
            {
                foreach (var item in GlobalVariables.globalProductsOnCart)
                {
                    if (item.productId == mProducts[position].productId)
                    {
                        item.productPrice = item.productOrigPrice; //update price on click to reset the price on cart
                        item.productDiscountAmount = 0.00M; //reset discount amt to 0 if already on cart
                        item.productDiscountPercentage = 0.00M; //reset discount percentage to 0 if already on cart
                        item.productCountOnCart = item.productCountOnCart + 1;
                        item.productSubTotalPrice = (item.productCountOnCart) * item.productPrice;
                    }
                }
            }
            else {
                GlobalVariables.globalProductsOnCart.Add(new ProductsOnCart()
                {
                    productId = mProducts[position].productId,
                    productName = mProducts[position].productName,
                    productOrigPrice = Convert.ToDecimal(mProducts[position].productRetailPrice),
                    productPrice = Convert.ToDecimal(mProducts[position].productRetailPrice),
                    productCountOnCart = 1,
                    productCategoryId = 1,
                    productSubTotalPrice = Convert.ToDecimal(mProducts[position].productRetailPrice),
                    productDiscountAmount = 0.00M,
                    productDiscountPercentage = 0.00M
                }); 
            }
            //update checkoutbutton.
            mCheckoutFragment.SetCheckoutButtonTotal(mBtnCheckoutButton, mCheckoutContext);
        }

        public override int ItemCount
        {
            get
            {
                return mProducts.Count;
            }
        }

    }
}