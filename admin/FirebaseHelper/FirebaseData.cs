using Android.App;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace admin.FirebaseHelper
{
    class FirebaseData
    {

        public static FirebaseAdmin.Auth.FirebaseAuth GetFirebaseAdminAuth(Stream input)
        {
            FirebaseAdmin.Auth.FirebaseAuth auth;
            FirebaseAdmin.AppOptions options = new FirebaseAdmin.AppOptions()
            {
                Credential = GoogleCredential.FromStream(input),
                ProjectId = "even-and-odds",
                ServiceAccountId = "firebase-adminsdk-xcwnu@even-and-odds.iam.gserviceaccount.com",

            };
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                var app = FirebaseAdmin.FirebaseApp.Create(options);
                auth = FirebaseAdmin.Auth.FirebaseAuth.GetAuth(app);
            }
            else
            {
                auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            }
            return auth;
        }
 
    }
}