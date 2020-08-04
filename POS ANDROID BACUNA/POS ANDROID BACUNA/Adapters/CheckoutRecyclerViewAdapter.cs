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
using POS_ANDROID_BACUNA.Fragments;

namespace POS_ANDROID_BACUNA.Adapters
{
    class CheckoutRecyclerViewAdapter : RecyclerView.Adapter
    {
        private SupportFragment mSupportFragment;
        RecyclerView mRecyclerView;
        CardView mCardViewRowItem;
        CardView mCardViewItemHolder;
        LinearLayout mllItemHolder;
        int mGridHeight;
        float mDpVal;
        private List<Product> mProducts;
        private List<ParentProducts> mParentProducts;
        bool mShowSizes;
        bool mIsGrid;
        CheckoutFragment mCheckoutFragment;
        Button mBtnCheckoutButton;
        Context mCheckoutContext;
        string mCurrentSelectedPricingType;

        public CheckoutRecyclerViewAdapter(float dpVal, int gridHeight, List<Product> products,
            List<ParentProducts> parentProducts, bool isGrid, RecyclerView recyclerView, 
            Button btnCheckoutButton, Context checkoutContext, bool showSizes, SupportFragment supportFragment, string pricingType)
        {
            mGridHeight = gridHeight;
            mDpVal = dpVal;
            mProducts = products;
            mParentProducts= parentProducts;
            mIsGrid = isGrid;
            mRecyclerView = recyclerView;
            mCheckoutFragment = new CheckoutFragment();
            mBtnCheckoutButton = btnCheckoutButton;
            mCheckoutContext = checkoutContext;
            mShowSizes = showSizes;
            mSupportFragment = supportFragment;
            mCurrentSelectedPricingType = pricingType;
        }

        public class MyViewHolder : RecyclerView.ViewHolder
        {
            public View mMainView { get; set; }
            public TextView mProductId { get; set; }
            public TextView mProductName { get; set; }
            public TextView mProductPrice { get; set; }
            public LinearLayout mItemBackgroudHolderGrid { get; set; }
            public CardView mItemBackgroudHolderListView { get; set; }
            public ImageView mItemImage { get; set; }
            public TextView mItemAlias { get; set; }
            public TextView mQtyOncart { get; set; }
            public FrameLayout mAliasContainer { get; set; }
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
            TextView txtProductAlias = itemView.FindViewById<TextView>(Resource.Id.txtItemAlias);
            TextView txtProductId = itemView.FindViewById<TextView>(Resource.Id.txtProductId);
            TextView txtProductName = itemView.FindViewById<TextView>(Resource.Id.txtProductName);
            TextView txtProductPrice = itemView.FindViewById<TextView>(Resource.Id.txtProductPrice);
            TextView txtQtyOnCart = itemView.FindViewById<TextView>(Resource.Id.txtQtyOncart);
            FrameLayout flAliasContainer = itemView.FindViewById<FrameLayout>(Resource.Id.flAliasContainer);


