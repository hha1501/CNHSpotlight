using System.Threading.Tasks;

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
using Android.Views.InputMethods;
using Android.Support.V4.View;

using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using Newtonsoft.Json;

using CNHSpotlight.WordPress;
using CNHSpotlight.WordPress.Models;

namespace CNHSpotlight
{
    [Activity(Label = "CNH Spotlight", MainLauncher = true, Icon = "@drawable/CNHIcon_", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HostActivity : AppCompatActivity
    {

        // UIs
        DrawerLayout drawerLayout;

        Toolbar toolbar;
        SearchView searchView;

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

            // update navigationView item selected state
            navigationView.Menu.FindItem(Resource.Id.navigation_menu_item_latest).SetChecked(true);

            PrepareNewsFragment();

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

        /// <summary>
        /// Call fetch news from NewsFragment
        /// </summary>
        private async Task FetchNews(CNHCategory category)
        {
            await newsFragment.FetchNews(category);
        }

        void PrepareNewsFragment()
        {
            if (newsFragment != null)
            {
                return;
            }

            // place in NewsFragment
            newsFragment = new NewsFragment();
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.hostActivity_fragment_layout, newsFragment).Commit();
        }

        public void ReadNews(Post post)
        {
            Intent intent = new Intent(this, typeof(ReadNewsActivity));
            intent.PutExtra("post_Json_extra", JsonConvert.SerializeObject(post));

            StartActivity(intent);
        }

        void OpenAbout()
        {
            Intent intent = new Intent(this, typeof(AboutActivity));

            StartActivity(intent);
        }

        private async void OnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawerLayout.CloseDrawer(navigationView);
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.navigation_menu_item_latest:
                    await FetchNews(CNHCategory.Latest);
                    break;
                case Resource.Id.navigation_menu_item_news:
                    await FetchNews(CNHCategory.News);
                    break;
                case Resource.Id.navigation_menu_item_education:
                    await FetchNews(CNHCategory.Education);
                    break;
                case Resource.Id.navigation_menu_item_education_abroadeducation:
                    await FetchNews(CNHCategory.StudyAbroad);
                    break;
                case Resource.Id.navigation_menu_item_education_contests:
                    await FetchNews(CNHCategory.Contest);
                    break;
                case Resource.Id.navigation_menu_item_club:
                    await FetchNews(CNHCategory.Club);
                    break;
                case Resource.Id.navigation_menu_item_entertainment:
                    await FetchNews(CNHCategory.Entertainment);
                    break;
                case Resource.Id.navigation_menu_item_cnhicon:
                    await FetchNews(CNHCategory.NHIcon);
                    break;
                case Resource.Id.navigation_menu_item_cnhinme:
                    await FetchNews(CNHCategory.NHInMe);
                    break;
                case Resource.Id.navigation_menu_item_outsideclass:
                    await FetchNews(CNHCategory.OutsideClass);
                    break;
                case Resource.Id.navigation_menu_item_trivial:
                    await FetchNews(CNHCategory.Trivial);
                    break;
                case Resource.Id.navigation_menu_item_about:
                    OpenAbout();
                    break;
                default:
                    break;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);

            IMenuItem searchItem = menu.FindItem(Resource.Id.toolbar_menu_item_searchview);

            searchView = (SearchView)searchItem.ActionView;
            searchView.SubmitButtonEnabled = true;
            searchView.QueryTextSubmit += async (o, e) =>
            {

                // clear focus close and keyboard
                View focusedView = CurrentFocus;

                if (focusedView != null)
                {
                    InputMethodManager.FromContext(this).HideSoftInputFromWindow(focusedView.WindowToken, HideSoftInputFlags.None); 
                    focusedView.ClearFocus();
                }

                e.Handled = true;

                await newsFragment.Search(e.Query);
            };

            SearchViewActionExpandListener expandListener = new SearchViewActionExpandListener();
            expandListener.Collapse += async (o, e) =>
            {
                await newsFragment.Search("");
            };

            MenuItemCompat.SetOnActionExpandListener(searchItem, expandListener);
            

            return base.OnCreateOptionsMenu(menu);
        }
    }
}

