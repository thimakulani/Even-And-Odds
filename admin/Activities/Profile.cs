using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "Profile")]
    public class Profile : Activity, IValueEventListener
    {
        private EditText InputNames;
        private EditText InputSurname;
        private EditText InputPhone;
        private EditText InputAltPhone;
        private EditText InputEmail;

        private MaterialButton BtnAppyChanges;

        private Android.Support.V7.Widget.Toolbar include_app_toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.update_profile_dialog);
            include_app_toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.include_app_toolbar);
            InputNames = FindViewById<EditText>(Resource.Id.ProfileUpdateName);
            InputSurname = FindViewById<EditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = FindViewById<EditText>(Resource.Id.ProfileUpdatePhone);
            InputAltPhone = FindViewById<EditText>(Resource.Id.ProfileUpdateAltPhone);
            InputEmail = FindViewById<EditText>(Resource.Id.ProfileUpdateEmail);
            BtnAppyChanges = FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);
            InputEmail.Enabled = false;
            BtnAppyChanges.Click += BtnAppyChanges_Click;

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(FirebaseAuth.Instance.CurrentUser.Uid)
                .AddValueEventListener(this);
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick;
        }

        private void Include_app_toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
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
        private bool IsEmailValid()
        {
            return Android.Util.Patterns.EmailAddress.Matcher(InputEmail.Text).Matches();
        }
        private string CheckPhoneNumber(string phone)
        {
            phone = phone.Trim();
            if (phone.StartsWith("0"))
            {
                phone = $"+27{phone.Remove(0, 1)}";
            }
            return phone;
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