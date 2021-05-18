using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Github.Library.Bubbleview;
using client.Classes;
using Firebase.Database;
using Java.Util;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;

namespace client.Fragments
{
    public class HelpFragment : Android.Support.V4.App.Fragment, Firebase.Database.IValueEventListener
    {
        private TextInputEditText InputMessage;
        private FloatingActionButton BtnSend;
        private RecyclerView recyclerView;
        private string UserKeyId;
        private string Names;
        private List<Queries> items = new List<Queries>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.activity_help_queries, container, false);
            UserKeyId = Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid;
            ConnectViews(view);
            FirebaseDatabase.Instance.GetReference("AppUsers")
                .Child(UserKeyId)
                .AddValueEventListener(this);
            return view;
        }

        private void ConnectViews(View view)
        {
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.TInputMessage);

            BtnSend = view.FindViewById<FloatingActionButton>(Resource.Id.TBtnSendMessage);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.TRecyclerChatMessage);
            BtnSend.Click += BtnSend_Click;
            InputMessage.SetTextColor(Android.Graphics.Color.Black);

            LinearLayoutManager linear = new LinearLayoutManager(Application.Context);
            QueriesAdapter adapter = new QueriesAdapter(items, UserKeyId);
            recyclerView.SetLayoutManager(linear);
            recyclerView.SetAdapter(adapter);
        }

    

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputMessage.Text))
            {
                Toast.MakeText(Context.ApplicationContext, "Type in a message", ToastLength.Long).Show();
                return;
            }
            HashMap senderTime = new HashMap();
            senderTime.Put("SenderName", Names);
            senderTime.Put("DateTime", DateTime.Now.ToString("dddd, dd MMM yyyy HH:mm:ss tt"));
            FirebaseDatabase.Instance.GetReference("Query").Child(UserKeyId)
                .Child("senderDateTime")
                .SetValue(senderTime);

            HashMap hash = new HashMap();
            hash.Put("Message", InputMessage.Text);
            hash.Put("DateTime", DateTime.Now.ToString("dddd, dd MMM yyyy HH:mm:ss tt"));
            hash.Put("SenderId", UserKeyId);
            FirebaseDatabase.Instance.GetReference("Query")
                .Child(UserKeyId)
                .Child("Messages")
                .Push().SetValue(hash);
            InputMessage.Text = string.Empty;
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                Names = snapshot.Child("Name").Value.ToString()
                    + " " + snapshot.Child("Surname").Value.ToString();
            }
        }
    }
    class QueriesAdapter : RecyclerView.Adapter
    {
        readonly List<Queries> items = new List<Queries>();
        string KeyId;
        public QueriesAdapter(List<Queries> data, string key)
        {
            items = data;
            KeyId = key;
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ChatsView)
            {
                ChatsView chatsView = holder as ChatsView;
                chatsView.TxtTimeDate.Text = items[position].TimeSent;
                chatsView.TxtMessage.Text = items[position].QueryMessage;
                //chatsView.TxtName.Text = items[position].SenderName;
            }
            else
            {
                SenderChats senderView = holder as SenderChats;
                senderView.SenderTxtMessage.Text = items[position].QueryMessage;
                senderView.SenderTxtTimeDate.Text = items[position].TimeSent;
            }

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.chat_message_sender_row)
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_message_reciever_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.TxtMessage);
                TextView TxtDt = row.FindViewById<TextView>(Resource.Id.TxtMsgTime);
                //TextView TxtName = row.FindViewById<TextView>(Resource.Id.TxtMsgSenderName);
                ChatsView view = new ChatsView(row)
                {
                    TxtMessage = TxtMessage,
                    TxtTimeDate = TxtDt,
                    //TxtName = TxtName

                };
                return view;
            }
            else
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_message_sender_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.SenderTxtMessage);
                TextView TxtDt = row.FindViewById<TextView>(Resource.Id.SenderTxtMsgTime);
                SenderChats view = new SenderChats(row)
                {
                    SenderTxtMessage = TxtMessage,
                    SenderTxtTimeDate = TxtDt,

                };
                return view;
            }

        }
        public override int GetItemViewType(int position)
        {
            if (items[position].SenderId == KeyId)
            {
                return Resource.Layout.chat_message_reciever_row;
            }
            else
            {
                return Resource.Layout.chat_message_sender_row;
            }

        }
        public class ChatsView : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView TxtMessage { get; set; }
            public TextView TxtName { get; set; }
            public TextView TxtTimeDate { get; set; }

            public ChatsView(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
        public class SenderChats : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView SenderTxtMessage { get; set; }
            public TextView SenderTxtTimeDate { get; set; }
            public SenderChats(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
    }
}