using System;
using Android.Content;
using Android.OS;
using Google.Android.Material.Button;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System.Collections.Generic;
using AndroidHUD;
using Firebase.Auth;

namespace client.Fragments
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputEmail;
        
        private MaterialButton BtnAppyChanges;
        private Context context;

        //userkeyId
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
         
            
        }
        
      
        //public event EventHandler FailUpdateHandler; 
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
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { "Name", InputNames.Text.Trim() },
                { "Phone", InputPhone.Text.Trim() },
                { "Surname", InputSurname.Text.Trim() }
            };
            await CrossCloudFirestore.Current
                .Instance
                .Collection("AppUsers")
                .Document(FirebaseAuth.Instance.Uid)
                .UpdateAsync(keyValues);



            AndHUD.Shared.ShowSuccess(context, "Profile has been successfully updated!!", MaskType.Black, TimeSpan.FromSeconds(3));



        }

  
        
    }
}