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
using Android.Support.V4.View;

namespace CNHSpotlight
{
    class SearchViewActionExpandListener : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
    {

        public event EventHandler Collapse;

        public bool OnMenuItemActionCollapse(IMenuItem item)
        {
            Collapse?.Invoke(this, EventArgs.Empty);

            return true;   
        }

        public bool OnMenuItemActionExpand(IMenuItem item)
        {
            return true;
        }
    }
}