﻿
using Android.App;
using Android.OS;
using Android.Views;
using admin.Fragments;
using Google.Android.Material.AppBar;
using AndroidX.AppCompat.App;

namespace admin.Activities
{
    [Activity(Label = "Queries")]
    public class Queries : AppCompatActivity
    {
        private MaterialToolbar queries_toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_query_host);
            queries_toolbar = FindViewById<MaterialToolbar>(Resource.Id.queries_toolbar);
            SetSupportActionBar(queries_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            
            QueriesFragment frag = new QueriesFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.fragment_host, frag)
                .AddToBackStack(frag.Tag)
                .Commit();
            frag.QueriesEvneHandler += Frag_QueriesEvneHandler;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Android.Resource.Id.Home == item.ItemId)
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
        private void Frag_QueriesEvneHandler(object sender, QueriesFragment.QueryClickedEventHandler e)
        {
            QueryRepliesFragment frag = new QueryRepliesFragment(e.Items.QueryId);
            var ft = SupportFragmentManager.BeginTransaction();
            frag.Show(ft, null);
        }

       
    }
}