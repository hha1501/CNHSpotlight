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

using CNHSpotlight.Components;

namespace CNHSpotlight
{
    class RecyclerViewEndlessScrollListener : RecyclerView.OnScrollListener
    {
        /// <summary>
        /// Triggered when scroll to end, providing the number of scrolled items
        /// </summary>
        public event Action<int> OnScrollToEnd;

        public int TotalItemCount { get; private set; }

        public int LastVisibleItemPosition { get; private set; }

        public int PassedItemCount { get; private set; }

        public LinearLayoutManager LayoutManager { get; private set; }

        public NewsRecyclerAdapter RecyclerViewAdapter { get; private set; }

        private bool triggerLock;

        // constructor
        public RecyclerViewEndlessScrollListener(LinearLayoutManager layoutManager, NewsRecyclerAdapter adapter)
        {
            LayoutManager = layoutManager;
            RecyclerViewAdapter = adapter;
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

                int dummyLoadingItemSubstitution = RecyclerViewAdapter.IsLoadingShown ? 1 : 0;

                // reach end
                if (PassedItemCount >= TotalItemCount)
                {
                    if (!triggerLock)
                    {
                        OnScrollToEnd?.Invoke(PassedItemCount - dummyLoadingItemSubstitution);

                        triggerLock = true; 
                    }
                }
                else
                {
                    triggerLock = false;
                }

            }
        }
    }
}