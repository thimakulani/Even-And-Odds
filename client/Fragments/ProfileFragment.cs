using System;
using Android.Content;
using Android.OS;
using Google.Android.Material.Button;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.TextField;

namespace client.Fragments
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputAltPhone;
        private TextInputEditText InputEmail;
        
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
            InputNames = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateName);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdatePhone);
            InputAltPhone = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateAltPhone);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateEmail);
            
            BtnAppyChanges = view.FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);

            BtnAppyChanges.Click += BtnAppyChanges_Click;
         
            
        }
        
      
        //public event EventHandler FailUpdateHandler; 
        public event EventHandler SuccessUpdateHandler;
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
            var stream = Resources.Assets.Open("service_account.json"); 

         

        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        
    }
}