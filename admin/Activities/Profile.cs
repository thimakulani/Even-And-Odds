using Android.App;
using Android.OS;
using Google.Android.Material.Button;
using Firebase.Auth;
using Firebase.Database;
using System;
using Google.Android.Material.TextField;
using Google.Android.Material.AppBar;

namespace admin.Activities
{
    [Activity(Label = "Profile")]
    public class Profile : Activity, IValueEventListener
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputAltPhone;
        private TextInputEditText InputEmail;

        private MaterialButton BtnAppyChanges;

        private MaterialToolbar include_app_toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.update_profile_dialog);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            InputNames = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateName);
            InputSurname = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdatePhone);
            InputAltPhone = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateAltPhone);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateEmail);
            BtnAppyChanges = FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);
            InputEmail.Enabled = false;
            BtnAppyChanges.Click += BtnAppyChanges_Click;

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .AddValueEventListener(this);
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick;
        }

        private void Include_app_toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void BtnAppyChanges_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputNames.Text))
            {
                InputNames.Error = "Name cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text))
            {
                InputSurname.Error = "Surname cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text))
            {
                InputPhone.Error = "Phone number cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text))
            {
                InputEmail.Error = "Email cannot be empty";
                return;
            }
            
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(FirebaseAuth.Instance.CurrentUser.Uid).Child("Name").SetValue(InputNames.Text);
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(FirebaseAuth.Instance.CurrentUser.Uid).Child("Surname").SetValue(InputSurname.Text);
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(FirebaseAuth.Instance.CurrentUser.Uid).Child("Phone").SetValue(InputPhone.Text);
            if (!string.IsNullOrWhiteSpace(InputAltPhone.Text))
            {
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(FirebaseAuth.Instance.CurrentUser.Uid).Child("AltPhone").SetValue(InputAltPhone.Text);
            }
            //FirebaseDatabase.Instance.GetReference("AppUsers").Child(FirebaseAuth.Instance.CurrentUser.Uid).Child("Email").SetValue(InputEmail.Text);

        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                if (snapshot.Child("Name").Exists())
                {
                    InputNames.Text = snapshot.Child("Name").Value.ToString();
                }
                if (snapshot.Child("Surname").Exists())
                {
                    InputSurname.Text = snapshot.Child("Surname").Value.ToString();
                }
                if (snapshot.Child("Phone").Exists())
                {
                    InputPhone.Text = snapshot.Child("Phone").Value.ToString();
                }
                if (snapshot.Child("AltPhone").Exists())
                {
                    InputAltPhone.Text = snapshot.Child("AltPhone").Value.ToString();
                }
                if (snapshot.Child("Email").Exists())
                {
                    InputEmail.Text = snapshot.Child("Email").Value.ToString();
                }

            }
        }
    }
}