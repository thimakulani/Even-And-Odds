using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using admin.Models;
using Firebase.Database;
using AndroidX.RecyclerView.Widget;
using Plugin.CloudFirestore;

namespace admin.Adapters
{
    class QueriesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<QuerySendersAdapterClickEventArgs> ItemClick;
        public event EventHandler<QuerySendersAdapterClickEventArgs> ItemLongClick;
        private readonly List<QueriesModel> items = new List<QueriesModel>();

        public QueriesAdapter(List<QueriesModel> data)
        {
            items = data;
            
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.query_view_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);
            var vh = new QuerySendersAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as QuerySendersAdapterViewHolder;
            
            holder.TimeSent.Text = items[position].Datetime.ToString();


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("AppUsers")
                .Document(items[position].QueryId)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        
                    }
                });
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(items[position].QueryId)
                .AddValueEventListener(new ValueListener(holder));
        }

        public override int ItemCount => items.Count;

        void OnClick(QuerySendersAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(QuerySendersAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        private class ValueListener : Java.Lang.Object, IValueEventListener
        {
            private readonly QuerySendersAdapterViewHolder holder;

            public ValueListener(QuerySendersAdapterViewHolder holder)
            {
                this.holder = holder;
            }

            public void OnCancelled(DatabaseError error)
            {
                
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    if(snapshot.Child("Name").Exists() && snapshot.Child("Surname").Exists())
                    {
                        holder.SenderName.Text = snapshot.Child("Name").Value.ToString() + snapshot.Child("Surname").Value.ToString();
                    }
                }
            }
        }
    }

    public class QuerySendersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView SenderName { get; set; }
        public TextView TimeSent { get; set; }


        public QuerySendersAdapterViewHolder(View itemView, Action<QuerySendersAdapterClickEventArgs> clickListener,
                            Action<QuerySendersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            SenderName = itemView.FindViewById<TextView>(Resource.Id.QueryRowName);
            TimeSent = itemView.FindViewById<TextView>(Resource.Id.QueryRowDates);

            itemView.Click += (sender, e) => clickListener(new QuerySendersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new QuerySendersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class QuerySendersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}