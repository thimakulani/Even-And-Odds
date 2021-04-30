﻿using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using driver.Models;
using Firebase.Database;

namespace driver.Adapters
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestAdapterClickEventArgs> ItemLongClick;
        private List<DelivaryModal> items = new List<DelivaryModal>();

        public RequestAdapter(List<DelivaryModal> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.delivery_requests_row, parent, false);

            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new RequestAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestAdapterViewHolder;
            int indexPos = (items.Count - 1) - position;

            holder.RequestName.Text = items[indexPos].Name;
            holder.RequestPickupLocation.Text = items[indexPos].PickupAddress;
            holder.RequestDestination.Text = items[indexPos].DestinationAddress;
            holder.RequestDate.Text = items[indexPos].RequestTime;
            //holder.TextView.Text = items[position];
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(items[indexPos].UserId)
                .AddValueEventListener(new ClientValues(holder));
            
        }


        public override int ItemCount => items.Count;

        void OnClick(RequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        private class ClientValues : Java.Lang.Object, IValueEventListener
        {
            private RequestAdapterViewHolder holder;

            public ClientValues(RequestAdapterViewHolder holder)
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
                    holder.RequestName.Text = $"{snapshot.Child("Name").Value} {snapshot.Child("Surname").Value}";
                }
            }
        }
    }

    public class RequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView RequestName { get; set; }
        public TextView RequestPickupLocation { get; set; }
        public TextView RequestDestination { get; set; }
        public TextView RequestDate { get; set; }


        public RequestAdapterViewHolder(View itemView, Action<RequestAdapterClickEventArgs> clickListener,
                            Action<RequestAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            RequestName = itemView.FindViewById<TextView>(Resource.Id.RequestName);
            RequestPickupLocation = itemView.FindViewById<TextView>(Resource.Id.RequestPickupLocation);
            RequestDestination = itemView.FindViewById<TextView>(Resource.Id.RequestDestination);
            RequestDate = itemView.FindViewById<TextView>(Resource.Id.RequestDate);

            //TextView = v;


            
            itemView.Click += (sender, e) => clickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class RequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}