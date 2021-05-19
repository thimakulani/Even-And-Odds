using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
 
using admin.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Android.Material.AppBar;

namespace admin.Activities
{
    [Activity(Label = "ReasignDriver")]
    public class ReasignDriver : Activity
    {
        private RecyclerView recyclerReasign;
        private MaterialToolbar include_app_toolbar;
        private readonly List<ReasignModel> items = new List<ReasignModel>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_reasign);
            recyclerReasign = FindViewById<RecyclerView>(Resource.Id.recyclerReasign);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            Adapters.ReassignAdapter adapter = new Adapters.ReassignAdapter(items);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerReasign.SetLayoutManager(linearLayoutManager);
            recyclerReasign.SetAdapter(adapter);
            adapter.BtnClick += Adapter_BtnClick;
            adapter.BtnRejectClick += Adapter_BtnRejectClick;
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick;
        }

        private void Include_app_toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void Adapter_BtnRejectClick(object sender, Adapters.ReassignAdapterClickEventArgs e)
        {
            FirebaseDatabase.Instance.GetReference("Reassign")
                .Child(items[e.Position].Key)
                .Child("Status").SetValue("Rejected");
        }

        private void Adapter_BtnClick(object sender, Adapters.ReassignAdapterClickEventArgs e)
        {
            FirebaseDatabase.Instance.GetReference("Reassign")
                .Child(items[e.Position].Key)
                .Child("Status").SetValue("Approved");
        }
    }
}