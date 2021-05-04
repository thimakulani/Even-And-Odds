using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using client.Adapters;
using client.AppData;
using client.Classes;
using Firebase.Auth;

namespace client.Fragments
{
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView RecyclerHistory;
        private List<DelivaryModal> items = new List<DelivaryModal>();
        private HistoryData data;
        private string keyId;
        private Context context;

        public HistoryFragment(Context context)
        {
            this.context = context;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.activity_history, container, false);
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            keyId = FirebaseAuth.Instance.CurrentUser.Uid;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            RecyclerHistory = view.FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);


            data = new HistoryData();
            data.GetDeliveryRequests(keyId);
            data.RetrievedDeliveries += Data_RetrievedDeliveries;
        }
    
        private void Data_RetrievedDeliveries(object sender, HistoryData.RetriveDelivaryHystoryHandler e)
        {
            items = e.itemList;
            SetRecycler();
        }
        private void SetRecycler()
        {
            LinearLayoutManager linearLayout = new LinearLayoutManager(context.ApplicationContext);
            RequestAdapter adapter = new RequestAdapter(items);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);
            adapter.BtnCancelClick += Adapter_BtnCancelClick;
            adapter.BtnViewDriverClick += Adapter_BtnViewDriverClick;
        }

        private void Adapter_BtnViewDriverClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = (items.Count - 1) - e.Position;
            DriverDialogFragment driverDialogFragment = new DriverDialogFragment(items[index].DriverId);
            driverDialogFragment.Show(ChildFragmentManager.BeginTransaction(), "Driver Info");
        }

        public event EventHandler<CancelRequestEventHandler> CancelHandler;
        public class CancelRequestEventHandler : EventArgs
        {
            public DelivaryModal Items { get; set; }
        }
        private void Adapter_BtnCancelClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = (items.Count - 1) - e.Position;
            CancelHandler.Invoke(this, new CancelRequestEventHandler { Items = items[index] });

        }

    }
}