using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Java.Util;
using Firebase.Database;
using Firebase.Auth;
using Android.Support.Design.Card;
using System.Collections.Generic;
using Even_Odds_Delivary.Models;
using Even_Odds_Delivary.Adapters;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "Dashboad")]
    public class Dashboad : Activity, IValueEventListener
    {
        
        private string UserKeyId;

        //loading progress dialog
        private AlertDialog loading;
        private AlertDialog.Builder loadingBuilder;

        //dialogs
        private TextView txtDashboardUsername;
        private RecyclerView RecyclerMennu;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_dashboard);
            UserKeyId = FirebaseAuth.Instance.CurrentUser.Uid;
            RecyclerMennu = FindViewById<RecyclerView>(Resource.Id.RecyclerMennu);
            txtDashboardUsername = FindViewById<TextView>(Resource.Id.txtDashboardUsername);
           
            LoadingProgress();
            GetUserInfo();
            SetUpMenu();
 
        }
        private List<Menu_Items> items = new List<Menu_Items>();
        private void SetUpMenu()
        {
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_admin_panel_settings_black_18dp, Title = "My Profile" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_admin_panel_settings_black_18dp, Title = "Super Users" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.user_profile_icon, Title = "Drivers and Clients" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_announcement_black_18dp, Title = "Announce" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_commute_white_18dp, Title = "Driver Requests" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_bar_chart_black_18dp, Title = "Stats" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_addchart_black_18dp, Title = "Manage Fee" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_account_tree_black_18dp, Title = "Invoice" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_assignment_black_18dp, Title = "Delivery Report" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_attach_email_black_18dp, Title = "Queries" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_exit_to_app_black_18dp, Title = "Logout" });

            MenuAdapter adapter = new MenuAdapter(items);
            GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 2);
            RecyclerMennu.SetLayoutManager(gridLayoutManager);
            RecyclerMennu.SetAdapter(adapter);
            RecyclerMennu.HasFixedSize = true;
            adapter.ItemClick += Adapter_ItemClick;
        }

        private void Adapter_ItemClick(object sender, MenuAdapterClickEventArgs e)
        {
            if (e.Position == 0)
            {
                Intent intent = new Intent(this, typeof(Profile));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 1)
            {
                Intent intent = new Intent(this, typeof(SuperUser));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 2)
            {
                Intent intent = new Intent(this, typeof(RegisterDriver));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 3)
            {
                Intent intent = new Intent(this, typeof(Anouncements));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 4)
            {
                Intent intent = new Intent(this, typeof(DriverRequests));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 5)
            {
                Intent intent = new Intent(this, typeof(StatsActivity));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 6)
            {
                
                Intent intent = new Intent(this, typeof(TripPrice));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 7)
            {
                Intent intent = new Intent(this, typeof(Invoice));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 8)
            {
                Intent intent = new Intent(this, typeof(Report));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 9)
            {
                Intent intent = new Intent(this, typeof(Queries));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 10)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage("Are you sure that you want to exit");
                builder.SetPositiveButton("Yes", delegate
                {
                    builder.Dispose();
                    //  base.OnBackPressed();
                    FirebaseAuth.Instance.SignOut();
                    Finish();
                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();
            }
        }
        private void CVAbout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(About));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
        }
        private void LoadingProgress()
        {
            loadingBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.loading_progress, null);


            loadingBuilder.SetView(view);
            loadingBuilder.SetCancelable(false);
            loading = loadingBuilder.Create();
            loading.Show();
        }
        private void GetUserInfo()
        {
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).AddValueEventListener(this);
        }

        private void CVLogout_Click(object sender, EventArgs e)
        {
            
        }
        public override void OnBackPressed()
        {
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirm");
            // builder.SetMessage("Reset password link has been sent to your email address");
            builder.SetMessage("Are you sure that you want to exit");
            builder.SetPositiveButton("Yes", delegate
            {
                
                builder.Dispose();
                base.OnBackPressed();
                Finish();
            });
            builder.SetNegativeButton("No", delegate
            {
                builder.Dispose();
                return;
            });
            builder.Show();
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                if(snapshot.Child("UserType").Value.ToString() == "Admin")
                {
                    txtDashboardUsername.Text = "Welcome: " + snapshot.Child("Name").Value.ToString() + ", " + snapshot.Child("Surname").Value.ToString();
                }
                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Error");
                    builder.SetMessage("Unauthorized user");
                    builder.SetNeutralButton("Ok", delegate
                    {
                        builder.Dispose();
                        FirebaseAuth.Instance.SignOut();
                        Finish();
                    }).Show();
                    
                }
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage("Unauthorized user");
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                    FirebaseAuth.Instance.SignOut();
                    Finish();
                }).Show();
            }
            loading.Dismiss();
            //loading.Dispose();
        }
    }

}