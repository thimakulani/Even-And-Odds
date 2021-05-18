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
using Java.Interop;

namespace admin.AppData
{
    public class AnnouncementData : Java.Lang.Object, IValueEventListener
    {
        public List<AnnouncementModel> items = new List<AnnouncementModel>();
        public event EventHandler<RetrieveAnnouncementsEventHandler> RetrieveHandler;
        public void RetrieveAnnouncement()
        {
            FirebaseDatabase.Instance.GetReference("Announcements")
                .AddValueEventListener(this);
        }
        public class RetrieveAnnouncementsEventHandler : EventArgs
        {
            public List<AnnouncementModel> Items { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            items.Clear();
            if (snapshot.Exists())
            {
                items.Clear();
                foreach (var data in snapshot.Children.ToEnumerable<DataSnapshot>())
                {
                    AnnouncementModel announcement = new AnnouncementModel()
                    {
                        Date_Time = DateTime.Parse(data.Child("Dates").Value.ToString()),
                        Id = data.Key,
                        Message = $"{data.Child("Message").Value}",
                    };
                    items.Add(announcement);
                }
                RetrieveHandler.Invoke(this, new RetrieveAnnouncementsEventHandler { Items = items });
            }
            else
            {
                RetrieveHandler.Invoke(this, new RetrieveAnnouncementsEventHandler { Items = items });
            }
        }
    }
}