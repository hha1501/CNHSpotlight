using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Text.Style;

using Android.Support.V7.App;

using Toolbar = Android.Support.V7.Widget.Toolbar;

using WordPressPCL.Models;
using Newtonsoft.Json;
using CNHSpotlight.WordPress;


namespace CNHSpotlight
{
    [Activity(Label = "ReadNewsActivity", ParentActivity = typeof(HostActivity))]
    public class ReadNewsActivity : AppCompatActivity
    {
        // UIs
        Toolbar toolbar;

        TextView textviewTitle;

        TextView textviewExtraInfo;

        TextView textviewContent;

    
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ReadNewsActivity);

            // toolbar
            toolbar = FindViewById<Toolbar>(Resource.Id.readnewsActivity_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "News";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            // title
            textviewTitle = FindViewById<TextView>(Resource.Id.readnewsActivity_text_title);

            // extra info
            textviewExtraInfo = FindViewById<TextView>(Resource.Id.readnewsActivity_text_extrainfo);

            // content
            textviewContent = FindViewById<TextView>(Resource.Id.readnewsActivity_text_content);

            Post post = JsonConvert.DeserializeObject<Post>(Intent.GetStringExtra("news_post_extra"));
            await LoadNews(post);
        }

        private async Task LoadNews(Post currentPost)
        {
            textviewTitle.TextFormatted = HtmlReader.GetReadableFromHtml(currentPost.Title.Rendered);

            string userName = await WordPressExtension.GetUserName(currentPost.Author);

            textviewExtraInfo.TextFormatted = HtmlReader.GetReadableFromHtml(
                string.Format("By {0}      {1}", userName, currentPost.Date));

            textviewContent.TextFormatted = await HtmlReader.GetReadableFromHtml(currentPost.Content.Rendered, currentPost.Id);
        }


    }
}