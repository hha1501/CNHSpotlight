using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text.Style;
using Android.Graphics;

using HtmlAgilityPack;
using WordPressPCL.Models;

using CNHSpotlight.WordPress;
using CNHSpotlight.Scripts.ConnectionInfo;

namespace CNHSpotlight.ImageResource
{
    static class ImageGetter
    {
        #region Generic online image getter
        /// <summary>
        /// Returns <see cref="Bitmap"/> from source or null if error occured
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<Bitmap> GetImageOnline(Uri source)
        {
            Bitmap image = null;

            // no internet connection, return immediately
            if (!ConnectionInfo.InternetConnected())
            {
                return image;
            }
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    byte[] imageData = await httpClient.GetByteArrayAsync(source);

                    image = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                }
            }
            catch (Exception)
            {
            }

            return image;
        }
        #endregion

        /// <summary>
        /// Either get image from cached file or from the internet
        /// <para>
        /// Could return null if task fails
        /// </para>
        /// </summary>
        /// <param name="htmlContainsImages"></param>
        /// <returns></returns>
        public static async Task<ImageSpan> GetPostImage(string htmlContainsImages, int postId, int imageIndex)
        {
            ImageSpan imageSpan = null;
            Bitmap image = null;
            // this is for offline lookup
            image = DataManager.GetPostImageOffline(postId, imageIndex);
            if (image != null)
            {
                return new ImageSpan(image);
            }

            // this is for internet implementation
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContainsImages);

            string imageUriString = htmlDoc.DocumentNode.Descendants("img")
                .ElementAt(imageIndex).GetAttributeValue("src", null);

            Uri imageUri = new Uri(imageUriString);

            // get image online
            image = await GetImageOnline(imageUri);
            // save image for future uses

            if (image != null)
            {
                DataManager.SavePostImage(image, postId, imageIndex); 
            }

            // construct new image span
            imageSpan = new ImageSpan(image);

            return imageSpan;
        }

        /// <summary>
        /// Implemented both online and offline
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static async Task<Bitmap> GetMediaImage(int mediaId)
        {
            Bitmap image = null;

            // this is for offline lookup
            image = DataManager.GetMediaImageOffline(mediaId);
            if (image != null)
            {
                return image;
            }

            // this is for internet implementation
            image = await GetMediaImageOnline(mediaId);

            // save image for further uses
            DataManager.SaveMediaImage(image, mediaId);

            return image;
        }

        public static async Task<List<Bitmap>> GetAllMediaImages(List<Post> postsList)
        {
            List<Bitmap> imagesList = new List<Bitmap>();

            foreach (Post post in postsList)
            {
                Bitmap image = await GetMediaImage(post.FeaturedMedia);

                // image retrieved, add to the list
                imagesList.Add(image);
            }

            return imagesList;
        }



        /// <summary>
        /// Return <see cref="Bitmap"/> embedded in mediaId or null if error occured
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        static async Task<Bitmap> GetMediaImageOnline(int mediaId)
        {
            Bitmap bitmap = null;

            var media = await WordPressManager.GetMediaOnline(mediaId);

            if (media.Result == TaskResult.Success)
            {
                string mediaUriString = media.Data.SourceUrl;

                bitmap = await GetImageOnline(new Uri(mediaUriString));
            }

            return bitmap;
        }
    }
}