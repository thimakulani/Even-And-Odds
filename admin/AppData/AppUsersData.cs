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
using Even_Odds_Delivary.Models;

namespace Even_Odds_Delivary.AppData
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
            Toast.MakeText(Application.Context, error.Message, ToastLength.Long).Show();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            var child = snapshot.Children.ToEnumerable<DataSnapshot>();
            items.Clear();
            foreach (DataSnapshot data in child)
            {
                string phone = null;
                string car_regno = null;
                string car_type = null;
                string car_make = null;
                string car_color = null;


                if (data.Child("Make").Exists())
                {
                    car_make = data.Child("Make").Value.ToString();
                }
                if (data.Child("Type").Exists())
                {
                    car_type = data.Child("Type").Value.ToString();
                }
                if (data.Child("RegNo").Exists())
                {
                    car_regno = data.Child("RegNo").Value.ToString();
                }
                if (data.Child("Color").Exists())
                {
                    car_color = data.Child("Color").Value.ToString();
                }



                if (data.Child("AltPhone").Exists())
                {
                    phone = data.Child("AltPhone").Value.ToString();
                }
                if (data.Child("UserType").Value.ToString() != "Admin")
                {
                    var users = new AppUsers
                    {
                        AltPhoneNumber = phone,
                        PhoneNumber = data.Child("Phone").Value.ToString(),
                        Name = data.Child("Name").Value.ToString(),
                        Surname = data.Child("Surname").Value.ToString(),
                        Email = data.Child("Email").Value.ToString(),
                        KeyId = data.Key,
                        UserType = data.Child("UserType").Value.ToString(),
                        Color = car_color,
                        Make = car_make,
                        RegNo = car_regno,
                        Type = car_type
                    };
                    items.Add(users);
                }
            }
            RetrivedData.Invoke(this, new UsersDataEventArgs { users_list = items });
        }
    }
}