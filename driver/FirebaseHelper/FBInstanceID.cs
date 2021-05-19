using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.Media;
using Firebase.Iid;
using Firebase.Messaging;
using driver.Activities;
using AndroidX.Core.App;

namespace driver.FirebaseHelper
{

    public class FBInstanceID: FirebaseMessagingService
    {
        public async override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);
            var instanceIdResult = await FirebaseInstanceId.Instance.GetInstanceId().AsAsync<IInstanceIdResult>();
            //var refresh = FirebaseInstanceId.Instance.Token;//.AddOnSuccessListener(this);

            if (!string.IsNullOrEmpty(instanceIdResult.Token))
            {
                
                await FirebaseMessaging.Instance.SubscribeToTopic("request");

            }

        }
        public override void OnMessageReceived(RemoteMessage p0)
        {
            
            base.OnMessageReceived(p0);
            CreateNotificationChannel(p0);
        }
        void CreateNotificationChannel(RemoteMessage p0)
        {
            var intent = new Intent(this, typeof(Dashboad));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0 /* Request code */, intent, PendingIntentFlags.OneShot);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new NotificationCompat.Builder(this, "100")
                .SetSmallIcon(Resource.Drawable.delivary_icon_2)
                .SetContentTitle("FCM Message")
                .SetContentText(p0.GetNotification().Body)
                .SetAutoCancel(true)
                .SetSound(defaultSoundUri)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);

            notificationManager.Notify(0, notificationBuilder.Build());
        }
        //{
        //    if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        //    {
        //        // Notification channels are new in API 26 (and not a part of the
        //        // support library). There is no need to create a notification
        //        // channel on older versions of Android.
        //        return;
        //    }

        //    var channel = new NotificationChannel("100", "Alert", NotificationImportance.Default)
        //    {
        //        Description = p0.GetNotification().Body,
        //    };

        //    var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
        //    notificationManager.CreateNotificationChannel(channel);
        //}

    }
}