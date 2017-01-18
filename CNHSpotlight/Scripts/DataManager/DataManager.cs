using System;
using Path = System.IO.Path;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Style;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Newtonsoft.Json;

using CNHSpotlight.WordPress;
using CNHSpotlight.WordPress.Models;
using CNHSpotlight.WordPress.Utils;


namespace CNHSpotlight
{
    /// <summary>
    /// Cover all caching tasks
    /// </summary>
    static class DataManager
    {

        #region Posts
        public static void SavePosts(List<Post> data, CNHCategory category)
        {
            List<Post> finalPostList = new List<Post>();

            var posts = GetAllPostsOffline(new WordPressManager.PostRequest().Category(category));

            // try to get saved postList if it exists
            if (posts.Result == TaskResult.Success)
            {
                finalPostList = posts.Data;
            }

            finalPostList = finalPostList.ReplacePosts(data);

            using (StreamWriter streamWriter = new StreamWriter(GetPostFilePath(category)))
            {
                streamWriter.Write(JsonConvert.SerializeObject(finalPostList));
            }
        }

        /// <summary>
        /// Create necessary directory and return file path
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        static string GetPostFilePath(CNHCategory category)
        {
            // CNHSpotlightCache/posts/{category}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "posts");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string file = Path.Combine(path, category.ToString());

            return file;
        }

        /// <summary>
        /// Return all saved posts if they exist 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static ModelWrapper<List<Post>> GetAllPostsOffline(WordPressManager.PostRequest postRequest)
        {
            List<Post> postsList = new List<Post>();

            string filePath = GetPostFilePath(postRequest.CurrentCategory);

            if (File.Exists(filePath))
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    postsList = JsonConvert.DeserializeObject<List<Post>>(streamReader.ReadToEnd());

                    if (postsList != null && postsList.Count > 0)
                    {
                        return new ModelWrapper<List<Post>>(postsList, TaskResult.Success);
                    }
                }
            }

            // at this point either postsList is empty or file does not exist
            return new ModelWrapper<List<Post>>(TaskResult.NoData);
        }

        /// <summary>
        /// Return a range of all saved posts if they exist
        /// </summary>
        /// <param name="category"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ModelWrapper<List<Post>> GetPostsOffline(WordPressManager.PostRequest postRequest)
        {

            var allPosts = GetAllPostsOffline(postRequest);

            // there is some posts
            if (allPosts.Result == TaskResult.Success)
            {

                List<Post> queriedPostList = allPosts.Data;

                // apply search
                if (postRequest.CurrentSearch != string.Empty)
                {
                    CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;

                    queriedPostList = queriedPostList
                        .Where(post =>
                            compareInfo.IndexOf(post.Title.Rendered, postRequest.CurrentSearch, CompareOptions.IgnoreCase) >= 0 ||
                            compareInfo.IndexOf(post.Content.Rendered, postRequest.CurrentSearch, CompareOptions.IgnoreCase) >= 0)
                        .ToList();
                }

                // apply range
                int validCount = Math.Min(queriedPostList.Count - postRequest.CurrentOffset, postRequest.CurrentQuantity);

                if (validCount > 0)
                {
                    queriedPostList = queriedPostList.GetRange(postRequest.CurrentOffset, validCount);

                    return new ModelWrapper<List<Post>>(queriedPostList, TaskResult.Success);
                }

            }

            // at this point either postsList is empty or file does not exist
            return new ModelWrapper<List<Post>>(TaskResult.NoData);
        }
        #endregion

        #region Users
        public static void SaveUsers(string userJsondata)
        {
            // CNHSpotlightCache/users/{usersfile}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "users");

            string file = Path.Combine(path, "users");

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(userJsondata);
            }
        }

        public static ModelWrapper<List<User>> GetUsersOffline()
        {
            List<User> usersList = new List<User>();

            // CNHSpotlightCache/users/{usersfile}
            string file = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "users",
                "users");


            if (File.Exists(file))
            {
                using (StreamReader streamReader = new StreamReader(file))
                {
                    usersList = JsonConvert.DeserializeObject<List<User>>(streamReader.ReadToEnd());

                    if (usersList != null && usersList.Count > 0)
                    {
                        return new ModelWrapper<List<User>>(usersList, TaskResult.Success);
                    }
                }
            }

            // at this point either postsList is empty or file does not exist
            return new ModelWrapper<List<User>>(TaskResult.NoData);
        }
        #endregion

        /// <summary>
        /// An extension for <see cref="List{T}"/> which supports replace item range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        static void ReplaceItemRange<T>(this List<T> source, int index, List<T> collection)
        {
            // perform some range validation

            // index is outside source range
            if (index > source.Count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index is outside source range");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection", "collection can not be null");
            }

            // no items to replace
            if (collection.Count <= 0)
            {
                return;
            }

            // calculation

            // clamp maxRemoveableItemCount to be greater than or equal to zero
            int maxRemovableItemCount = Math.Max(source.Count - index, 0);

            // clamp intersectItemCount to be less than or equal to maxRemovableItemCount
            int intersectItemCount = Math.Min(collection.Count, maxRemovableItemCount);

            // remove unnecessary items
            source.RemoveRange(index, intersectItemCount);

            // insert new items back into intersection range
            source.InsertRange(index, collection);

        }

    }
}