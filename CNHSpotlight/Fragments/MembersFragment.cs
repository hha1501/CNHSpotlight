using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;

using CNHSpotlight.WordPress.Models;
using CNHSpotlight.WordPress;

namespace CNHSpotlight
{
    public class MembersFragment : Android.Support.V4.App.Fragment
    {

        // UIs
        RelativeLayout rootView;

        ListView listviewMembers;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            rootView = (RelativeLayout)inflater.Inflate(Resource.Layout.MembersFragment, container, false);

            listviewMembers = rootView.FindViewById<ListView>(Resource.Id.membersfragment_listview_members);

            return rootView;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (listviewMembers.Count <= 0)
            {
                Task.Run(async () => await ListMembers()); 
            }
        }

        async Task ListMembers()
        {
            var usersOffline = DataManager.GetUsersOffline();


            switch (usersOffline.Result)
            {
                case TaskResult.NoData:
                    WordPressManager.UserRequest userRequest = new WordPressManager.UserRequest();
                    userRequest.Quantity(WordPressManager.ModelRequest.MaxUserCount);

                    var usersOnline = await WordPressManager.GetUsersOnline(userRequest);
                    switch (usersOnline.Result)
                    {
                        case TaskResult.Error:
                            Activity.RunOnUiThread(() =>
                            {
                                Snackbar
                                    .Make(rootView, "Error occured", Snackbar.LengthShort)
                                    .Show();
                            });
                            break;
                        case TaskResult.NoInternet:
                            Activity.RunOnUiThread(() =>
                            {
                                Snackbar
                                    .Make(rootView, "No internet connection", Snackbar.LengthIndefinite)
                                    .SetAction("Retry", async (view) => await ListMembers())
                                    .Show();
                            });
                            break;
                        case TaskResult.Success:
                            ArrayAdapter adapterOn = new ArrayAdapter(
                                Activity,
                                Android.Resource.Layout.SimpleListItem1,
                                usersOnline.Data.Select(user => user.Name).ToList()
                                );
                            Activity.RunOnUiThread(() =>
                            {
                                listviewMembers.Adapter = adapterOn;
                            });
                            break;
                        default:
                            break;
                    }
                    break;
                case TaskResult.Success:
                    ArrayAdapter adapterOff = new ArrayAdapter(
                        Activity,
                        Android.Resource.Layout.SimpleListItem1,
                        usersOffline.Data.Select(user => user.Name).ToList()
                        );
                    Activity.RunOnUiThread(() =>
                    {
                        listviewMembers.Adapter = adapterOff;
                    });
                    break;
                default:
                    break;
            }
        }
    }
}