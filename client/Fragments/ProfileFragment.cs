using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using client.FirebaseHelper;
using Firebase.Auth;
using Firebase.Database;
using FirebaseAdmin.Auth;

namespace client.Fragments
{
    public class ProfileFragment : Android.Support.V4.App.Fragment, IValueEventListener
    {
        private EditText InputNames;
        private EditText InputSurname;
        private EditText InputPhone;
        private EditText InputAltPhone;
        private EditText InputEmail;
        
        private MaterialButton BtnAppyChanges;


        //userkeyId
        private string UserKeyId;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.update_profile_dialog, container, false);
            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            UserKeyId = Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            InputNames = view.FindViewById<EditText>(Resource.Id.ProfileUpdateName);
            InputSurname = view.FindViewById<EditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = view.FindViewById<EditText>(Resource.Id.ProfileUpdatePhone);
            InputAltPhone = view.FindViewById<EditText>(Resource.Id.ProfileUpdateAltPhone);
            InputEmail = view.FindViewById<EditText>(Resource.Id.ProfileUpdateEmail);
            
            BtnAppyChanges = view.FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);

            BtnAppyChanges.Click += BtnAppyChanges_Click;
         
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(UserKeyId)
                .AddValueEventListener(this);
        }
        
      
        //public event EventHandler FailUpdateHandler; 
        public event EventHandler SuccessUpdateHandler;
        private async void BtnAppyChanges_Click(object sender, EventArgs e)
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
            var stream = Resources.Assets.Open("service_account.json");
            var auth = FirebaseData.GetFirebaseAdminAuth(stream);

            try
            {

                UserRecordArgs user = new UserRecordArgs()
                {
                    DisplayName = $"{InputNames.Text.Trim()} {InputSurname.Text.Trim()}",
                    Email = InputEmail.Text.Trim(),
                    Uid = UserKeyId,
                    //PhoneNumber = CheckPhoneNumber(InputPhone.Text.Trim()),

                };
                await auth.UpdateUserAsync(user);
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).Child("Name").SetValue(InputNames.Text.Trim());
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).Child("Surname").SetValue(InputSurname.Text.Trim());
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).Child("Phone").SetValue(CheckPhoneNumber(InputPhone.Text.Trim()));
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).Child("AltPhone").SetValue(CheckPhoneNumber(InputAltPhone.Text.Trim()));
                FirebaseDatabase.Instance.GetReference("AppUsers").Child(UserKeyId).Child("Email").SetValue(InputEmail.Text.Trim());
                
                SuccessUpdateHandler(sender, e);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Context.ApplicationContext, ex.Message, ToastLength.Long).Show();
            }
            finally
            {

            }

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
            if(snapshot != null)
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