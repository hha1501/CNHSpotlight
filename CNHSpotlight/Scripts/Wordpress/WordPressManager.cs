using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;


using Android.Text;
using Android.Text.Style;
using Android.Graphics;

using Newtonsoft.Json;
using HtmlAgilityPack;

using WordPressPCL.Models;

using CNHSpotlight.Scripts.ConnectionInfo;

namespace CNHSpotlight.WordPress
{

    /// <summary>
    /// Interface for pulling data from WordPress site
    /// </summary>
    static class WordPressManager
    {
        static readonly string BaseUrl;
        static readonly string PostUrl;
        static readonly string UserUrl;
        static readonly string MediaUrl;

        static WordPressManager()
        {
            BaseUrl = "https://chuyennguyenhue.com/wp-json/wp/v2/";
            PostUrl = string.Format("{0}posts/", BaseUrl);
            UserUrl = string.Format("{0}users/", BaseUrl);
            MediaUrl = string.Format("{0}media/", BaseUrl);
        }

        #region Urls
        static string GetPostQueryUrl(CNHCategory category, int numberOfPosts, int page)
        {
            UriBuilder uriBuilder = new UriBuilder(PostUrl);

            // prepare query string
            string queryString = string.Format("{0}={1}&{2}={3}&{4}={5}",
                "categories", (int)category,
                "per_page", numberOfPosts,
                "page", page);

            uriBuilder.Query = queryString;

            return uriBuilder.Uri.ToString();
        }

        static string GetUserUrl(int userId)
        {
            Uri baseUri = new Uri(UserUrl);

            Uri finalUri = new Uri(baseUri, userId.ToString());

            return finalUri.ToString();
        }

        static string GetMediaUrl(int mediaId)
        {
            Uri baseUri = new Uri(MediaUrl);

            Uri finalUri = new Uri(baseUri, mediaId.ToString());

            return finalUri.ToString();
        }
        #endregion


        /// <summary>
        /// Implemeted only online lookup
        /// </summary>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<ModelWrapper<List<Post>>> GetPostsOnline(CNHCategory category , int numberOfPosts = 5, int page = 1)
        {
            // check internet connection
            if (!ConnectionInfo.InternetConnected())
            {
                return new ModelWrapper<List<Post>>(TaskResult.NoInternet);
            }

            // get posts online
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string postsData = await httpClient.GetStringAsync(GetPostQueryUrl(category, numberOfPosts, page));

                    List<Post> tempList = JsonConvert.DeserializeObject<List<Post>>(postsData);

                    if (tempList != null)
                    {
                 
                        // successfully retrieve posts, so save them
                        DataManager.SavePosts(tempList, category);

                        return new ModelWrapper<List<Post>>(tempList, TaskResult.Success);
                    }
                    else
                    {
                        return new ModelWrapper<List<Post>>(TaskResult.NoData);
                    }
                }
            }
            catch (Exception)
            {
                return new ModelWrapper<List<Post>>(TaskResult.Error);
            }

        }

        /// <summary>
        /// Implemeted only online lookup
        /// <para>
        /// Could return null if retriving <see cref="User"/> unsuccessfully
        /// </para>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resultCallback">Extended result. See <see cref="ExtendedResultEnum"/> for possible results</param>
        /// <returns></returns>
        public static async Task<ModelWrapper<User>> GetUserOnline(int userId)
        {
            // check for internet connection
            if (!ConnectionInfo.InternetConnected())
            {
                return new ModelWrapper<User>(TaskResult.NoInternet); 
            }

            // online lookup
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string userData = await httpClient.GetStringAsync(GetUserUrl(userId));

                    User tempUser = JsonConvert.DeserializeObject<User>(userData);

                    if (tempUser != null)
                    {
                        // save User for future uses
                        DataManager.SaveUser(tempUser);

                        return new ModelWrapper<User>(tempUser, TaskResult.Success);
                    }
                    else
                    {
                        return new ModelWrapper<User>(TaskResult.NoData);
                    }
                }
            }
            catch (Exception)
            {
                return new ModelWrapper<User>(TaskResult.Error);
            }

        }

        /// <summary>
        /// Implemeted only online lookup
        /// <para>
        /// Could return null if retriving <see cref="Media"/> unsuccessfully
        /// </para>
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public static async Task<ModelWrapper<Media>> GetMediaOnline(int mediaId)
        {

            // check for internet connection
            if (!ConnectionInfo.InternetConnected())
            {
                return new ModelWrapper<Media>(TaskResult.NoInternet);
            }

            // online lookup
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string mediaData = await httpClient.GetStringAsync(GetMediaUrl(mediaId));

                    Media tempMedia = JsonConvert.DeserializeObject<Media>(mediaData);

                    if (tempMedia != null)
                    {
                        // save Media for future uses
                        DataManager.SaveMedia(tempMedia);

                        return new ModelWrapper<Media>(tempMedia, TaskResult.Success);
                    }
                    else
                    {
                        return new ModelWrapper<Media>(TaskResult.NoData);
                    }
                }
            }
            catch (Exception)
            {
                return new ModelWrapper<Media>(TaskResult.Error);
            }

        }

    }

    public enum CNHCategory
    {
        News = 2,
        OutsideClass = 38,
        Club = 36,
        Contest = 178,
        StudyAbroad = 39,
        Trivial = 4,
        Entertainment = 5,
        Education = 6,
        NHInMe = 7,
        NHIcon = 8
    }
}