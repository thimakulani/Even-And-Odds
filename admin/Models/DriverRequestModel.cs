﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace admin.Models
{
    public class DriverRequestModel
    {
        public string AltPhoneNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string KeyId { get; set; } 
        public string Status { get; set; }
        public string Make { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}