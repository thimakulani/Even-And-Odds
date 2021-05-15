using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "TripPrice")]
    public class TripPrice : Activity, IValueEventListener
    {
        private Toolbar toolbar;
        private EditText InputPrice;
        private EditText InputPriceAfter;
        private MaterialButton BtnApplyChanges;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_trip_price);
            // Create your application here
            ConnectViews();
        }

        private void ConnectViews()
        {
            toolbar = FindViewById<Toolbar>(Resource.Id.include_app_toolbar);
            InputPrice = FindViewById<EditText>(Resource.Id.InitialPrice);
            InputPriceAfter = FindViewById<EditText>(Resource.Id.InitialAfter);
            BtnApplyChanges = FindViewById<MaterialButton>(Resource.Id.BtnApplyChanges);
            BtnApplyChanges.Click += BtnApplyChanges_Click;
            FirebaseDatabase.Instance.GetReference("TripPrice").AddValueEventListener(this);
            toolbar.NavigationClick += Toolbar_NavigationClick;
        }

        private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Finish();
            FirebaseDatabase.Instance.GetReference("TripPrice").RemoveEventListener(this);
        }

        private void BtnApplyChanges_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputPrice.Text))
            {
                InputPrice.Error = "Cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputPriceAfter.Text))
            {
                InputPriceAfter.Error = "Cannot be empty";
                return;
            }
            HashMap hashMap = new HashMap();
            hashMap.Put("InitialPrice", InputPrice.Text);
            hashMap.Put("PricePerKillos", InputPriceAfter.Text);
            FirebaseDatabase.Instance
                .GetReference("TripPrice")
                .SetValue(hashMap);
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                if (snapshot.Child("InitialPrice").Exists())
                {
                    InputPrice.Text = snapshot.Child("InitialPrice").Value.ToString();
                }
                if (snapshot.Child("PricePerKillos").Exists())
                {
                    InputPriceAfter.Text = snapshot.Child("PricePerKillos").Value.ToString();
                }
            }
        }
    }
}