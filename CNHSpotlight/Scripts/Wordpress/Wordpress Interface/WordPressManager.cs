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
        static Uri GetPostQueryUrl(CNHCategory category, int numberOfPosts = 10, int page = 1, int offset = 0)
        {
            QueryUrlBuilder uriBuilder = new QueryUrlBuilder(PostUrl);
            uriBuilder
                .AddQueryParam("per_page", numberOfPosts)
                .AddQueryParam("page", page)
                .AddQueryParam("offset", offset)
                .AddQueryParam("_embed", null);

            // we have to handle category specially
            if (category != CNHCategory.Latest)
            {
                uriBuilder.AddQueryParam("categories", (int)category);
            }

            return uriBuilder.Build();

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

        class QueryUrlBuilder
        {
            UriBuilder uriBuilder;
            Dictionary<string, object> queryParams;

            public QueryUrlBuilder(string baseUrl)
            {
                uriBuilder = new UriBuilder(baseUrl);
                queryParams = new Dictionary<string, object>();
            }

            public QueryUrlBuilder AddQueryParam(string key, object data)
            {
                try
                {
                    queryParams.Add(key, data);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"Key with value {key} already exists");
                }

                return this;
            }

            public Uri Build()
            {
                uriBuilder.Query = GetQueryString();

                return uriBuilder.Uri;
            }

            private string GetQueryString()
            {
                IEnumerable<string> queryCollection = queryParams
                    .Select(query =>
                    {
                        return (query.Value != null) ?
                        string.Format("{0}={1}", query.Key, query.Value) : query.Key;
                    });

                return string.Join("&", queryCollection);
            }
        }

    }

    public enum CNHCategory
    {
        Latest = 0,
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