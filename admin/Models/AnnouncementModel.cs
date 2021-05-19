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
using Plugin.CloudFirestore;

namespace admin.Models 
{
    public class AnnouncementModel
    {
        public string Message { get; set; }
        public string Id { get; set; }
        public FieldValue TimeStamp { get; set; }
    }
}