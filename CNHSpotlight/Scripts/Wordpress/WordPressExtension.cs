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

namespace CNHSpotlight.WordPress
{
     static class WordPressExtension
    {
        /// <summary>
        /// Implemented both offline and online
        /// <para>
        /// Return name of user whose Id matches userId or "Unknown" if task failed
        /// </para>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<string> GetUserName(int userId)
        {

            // offline
            var userOff = DataManager.GetUserOffline(userId);

            if (userOff.Result == TaskResult.Success)
            {
                return userOff.Data.name;
            }

            // online
            var userOn = await WordPressManager.GetUserOnline(userId);

            if (userOn.Result == TaskResult.Success)
            {
                return userOn.Data.name;
            }

            return "Unknown";
        }
    }
}