using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.Adapters;
using Even_Odds_Delivary.AppData;
using Even_Odds_Delivary.Models;

namespace Even_Odds_Delivary.Fragments
{
    public class QueriesFragment : Android.Support.V4.App.Fragment
    {
        private List<QueriesModel> Items = new List<QueriesModel>();
        private RecyclerView Recycler;
        private QueriesData data = new QueriesData();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);


            var view =  inflater.Inflate(Resource.Layout.activity_queries, container, false);
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
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Application.Context);
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