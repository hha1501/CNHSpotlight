using System;
using Path = System.IO.Path;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace CNHSpotlight
{
    /// <summary>
    /// Cover all caching tasks
    /// </summary>
    static class DataManager
    {
        #region Generic image I/O
        static Bitmap GetImageOffline(string imageFilePath)
        {
            Bitmap bitmap = null;

            if (File.Exists(imageFilePath))
            {
                using (StreamReader streamReader = new StreamReader(imageFilePath))
                {
                    bitmap =
                        BitmapFactory.DecodeStream(streamReader.BaseStream);
                }
            }

            return bitmap;
        }

        static void SaveImageOffline(Bitmap data, string imagePath, string imageName)
        {

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            string imageFilePath = Path.Combine(imagePath, imageName);

            using (StreamWriter streamWriter = new StreamWriter(imageFilePath))
            {
                data.Compress(Bitmap.CompressFormat.Jpeg, 95, streamWriter.BaseStream);
            }
        }
        #endregion

        #region Posts
        public static void SavePosts(List<Post> data, CNHCategory category, int index)
        {
            List<Post> finalPostList = new List<Post>();

            var posts = GetPostsOffline(category);

            // try to get saved postList if it exists
            if (posts.Result == TaskResult.Success)
            {
                finalPostList = posts.Data;
            }

            finalPostList.ReplaceItemRange(index, data);

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
        public static ModelWrapper<List<Post>> GetPostsOffline(CNHCategory category)
        {
            List<Post> postsList = new List<Post>();

            string filePath = GetPostFilePath(category);

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
        public static ModelWrapper<List<Post>> GetPostsOffline(CNHCategory category, int index, int count)
        {
            List<Post> postsList = new List<Post>();

            string filePath = GetPostFilePath(category);

            if (File.Exists(filePath))
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    postsList = JsonConvert.DeserializeObject<List<Post>>(streamReader.ReadToEnd());

                    if (postsList != null && postsList.Count > 0)
                    {
                        int validCount = Math.Min(postsList.Count - index, count);
                        if (validCount > 0)
                        {
                            return new ModelWrapper<List<Post>>(postsList.GetRange(index, validCount), TaskResult.Success); 
                        }
                    }
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