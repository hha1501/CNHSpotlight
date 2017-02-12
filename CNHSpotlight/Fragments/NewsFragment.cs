using System;
using System.Threading.Tasks;

using Android.OS;
using Android.Views;

using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;


using CNHSpotlight.WordPress;
using CNHSpotlight.WordPress.Models;
using CNHSpotlight.Components;

namespace CNHSpotlight
{
    public class NewsFragment : Android.Support.V4.App.Fragment
    {
        // UIs
        SwipeRefreshLayout swipeRefreshLayout;

        RecyclerView recyclerView;

        // Endless scrolling listener
        RecyclerViewEndlessScrollListener endlessScrollListener;

        // current RecyclerViewAdapter
        NewsRecyclerAdapter currentAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // swipe refresh layout
            swipeRefreshLayout = (SwipeRefreshLayout)inflater.Inflate(Resource.Layout.NewsFragment, container, false);

            // recycler view & setup
            recyclerView = swipeRefreshLayout.FindViewById<RecyclerView>(Resource.Id.newsfragment_recyclerview);
            recyclerView.HasFixedSize = true;

            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(linearLayoutManager);

            // prepare adapter (an empty one)
            currentAdapter = new NewsRecyclerAdapter(Activity);
            currentAdapter.ItemClick += (o, e) => OnNewsClick(e.ItemPosition);
            currentAdapter.Error += OnError;
            currentAdapter.ConnectionError += OnConnectionError;
            currentAdapter.NoData += OnNoData;
            currentAdapter.Loading += OnLoading;
            currentAdapter.Loaded += OnLoaded;


            recyclerView.SetAdapter(currentAdapter);

            // apply scroll listener to recyclerview
            endlessScrollListener = new RecyclerViewEndlessScrollListener(linearLayoutManager, currentAdapter);
            endlessScrollListener.ThresholdReached += async (o, e) => await currentAdapter.LoadMore();
            endlessScrollListener.Scroll += HandleScroll;

            recyclerView.AddOnScrollListener(endlessScrollListener);

            // swipe refresh layout events setup
            swipeRefreshLayout.Refresh += async (o, e) => await currentAdapter.RefreshNews();

            return swipeRefreshLayout;
        }

        public async Task FetchNews(CNHCategory category)
        {
            await currentAdapter.FetchNews(category);
        }

        public async Task Search(string keyword)
        {
            await currentAdapter.Search(keyword);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            swipeRefreshLayout.Refreshing = false;
        }

        private void OnLoading(object sender, EventArgs e)
        {
            swipeRefreshLayout.Refreshing = true;
        }

        private void OnNoData(object sender, EventArgs e)
        {
            Snackbar.Make(swipeRefreshLayout, "No data", Snackbar.LengthShort).Show();
        }

        private void OnConnectionError(object sender, EventArgs e)
        {
            Snackbar.Make(swipeRefreshLayout, "No connection", Snackbar.LengthLong).Show();
        }

        private void OnError(object sender, EventArgs e)
        {
            Snackbar.Make(swipeRefreshLayout, "Error occured", Snackbar.LengthShort).Show();
        }

        private void HandleScroll(object sender, RecyclerViewEndlessScrollListener.ScrollEventArgs e)
        {
            if (e.IsOnTop)
            {
                swipeRefreshLayout.SetEnabled(true);
            }
            else
            {
                if (!swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.SetEnabled(false);
                }
            }
        }

        public override async void OnResume()
        {
            base.OnResume();

            if (currentAdapter.PostList.Count <= 0)
            {
                await currentAdapter.FetchNews(currentAdapter.Category); 
            }
        }

        private void OnNewsClick(int position)
        {
            if (!currentAdapter.IsLoading)
            {

                // get post
                Post post = currentAdapter.GetPost(position);

                if (post == null)
                {
                    return;
                }
                
                // start readnewsFragment
                HostActivity hostActivity = (HostActivity)Activity;

                hostActivity.ReadNews(post); 
            }
        }
    }
}