using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Locations;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using AndroidHUD;
using Firebase.Messaging;
using driver.Fragments;
using System;
using AlertDialog = Android.App.AlertDialog;
using Plugin.CloudFirestore;
using Firebase.Auth;
using driver.Models;
using Google.Android.Material.BottomNavigation;

namespace driver.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class Dashboad : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

       // private FirebaseMessaging firebaseMessaging;// = new FirebaseMessaging();
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_dashboard);
            this.RequestedOrientation = ScreenOrientation.Portrait;

            ///*OUT*/
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            
            CheckGps();
            WelcomeFragment welcomeFrag = new WelcomeFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.frameLayout_container, welcomeFrag)
                .Commit();
            welcomeFrag.RequestEventHandler += WelcomeFrag_RequestEventHandler;
            await FirebaseMessaging.Instance.SubscribeToTopic("requests");



            CheckUserType();


        }
        private async void CheckUserType()
        {
            var results = await CrossCloudFirestore
                .Current
                .Instance
                .Collection("AppUsers")
                .Document(FirebaseAuth.Instance.Uid)
                .GetAsync();
            DriverModel driver = results.ToObject<DriverModel>();
            if(driver.Role == "D")
            {
                var requests = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("DeliveryRequests")
                    .WhereEqualsTo("DriverId", FirebaseAuth.Instance.Uid)
                    .WhereIn("Status", new[] { "A", "P" })
                    .GetAsync();
                if(requests.Count > 0)
                {
                    Intent intent = new Intent(this, typeof(DelivaryRequestActivity));
                    StartActivity(intent);
                }
                
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage("Your not registered as a driver, Please contact the administrator.");
                builder.SetNeutralButton("Continue", delegate
                {
                    builder.Dispose();
                    FirebaseAuth.Instance.SignOut();
                    Finish();
                });
                builder.Show();
            }

        }
        private void WelcomeFrag_RequestEventHandler(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DelivaryRequestActivity));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }

        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_requests:
                    WelcomeFragment welcomeFrag = new WelcomeFragment();
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, welcomeFrag)
                        .Commit();
                    welcomeFrag.RequestEventHandler += WelcomeFrag_RequestEventHandler;
                    return true;
                case Resource.Id.navigation_history:
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, new HistoryFragment())
                        .Commit();
                    return true;
                case Resource.Id.navigation_about:
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, new AboutFragment())
                        .Commit();
                    return true;
                case Resource.Id.navigation_profile:
                    ProfileFragment profileFrag = new ProfileFragment();

                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, profileFrag)
                        .Commit();
                    return true;
                case Resource.Id.navigation_logout:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Confirm");
                    
                    builder.SetMessage("Are you sure that you want to exit");
                    builder.SetPositiveButton("Yes", delegate
                    {
                        builder.Dispose();
                        Firebase.Auth.FirebaseAuth.Instance.SignOut();
                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            base.FinishAndRemoveTask();
                        }
                        else
                        {
                            base.Finish();
                        }
                    });
                    builder.SetNegativeButton("No", delegate
                    {
                        builder.Dispose();
                        return;
                    });
                    builder.Show();
                    return true;
            }
            return false;
        }


        private void ProfileFrag_SuccessUpdateHandler(object sender, EventArgs e)
        {
            HUD("Profile successfully updated");
        }

        private void CheckGps()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                builder.SetMessage("Enable location");
                builder.SetNegativeButton("Cancel", delegate
                {
                    builder.Dispose();
                });
                builder.SetPositiveButton("Settings", delegate
                {

                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                });
                builder.Show();

            }

        }



    }
}

