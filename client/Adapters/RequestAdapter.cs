using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using client.Classes;
using Firebase.Database;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;

namespace client.Adapters
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<RequestAdapterClickEventArgs> BtnCancelClick;
        public event EventHandler<RequestAdapterClickEventArgs> BtnViewDriverClick;
        private readonly List<DelivaryModal> items = new List<DelivaryModal>();

        public RequestAdapter(List<DelivaryModal> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_row, parent, false);

            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new RequestAdapterViewHolder(itemView, OnClick, OnLongClick, OnCancelClick, OnViewDriverClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public  override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestAdapterViewHolder;
            int indexPos = (items.Count - 1) - position;
            //if(items[indexPos].DriverName == null)
            //{
            //    holder.DriverName.Text = "Waiting for driver";
            //}
            //else
            //{
            //    holder.DriverName.Text = items[indexPos].DriverName;
            //}
            
            holder.PickupLocation.Text = items[indexPos].PickupAddress;
            holder.Destination.Text = items[indexPos].DestinationAddress;
            holder.Price.Text = items[indexPos].Price;
            holder.Distance.Text = items[indexPos].Distance;
            holder.HistoryRequestStatus.Text = items[indexPos].Status;
            holder.HistoryDatesTimeCreated.Text = items[indexPos].RequestTime;
            if (items[indexPos].Status == "C")
            {
                holder.BtnCancelRequest.Visibility = ViewStates.Gone;
                holder.DriverName.Text = "==== No driver ====";
            }
            if(items[indexPos].Status == "A" || items[indexPos].Status == "D" || items[indexPos].Status == "P")
            {
                holder.BtnCancelRequest.Visibility = ViewStates.Gone;
                // holder.DriverName.Text = items[indexPos].DriverName;
                holder.BtnViewDriver.Visibility = ViewStates.Visible;
                
                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("AppUsers")
                    .Document(items[indexPos].DriverId)
                    .AddSnapshotListener((value, error) =>
                    {
                        if (value.Exists)
                        {
                            AppUsers users = value.ToObject<AppUsers>();
                            holder.DriverName.Text = $"{users.Name} {users.Surname}";
                        }
                    });

            }
            else if(items[indexPos].Status == "Waiting")
            {
                holder.DriverName.Text = "Waiting for driver";
            }
            //holder.TextView.Text = items[position];
        }

        
        public override int ItemCount => items.Count;
        
        void OnClick(RequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnCancelClick(RequestAdapterClickEventArgs args) => BtnCancelClick?.Invoke(this, args);
        void OnViewDriverClick(RequestAdapterClickEventArgs args) => BtnViewDriverClick?.Invoke(this, args);

       
    }
    
    public class RequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView DriverName { get; set; }
        public TextView PickupLocation { get; set; }
        public TextView Destination { get; set; }
        public TextView Distance { get; set; }
        public TextView Price { get; set; }
        public TextView HistoryRequestStatus { get; set; }
        public MaterialButton BtnCancelRequest { get; set; }
        public MaterialButton BtnViewDriver { get; set; }
        public TextView HistoryDatesTimeCreated { get; set; }

        public RequestAdapterViewHolder(View itemView, Action<RequestAdapterClickEventArgs> clickListener,
                            Action<RequestAdapterClickEventArgs> longClickListener, Action<RequestAdapterClickEventArgs> CancelClickListener,
                            Action<RequestAdapterClickEventArgs> viewDriverClickListener) : base(itemView)
        {
            
            DriverName = itemView.FindViewById<TextView>(Resource.Id.HistoryDriverName);
            PickupLocation = itemView.FindViewById<TextView>(Resource.Id.HistoryPickupLocation);
            Destination = itemView.FindViewById<TextView>(Resource.Id.HistoryDestination);
            HistoryDatesTimeCreated = itemView.FindViewById<TextView>(Resource.Id.HistoryDatesTimeCreated);
            Distance = itemView.FindViewById<TextView>(Resource.Id.HistoryDistance);
            Price = itemView.FindViewById<TextView>(Resource.Id.HistoryPrice);
            HistoryRequestStatus = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestStatus);
            BtnCancelRequest = itemView.FindViewById<MaterialButton>(Resource.Id.BtnCancelRequest);
            BtnViewDriver = itemView.FindViewById<MaterialButton>(Resource.Id.BtnViewDriver);
            //TextView = v;



            itemView.Click += (sender, e) => clickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            BtnCancelRequest.Click += (sender, e) => CancelClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            BtnViewDriver.Click += (sender, e) => viewDriverClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }

        
    }

    public class RequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}