using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using client.Fragments;
using Android.Locations;
using Android.Content;
using AndroidHUD;
using System;
using Firebase.Database;
using client.Activities;
using Android.Support.V4.Widget;
using Firebase.Auth;
using Android.Support.V7.Widget;
using Android.Content.PM;
using Google.Android.Material.Navigation;
using Google.Android.Material.AppBar;

namespace client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, IValueEventListener
    {
        private NavigationView nav_view;
       // HomeFragment homeFragment;
        TextView HeaderUsername;
        TextView TxtHeaderEmail;
        private DrawerLayout drawerLayout;
        private MaterialToolbar toolbar_main;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            string log = Intent.GetStringExtra("Log");
            RequestedOrientation = ScreenOrientation.Portrait;

            toolbar_main = FindViewById<MaterialToolbar>(Resource.Id.toolbar_main);
            nav_view = FindViewById<NavigationView>(Resource.Id.nav_view);
            nav_view.InflateMenu(Resource.Menu.nav_menu);
            nav_view.InflateHeaderView(Resource.Layout.nav_header_layout);
            HeaderUsername = nav_view.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TxtHeaderUsername);
            TxtHeaderEmail = nav_view.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TxtHeaderEmail);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.nav_drawer);
            nav_view.NavigationItemSelected += Nav_view_NavigationItemSelected;

            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid)
                .AddValueEventListener(this);

            WelcomeFragment homeFragment = new WelcomeFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.fragment_container, homeFragment)
                .Commit();
            toolbar_main.Title = "Home";
            homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;

            if (log == "History")
            {
                HistoryFragment frag = new HistoryFragment(this);
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "History";

                frag.CancelHandler += Frag_CancelHandler;
            }
           
            
            toolbar_main.SetNavigationIcon(Resource.Mipmap.ic_menu_white_18dp);
            toolbar_main.NavigationClick += Toolbar_main_NavigationClick1;
            CheckGps();


        }

        private void Toolbar_main_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
        }
        private void Frag_CancelHandler(object sender, HistoryFragment.CancelRequestEventHandler e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Confirm");
            alert.SetMessage("Are you sure you want to delete the delivery request:");
            alert.SetPositiveButton("Yes", delegate
            {

                FirebaseDatabase.Instance
                .GetReference("DelivaryRequest")
                .Child(e.Items.KeyId)
                .Child("Status")
                .SetValue("Canceled");
                HUD("Request Canceled");
                alert.Dispose();

            });
            alert.SetNegativeButton("No", delegate
            {
                alert.Dispose();
            });
            alert.Show();
        }

 
        private void HomeFragment_RequestEventHandler(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DeliveryRequest));
            intent.PutExtra("Log", "Other");
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
            if(toolbar_main.Title == "History")
            {
                WelcomeFragment homeFragment = new WelcomeFragment();
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.fragment_container, homeFragment)
                    .Commit();
                toolbar_main.Title = "Home";
                homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;
            }


        }

        private void Nav_view_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawerLayout.CloseDrawers();
            if (e.MenuItem.ItemId == Resource.Id.nav_profile)
            {
                

                ProfileFragment profileFragment = new ProfileFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, profileFragment)
                    .Commit();
                toolbar_main.Title = "User Profile";

                profileFragment.SuccessUpdateHandler += ProfileFragment_SuccessUpdateHandler;
            }
            if (e.MenuItem.ItemId == Resource.Id.nav_home)
            {
                WelcomeFragment homeFragment = new WelcomeFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, homeFragment)
                    .Commit();
               // TxtAppBarTitle.Text = "Home";
                homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;
                toolbar_main.Title = "Home";


            }
            if (e.MenuItem.ItemId == Resource.Id.nav_help)
            {
                HelpFragment helpFrag = new HelpFragment();
                SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.fragment_container, helpFrag)
                .Commit();
                toolbar_main.Title = "Help";


            }
            if (e.MenuItem.ItemId == Resource.Id.nav_history)
            {
                HistoryFragment frag = new HistoryFragment(this);
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "History";

                frag.CancelHandler += Frag_CancelHandler;

            }
            if (e.MenuItem.ItemId == Resource.Id.nav_about)
            {
                AboutFragment frag = new AboutFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "About";

                

            }
            if (e.MenuItem.ItemId == Resource.Id.nav_logout)
            {

                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Confirm");
                alert.SetMessage("Are you sure you want to exit:");
                alert.SetPositiveButton("Yes", delegate
                {

                    
                    alert.Dispose();
                    FirebaseAuth.Instance.SignOut();
                    if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        base.FinishAndRemoveTask();
                    }
                    else
                    {
                        base.Finish();
                    }

                });
                alert.SetNegativeButton("No", delegate
                {
                    alert.Dispose();
                });
                alert.Show();
            }

        }
        public override void OnBackPressed()
        {
            
        }
        private void CheckGps()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this, Resource.Style.AlertDialog_AppCompat);
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
        private void ProfileFragment_SuccessUpdateHandler(object sender, System.EventArgs e)
        {
            HUD("Profile successfully updated");
        }
        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
        public void CheckLocationEnabled()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
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
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot != null)
            {
                if(snapshot.Child("Name").Exists() && snapshot.Child("Email").Exists() && snapshot.Child("Surname").Exists())
                {
                    HeaderUsername.Text = snapshot.Child("Name").Value.ToString()
                        + " " + snapshot.Child("Surname").Value.ToString();
                    TxtHeaderEmail.Text = snapshot.Child("Email").Value.ToString();
                }
                
            }
        }
    }
}