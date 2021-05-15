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
using Even_Odds_Delivary.Models;
using Firebase.Database;

namespace Even_Odds_Delivary.AppData
{
    class GetDrivers : Java.Lang.Object, IValueEventListener
    {
        private List<AppUsers> items = new List<AppUsers>();
        public event EventHandler<DriverDataEventArgs> RetrivedDriver;
        public class DriverDataEventArgs : EventArgs
        {
            public List<AppUsers> users_list { get; set; }
        }
        public void GetUserss()
        {
            FirebaseDatabase.Instance.GetReference("AppUsers")
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
                if (data.Child("UserType").Value.ToString() == "Driver")
                {
                    var users = new AppUsers
                    {
                        AltPhoneNumber = data.Child("AltPhone").Value.ToString() != "null" ? data.Child("AltPhone").Value.ToString() : "",
                        PhoneNumber = data.Child("Phone").Value.ToString(),
                        Name = data.Child("Name").Value.ToString(),
                        Surname = data.Child("Surname").Value.ToString(),
                        Email = data.Child("Email").Value.ToString(),
                        KeyId = data.Key.ToString(),
                        UserType = data.Child("UserType").Value.ToString()
                    };
                    items.Add(users);
                }
            }
            RetrivedDriver.Invoke(this, new DriverDataEventArgs { users_list = items });
        }
    }
}