            MyViewHolder view = new MyViewHolder(itemView)
            {
                mProductId = txtProductId,
                mProductName = txtProductName,
                mProductPrice = txtProductPrice,
                mItemBackgroudHolderGrid = mllItemHolder,
                mItemBackgroudHolderListView = mCardViewItemHolder,
                mItemImage = imgItemImage,
                mItemAlias = txtProductAlias,
                mQtyOncart = txtQtyOnCart,
                mAliasContainer = flAliasContainer
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
            if (mShowSizes)
            {
                myHolder.mMainView.Click -= MMainView_Click;//unsubscibe to avoid multiple firing of clicks
                myHolder.mMainView.Click += MMainView_Click; //set click event for row
            }
            else
            {
                myHolder.mMainView.Click -= ParentProduct_Click;//unsubscibe to avoid multiple firing of clicks
                myHolder.mMainView.Click += ParentProduct_Click; //set click event for row
            }

            myHolder.mProductId.Text = mShowSizes ? mProducts[position].productId.ToString(): mParentProducts[position].parentProductId.ToString();
            myHolder.mProductName.Text = mShowSizes ? mProducts[position].productName : mParentProducts[position].parentProductName;
            //condition here to display price based on transaction pricing type 
            myHolder.mProductPrice.Text = mShowSizes ? "\u20b1 " +String.Format("{0:n}", GetProductPrice(position)) : "";

            if (mIsGrid)
            {
                myHolder.mItemBackgroudHolderGrid.SetBackgroundColor
                    (Android.Graphics.Color.ParseColor("#" + (mShowSizes ? mProducts[position].productColorBg: mParentProducts[position].productColorBg)));
            }
            else
            {
                myHolder.mItemBackgroudHolderListView.SetCardBackgroundColor
                    (Android.Graphics.Color.ParseColor("#" + (mShowSizes ? mProducts[position].productColorBg : mParentProducts[position].productColorBg)));
            }

            if (mProducts[position].productImage != null)
            {
                //set item image here
            }

            myHolder.mItemAlias.Text = mShowSizes ? mProducts[position].productAlias : mParentProducts[position].productAlias;

            int qtyOnCart = 0;
            if (mShowSizes)
            {
                qtyOnCart = GlobalVariables.globalProductsOnCart
                    .Where(x => x.productId == mProducts[position].productId)
                    .Sum(x => x.productCountOnCart);
            }
            else
            {
                qtyOnCart = GlobalVariables.globalProductsOnCart
                    .Where(x => x.parentProductId == mParentProducts[position].parentProductId)
                    .Sum(x => x.productCountOnCart);
            }

            //show qty on cart on view
            if (mIsGrid)
            {
                ShowCurrentQuantityOnCartGridOnBind(myHolder.mQtyOncart, myHolder.mAliasContainer,qtyOnCart);
            }
            else
            {
                ShowCurrentQuantityOnCartListOnBind(myHolder.mProductName, myHolder.mQtyOncart, qtyOnCart);
            }
        }

        private decimal GetProductPrice(int position)
        {
            decimal retval = 0;
            if (mCurrentSelectedPricingType == "RT")
            {
                retval = mProducts[position].productRetailPrice;
            }
            else if (mCurrentSelectedPricingType == "WS")
            {
                retval = mProducts[position].productWholesalePrice;
            }
            else //runner
            {
                retval = mProducts[position].productRunnerPrice;
            }
            
            return retval;
        }

        private void ShowCurrentQuantityOnCartListOnBind(TextView _mProductName, TextView _mQtyOncart, int _qtyOnCart)
        {
            if (_qtyOnCart > 0)
            {
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 50f);
                _mProductName.Gravity = GravityFlags.Bottom;
                _mProductName.LayoutParameters = layoutParams;
                _mQtyOncart.LayoutParameters = layoutParams;
                _mQtyOncart.Text = "on cart " + _qtyOnCart.ToString();
            }
            else
            {
                LinearLayout.LayoutParams layoutParams1 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 100f);
                LinearLayout.LayoutParams layoutParams2 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 0f);

