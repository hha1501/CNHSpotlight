using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;

using CNHSpotlight.WordPress;

namespace CNHSpotlight
{
    [Activity(Label = "CNHSpotlight", MainLauncher = true, Icon = "@drawable/CNHIcon")]
    public class HostActivity : AppCompatActivity
    {

        // UIs
        DrawerLayout drawerLayout;

        Toolbar toolbar;

        FrameLayout fragmentView;

        NavigationView navigationView;

        // fragment
        NewsFragment newsFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HostActivity);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.hostActivity_drawerlayout_root);

            toolbar = FindViewById<Toolbar>(Resource.Id.hostActivity_toolbar);
            SetSupportActionBar(toolbar);

            fragmentView = FindViewById<FrameLayout>(Resource.Id.hostActivity_fragment_layout);

            navigationView = FindViewById<NavigationView>(Resource.Id.hostActivity_navigationview);
            navigationView.NavigationItemSelected += (o, e) => OnNavigationItemSelected(o, e);

            // place in NewsFragment
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
            newsFragment = new NewsFragment();
            fragmentTransaction.Add(Resource.Id.hostActivity_fragment_layout, newsFragment).Commit();

            // update navigationView item selected state
            navigationView.Menu.FindItem(Resource.Id.navigation_menu_item_news).SetChecked(true);         
        }

        private void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawerLayout.CloseDrawer(navigationView);
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.navigation_menu_item_news:
                    newsFragment.FetchNews(CNHCategory.News);
                    break;
                case Resource.Id.navigation_menu_item_education:
                    newsFragment.FetchNews(CNHCategory.Education);
                    break;
                case Resource.Id.navigation_menu_item_education_abroadeducation:
                    newsFragment.FetchNews(CNHCategory.StudyAbroad);
                    break;
                case Resource.Id.navigation_menu_item_education_contests:
                    newsFragment.FetchNews(CNHCategory.Contest);
                    break;
                case Resource.Id.navigation_menu_item_club:
                    newsFragment.FetchNews(CNHCategory.Club);
                    break;
                case Resource.Id.navigation_menu_item_entertainment:
                    newsFragment.FetchNews(CNHCategory.Entertainment);
                    break;
                case Resource.Id.navigation_menu_item_cnhicon:
                    newsFragment.FetchNews(CNHCategory.NHIcon);
                    break;
                case Resource.Id.navigation_menu_item_cnhinme:
                    newsFragment.FetchNews(CNHCategory.NHInMe);
                    break;
                case Resource.Id.navigation_menu_item_outsideclass:
                    newsFragment.FetchNews(CNHCategory.OutsideClass);
                    break;
                case Resource.Id.navigation_menu_item_trivial:
                    newsFragment.FetchNews(CNHCategory.Trivial);
                    break;
                default:
                    break;
            }
        }
    }
}

