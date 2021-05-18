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
using admin.Models;
using Firebase.Database;

namespace admin.AppData
{
    class AppAdminUsers: Java.Lang.Object, IValueEventListener
    {
        private List<AppUsers> items = new List<AppUsers>();
        public event EventHandler<AdminDataEventArgs> AdminRetrivedData;
        public class AdminDataEventArgs : EventArgs
        {
            public List<AppUsers> admin_list { get; set; }
        }
        public void GetAdminData()
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
                if (data.Child("UserType").Value.ToString() == "Admin")
                {
                    string altPhone = null;
                    if (data.Child("AltPhone").Exists())
                    {
                        altPhone = data.Child("AltPhone").Value.ToString();
                    }
                    var users = new AppUsers
                    {
                        AltPhoneNumber = altPhone,
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
            AdminRetrivedData.Invoke(this, new AdminDataEventArgs { admin_list = items });
        }
    }
}