using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CNHSpotlight.WordPress.Models;

using CNHSpotlight.Scripts.ConnectionInfo;

namespace CNHSpotlight.WordPress
{
    static class WordpressExtension
    {
        public static async Task<ModelWrapper<List<Post>>> GetPosts(WordPressManager.PostRequest postRequest)
        {
            if (!postRequest.UpdateRequired)
            {
                // try to get offline posts
                var postsOffline = DataManager.GetPostsOffline(postRequest);

                // if offline posts does not exist, get post online
                if (postsOffline.Result == TaskResult.NoData)
                {
                    // online
                    var postsOnline = await WordPressManager.GetPostsOnline(postRequest);
                    return postsOnline;
                }

                return postsOffline;
            }
            else
            {
                var postsOffline = await WordPressManager.GetPostsOnline(postRequest);
                return postsOffline;
            }

        }
    }
}