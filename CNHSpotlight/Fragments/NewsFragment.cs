using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Net;

using Android.Support.Design.Widget;
using Android.Support.V4.Widget;

using WordPressPCL.Models;
using Newtonsoft.Json;

using CNHSpotlight.WordPress;

namespace CNHSpotlight
{
    public class NewsFragment : Android.Support.V4.App.Fragment
    {
        // UIs
        SwipeRefreshLayout swipeRefreshLayout;

        ListView listviewNews;

        // current category
        CNHCategory currentCategory { get; set; }

        // bool for refreshing indication
        bool IsRefreshing;

        // constructor
        public NewsFragment(CNHCategory category)
        {
            currentCategory = category;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // swipe refresh layout
            swipeRefreshLayout = (SwipeRefreshLayout)inflater.Inflate(Resource.Layout.NewsFragment, container, false);
            // fetch news with default category: News
            swipeRefreshLayout.Refresh += async (o, e) => await FetchLatestNews(currentCategory);

            // listview news
            listviewNews = swipeRefreshLayout.FindViewById<ListView>(Resource.Id.newsFragment_listview_newslist);
            listviewNews.ItemClick += NewsClick;


            return swipeRefreshLayout;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (listviewNews.Count <= 0)
            {
                Task.Run(() => FetchNews(currentCategory));
            }
        }

        private void NewsClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (!IsRefreshing)
            {
                NewsListViewAdapter adapter = (NewsListViewAdapter)listviewNews.Adapter;
                Post post = adapter[e.Position];

                // start readnewsFragment
                HostActivity hostActivity = (HostActivity)Activity;

                hostActivity.ReadNews(post); 
            }
        }

        public void FetchNews(CNHCategory category)
        {
            // clear list view if it is another category
            if (category != currentCategory)
            {
                Activity.RunOnUiThread(() =>
                {
                    listviewNews.Adapter = null;
                });
            }

            swipeRefreshLayout.Refreshing = true;
            IsRefreshing = true;

            var posts = DataManager.GetPostsOffline(category);
           

            Activity.RunOnUiThread(async () =>
            {

                switch (posts.Result)
                {
                    case TaskResult.Success:
                        var thumbnailImages = await ImageResource.ImageGetter.GetAllMediaImages(posts.Data);
                        NewsListViewAdapter newsAdapter = new NewsListViewAdapter(Activity, posts.Data, thumbnailImages);

                        listviewNews.Adapter = newsAdapter;
                        break;

                    case TaskResult.NoData:
                        Snackbar.Make(swipeRefreshLayout, "No data", Snackbar.LengthShort).Show();
                        break;

                    default:
                        break;
            }

            });

            currentCategory = category;
            swipeRefreshLayout.Refreshing = false;
            IsRefreshing = false;

        }

        public async Task FetchLatestNews(CNHCategory category)
        {

            // clear list view if it is another category
            if (category != currentCategory)
            {
                listviewNews.Adapter = null;
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
                    var thumbnailImages = await ImageResource.ImageGetter.GetAllMediaImages(posts.Data);
                    NewsListViewAdapter newsAdapter = new NewsListViewAdapter(Activity, posts.Data, thumbnailImages);
                    
                    listviewNews.Adapter = newsAdapter;
                    break;

                default:
                    break;
            }


            // set current category
            currentCategory = category;

            // stop refreshing animation
            swipeRefreshLayout.Refreshing = false;
            IsRefreshing = false;
        }
    }
}