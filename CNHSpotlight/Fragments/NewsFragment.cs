using System;
using System.Collections.Generic;
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
        CNHCategory currentCategory = CNHCategory.News;

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

        public override  void OnResume()
        {
            base.OnResume();

            var posts = DataManager.GetPostsOffline(currentCategory);

            switch (posts.Result)
            {
                case TaskResult.Success:
                    NewsListViewAdapter newsAdapter = new NewsListViewAdapter(Activity, posts.Data);

                    listviewNews.Adapter = newsAdapter;
                    break;

                default:
                    break;
            }
        }


        private void NewsClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            NewsListViewAdapter adapter = (NewsListViewAdapter)listviewNews.Adapter;
            Post post = adapter[e.Position];

            // start readnewsActivity
            Intent intent = new Intent(Activity, typeof(ReadNewsActivity));
            intent.PutExtra("news_post_extra", JsonConvert.SerializeObject(post));
            StartActivity(intent);
        }

        public void FetchNews(CNHCategory category)
        {
            var posts = DataManager.GetPostsOffline(currentCategory);

            switch (posts.Result)
            {
                case TaskResult.Success:
                    NewsListViewAdapter newsAdapter = new NewsListViewAdapter(Activity, posts.Data);

                    listviewNews.Adapter = newsAdapter;
                    break;

                default:
                    break;
            }
        }

        public async Task FetchLatestNews(CNHCategory category)
        {
            if (!swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = true; 
            }

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
                    NewsListViewAdapter newsAdapter = new NewsListViewAdapter(Activity, posts.Data);

                    listviewNews.Adapter = newsAdapter;
                    break;

                default:
                    break;
            }


            // set current category
            currentCategory = category;

            // stop refreshing animation
            swipeRefreshLayout.Refreshing = false;
        }
    }
}