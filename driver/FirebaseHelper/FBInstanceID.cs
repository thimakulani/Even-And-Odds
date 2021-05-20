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


    }
}