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
using Android.Support.V7.App;
using Android.Content.PM;

using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace CNHSpotlight
{
    [Activity(ParentActivity = typeof(HostActivity), ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutActivity : AppCompatActivity
    {

        // UIs
        Toolbar toolbar;

        FrameLayout fragmentContainer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AboutActivity);

            // toolbar
            toolbar = FindViewById<Toolbar>(Resource.Id.aboutactivity_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "About";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            fragmentContainer = FindViewById<FrameLayout>(Resource.Id.aboutactivity_fragment_container);

            ShowAbout();
        }

        void ShowCopyright()
        {
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder
                .SetTitle(GetString(Resource.String.cnhspotlight_copyright_title))
                .SetMessage(GetString(Resource.String.cnhspotlight_copyright_content))
                .SetPositiveButton("Ok", (o, e) => { });

            dialogBuilder.Create().Show();

        }

        void ShowAbout()
        {
            AboutFragment aboutFragment = new AboutFragment();

            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.aboutactivity_fragment_container, new AboutFragment()).Commit();
            
        }

        void ShowMember()
        {
            AboutFragment aboutFragment = new AboutFragment();

            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();

            if (SupportFragmentManager.FindFragmentByTag("member_fragment") == null)
            {
                transaction.AddToBackStack("member_fragment");
            }

            transaction
                .Replace(Resource.Id.aboutactivity_fragment_container, new MembersFragment());

            transaction.Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.about_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.about_menu_item_copyright:
                    ShowCopyright();
                    return true;
                case Resource.Id.about_menu_item_members:
                    ShowMember();
                    return true;
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

    }
}