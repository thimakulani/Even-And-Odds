using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace client.AppData
{

    public class PriceData : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<PriceEventHandler> PriceHandle;
        public class PriceEventHandler : EventArgs
        {
            public double InitialPrice { get; set; }
            public double AfrterPrice { get; set; }
        }
        public void GetPrice()
        {
            FirebaseDatabase.Instance.GetReference("TripPrice")
                .AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                double p = double.Parse(snapshot.Child("InitialPrice").Value.ToString());
                double u = double.Parse(snapshot.Child("PricePerKillos").Value.ToString());
                PriceHandle.Invoke(this, new PriceEventHandler { AfrterPrice = u, InitialPrice = p });
            }
            else
            {
                PriceHandle.Invoke(this, new PriceEventHandler { AfrterPrice = 7, InitialPrice =  25});
            }
        }
    }
}