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
    public static class WordPressManager
    {

        /// <summary>
        /// Implemeted only online lookup
        /// </summary>
        /// <param name="category"></param>
        /// <param name="numberOfPosts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static async Task<ModelWrapper<List<Post>>> GetPostsOnline(PostRequest postRequest)
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

                    string postsData = await httpClient.GetStringAsync(postRequest.GetUri());

                    List<Post> tempList = JsonConvert.DeserializeObject<List<Post>>(postsData);

                    if (tempList != null && tempList.Count > 0)
                    {
                        if (postRequest.SaveRequired)
                        {
                            // successfully retrieve posts, so save them
                            DataManager.SavePosts(tempList, postRequest.CurrentCategory);
                        }

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

        public static async Task<ModelWrapper<List<User>>> GetUsersOnline(UserRequest userRequest)
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
                    string usersData = await httpClient.GetStringAsync(userRequest.GetUri());

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


        public static async Task<ModelWrapper<Page>> GetAboutPageOnline(PageRequest pageRequest)
        {
            // check internet connection
            if (!ConnectionInfo.InternetConnected())
            {
                return new ModelWrapper<Page>(TaskResult.NoInternet);
            }

            // get users online
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string pageData = await httpClient.GetStringAsync(pageRequest.GetUri());

                    Page page = JsonConvert.DeserializeObject<Page>(pageData);

                    if (page != null)
                    {

                        return new ModelWrapper<Page>(page, TaskResult.Success);
                    }
                    else
                    {
                        return new ModelWrapper<Page>(TaskResult.NoData);
                    }
                }
            }
            catch (Exception)
            {
                return new ModelWrapper<Page>(TaskResult.Error);
            }
        }


        public class PostRequest : ModelRequest
        {
            public PostRequest()
            {
                BaseUrl = PostUrl;
            }

            public CNHCategory CurrentCategory { get; private set; }

            public int CurrentOffset { get; private set; }

            public int CurrentQuantity { get; private set; }

            public string CurrentSearch { get; private set; }


            public bool UpdateRequired { get; private set; }

            public bool SaveRequired { get; private set; }


            public PostRequest Category(CNHCategory category)
            {
                if (category != CNHCategory.Latest)
                {
                    AddParam("categories", (int)category);
                }
                CurrentCategory = category;
                return this;
            }

            public PostRequest Page(int page)
            {
                AddParam("page", page);

                return this;
            }

            public PostRequest Quantity(int numberOfPosts)
            {
                AddParam("per_page", numberOfPosts);
                CurrentQuantity = numberOfPosts;
                return this;
            }

            public PostRequest Offset(int offset)
            {
                AddParam("offset", offset);
                CurrentOffset = offset;

                return this;
            }

            public PostRequest Search(string keyword)
            {
                AddParam("search", keyword);
                CurrentSearch = keyword;

                return this;
            }

            public PostRequest Embeded()
            {
                AddParam("_embed", null);

                return this;
            }

            public PostRequest Update(bool update)
            {
                UpdateRequired = update;

                return this;
            }

            public PostRequest Save(bool save)
            {
                SaveRequired = save;

                return this;
            }
        }

        public class UserRequest : ModelRequest
        {
            public UserRequest()
            {
                BaseUrl = UserUrl;
            }

            public UserRequest Quantity(int numberOfUsers)
            {
                AddParam("per_page", numberOfUsers);

                return this;
            }
        }

        public class PageRequest : ModelRequest
        {
            public PageRequest()
            {
                BaseUrl = AboutPageUrl;
            }

        }

        /// <summary>
        /// Base class for implementing any model's parameters
        /// </summary>
        public class ModelRequest
        {
            public static readonly string PostUrl = "https://chuyennguyenhue.com/wp-json/wp/v2/posts";
            public static readonly string UserUrl = "https://chuyennguyenhue.com/wp-json/wp/v2/users";
            public static readonly string AboutPageUrl = "https://chuyennguyenhue.com/wp-json/wp/v2/pages/368";

            public static readonly int MaxUserCount = 100;

            public string BaseUrl { get; protected set; }

            protected Dictionary<string, object> ParamsList { get; set; }

            protected ModelRequest()
            {
                ParamsList = new Dictionary<string, object>();
            }

            protected virtual ModelRequest AddParam(string key, object value)
            {
                try
                {
                    ParamsList.Add(key, value);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"Key with value {key} already exists");
                }

                return this;
            }

            protected virtual string GetQueryString()
            {
                IEnumerable<string> queryCollection = ParamsList
                    .Select(queryItem =>
                    {
                        return (queryItem.Value != null) ?
                        string.Format("{0}={1}", queryItem.Key, queryItem.Value) : queryItem.Key;
                    });

                return string.Join("&", queryCollection);
            }

            public Uri GetUri()
            {
                UriBuilder uriBuilder = new UriBuilder(BaseUrl);

                uriBuilder.Query = GetQueryString();

                return uriBuilder.Uri;
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
