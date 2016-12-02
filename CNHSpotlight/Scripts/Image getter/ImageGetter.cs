using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text.Style;
using Android.Graphics;
using Android.Graphics.Drawables;

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

                    image = ImageExtension.DecodeResource(imageData);
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
        /// <param name="htmlContainsImages">Html string in raw format</param>
        /// <param name="postId">For offline lookup</param>
        /// <param name="imageWidth">To scale images to fit prefered width</param>
        /// <returns></returns>
       

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

                if (bitmap != null)
                {
                    DataManager.SaveMediaImage(bitmap, mediaId);
                }
            }

            return bitmap;
        }
    }
}