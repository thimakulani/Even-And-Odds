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
using admin.Adapters;
 
using admin.Models;
using Firebase.Database;
using Java.Util;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using Google.Android.Material.AppBar;

namespace admin.Activities
{
    [Activity(Label = "Anouncements")]
    public class Anouncements : Activity
    {
        private RecyclerView Recycler;
        
        private readonly List<AnnouncementModel> items = new List<AnnouncementModel>();
        private MaterialToolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_announcements);
            Recycler = FindViewById<RecyclerView>(Resource.Id.RecyclerAnnouncements);
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar_announcement);
            toolbar.InflateMenu(Resource.Menu.top_app_bar);
            toolbar.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_black_18dp);
            
            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            toolbar.NavigationClick += Toolbar_NavigationClick;
            /*Get announcements*/
            Recycler.SetLayoutManager(new LinearLayoutManager(this));
            AnnouncementAdapter adapter = new AnnouncementAdapter(items);
            Recycler.SetAdapter(adapter);


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("Announcements")
                .AddSnapshotListener((snapshop, error) =>
                {
                    if (!snapshop.IsEmpty)
                    {
                        foreach (var dc in snapshop.DocumentChanges)
                        {
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(dc.Document.ToObject<AnnouncementModel>());
                                    
                                    break;
                                case DocumentChangeType.Modified:
                                    items[dc.OldIndex] = dc.Document.ToObject<AnnouncementModel>();
                                    break;
                                case DocumentChangeType.Removed:
                                    items.RemoveAt(dc.OldIndex);
                                    break;
                            }
                        }
                    }
                });


        }

        private void Toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void Toolbar_MenuItemClick(object sender, AndroidX.AppCompat.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.nav_add_announcements)
            {
                DialogAddAnnouncement();
            }
        }

        private AlertDialog.Builder dialogBuilder;
        private AlertDialog AnnouncementDialog;
        private Button SubmitAnnouncement;
        private TextInputEditText InputMessage;
        private void DialogAddAnnouncement()
        {

            dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.add_announcement_dialog, null);
            SubmitAnnouncement = view.FindViewById<Button>(Resource.Id.dlgBtnSubmiAnnouncement);
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.dlgInputAnnouncement);
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
                    .Child(items[e.Position].Id)
                    .RemoveValue();
                builder.Dispose();
            });
            builder.Show();
        }
    }
}