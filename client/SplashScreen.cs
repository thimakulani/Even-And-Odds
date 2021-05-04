using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using client.Activities;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using static Firebase.Auth.FirebaseAuth;

namespace client
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen :Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            RequestedOrientation = ScreenOrientation.Portrait;


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
                    Intent intent = new Intent(Application.Context, typeof(MainActivity));
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