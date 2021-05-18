using admin.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace admin.AppData
{
    public class QueriesData : Java.Lang.Object, IValueEventListener
    {
        private List<Models.QueriesModel> items = new List<Models.QueriesModel>();
        public event EventHandler<QueriesRetrivedEventArgs> RetrivedQueries;
        public class QueriesRetrivedEventArgs : EventArgs
        {
            public List<QueriesModel> queries { get; set; }
        }
        public void CreateQueriesData()
        {
            FirebaseDatabase.Instance.
                GetReference("Query")
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

                    QueriesModel queries = new QueriesModel()
                    {
                        Datetime = data.Child("senderDateTime").Child("DateTime").Value.ToString(),
                       // Name = data.Child("senderDateTime").Child("SenderName").Value.ToString(),
                        QueryId = data.Key,
                    };
                    items.Add(queries);
                }
                RetrivedQueries.Invoke(this, new QueriesRetrivedEventArgs { queries = items });
            }
        }
    }
}