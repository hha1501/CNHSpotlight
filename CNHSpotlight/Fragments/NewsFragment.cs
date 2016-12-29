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

        // current category
        CNHCategory currentCategory { get; set; }

        // bool for refreshing indication
        bool IsRefreshing;

        // bool for loading more indication
        bool IsLoadingMore;

        // bool for loading more capability
        bool canLoadMore = true;

        // Endless scrolling listener
        RecyclerViewEndlessScrollListener endlessScrollListener;

        // current RecyclerViewAdapter
        NewsRecyclerAdapter currentAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;

            currentCategory = CNHCategory.News;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // swipe refresh layout
            swipeRefreshLayout = (SwipeRefreshLayout)inflater.Inflate(Resource.Layout.NewsFragment, container, false);
            // fetch news with default category: News
            swipeRefreshLayout.Refresh += async (o, e) => await FetchLatestNews(currentCategory);

            // recycler view & setup
            recyclerView = swipeRefreshLayout.FindViewById<RecyclerView>(Resource.Id.newsfragment_recyclerview);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(linearLayoutManager);

            // prepare adapter (an empty one)
            currentAdapter = new NewsRecyclerAdapter(Activity, recyclerView);
            currentAdapter.ItemClick += (o, e) => NewsClick(e.ItemPosition);

            recyclerView.SetAdapter(currentAdapter);

            // apply scroll listener to recyclerview
            endlessScrollListener = new RecyclerViewEndlessScrollListener(linearLayoutManager, currentAdapter);
            endlessScrollListener.OnScrollToEnd += async (int count) => await OnRequireLoadMore(count);

            recyclerView.AddOnScrollListener(endlessScrollListener);

            return swipeRefreshLayout;
        }

        private async Task OnRequireLoadMore(int scrolledItemCount)
        {
            if (canLoadMore && !IsLoadingMore)
            {
                IsLoadingMore = true;
                currentAdapter.SetLoadingAnimation(true);

                var posts = await WordpressExtension.GetPosts(currentCategory, scrolledItemCount, 10);

                switch (posts.Result)
                {
                    case TaskResult.Error:
                        Snackbar.Make(swipeRefreshLayout, "Error occured", Snackbar.LengthShort).Show();
                        break;
                    case TaskResult.NoInternet:
                        Snackbar
                            .Make(swipeRefreshLayout, "No connection", Snackbar.LengthLong)
                            .SetAction("Retry", async (view) =>
                            {
                                await OnRequireLoadMore(scrolledItemCount);
                            })
                            .Show();
                        break;
                    case TaskResult.NoData:
                        canLoadMore = false;
                        break;
                    case TaskResult.Success:
                        // add posts
                        currentAdapter.AddItems(posts.Data);

                        // scroll to the first item of new batch of posts
                        recyclerView.SmoothScrollToPosition(scrolledItemCount);
                        break;
                    default:
                        break;
                }

                IsLoadingMore = false;
                currentAdapter.SetLoadingAnimation(false);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (recyclerView.GetAdapter() == null || recyclerView.GetAdapter().ItemCount <= 0)
            {
                FetchNews(currentCategory);
            }
        }

        private void NewsClick(int position)
        {
            if (!IsRefreshing)
            {

                // get post
                Post post = currentAdapter.GetItem(position);

                if (post == null)
                {
                    return;
                }
                
                // start readnewsFragment
                HostActivity hostActivity = (HostActivity)Activity;

                hostActivity.ReadNews(post); 
            }
        }

        public void FetchNews(CNHCategory category)
        {
            // only refresh when idle
            if (IsRefreshing)
            {
                return;
            }

            // update state
            IsRefreshing = true;

            // start refreshing animation
            swipeRefreshLayout.Refreshing = true;

            // clear item if it is another cateogry
            if (currentCategory != category)
            {
                currentAdapter.ClearItems();
            }

            // try to get posts
            var posts = DataManager.GetPostsOffline(category, 0, 10);


            switch (posts.Result)
            {
                case TaskResult.Success:
                    currentAdapter.ReplaceItems(posts.Data);
                    break;

                case TaskResult.NoData:
                    Snackbar.Make(swipeRefreshLayout, "No data", Snackbar.LengthShort).Show();
                    break;

                default:
                    break;
            }

            // update state
            IsRefreshing = false;
            currentCategory = category;

            // stop refreshing animation
            swipeRefreshLayout.Refreshing = false;

            // reset load more state
            canLoadMore = true;


        }

        public async Task FetchLatestNews(CNHCategory category)
        {
            // only refresh when idle
            if (IsRefreshing)
            {
                return;
            }


            swipeRefreshLayout.Refreshing = true;
            IsRefreshing = true;

            var posts = await WordPressManager.GetPostsOnline(category, 10);

            switch (posts.Result)
            {
                case TaskResult.Error:
                    Snackbar.Make(swipeRefreshLayout, "Error occured", Snackbar.LengthShort).Show();
                    break;

                case TaskResult.NoInternet:
                    Snackbar
                        .Make(swipeRefreshLayout, "No internet connection", Snackbar.LengthIndefinite)
                        .SetAction("Retry", async (view) =>
                        {
                            await FetchLatestNews(currentCategory);
                        })
                        .Show();
                    break;

                case TaskResult.Success:
                    currentAdapter.ReplaceItems(posts.Data);
                    break;

                default:
                    break;
            }


            // set current category
            currentCategory = category;

            // stop refreshing animation
            swipeRefreshLayout.Refreshing = false;
            IsRefreshing = false;

            // reset load more state
            canLoadMore = true;
        }
    }
}