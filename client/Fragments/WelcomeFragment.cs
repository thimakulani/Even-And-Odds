using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Google.Android.Material.Button;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using client.Adapters;
 
using client.Classes;
using Firebase.Auth;
using Xamarin.Essentials;

namespace client.Fragments
{
    public class WelcomeFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        //string keyId;
        private MaterialButton RequestBtn;
        private RecyclerView recyclerFeeds;
        private List<AnnouncementModel> Items = new List<AnnouncementModel>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.welcome_fragment, container, false);


            //var mapFragment = (SupportMapFragment)FragmentManager.FindFragmentById(Resource.Id.fragMap);

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            //keyId = FirebaseAuth.Instance.CurrentUser.Uid;



            ConnectViews(view);
            return view;
        }
        private Context context;
        private void ConnectViews(View view)
        {
            RequestBtn = view.FindViewById<MaterialButton>(Resource.Id.BtnDeliveryRequest);
            recyclerFeeds = view.FindViewById<RecyclerView>(Resource.Id.recyclerFeeds);
            RequestBtn.Click += RequestBtn_Click;
            context = view.Context;
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context.ApplicationContext);
            AnnouncementAdapter adapter = new AnnouncementAdapter(Items);
            recyclerFeeds.SetLayoutManager(linearLayoutManager);
            recyclerFeeds.SetAdapter(adapter);

            /*GET ANNOUNCEMENTS*/
            
        }

       

        public event EventHandler RequestEventHandler;
        private void RequestBtn_Click(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
            {
                RequestEventHandler(sender, e);
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.SetTitle("Network Error");
                builder.SetMessage("Please check your Internet connection before you proceed");
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }
    }
}