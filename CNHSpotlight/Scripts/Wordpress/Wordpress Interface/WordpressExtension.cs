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
        public static async Task<ModelWrapper<List<Post>>> GetPosts(CNHCategory category, int index, int count)
        {
            // offline
            var postsOffline = DataManager.GetPostsOffline(category, index, count);

            switch (postsOffline.Result)
            {
                case TaskResult.NoData:
                    // online
                    var postsOnline = await WordPressManager.GetPostsOnline(category, count, 1, index);

                    switch (postsOnline.Result)
                    {
                        case TaskResult.Error:
                            return new ModelWrapper<List<Post>>(TaskResult.Error);
                        case TaskResult.NoInternet:
                            return new ModelWrapper<List<Post>>(TaskResult.NoInternet);
                        case TaskResult.NoData:
                            return new ModelWrapper<List<Post>>(TaskResult.NoData);
                        case TaskResult.Success:
                            return new ModelWrapper<List<Post>>(postsOnline.Data, TaskResult.Success);
                        default:
                            return new ModelWrapper<List<Post>>(TaskResult.NoData);
                    }
                case TaskResult.Success:
                    return new ModelWrapper<List<Post>>(postsOffline.Data, TaskResult.Success);
                default:
                    return new ModelWrapper<List<Post>>(TaskResult.NoData);
            }
        }
    }
}