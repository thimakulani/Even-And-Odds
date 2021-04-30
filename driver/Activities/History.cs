using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using driver.Adapters;
using driver.AppData;
using driver.Models;

namespace driver.Activities
{
    [Activity(Label = "History")]
    public class History : Activity
    {

        private RecyclerView RecyclerHistory;
        private ImageView ImgCloseHistory;
        private List<DelivaryModal> items = new List<DelivaryModal>();
        private HistoryData data;
        private string keyId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            keyId = pref.GetString("UserID", string.Empty);
            // Create your application here
            SetContentView(Resource.Layout.activity_history);
            ConnectViews();
            RequestedOrientation = ScreenOrientation.Portrait;




            data = new HistoryData(keyId);
            data.GetDeliveryRequests();
            data.RetrievedDeliveries += Data_RetrievedDeliveries;
        }
        private void ConnectViews()
        {
            RecyclerHistory = FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);
            
        }

        private void ImgCloseHistory_Click(object sender, EventArgs e)
        {
            Finish();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);


        }
        private void Data_RetrievedDeliveries(object sender, HistoryData.RetriveDelivaryHystoryHandler e)
        {
            items = e.itemList;
            SetRecycler();
        }
        private void SetRecycler()
        {
            LinearLayoutManager linearLayout = new LinearLayoutManager(this);
            RequestAdapter adapter = new RequestAdapter(items);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);
        }
    }
}