                _mProductName.Gravity = GravityFlags.Center;
                _mProductName.LayoutParameters = layoutParams1;
                _mQtyOncart.LayoutParameters = layoutParams2;
                _mQtyOncart.Text = "on cart " + _qtyOnCart.ToString();
            }
        }

        private void ShowCurrentQuantityOnCartGridOnBind(TextView _mQtyOncart, FrameLayout _mAliasContainer, int _qtyOnCart)
        {
            if (_qtyOnCart > 0)
            {
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 16f);
                _mQtyOncart.LayoutParameters = layoutParams;
                _mQtyOncart.Text = "on cart " + _qtyOnCart.ToString() + " ";
                LinearLayout.LayoutParams layoutParams2 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 50f);
                _mAliasContainer.LayoutParameters = layoutParams2;
            }
            else
            {
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 0f);
                _mQtyOncart.LayoutParameters = layoutParams;
                LinearLayout.LayoutParams layoutParams2 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 66f);
                _mAliasContainer.LayoutParameters = layoutParams2;
            }
        }

        private void MMainView_Click(object sender, EventArgs e)
        {
            int position = mRecyclerView.GetChildAdapterPosition((View)sender);
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
                    productOrigPrice = GetProductPrice(position),
                    productPrice = GetProductPrice(position),
                    productCountOnCart = 1,
                    productCategoryId = 1,
                    productSubTotalPrice = GetProductPrice(position),
                    productDiscountAmount = 0.00M,
                    productDiscountPercentage = 0.00M,
                    parentProductId = mProducts[position].parentProductId
                });
            }
            //update checkoutbutton.
            mCheckoutFragment.SetCheckoutButtonTotal(mBtnCheckoutButton, mCheckoutContext);

            var clickedProductRow = GlobalVariables.globalProductsOnCart.Where(o => o.productId == mProducts[position].productId).ToList();
            if (mIsGrid)
            {
                ShowCurrentQuantityOnCartGrid(sender, clickedProductRow[0].productCountOnCart);
            }
            else
            {
                ShowCurrentQuantityOnCartList(sender, clickedProductRow[0].productCountOnCart);
            }
        }

        private void ShowCurrentQuantityOnCartList(object _sender, int _itemQty)
        {
            //find the view clicked
            var clickedView = (View)_sender;
            TextView txtProductName = clickedView.FindViewById<TextView>(Resource.Id.txtProductName);
            TextView txtQtyOnCart = clickedView.FindViewById<TextView>(Resource.Id.txtQtyOncart);

            //change properties of view
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 50f);
            txtProductName.Gravity = GravityFlags.Bottom;
            txtProductName.LayoutParameters = layoutParams;
            txtQtyOnCart.LayoutParameters = layoutParams;
            txtQtyOnCart.Text = "on cart " + _itemQty.ToString();
        }

        private void ShowCurrentQuantityOnCartGrid(object _sender, int _itemQty)
        {
            //find the view clicked
            var clickedView = (View)_sender;
            TextView txtQtyOnCart = clickedView.FindViewById<TextView>(Resource.Id.txtQtyOncart);
            FrameLayout flAliasContainer = clickedView.FindViewById<FrameLayout>(Resource.Id.flAliasContainer);

            //change properties of view
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 16f);
            txtQtyOnCart.LayoutParameters = layoutParams;
            txtQtyOnCart.Text = "on cart " + _itemQty.ToString() + " ";
            LinearLayout.LayoutParams layoutParams2 = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0, 50f);
            flAliasContainer.LayoutParameters = layoutParams2;
        }

        private void ParentProduct_Click(object sender, EventArgs e)
        {
            if (!GlobalVariables.mIsCheckoutFragmentMultiSizeAddOpened)
            {
                GlobalVariables.mIsCheckoutFragmentMultiSizeAddOpened = true;
                int position = mRecyclerView.GetChildAdapterPosition((View)sender);

                TextView prodName = mRecyclerView.FindViewHolderForAdapterPosition(position).ItemView.FindViewById<TextView>(Resource.Id.txtProductName);
                TextView prodId = mRecyclerView.FindViewHolderForAdapterPosition(position).ItemView.FindViewById<TextView>(Resource.Id.txtProductId);

                GlobalVariables.mCurrentSelectedItemNameMultiSize = prodName.Text;
                GlobalVariables.mCurrentSelectedItemIdMultiSize = Convert.ToInt32(prodId.Text);

                Intent intent = new Intent(mSupportFragment.Context, typeof(CheckoutFragmentMultiSizeAdd));
                intent.AddFlags(ActivityFlags.NoAnimation);
                mSupportFragment.StartActivityForResult(intent, 8);

                //Intent intent = new Intent(mCheckoutContext, typeof(CheckoutFragmentMultiSizeAdd));
                //intent.AddFlags(ActivityFlags.NoAnimation);
                //((Activity)mCheckoutContext).StartActivityForResult(intent, 8); //callback not working used global var instead to prevent double click
            }
        }

        public override int ItemCount
        {
            get
            {
                return mShowSizes ? mProducts.Count : mParentProducts.Count;
            }
        }

    }
}