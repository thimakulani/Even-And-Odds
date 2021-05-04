using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using client.Classes;
using Firebase.Database;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client.Fragments
{
    public class DriverDialogFragment : DialogFragment, IValueEventListener
    {
        private string driverId;
        private string PhoneNo;

        public DriverDialogFragment(string driverId)
        {
            this.driverId = driverId;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private TextView Names { get; set; }
        private TextView Surname { get; set; }
        private TextView Make { get; set; }
        private TextView Color { get; set; }
        private TextView RegNo { get; set; }
        private TextView Type { get; set; }

        private FloatingActionButton FabMakeCall { get; set; }
        private FloatingActionButton FabClose { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

           base.OnCreateView(inflater, container, savedInstanceState);
           View view =  inflater.Inflate(Resource.Layout.driver_info_dialog, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View itemView)
        {
            Make = itemView.FindViewById<TextView>(Resource.Id.RowUserMake);
            Color = itemView.FindViewById<TextView>(Resource.Id.RowUserColor);
            Type = itemView.FindViewById<TextView>(Resource.Id.RowUserCarType); 
            RegNo = itemView.FindViewById<TextView>(Resource.Id.RowUserRegNo);

            //TextView = v;
            Surname = itemView.FindViewById<TextView>(Resource.Id.RowSurname);
            Names = itemView.FindViewById<TextView>(Resource.Id.RowNames);
            FabMakeCall = itemView.FindViewById<FloatingActionButton>(Resource.Id.RowFabCall);
            FabClose = itemView.FindViewById<FloatingActionButton>(Resource.Id.FabClose);
            FabMakeCall.Click += FabMakeCall_Click;
            FabClose.Click += FabClose_Click;

            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(driverId)
                .AddValueEventListener(this);
                

        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private void FabMakeCall_Click(object sender, EventArgs e)
        {
            try
            {

                Xamarin.Essentials.PhoneDialer.Open(PhoneNo);
            }
            catch
            {

            }
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                if (snapshot.Child("Name").Exists())
                {
                    Names.Text = snapshot.Child("Name").Value.ToString();
                }
                if (snapshot.Child("Surname").Exists())
                {
                    Surname.Text = snapshot.Child("Surname").Value.ToString();
                }
                if (snapshot.Child("Phone").Exists())
                {
                    PhoneNo = snapshot.Child("Phone").Value.ToString();
                }
                if (snapshot.Child("RegNo").Exists())
                {
                    RegNo.Text = snapshot.Child("RegNo").Value.ToString();
                }
                if (snapshot.Child("Make").Exists())
                {
                    Make.Text = snapshot.Child("Make").Value.ToString();
                }
                if (snapshot.Child("Color").Exists())
                {
                    Color.Text = snapshot.Child("Color").Value.ToString();
                }
                if (snapshot.Child("Type").Exists())
                {
                    Type.Text = snapshot.Child("Type").Value.ToString();
                }

            }
        }
    }
}