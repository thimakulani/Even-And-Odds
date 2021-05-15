using Even_Odds_Delivary.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Even_Odds_Delivary.AppDataHelper
{
    public class ReplyData : Java.Lang.Object, IValueEventListener
    {
        private List<Replies> items = new List<Replies>();
        public event EventHandler<ReplyRetrivedEventArgs> RetrivedReply;
        public class ReplyRetrivedEventArgs : EventArgs
        {
            public List<Replies> replies { get; set; }
        }
        public ReplyData(string queryid)
        {
            FirebaseDatabase.Instance.GetReference("Query")
                .Child(queryid)
                .Child("Messages")
                .AddValueEventListener(this);

        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                items.Clear();
                var child = snapshot.Children.ToEnumerable<DataSnapshot>();
                foreach (var data in child)
                {
                    Replies replies = new Replies()
                    {
                        Msg = data.Child("Message").Value.ToString(),
                        MsgId = data.Key,
                        SenderId = data.Child("SenderId").Value.ToString(),
                        DateTime = data.Child("DateTime").Value.ToString()
                    };
                    items.Add(replies);
                }
                RetrivedReply.Invoke(this, new ReplyRetrivedEventArgs { replies = items });
            }
        }
    }
}