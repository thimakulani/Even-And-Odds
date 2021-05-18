using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using admin.Adapters;
using admin.Models;
using Firebase.Database;
using Java.Util;
using System.Collections.Generic;
using Google.Android.Material.AppBar;

namespace admin.Activities
{
    [Activity(Label = "DriverRequests")]
    public class DriverRequests : AppCompatActivity
    {
        private MaterialToolbar include_app_toolbar;
        private RecyclerView recycler_driver_requests;
        private readonly List<DriverRequestModel> items = new List<DriverRequestModel>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_requests);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            recycler_driver_requests = FindViewById<RecyclerView>(Resource.Id.recycler_driver_requests);
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick1; ;

            recycler_driver_requests.SetLayoutManager(new LinearLayoutManager(this));
            DriverRequestAdapter adapter = new DriverRequestAdapter(items);
            recycler_driver_requests.SetAdapter(adapter);
            adapter.BtnApproveClick += Adapter_BtnApproveClick;
            adapter.BtnDeclinelick += Adapter_BtnDeclinelick;
        }

        private void Include_app_toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }


        private void Adapter_BtnDeclinelick(object sender, DriverRequestAdapterClickEventArgs e)
        {
            FirebaseDatabase.Instance.GetReference("DriverRequests")
                .Child(items[e.Position].KeyId)
                .Child("Status")
                .SetValue("Declined");
        }

        private void Adapter_BtnApproveClick(object sender, DriverRequestAdapterClickEventArgs e)
        {
            FirebaseDatabase.Instance.GetReference("DriverRequests")
                .Child(items[e.Position].KeyId)
                .Child("Status")
                .SetValue("Approved");

            HashMap data = new HashMap();
            data.Put("Name", items[e.Position].Name);
            data.Put("Phone", items[e.Position].PhoneNumber);
            data.Put("Surname", items[e.Position].Surname);
            data.Put("Email", items[e.Position].Email);
            data.Put("Make", items[e.Position].Make);
            data.Put("Color", items[e.Position].Color);
            data.Put("Type", items[e.Position].Type);
            data.Put("RegNo", items[e.Position].RegNo);
            data.Put("UserType", "Driver");

            //data.Put("UserType", "Driver");

            var database = FirebaseDatabase
                .Instance
                .GetReference("AppUsers")
                .Child(items[e.Position].KeyId);
            database.SetValue(data);


        }

      
    }
}