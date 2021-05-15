using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Threading.Tasks;
using Android.Content;
using Even_Odds_Delivary.Activities;
using static Firebase.Auth.FirebaseAuth;
using Firebase.Auth;
using System;
using Firebase.Database;

namespace Even_Odds_Delivary
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : Activity
    {
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            if(savedInstanceState == null)
            {
                FirebaseDatabase.Instance.SetPersistenceEnabled(true);
            }


        }
        protected override void OnResume()
        {
            base.OnResume();
            Task startWork = new Task(() =>
            {
                Task.Delay(3000);
            });
            startWork.ContinueWith(t =>
            {
                var user = FirebaseAuth.Instance.CurrentUser;
                if (user != null)
                {
                    Intent intent = new Intent(Application.Context, typeof(Dashboad));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                }
                else
                {
                    Intent intent = new Intent(Application.Context, typeof(Login));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            startWork.Start();
        }

    }
}