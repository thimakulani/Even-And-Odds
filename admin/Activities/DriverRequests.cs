using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.Adapters;
using Even_Odds_Delivary.AppData;
using Even_Odds_Delivary.Models;
using Firebase.Database;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "DriverRequests")]
    public class DriverRequests : AppCompatActivity
    {
        private Toolbar include_app_toolbar;
        private RecyclerView recycler_driver_requests;
        DriverRequestData driverRequestData = new DriverRequestData();
        private List<DriverRequestModel> items = new List<DriverRequestModel>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_requests);
            include_app_toolbar = FindViewById<Toolbar>(Resource.Id.include_app_toolbar);
            recycler_driver_requests = FindViewById<RecyclerView>(Resource.Id.recycler_driver_requests);
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick;
            driverRequestData.GetDriverRequestData();
            driverRequestData.RetrivedData += DriverRequestData_RetrivedData;

        }

        private void DriverRequestData_RetrivedData(object sender, DriverRequestData.DriverRequestDataEventArgs e)
        {
            items = e.item;
            recycler_driver_requests.SetLayoutManager(new LinearLayoutManager(this));
            DriverRequestAdapter adapter = new DriverRequestAdapter(items);
            recycler_driver_requests.SetAdapter(adapter);
            adapter.BtnApproveClick += Adapter_BtnApproveClick;
            adapter.BtnDeclinelick += Adapter_BtnDeclinelick;
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

        private void Include_app_toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }


      
    }
}