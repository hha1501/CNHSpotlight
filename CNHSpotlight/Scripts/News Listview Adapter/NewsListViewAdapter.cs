using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Text;

using WordPressPCL.Models;

using CNHSpotlight.ImageResource;
using CNHSpotlight.WordPress;

namespace CNHSpotlight
{
    class NewsListViewAdapter : BaseAdapter<Post>
    {
        // fields
        List<Post> PostsList { get; }

        Activity hostActivity;

        // thumbnail images
        List<Bitmap> thumbnailImagesList;

        // constructor
        public NewsListViewAdapter(Activity hostActivity, List<Post> postsList, List<Bitmap> thumbnailImages)
        {
            this.hostActivity = hostActivity;
            PostsList = postsList;

            // initialize imagesList first
            thumbnailImagesList = new List<Bitmap>();

            // cached all thumbnail images
            thumbnailImagesList = thumbnailImages;
        }


        public override Post this[int position]
        {
            get
            {
                return PostsList[position];
            }
        }

        public override int Count
        {
            get
            {
                return PostsList.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View itemView = convertView;

            if (itemView == null)
            {
                itemView = hostActivity.LayoutInflater.Inflate(Resource.Layout.NewsListViewItem, parent, false);
            }

            Post currentPost = PostsList[position];

            TextView title = itemView.FindViewById<TextView>(Resource.Id.newslistviewitem_text_title);
            ImageView thumbnailImage = itemView.FindViewById<ImageView>(Resource.Id.newslistviewitem_imageview_thumbnailimage);

            #pragma warning disable CS0618
            title.TextFormatted = Html.FromHtml(currentPost.Title.Rendered);

            // set image to thumbnailImage
            if (position < thumbnailImagesList.Count)
            {
                thumbnailImage.SetImageBitmap(thumbnailImagesList[position]);
            }

            return itemView;
        }
    }
}