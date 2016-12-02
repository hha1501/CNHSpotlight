using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content.Res;
using Android.Content;

using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;

using WordPressPCL.Models;
using Newtonsoft.Json;

using CNHSpotlight.WordPress;

namespace CNHSpotlight
{
    [Activity(Label = "CNHSpotlight", MainLauncher = true, Icon = "@drawable/CNHIcon_", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HostActivity : AppCompatActivity
    {

        // UIs
        DrawerLayout drawerLayout;

        Toolbar toolbar;

        FrameLayout fragmentView;

        NavigationView navigationView;

        // actionbardrawertoggle
        ActionBarDrawerToggle actionBarDrawerToggle;

        // fragment
        NewsFragment newsFragment;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HostActivity);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.hostActivity_drawerlayout_root);

            toolbar = FindViewById<Toolbar>(Resource.Id.hostActivity_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "CNH Spotlight";

            // configure action bar drawer toggle
            actionBarDrawerToggle = new ActionBarDrawerToggle(
                this, 
                drawerLayout,
                toolbar,
                Resource.String.actionbardrawertoggle_description_open,
                Resource.String.actionbardrawertoggle_description_close);
            drawerLayout.AddDrawerListener(actionBarDrawerToggle);
            
            // fragmentView
            fragmentView = FindViewById<FrameLayout>(Resource.Id.hostActivity_fragment_layout);

            // navigationView
            navigationView = FindViewById<NavigationView>(Resource.Id.hostActivity_navigationview);
            navigationView.NavigationItemSelected += (o, e) => OnNavigationItemSelected(o, e);

            // load news from default category
            FetchNews(CNHCategory.News);

            // update navigationView item selected state
            navigationView.Menu.FindItem(Resource.Id.navigation_menu_item_news).SetChecked(true);         
        }

        #region Overrirden methods required by actionBarDrawerToggle
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            actionBarDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            actionBarDrawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            actionBarDrawerToggle.OnOptionsItemSelected(item);

            return base.OnOptionsItemSelected(item);
        }
        #endregion

        void FetchNews(CNHCategory category)
        {

            // the first time NewsFragement is loaded
            if (newsFragment == null)
            {
                newsFragment = new NewsFragment(category);

                // place in NewsFragment
                Android.Support.V4.App.FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.hostActivity_fragment_layout, newsFragment).Commit();
            }
            else
            {
                newsFragment.FetchNews(category);
            }
        }

        public void ReadNews(Post post)
        {
            Intent intent = new Intent(this, typeof(ReadNewsActivity));
            intent.PutExtra("post_Json_extra", JsonConvert.SerializeObject(post));

            StartActivity(intent);
        }

        private void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawerLayout.CloseDrawer(navigationView);
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.navigation_menu_item_news:
                    FetchNews(CNHCategory.News);
                    break;
                case Resource.Id.navigation_menu_item_education:
                    FetchNews(CNHCategory.Education);
                    break;
                case Resource.Id.navigation_menu_item_education_abroadeducation:
                    FetchNews(CNHCategory.StudyAbroad);
                    break;
                case Resource.Id.navigation_menu_item_education_contests:
                    FetchNews(CNHCategory.Contest);
                    break;
                case Resource.Id.navigation_menu_item_club:
                    FetchNews(CNHCategory.Club);
                    break;
                case Resource.Id.navigation_menu_item_entertainment:
                    FetchNews(CNHCategory.Entertainment);
                    break;
                case Resource.Id.navigation_menu_item_cnhicon:
                    FetchNews(CNHCategory.NHIcon);
                    break;
                case Resource.Id.navigation_menu_item_cnhinme:
                    FetchNews(CNHCategory.NHInMe);
                    break;
                case Resource.Id.navigation_menu_item_outsideclass:
                    FetchNews(CNHCategory.OutsideClass);
                    break;
                case Resource.Id.navigation_menu_item_trivial:
                    FetchNews(CNHCategory.Trivial);
                    break;
                default:
                    break;
            }
        }

    }
}

