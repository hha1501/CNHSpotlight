using System;
using System.Threading.Tasks;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Webkit;

using Toolbar = Android.Support.V7.Widget.Toolbar;

using Newtonsoft.Json;

using CNHSpotlight.HtmlParser;
using CNHSpotlight.WordPress.Models;

namespace CNHSpotlight
{
    [Activity(ParentActivity = typeof(HostActivity), ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReadNewsActivity : AppCompatActivity
    {
        // UIs
        Toolbar toolbar;

        WebView webviewContent; 

        // current post
        Post currentPost;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ReadNewsActivity);

            // toolbar
            toolbar = FindViewById<Toolbar>(Resource.Id.readnewsActivity_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = null;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            // content
            webviewContent = FindViewById<WebView>(Resource.Id.readnewsActivity_webview_content);
            webviewContent.Settings.JavaScriptEnabled = true;

            // get current post
            string postJson = Intent.GetStringExtra("post_Json_extra");
            currentPost = JsonConvert.DeserializeObject<Post>(postJson);

        }

        protected override void OnResume()
        {
            base.OnResume();

            Task.Run(() => 
            {
                try
                {
                    LoadNews(currentPost);
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        private void LoadNews(Post currentPost)
        {
            string html = HtmlFormatter.FormatPost(currentPost);

            RunOnUiThread(() =>
            {
                webviewContent.LoadDataWithBaseURL(null, html, "text/html", "utf-8", null);
            });

        }

    }
}