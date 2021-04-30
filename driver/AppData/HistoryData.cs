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
using driver.Models;

namespace driver.AppData
{
    class HistoryData : Java.Lang.Object, IValueEventListener
    {
        private string UserKeyId;
        private List<DelivaryModal> items = new List<DelivaryModal>();
        public HistoryData(string KeyId)
        {
            UserKeyId = KeyId;
        }

        public event EventHandler<RetriveDelivaryHystoryHandler> RetrievedDeliveries;
        public class RetriveDelivaryHystoryHandler: EventArgs
        {
            public List<DelivaryModal> itemList { get; set; }
        }

        public void GetDeliveryRequests()
        {
            FirebaseDatabase.Instance
                .GetReference("DelivaryRequest")
                .AddValueEventListener(this);
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach(DataSnapshot data in child)
                {

                    if (data.Child("Driver").Exists())
                    {
                        if (UserKeyId == data.Child("Driver").Child("DriverId").Value.ToString() && data.Child("Status").Value.ToString() !="Waiting")
                        {

                            DelivaryModal delivary = new DelivaryModal
                            {
                                DestinationAddress = data.Child("Locations").Child("DestinationAddress").Value.ToString(),
                                Name = data.Child("User").Child("Name").Value.ToString(),
                                Surname = data.Child("User").Child("Surname").Value.ToString(),
                                KeyId = data.Key,
                                PickupAddress = data.Child("Locations").Child("PickupAddress").Value.ToString(),
                                //Date = data.Child("Date").Value.ToString(),
                                //Time = data.Child("Time").Value.ToString(),
                                RequestTime = data.Child("RequestTime").Value.ToString(),
                                Status = data.Child("Status").Value.ToString(),
                                UserId = data.Child("User").Child("UserId").Value.ToString(),
                                Distance = data.Child("Distance").Value.ToString(),
                                Price = data.Child("Price").Value.ToString()
                            };
                            items.Add(delivary);
                        }
                    }
                }
                RetrievedDeliveries.Invoke(this, new RetriveDelivaryHystoryHandler { itemList = items });
            }
        }
    }
}