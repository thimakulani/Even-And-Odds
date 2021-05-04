using System;
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
using client.Classes;

namespace client.AppData
{
    public class HistoryData : Java.Lang.Object, IValueEventListener
    {
        private string UserKeyId;
        private List<DelivaryModal> items = new List<DelivaryModal>();

        public event EventHandler<RetriveDelivaryHystoryHandler> RetrievedDeliveries;
        public class RetriveDelivaryHystoryHandler: EventArgs
        {
            public List<DelivaryModal> itemList { get; set; }
        }

        public void GetDeliveryRequests(string KeyId)
        {
            UserKeyId = KeyId;
            FirebaseDatabase.Instance.GetReference("DelivaryRequest").AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {
            Toast.MakeText(Application.Context, error.ToString(), ToastLength.Long).Show();

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(DataSnapshot data in child)
                {
                    if (UserKeyId == data.Child("User").Child("UserId").Value.ToString())
                    {
                        string driverId = null;
                        string driverName = null;
                        string dest = null;
                        string pickup = null;
                        string price = null;
                        string distance = null;
                        string status = null;
                        string time = null;

                        if (data.Child("Driver").Exists())
                        {
                            //driverName = data.Child("Driver").Child("DriverName").Value.ToString();
                            driverId = data.Child("Driver").Child("DriverId").Value.ToString();

                            //Toast.MakeText(Application.Context, driverName, ToastLength.Long).Show();
                        }
                        if (data.Child("Locations").Child("DestinationAddress").Exists())
                        {
                            dest = data.Child("Locations").Child("DestinationAddress").Value.ToString();
                        }
                        if (data.Child("Locations").Child("PickupAddress").Exists())
                        {
                            pickup = data.Child("Locations").Child("PickupAddress").Value.ToString();
                        }
                        if (data.Child("Price").Exists())
                        {
                            price = data.Child("Price").Value.ToString();
                        }
                        if (data.Child("Distance").Exists())
                        {
                            distance = data.Child("Distance").Value.ToString();
                        }
                        if (data.Child("Status").Exists())
                        {
                            status = data.Child("Status").Value.ToString();
                        }
                        if (data.Child("RequestTime").Exists())
                        {
                            time = data.Child("RequestTime").Value.ToString();
                        }
                        DelivaryModal delivary = new DelivaryModal
                        { 
                           // ContactNo = data.Child("User").Child("ContactNo").Value.ToString(),
                            DestinationAddress = dest,
                            DriverName = driverName,
                            DriverId = driverId,
                            KeyId = data.Key,
                            PickupAddress = pickup,
                            RequestTime = time,
                            Status = status,
                            Distance = distance,
                            Price = price
                        };
                        items.Add(delivary);
                    }
                }
                RetrievedDeliveries.Invoke(this, new RetriveDelivaryHystoryHandler { itemList = items });
            }
        }
    }
}