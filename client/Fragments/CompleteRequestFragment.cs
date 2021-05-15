using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Google.Android.Material.Button;

namespace client.Fragments
{
    public class CompleteRequestFragment : Android.Support.V4.App.Fragment, IValueEventListener
    {
        private EditText InputName;
        private EditText InputSurname;
        //  private EditText InputEmail;
        private EditText InputContactNo;
        private EditText InputAltNumber;
        private EditText InputItemType;
        private EditText InputPickUpLocation;
        private EditText InputDestinationLocation;
        private EditText InputPersonName;
        private EditText InputPersonContact;
        private MaterialButton BtnSubmitDeliveryRequest;
        private MaterialButton BtnDatePick;
        private MaterialButton BtnTimePick;
        private RadioButton RdbCash;
        private RadioButton RdbOnline;
        private TextView txtPaymentMethod;
        private string keyId;

        public void OnCancelled(DatabaseError error)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.activity_pascel_details, container, false);
            ConnectViews(view);
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            keyId = pref.GetString("UserID", string.Empty);
            return view;
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                if (snapshot.Child("Name").Exists())
                {
                    InputName.Text = snapshot.Child("Name").Value.ToString();
                }
                if (snapshot.Child("Surname").Exists())
                {
                    InputSurname.Text = snapshot.Child("Surname").Value.ToString();
                }
                if (snapshot.Child("Phone").Exists())
                {
                    InputPersonContact.Text = snapshot.Child("Phone").Value.ToString();
                }
            }
        }

        private void ConnectViews(View view)
        {
            InputName = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelFullnames);
            InputSurname = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelSurname);
            InputContactNo = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelContact);
            InputAltNumber = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelAltContact);
            InputItemType = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelItemType);
            InputPickUpLocation = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelPickupLocation);
            InputDestinationLocation = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelDestination);
            InputPersonName = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelPersonN);
            InputPersonContact = view.FindViewById<com.google.android.material.textfield.TextInputEditText>(Resource.Id.InputPacelPersonContacts);
            BtnSubmitDeliveryRequest = view.FindViewById<MaterialButton>(Resource.Id.BtnSubmitDeliveryRequest);
            RdbCash = view.FindViewById<RadioButton>(Resource.Id.RdbCash);
            RdbOnline = view.FindViewById<RadioButton>(Resource.Id.RdbOnline);
            txtPaymentMethod = view.FindViewById<TextView>(Resource.Id.txtPaymentMethod);
            //buttons
            BtnDatePick = view.FindViewById<MaterialButton>(Resource.Id.BtnPicupDate);
            BtnTimePick = view.FindViewById<MaterialButton>(Resource.Id.BtnPicupTime);



            FirebaseDatabase.Instance.GetReference("AppUsers").Child(keyId)
                .AddValueEventListener(this);
        }
    }
}