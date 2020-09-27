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

namespace POS_ANDROID_BACUNA.Interfaces
{
    public interface ITemTouchHelperAdapter
    {
        /** 
        * @param fromPosition The start position of the moved item.
        * @param toPosition   Then resolved position of the moved item.
        * @return True if the item was moved to the new adapter position. 
        */
        bool OnItemMove(int fromPosition, int toPosition);
        
        /**  
         * @param position The position of the item dismissed. 
         * @see RecyclerView#getAdapterPositionFor(RecyclerView.ViewHolder)
         * @see RecyclerView.ViewHolder#getAdapterPosition()
         */
        void OnItemDismiss(int position);

        bool OnItemDrop(int fromPosition, int toPosition);
    }
}