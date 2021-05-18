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
using Firebase.Firestore;

namespace admin.Models 
{
    public class AnnouncementModel
    {
        public DateTime Date_Time { get; set; }
        public string Message { get; set; }
        [DocumentId]
        public string Id { get; set; }
    }
}