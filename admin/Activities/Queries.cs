using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.Fragments;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "Queries")]
    public class Queries : AppCompatActivity
    {
        private Toolbar queries_toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_query_host);
            queries_toolbar = FindViewById<Toolbar>(Resource.Id.queries_toolbar);
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