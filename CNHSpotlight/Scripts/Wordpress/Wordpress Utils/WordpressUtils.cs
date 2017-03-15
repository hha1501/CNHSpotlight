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
            Dictionary<int, Post> sourceDictionary = source.AsEnumerable().Reverse().ToDictionary(post => post.Id);

            foreach (Post newPost in collection.AsEnumerable().Reverse())
            {
                if (sourceDictionary.ContainsKey(newPost.Id))
                {
                    sourceDictionary[newPost.Id] = newPost;
                }
                else
                {
                    sourceDictionary.Add(newPost.Id, newPost);
                }
            }

            return sourceDictionary.Select(item => item.Value).Reverse().ToList();
        }
    }
}