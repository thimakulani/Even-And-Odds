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
    class AppUsersData: Java.Lang.Object, IValueEventListener
    {
        private List<AppUsers> items = new List<AppUsers>();
        public event EventHandler<UsersDataEventArgs> RetrivedData;
        public class UsersDataEventArgs : EventArgs
        {
            public List<AppUsers> users_list { get; set; }
        }
        public void GetUsers()
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
                var users = new AppUsers
                {
                    AltPhoneNumber = data.Child("AltPhone").Value.ToString() != "null" ? data.Child("AltPhone").Value.ToString() : "",
                    PhoneNumber = data.Child("Phone").Value.ToString(),
                    Name = data.Child("Name").Value.ToString(),
                    Surname = data.Child("Surname").Value.ToString(),
                    Email = data.Child("Email").Value.ToString(),
                    KeyId = data.Key.ToString()
                };
                items.Add(users);
            }
            RetrivedData.Invoke(this, new UsersDataEventArgs { users_list = items });
        }
    }
}