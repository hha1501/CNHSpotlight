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

using CNHSpotlight.WordPress.Models;

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

        static readonly int MaxUserCount;

        static WordPressManager()
        {
            BaseUrl = "https://chuyennguyenhue.com/wp-json/wp/v2/";
            PostUrl = string.Format("{0}posts/", BaseUrl);
            UserUrl = string.Format("{0}users/", BaseUrl);

            MaxUserCount = 100;
        }

        #region Urls
        static string GetPostQueryUrl(CNHCategory category, int numberOfPosts, int page, int offset)
        {
            UriBuilder uriBuilder = new UriBuilder(PostUrl);

            // prepare query string
            // '_embed' query param requests posts with embedded data
            string queryString = string.Format("_embed&{0}={1}&{2}={3}&{4}={5}&{6}={7}",
                "categories", (int)category,
                "per_page", numberOfPosts,
                "page", page,
                "offset", offset);

            uriBuilder.Query = queryString;

            return uriBuilder.Uri.ToString();
        }

        static string GetUsersQueryUrl()
        {
            UriBuilder uriBuilder = new UriBuilder(UserUrl);

            // prepare query string
            string queryString = string.Format("per_page={0}", MaxUserCount);

            uriBuilder.Query = queryString;

            return uriBuilder.Uri.ToString();
        }

        #endregion


        /// <summary>
        /// Implemeted only online lookup
        /// </summary>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<ModelWrapper<List<Post>>> GetPostsOnline(CNHCategory category, int numberOfPosts = 10, int page = 1, int offset = 0)
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
                    string postsData = await httpClient.GetStringAsync(GetPostQueryUrl(category, numberOfPosts, page, offset));

                    List<Post> tempList = JsonConvert.DeserializeObject<List<Post>>(postsData);

                    if (tempList != null && tempList.Count > 0)
                    {

                        // successfully retrieve posts, so save them
                        DataManager.SavePosts(tempList, category, offset);

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

        public static async Task<ModelWrapper<List<User>>> GetUsersOnline()
        {
            // check internet connection
            if (!ConnectionInfo.InternetConnected())
            {
                return new ModelWrapper<List<User>>(TaskResult.NoInternet);
            }

            // get users online
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string usersData = await httpClient.GetStringAsync(GetUsersQueryUrl());

                    List<User> tempList = JsonConvert.DeserializeObject<List<User>>(usersData);

                    if (tempList != null)
                    {

                        // successfully retrieve posts, so save them
                        DataManager.SaveUsers(usersData);

                        return new ModelWrapper<List<User>>(tempList, TaskResult.Success);
                    }
                    else
                    {
                        return new ModelWrapper<List<User>>(TaskResult.NoData);
                    }
                }
            }
            catch (Exception)
            {
                return new ModelWrapper<List<User>>(TaskResult.Error);
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