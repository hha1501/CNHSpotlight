using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CNHSpotlight.ImageResource
{
    static class ImageExtension
    {
        public static string GetSHA1Hash(this string source)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
                return string.Join("", hashed.Select(b => b.ToString("x2")));
            }
        }

        public static Bitmap DecodeResource(byte[] resource)
        {
            return BitmapFactory.DecodeByteArray(resource, 0, resource.Length);
        }
    }
}