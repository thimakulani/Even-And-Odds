using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Firebase.Auth;
using driver.Adapters;
using driver.Models;
using Plugin.CloudFirestore;
using AndroidX.RecyclerView.Widget;

namespace driver.Fragments
{
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView RecyclerHistory;
        private readonly List<DelivaryModal> items = new List<DelivaryModal>();
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            base.OnCreateView(inflater, container, savedInstanceState);
            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var view =  inflater.Inflate(Resource.Layout.activity_history, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {

            RecyclerHistory = view.FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);
            LinearLayoutManager linearLayout = new LinearLayoutManager(Application.Context);
            RequestAdapter adapter = new RequestAdapter(items);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);



            CrossCloudFirestore.Current
                .Instance
                .Collection("DeliveryRequests")
                .WhereEqualsTo("DriverId", FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if(!snapshot.IsEmpty)
                    {
                        foreach(var dc in snapshot.DocumentChanges)
                        {
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    var doc = dc.Document.ToObject<DelivaryModal>();
                                    doc.KeyId = dc.Document.Id;
                                    items.Add(doc);
                                    
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }
                });
        }

    }
}