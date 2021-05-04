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
using client.Classes;
using client.FirebaseHelper;

namespace Therapy_In_A_Pocket.AppData
{
    class QueriesData:Java.Lang.Object, IValueEventListener
    {
        private List<Queries> items = new List<Queries>();


        public event EventHandler<RetriveQueriesMessagesInbox> RetrivedQueriesMessages;
        public class RetriveQueriesMessagesInbox : EventArgs
        {
            public List<Queries> Queries { get; set; }
        }

        public void RetrieveGroupChats(string userId)
        {
            FirebaseDatabase.Instance.GetReference("Query").Child(userId).Child("Messages")
                .AddValueEventListener(this);
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (DataSnapshot data in child)
                {
                    Queries chats = new Queries
                    {
                        TimeSent = data.Child("DateTime").Value.ToString(),
                        QueryMessage = data.Child("Message").Value.ToString(),
                        SenderId = data.Child("SenderId").Value.ToString(),
                        QueryID = data.Key
                       
                    };
                    items.Add(chats);
                }
                RetrivedQueriesMessages.Invoke(this, new RetriveQueriesMessagesInbox { Queries = items });
            }
        }
    }
}