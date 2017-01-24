using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace CNHSpotlight.Asset
{
    public static class AssetUtils
    {

        private static Lazy<Typeface> _font;

        public static Typeface CustomFont { get { return _font.Value; } }

        static AssetUtils()
        {
            _font = new Lazy<Typeface>(() => CreateTypeface("Fonts/Cabin-Bold.ttf"));
        }

        static Typeface CreateTypeface(string filePath)
        {
            return Typeface.CreateFromAsset(Application.Context.Assets, filePath);
        }

        public static string CreatePostTemplate(string file)
        {
            string path = $"Post Templates/{file}";

            string data = string.Empty;
            using (StreamReader streamReader = new StreamReader(Application.Context.Assets.Open(path)))
            {
                data = streamReader.ReadToEnd();
            }

            return data;
        }

        public static string CreateAbout(string file)
        {
            string path = $"About/{file}";

            string data = string.Empty;
            using (StreamReader streamReader = new StreamReader(Application.Context.Assets.Open(path)))
            {
                data = streamReader.ReadToEnd();
            }

            return data;
        }
    }
}