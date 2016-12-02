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

using WordPressPCL.Models;

using HtmlAgilityPack;

using CNHSpotlight.WordPress;

namespace CNHSpotlight.HtmlParser
{
    static class HtmlFormatter
    {
        static readonly string HtmlWrap = @"<html></html>";
        static readonly string ImageStyle = 
            @"<head>
                <style>
                    img{display: inline;height: auto;max-width: 100%;}
                    figure{margin-left: 0;margin-right: 0;}
                    iframe{width: 100%; height: auto;}
                </style>
              </head>";

        public static async Task<string> FormatPost(Post post)
        {
            string author = await WordPressExtension.GetUserName(post.Author);

            DateTime date = post.Date;

            // load html to modify
            HtmlDocument htmlDocument = new HtmlDocument();

            // create a wrapper to wrap content
            htmlDocument.LoadHtml(HtmlWrap);

            // wrap content by root node
            // First is <html></html>
            HtmlNode topNode = htmlDocument.DocumentNode.FirstChild;

            // add style to node
            topNode.AppendChild(HtmlNode.CreateNode(ImageStyle));

            // extra info & content
            string extraInfo = string.Format("{0}     {1}", author, date);

            string content = string.Format("<body><div>{0}\n{1}</div></body>", extraInfo, post.Content.Rendered);

            topNode.InnerHtml += content;

            // modify images

            // 'figure' block modification
            foreach (HtmlNode figureNode in htmlDocument.DocumentNode.Descendants("figure").ToList())
            {
                figureNode.Attributes.RemoveAll();
            }

            // inline modification
            foreach (HtmlNode imageNode in htmlDocument.DocumentNode.Descendants("img"))
            {

                foreach (HtmlAttribute imageAttribute in imageNode.Attributes.ToList())
                {
                    // skip 'src' and 'alt' attributes
                    if (imageAttribute.Name == "src" || imageAttribute.Name == "alt")
                    {
                        continue;
                    }

                    // modify width
                    if (imageAttribute.Name == "width")
                    {
                        imageAttribute.Value = "100%";
                        continue;
                    }

                    // modify height
                    if (imageAttribute.Name == "height")
                    {
                        imageAttribute.Value = "auto";
                        continue;
                    }


                    // other attributes are excluded
                    imageAttribute.Remove();
                }
            }

            // modify iframes

            foreach (HtmlNode iframeNode in htmlDocument.DocumentNode.Descendants("iframe"))
            {

            }

            return htmlDocument.DocumentNode.InnerHtml;
        }
    }
}