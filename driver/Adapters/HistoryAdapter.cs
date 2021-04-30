using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using driver.Models;
using driver.FirebaseHelper;
using Firebase.Database;

namespace driver.Adapters
{
    class HistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryAdapterClickEventArgs> ItemLongClick;
        private List<DelivaryModal> items = new List<DelivaryModal>();

        public HistoryAdapter(List<DelivaryModal> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_row, parent, false);

            var vh = new HistoryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryAdapterViewHolder;
            int indexPos = (items.Count - 1) - position;
            //holder.TextView.Text = items[position];
            //holder.TxtClientName.Text = items[indexPos].Name;
            holder.TxtDestination.Text = items[indexPos].DestinationAddress;
            holder.TxtDistance.Text = items[indexPos].Distance;
            holder.TxtPickUploaction.Text = items[indexPos].PickupAddress;
            holder.TxtPrice.Text = items[indexPos].Price;
            holder.TxtStatus.Text = items[indexPos].Status;
            holder.txtHistoryRequestDatesTimeCreated.Text = items[indexPos].RequestTime;
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(items[indexPos].UserId)
                .AddValueEventListener(new ClientValues(holder));

        }

        public override int ItemCount => items.Count;

        void OnClick(HistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        private class ClientValues : Java.Lang.Object, IValueEventListener
        {
            private HistoryAdapterViewHolder holder;

            public ClientValues(HistoryAdapterViewHolder holder)
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
                    holder.TxtClientName.Text = $"{snapshot.Child("Name").Value} {snapshot.Child("Surname").Value}";
                }
            }
        }
    }

    public class HistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView TxtClientName { get; set; }
        public TextView TxtDestination { get; set; }
        public TextView TxtPickUploaction { get; set; }
        public TextView TxtStatus { get; set; }
        public TextView TxtDistance { get; set; }
        public TextView TxtPrice { get; set; }
        public TextView txtHistoryRequestDatesTimeCreated { get; set; }


        public HistoryAdapterViewHolder(View itemView, Action<HistoryAdapterClickEventArgs> clickListener,
                            Action<HistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            TxtClientName = itemView.FindViewById<TextView>(Resource.Id.HistoryClientName);
            TxtDestination = itemView.FindViewById<TextView>(Resource.Id.HistoryDestination);
            TxtPickUploaction = itemView.FindViewById<TextView>(Resource.Id.HistoryPickupLocation);
            TxtStatus = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestStatus);
            TxtDistance = itemView.FindViewById<TextView>(Resource.Id.HistoryDistance);
            TxtPrice = itemView.FindViewById<TextView>(Resource.Id.HistoryPrice);
            txtHistoryRequestDatesTimeCreated = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestDatesTimeCreated);

            itemView.Click += (sender, e) => clickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class HistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}