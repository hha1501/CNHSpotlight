using System;
using System.IO;
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


using HtmlAgilityPack;

using CNHSpotlight.WordPress;
using CNHSpotlight.WordPress.Models;

namespace CNHSpotlight.HtmlParser
{
    static class HtmlFormatter
    {

        /// <summary>
        /// The formatting process might be time consuming. Consider executing this on another thread
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static string FormatPost(Post post)
        {
            // inflate post-heading -- which contains Title and Author data
            HtmlDocument headingDoc = new HtmlDocument();
            headingDoc.LoadHtml(GetAssetData("post-heading.html"));

            // insert title
            headingDoc.GetElementbyId("title").InnerHtml = post.Title.Rendered;

            // insert meta (author)
            headingDoc.GetElementbyId("author").InnerHtml = post.Embedded.Author.FirstOrDefault().Name;

            // insert meta (publish date)
            headingDoc.GetElementbyId("publish-date").InnerHtml = post.Date.ToString("D");

            // insert thumbnail (adding src)
            headingDoc.GetElementbyId("thumbnail")
                .SetAttributeValue("src", post.Embedded.WpFeaturedMedia.FirstOrDefault().SourceUrl);

            // inflate post-template
            HtmlDocument templateDoc = new HtmlDocument();
            templateDoc.LoadHtml(GetAssetData("post-template.html"));

            // add css
            templateDoc.GetElementbyId("style").InnerHtml = GetAssetData("post-detail.css");

            // add heading
            templateDoc.GetElementbyId("heading").InnerHtml = headingDoc.DocumentNode.OuterHtml;

            // add content
            templateDoc.GetElementbyId("content").InnerHtml = post.Content.Rendered;

            return templateDoc.DocumentNode.OuterHtml;
        }

        static string GetAssetData(string filePath)
        {
            string data = string.Empty;
            using (StreamReader streamReader = new StreamReader(Application.Context.Assets.Open(filePath)))
            {
                data = streamReader.ReadToEnd();
            }

            return data;
        }
    }


}