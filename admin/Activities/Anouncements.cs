using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.Adapters;
using Even_Odds_Delivary.AppData;
using Even_Odds_Delivary.Models;
using Firebase.Database;
using Java.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "Anouncements")]
    public class Anouncements : Activity
    {
        private RecyclerView Recycler;
        private AnnouncementData data = new AnnouncementData();
        private List<AnnouncementModel> items = new List<AnnouncementModel>();
        private Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_announcements);
            Recycler = FindViewById<RecyclerView>(Resource.Id.RecyclerAnnouncements);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_announcement);
            toolbar.InflateMenu(Resource.Menu.top_app_bar);
            toolbar.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_black_18dp);
            toolbar.NavigationClick += Toolbar_NavigationClick;
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            data.RetrieveAnnouncement();
            data.RetrieveHandler += Data_RetrieveHandler;


        }

        private void Toolbar_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            if(e.Item.ItemId == Resource.Id.nav_add_announcements)
            {
                DialogAddAwareness();
            }
        }

        private AlertDialog.Builder dialogBuilder;
        private AlertDialog AnnouncementDialog;
        private Button SubmitAnnouncement;
        private EditText InputMessage;
        private void DialogAddAwareness()
        {

            dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.add_announcement_dialog, null);
            SubmitAnnouncement = view.FindViewById<Button>(Resource.Id.dlgBtnSubmiAnnouncement);
            InputMessage = view.FindViewById<EditText>(Resource.Id.dlgInputAnnouncement);
            SubmitAnnouncement.Click += SubmitAnnouncement_Click;

            dialogBuilder.SetView(view);
            dialogBuilder.SetCancelable(true);
            AnnouncementDialog = dialogBuilder.Create();
            AnnouncementDialog.Show();
        }

        private void SubmitAnnouncement_Click(object sender, EventArgs e)
        {
            HashMap data = new HashMap();
            data.Put("Dates", DateTime.Now.ToString("dddd, dd/MMMM/yyyy, HH:mm tt"));
            data.Put("Message", InputMessage.Text);

            if (!string.IsNullOrEmpty(InputMessage.Text) && !string.IsNullOrWhiteSpace(InputMessage.Text))
            {
                var dbRef = FirebaseDatabase.Instance.GetReference("Announcements").Push();
                dbRef.SetValue(data);
            }
            InputMessage.Text = string.Empty;
        }

        private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void Data_RetrieveHandler(object sender, AnnouncementData.RetrieveAnnouncementsEventHandler e)
        {
            items = e.Items;
            AnnouncementAdapter adapter = new AnnouncementAdapter(items);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            Recycler.SetLayoutManager(linearLayoutManager);
            Recycler.SetAdapter(adapter);
            adapter.ItemDeleteClick += Adapter_ItemDeleteClick;
        }

        private void Adapter_ItemDeleteClick(object sender, AnnouncementAdapterClickEventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Confirm");
            builder.SetMessage("Are you sure you want to delete the announcement");
            builder.SetNegativeButton("No", delegate
            {
                builder.Dispose();
            });
            builder.SetPositiveButton("Yes", delegate
            {
                FirebaseDatabase.Instance.GetReference("Announcements")
                    .Child(items[e.Position].KeyId)
                    .RemoveValue();
                builder.Dispose();
            });
            builder.Show();
        }
    }
}