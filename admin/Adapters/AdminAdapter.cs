﻿using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Even_Odds_Delivary.AppData;
using Even_Odds_Delivary.Models;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Views.Animations;

namespace Even_Odds_Delivary.Adapters
{
    class AdminAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AdminAdapterClickEventArgs> ItemClick;
        public event EventHandler<AdminAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> FabCallClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> FabEmailClick;
        private List<AppUsers> items = new List<AppUsers>();
        private Context context;

        public AdminAdapter(List<AppUsers> data, Context context)
        {
            items = data;
            this.context = context;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.super_user_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new AdminAdapterViewHolder(itemView, OnClick, OnLongClick, OnCallClick, OnEmailClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as AdminAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.Email.Text = items[position].Email;
            holder.Names.Text = items[position].Name;
            holder.Surname.Text = items[position].Surname;
            holder.UserType.Text = items[position].UserType;
            holder.PhoneNumber.Text = items[position].PhoneNumber;

            //ViewAnimationUtils(viewHolder.ItemView, position);
        }
        private void ViewAnimationUtils(View view, int pos)
        {
            if (pos % 2 == 0)
            {
                Animation anim = AnimationUtils.LoadAnimation(context, Resource.Animation.Side_in_right);
                view.StartAnimation(anim);
            }
            else
            {
                Animation anim = AnimationUtils.LoadAnimation(context, Resource.Animation.Side_in_left);
                view.StartAnimation(anim);
            }
        }

        public override int ItemCount => items.Count;

        void OnClick(AdminAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AdminAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnCallClick(AppUsersAdapterClickEventArgs args) => FabCallClick.Invoke(this, args);
        void OnEmailClick(AppUsersAdapterClickEventArgs args) => FabEmailClick.Invoke(this, args);
    }

    public class AdminAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView Names { get; set; }
        public TextView Surname { get; set; }
        public TextView PhoneNumber { get; set; }
        public TextView UserType { get; set; }
        public TextView Email { get; set; }
        public FloatingActionButton FabSendEmail { get; set; }
        public FloatingActionButton FabMakeCall { get; set; }

        public AdminAdapterViewHolder(View itemView, Action<AdminAdapterClickEventArgs> clickListener,
                            Action<AdminAdapterClickEventArgs> longClickListener, Action<AppUsersAdapterClickEventArgs> MakeCallClickListener, Action<AppUsersAdapterClickEventArgs> SendEmailClickListener) : base(itemView)
        {
            //TextView = v;
            UserType = itemView.FindViewById<TextView>(Resource.Id.SuperUserRowUserType);
            Surname = itemView.FindViewById<TextView>(Resource.Id.SuperUserRowSurname);
            Names = itemView.FindViewById<TextView>(Resource.Id.SuperUserRowNames);
            PhoneNumber = itemView.FindViewById<TextView>(Resource.Id.SuperUserRowPhoneNumber);
            Email = itemView.FindViewById<TextView>(Resource.Id.SuperUserRowEmail);
            FabSendEmail = itemView.FindViewById<FloatingActionButton>(Resource.Id.SuperUserRowFabEmail);
            FabMakeCall = itemView.FindViewById<FloatingActionButton>(Resource.Id.SuperUserRowFabCall);



            itemView.Click += (sender, e) => clickListener(new AdminAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AdminAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            FabMakeCall.Click += (sender, e) => MakeCallClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            FabSendEmail.Click += (sender, e) => SendEmailClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

        }
    }

    public class AdminAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}