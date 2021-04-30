using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Firebase.Database;
using driver.Adapters;
using driver.AppData;
using driver.FirebaseHelper;
using driver.MapsHelper;
using driver.Models;
using Xamarin.Essentials;

namespace driver.Fragments
{
    public class WelcomeFragment : Android.Support.V4.App.Fragment
    {


        string keyId;
        private MaterialButton RequestBtn;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.welcome_fragment, container, false);


            //var mapFragment = (SupportMapFragment)FragmentManager.FindFragmentById(Resource.Id.fragMap);

            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            keyId = pref.GetString("UserID", string.Empty);



            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            RequestBtn = view.FindViewById<MaterialButton>(Resource.Id.BtnGoOnline);
            RequestBtn.Click += RequestBtn_Click;
        }
        public event EventHandler RequestEventHandler;
        private void RequestBtn_Click(object sender, EventArgs e)
        {
            RequestEventHandler(sender, e);
        }
    }
}