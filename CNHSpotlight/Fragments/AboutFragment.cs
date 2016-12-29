using System;
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

namespace CNHSpotlight
{
    public class AboutFragment : Android.Support.V4.App.Fragment
    {

        // UIs
        RelativeLayout rootView;

        TextView textviewAbout;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            rootView = (RelativeLayout)inflater.Inflate(Resource.Layout.AboutFragment, container, false);

            textviewAbout = rootView.FindViewById<TextView>(Resource.Id.aboutfragment_textview_content);

            return rootView;
        }
    }
}