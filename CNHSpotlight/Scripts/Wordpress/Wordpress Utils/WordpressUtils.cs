using System;
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

namespace CNHSpotlight.WordPress.Utils
{
    static class WordpressUtils
    {
        public static List<Post> ReplacePosts(this List<Post> source, List<Post> collection)
        {
            return source.Union(collection).OrderByDescending(post => post.DateGmt).ToList();
        }
    }
}