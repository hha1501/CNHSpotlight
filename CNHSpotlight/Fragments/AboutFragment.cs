using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Android.Support.Design.Widget;

using CNHSpotlight.WordPress;
using CNHSpotlight.Asset;

namespace CNHSpotlight
{
    public class AboutFragment : Android.Support.V4.App.Fragment
    {

        // UIs
        RelativeLayout rootView;

        WebView webViewAbout;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            rootView = (RelativeLayout)inflater.Inflate(Resource.Layout.AboutFragment, container, false);

            webViewAbout = rootView.FindViewById<WebView>(Resource.Id.aboutfragment_webview_about);

            return rootView;
        }

        public override void OnResume()
        {
            base.OnResume();

            LoadAbout();
        }

        private void LoadAbout()
        {
            string html = AssetUtils.CreateAbout("about");
            webViewAbout.LoadDataWithBaseURL(null, html, "text/html", "utf-8", null);
        }
    }
}