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

namespace client.Classes
{
    public class Queries
    {
        public string QueryID { get; set; }
        public string QueryMessage { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string TimeSent { get; set; }
    }
}