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
using WordPressPCL.Models;

using CNHSpotlight.WordPress;

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
        public static void SavePosts(List<Post> data, CNHCategory category)
        {
            // CNHSpotlightCache/posts/{category}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "posts");

            string file = Path.Combine(path, category.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }
        }

        public static ModelWrapper<List<Post>> GetPostsOffline(CNHCategory category)
        {
            List<Post> postsList = new List<Post>();

            // CNHSpotlightCache/posts/{category}
            string file = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "posts",
                category.ToString());

            if (File.Exists(file))
            {
                using (StreamReader streamReader = new StreamReader(file))
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
        #endregion

        #region PostImage
        public static void SavePostImage(Bitmap data, int postId, int imageIndex)
        {

            // CNHSpotlightCache/postimages/postId/cachedimage{imageIndex}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "postimages",
                postId.ToString());

            string fileName = string.Format("cachedimage{0}", imageIndex);

            SaveImageOffline(data, path, fileName);
        }

        public static List<Bitmap> GetAllPostImagesOffline(int postId)
        {
            List<Bitmap> imagesList = new List<Bitmap>();

            // CNHSpotlightCache/postimages/postId/cachedimage{imageIndex}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "postimages",
                postId.ToString());


            if (Directory.Exists(path))
            {
                // find all images in the cache path
                foreach (string imageFile in Directory.GetFiles(path))
                {
                    imagesList.Add(GetImageOffline(imageFile));
                }
            }

            return imagesList;
        }

        /// <summary>
        /// Returns a bitmap corresponds to postId and imageIndex or null if threre is none
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="imageIndex"></param>
        /// <returns></returns>
        public static Bitmap GetPostImageOffline(int postId, int imageIndex)
        {
            Bitmap bitmap = null;

            // CNHSpotlightCache/postimages/postId/cachedimage{imageIndex}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "postimages",
                postId.ToString());

            string filePath = Path.Combine(path, string.Format("cachedimage{0}", imageIndex));

            bitmap = GetImageOffline(filePath);

            return bitmap;
        }
        #endregion

        #region User
        public static void SaveUser(User data)
        {
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "users");

            string file = Path.Combine(path, data.id.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }
        }

        /// <summary>
        /// Returns <see cref="User"/> with corresponding userId or null if there is none
        /// </summary>
        /// <param name="userId"></param>
        public static ModelWrapper<User> GetUserOffline(int userId)
        {
            User user = null;

            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "users");

            string file = Path.Combine(path, userId.ToString());

            if (File.Exists(file))
            {
                using (StreamReader streamReader = new StreamReader(file))
                {
                    user = JsonConvert.DeserializeObject<User>(streamReader.ReadToEnd());

                    if (user != null)
                    {
                        return new ModelWrapper<User>(user, TaskResult.Success);
                    }
                }
            }

            // at this point, either user is null or file does not exist
            return new ModelWrapper<User>(TaskResult.NoData);
        }
        #endregion

        #region Media
        public static void SaveMedia(Media data)
        {
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "media");

            string file = Path.Combine(path, data.Id.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }
        }

        /// <summary>
        /// Returns <see cref="Media"/> with corresponding mediaId or null if there is none
        /// </summary>
        /// <param name="mediaId"></param>
        public static ModelWrapper<Media> GetMediaOffline(int mediaId)
        {
            Media media = null;

            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "media");

            string file = Path.Combine(path, mediaId.ToString());

            if (File.Exists(file))
            {
                using (StreamReader streamReader = new StreamReader(file))
                {
                    media = JsonConvert.DeserializeObject<Media>(streamReader.ReadToEnd());

                    if (media != null)
                    {
                        return new ModelWrapper<Media>(media, TaskResult.Success);
                    }
                }
            }

            // ai this point, either media is null or file does not exist
            return new ModelWrapper<Media>(TaskResult.NoData);
        }
        #endregion

        #region MediaImage
        public static Bitmap GetMediaImageOffline(int mediaId)
        {
            Bitmap bitmap = null;

            // CNHSpotlightCache/mediaimages/cachedimage{mediaId}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "mediaimages");

            string filePath = Path.Combine(path, string.Format("cachedimage{0}", mediaId));

            bitmap = GetImageOffline(filePath);


            return bitmap;
        }

        public static void SaveMediaImage(Android.Graphics.Bitmap data, int mediaId)
        {

            // CNHSpotlightCache/mediaimages/cachedimage{mediaId}
            string path = Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.Path,
                "CNHSpotlightCache",
                "mediaimages");

            string fileName = string.Format("cachedimage{0}", mediaId);

            SaveImageOffline(data, path, fileName);
        }
        #endregion
    }
}