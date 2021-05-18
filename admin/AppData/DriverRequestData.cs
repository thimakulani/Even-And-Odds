using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using admin.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace admin.AppData
{
    public class DriverRequestData : Java.Lang.Object, IValueEventListener
    {
        private List<DriverRequestModel> items = new List<DriverRequestModel>();
        public event EventHandler<DriverRequestDataEventArgs> RetrivedData;
        public class DriverRequestDataEventArgs : EventArgs
        {
            public List<DriverRequestModel> item;
        }
        public void GetDriverRequestData()
        {
            FirebaseDatabase
                .Instance
                .GetReference("DriverRequests")
                .AddValueEventListener(this);
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            var child = snapshot.Children.ToEnumerable<DataSnapshot>();
            items.Clear();
            foreach (DataSnapshot data in child)
            {
                string status = null;
                if (data.Child("Status").Exists())
                {
                    status = data.Child("Status").Value.ToString();
                }
                /*
                 data.Put("Make", items[e.Position].Make);
            data.Put("Color", items[e.Position].Color);
            data.Put("Type", items[e.Position].Type);
            data.Put("RegNo", items[e.Position].RegNo);
                 */

                var requests = new DriverRequestModel
                {
                    PhoneNumber = data.Child("Phone").Value.ToString(),
                    Name = data.Child("Name").Value.ToString(),
                    Surname = data.Child("Surname").Value.ToString(),
                    Email = data.Child("Email").Value.ToString(),
                    RegNo = data.Child("RegNo").Value.ToString(),
                    Make = data.Child("Make").Value.ToString(),
                    Type = data.Child("Type").Value.ToString(),
                    Color = data.Child("Color").Value.ToString(),
                    KeyId = data.Key,
                    Status = status
                };
                items.Add(requests);
                RetrivedData.Invoke(this, new DriverRequestDataEventArgs { item = items });
            }
            
        }
    }
}