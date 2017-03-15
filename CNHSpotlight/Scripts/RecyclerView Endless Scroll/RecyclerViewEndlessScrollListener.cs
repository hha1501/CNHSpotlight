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
using Android.Support.V7.Widget;

using Android.Util;


using CNHSpotlight.Components;

namespace CNHSpotlight
{
    class RecyclerViewEndlessScrollListener : RecyclerView.OnScrollListener
    {
        /// <summary>
        /// Triggered when scroll to threshold
        /// </summary>
        public event EventHandler ThresholdReached;

        public event EventHandler<ScrollEventArgs> Scroll;

        public int TotalItemCount { get; private set; }

        public int LastVisibleItemPosition { get; private set; }

        public int PassedItemCount { get; private set; }

        public int TriggerThreshold { get; private set; }

        public LinearLayoutManager LayoutManager { get; private set; }

        public NewsRecyclerAdapter RecyclerViewAdapter { get; private set; }

        private bool triggerLock;

        // constructor
        public RecyclerViewEndlessScrollListener(LinearLayoutManager layoutManager, NewsRecyclerAdapter adapter)
        {
            LayoutManager = layoutManager;
            RecyclerViewAdapter = adapter;

            TriggerThreshold = 1;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            // dY > 0 means scroll down
            if (dy > 0)
            {
                TotalItemCount = LayoutManager.ItemCount;
                LastVisibleItemPosition = LayoutManager.FindLastCompletelyVisibleItemPosition();

                // as the number of items passed equals to LastVisibleItemPosition + 1
                PassedItemCount = LastVisibleItemPosition + 1;

                // Check the presence of loading item
                int indexSubstitution = RecyclerViewAdapter.IsLoadingMore ? 1 : 0;

                // reach threshold
                if (PassedItemCount + TriggerThreshold == (TotalItemCount - indexSubstitution))
                {
                    if (!triggerLock)
                    {
                        ThresholdReached?.Invoke(this, EventArgs.Empty);

                        triggerLock = true; 
                    }
                }
                else
                {
                    triggerLock = false;
                }

            }

            bool firstItemOnTop = LayoutManager.FindFirstVisibleItemPosition() == 0 &&
                                LayoutManager.GetChildAt(0).Top == 0;

            bool isOnTop = firstItemOnTop || TotalItemCount == 0;

            Scroll?.Invoke(this, new ScrollEventArgs(isOnTop));
        }
     
        public class ScrollEventArgs : EventArgs
        {
            public bool IsOnTop { get; private set; }

            public ScrollEventArgs(bool isOnTop)
            {
                IsOnTop = isOnTop;
            }
        }

    }
}