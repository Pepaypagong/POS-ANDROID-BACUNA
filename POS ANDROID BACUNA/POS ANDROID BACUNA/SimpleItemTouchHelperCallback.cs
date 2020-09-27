using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Support.V7.Widget;
using POS_ANDROID_BACUNA.Interfaces;

namespace POS_ANDROID_BACUNA
{
    public class SimpleItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private readonly ITemTouchHelperAdapter _mAdapter;
        int dragFrom = -1;
        int dragTo = -1;

        public SimpleItemTouchHelperCallback(ITemTouchHelperAdapter adapter)
        {
            _mAdapter = adapter;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            const int swipeFlags = ItemTouchHelper.ActionStateIdle;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }
        public bool isLongPressDragEnabled()
        {
            return true;
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            if (viewHolder.ItemViewType != target.ItemViewType)
            {
                return false;
            }
            if (dragFrom == -1)
            {
                dragFrom = viewHolder.AdapterPosition;
            }
            dragTo = target.AdapterPosition;

            // Notify the adapter of the move
            _mAdapter.OnItemMove(viewHolder.AdapterPosition, target.AdapterPosition);
            return true;
        }

        private void ItemDrop(int from, int to)
        {
            _mAdapter.OnItemDrop(from, to);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            base.ClearView(recyclerView, viewHolder);
            if (dragFrom != -1 && dragTo != -1 && dragFrom != dragTo)
            {
                ItemDrop(dragFrom, dragTo);
            }

            dragFrom = dragTo = -1;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Notify the adapter of the dismissal
            _mAdapter.OnItemDismiss(viewHolder.AdapterPosition);
        }
    }
}