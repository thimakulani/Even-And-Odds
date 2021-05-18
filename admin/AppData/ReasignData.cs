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
    class ReasignData : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RetrieveReasignEventHandler> ReassignHandler;
        private List<ReasignModel> items = new List<ReasignModel>();
        public class RetrieveReasignEventHandler: EventArgs
        {
            public List<ReasignModel> reasign { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }
        public void GetInformation()
        {
            FirebaseDatabase.Instance
                .GetReference("Reassign").AddValueEventListener(this);
        }
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (var data in child)
                {
                    if (data.Child("Status").Exists()) 
                    {
                        if (data.Child("Status").Value.ToString() != "Approved" )
                        {
                            ReasignModel reasign = new ReasignModel()
                            {
                                Address = "",
                                DatesTime = "",
                                DriverNames = "",
                                Key = data.Key,
                                Status = ""
                            };
                            items.Add(reasign); 
                        }
                    }
                    
                }
               
            }
        }
    }
}