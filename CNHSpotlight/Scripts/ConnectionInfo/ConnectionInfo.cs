using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Net;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CNHSpotlight.Scripts.ConnectionInfo
{
    static class ConnectionInfo
    {
        public static bool InternetConnected()
        {
            ConnectivityManager connectivityManager = ConnectivityManager.FromContext(Application.Context);

            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            return (networkInfo != null && networkInfo.IsConnected);
        }
    }
}