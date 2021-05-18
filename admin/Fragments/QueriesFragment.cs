using System;
using System.Collections.Generic;

using Android.OS;
using Android.Views;
using admin.Adapters;
using admin.AppData;
using admin.Models;
using AndroidX.RecyclerView.Widget;
using AndroidX.Fragment.App;
using Android.Content;

namespace admin.Fragments
{
    public class QueriesFragment : Fragment
    {
        private List<QueriesModel> Items = new List<QueriesModel>();
        private RecyclerView Recycler;
        private QueriesData data = new QueriesData();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);


            var view =  inflater.Inflate(Resource.Layout.activity_queries, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            Recycler = view.FindViewById<RecyclerView>(Resource.Id.RecyclerQueriesList);
            data.CreateQueriesData();
            data.RetrivedQueries += Data_RetrivedQueries;
        }

        private void Data_RetrivedQueries(object sender, QueriesData.QueriesRetrivedEventArgs e)
        {
            Items = e.queries;
            Items.Sort((x, y) => y.Datetime.CompareTo(x.Datetime));
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            QueriesAdapter adapter = new QueriesAdapter(Items);
            Recycler.SetLayoutManager(linearLayoutManager);
            Recycler.SetAdapter(adapter);
            adapter.ItemClick += Adapter_ItemClick;
        }
        public event EventHandler<QueryClickedEventHandler> QueriesEvneHandler;
        public class QueryClickedEventHandler : EventArgs
        {
            public QueriesModel Items { get; set; }
        }
        private void Adapter_ItemClick(object sender, QuerySendersAdapterClickEventArgs e)
        {
            QueriesEvneHandler.Invoke(this, new QueryClickedEventHandler { Items = Items[e.Position] });
        }
    }
}