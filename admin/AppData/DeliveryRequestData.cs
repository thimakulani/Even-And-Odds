﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Even_Odds_Delivary.Models;
using Even_Odds_Delivary.Activities;

namespace Even_Odds_Delivary.AppData
{
    class DeliveryRequestData: Java.Lang.Object, IValueEventListener
    {
        private List<DelivaryModal> items = new List<DelivaryModal>();
        public event EventHandler<DeliveryRequestEventArgs> RequestRetrived;
        private ProgressBar progressBar;

        public DeliveryRequestData(ProgressBar reportProgress)
        {
            progressBar = reportProgress;
        }

        public class DeliveryRequestEventArgs: EventArgs
        {
            public List<DelivaryModal> delivaryModals { get; set; }
        }
        public void DeliveryRequests()
        {

            FirebaseDatabase.Instance.GetReference("DelivaryRequest")
                .AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (DataSnapshot data in child)
                {

                    string driverId = null;
                    //string driverName = null;
                    if (data.Child("Driver").Exists())
                    {
                        //driverName = data.Child("Driver").Child("DriverName").Value.ToString();
                        driverId = data.Child("Driver").Child("DriverId").Value.ToString();
                    }
                    DelivaryModal delivary = new DelivaryModal
                    {
                        AlteContactNo = data.Child("User").Child("AlteContactNo").Value.ToString(),
                        ContactNo = data.Child("User").Child("ContactNo").Value.ToString(),
                        
                        DestinationAddress = data.Child("Locations").Child("DestinationAddress").Value.ToString(),
                        DestinationLat = data.Child("Locations").Child("DestinationLatitude").Value.ToString(),
                        DestinationLong = data.Child("Locations").Child("DestinationLongitude").Value.ToString(),
                        //DriverName = driverName,
                        DriverId = driverId,
                        ItemType = data.Child("ItemType").Value.ToString(),
                        KeyId = data.Key,
                        Name = data.Child("User").Child("Name").Value.ToString(),
                        PaymentType = data.Child("PaymentType").Value.ToString(),
                        PersonContact = data.Child("PersonContact").Value.ToString(),
                        PersonName = data.Child("PersonName").Value.ToString(),
                        PickupAddress = data.Child("Locations").Child("PickupAddress").Value.ToString(),
                        PickupLat = data.Child("Locations").Child("PickupLat").Value.ToString(),
                        PickupLong = data.Child("Locations").Child("PickupLong").Value.ToString(),
                        RequestTime = data.Child("RequestTime").Value.ToString(),
                        Status = data.Child("Status").Value.ToString(),
                        Surname = data.Child("User").Child("Surname").Value.ToString(),
                        UserId = data.Child("User").Child("UserId").Value.ToString(),
                        Distance = data.Child("Distance").Value.ToString(),
                        Price = data.Child("Price").Value.ToString()
                    };
                    items.Add(delivary);

                }
                RequestRetrived.Invoke(this, new DeliveryRequestEventArgs { delivaryModals = items });
            }
            progressBar.Visibility = ViewStates.Gone;
        }
    }
}