using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Firebase.Events;
using driver.FirebaseHelper;
using Plugin.CloudFirestore;
using Firebase.Auth;
using driver.Models;
using AndroidHUD;
using Java.Util;
using Google.Android.Material.TextField;
using Google.Android.Material.Button;

namespace driver.Fragments
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputEmail;
        
        private MaterialButton BtnAppyChanges;


        //userkeyId
        private Context context;
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
            context = view.Context;
            ConnectViews(view);
            
            return view;
        }

        private void ConnectViews(View view)
        {
            InputNames = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateName);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdatePhone);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateEmail);
            BtnAppyChanges = view.FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);

            BtnAppyChanges.Click += BtnAppyChanges_Click;
           
            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            //UserKeyId = FirebaseAuth.Instance.CurrentUser.Uid;
            CrossCloudFirestore.Current.Instance.Collection("AppUsers")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot.Exists)
                    {
                        var user = snapshot.ToObject<DriverModel>();
                        InputNames.Text = user.Name;
                        InputSurname.Text = user.Surname;
                        InputPhone.Text = user.Phone;
                        InputEmail.Text = user.Email;
                    }
                });
        }
        
        
       
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

            DriverModel d = new DriverModel()
            {
                Email = InputEmail.Text,
                Name = InputNames.Text,
                Phone = InputPhone.Text,
                Surname = InputSurname.Text,

            };

            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("Name", InputNames.Text.Trim());
            keyValues.Add("Phone", InputPhone.Text.Trim());
            keyValues.Add("Surname", InputSurname.Text.Trim());
            await CrossCloudFirestore.Current
                .Instance
                .Collection("AppUsers")
                .Document(FirebaseAuth.Instance.Uid)
                .UpdateAsync(keyValues);
                
                

            AndHUD.Shared.ShowSuccess(context, "Profile has been successfully updated!!", MaskType.Black, TimeSpan.FromSeconds(3));


        }

    }
}