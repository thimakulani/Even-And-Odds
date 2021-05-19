using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidHUD;
using AndroidX.RecyclerView.Widget;
using client.Adapters;
 
using client.Classes;
using Firebase.Auth;
using Plugin.CloudFirestore;

namespace client.Fragments
{
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView RecyclerHistory;
        private readonly List<DelivaryModal> items = new List<DelivaryModal>();
        private Context context;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.activity_history, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            RecyclerHistory = view.FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);

            LinearLayoutManager linearLayout = new LinearLayoutManager(view.Context);
            RequestAdapter adapter = new RequestAdapter(items);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);
            adapter.BtnCancelClick += Adapter_BtnCancelClick;
            adapter.BtnViewDriverClick += Adapter_BtnViewDriverClick;
            /*get history*/
        }

        private void Adapter_BtnViewDriverClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = (items.Count - 1) - e.Position;
            DriverDialogFragment driverDialogFragment = new DriverDialogFragment(items[index].DriverId);
            driverDialogFragment.Show(ChildFragmentManager.BeginTransaction(), "Driver Info");
        }

        
        private void Adapter_BtnCancelClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = (items.Count - 1) - e.Position;
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle("Confirm");
            alert.SetMessage("Are you sure you want to cancel the delivery request:");
            alert.SetPositiveButton("Yes", delegate
            {
                Dictionary<string, object> valuePairs = new Dictionary<string, object>();
                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("DelivaryRequest")
                    .Document(items[index].KeyId)
                    .UpdateAsync("Status", "C");

                AndHUD.Shared.ShowSuccess(context, "Your request has been cancelled", MaskType.Black, TimeSpan.FromSeconds(10));
                alert.Dispose();

            });
            alert.SetNegativeButton("No", delegate
            {
                alert.Dispose();
            });
            alert.Show();

        }

    }
}