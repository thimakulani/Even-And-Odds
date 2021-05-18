using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Google.Android.Material.Button;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Util;

using Google.Android.Material.TextField;
using Google.Android.Material.AppBar;

namespace admin.Activities
{
    [Activity(Label = "TripPrice")]
    public class TripPrice : Activity, IValueEventListener
    {
        private MaterialToolbar toolbar;
        private TextInputEditText InputPrice;
        private TextInputEditText InputPriceAfter;
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
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            InputPrice = FindViewById<TextInputEditText>(Resource.Id.InitialPrice);
            InputPriceAfter = FindViewById<TextInputEditText>(Resource.Id.InitialAfter);
            BtnApplyChanges = FindViewById<MaterialButton>(Resource.Id.BtnApplyChanges);
            BtnApplyChanges.Click += BtnApplyChanges_Click;
            FirebaseDatabase.Instance.GetReference("TripPrice").AddValueEventListener(this);
            toolbar.NavigationClick += Toolbar_NavigationClick1;
        }

        private void Toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
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