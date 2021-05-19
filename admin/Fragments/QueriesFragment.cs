﻿using System;
using System.Collections.Generic;

using Android.OS;
using Android.Views;
using admin.Adapters;
 
using admin.Models;
using AndroidX.RecyclerView.Widget;
using AndroidX.Fragment.App;
using Android.Content;
using Plugin.CloudFirestore;

namespace admin.Fragments
{
    public class QueriesFragment : Fragment
    {
        private readonly List<QueriesModel> Items = new List<QueriesModel>();
        private RecyclerView Recycler;
       
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
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            QueriesAdapter adapter = new QueriesAdapter(Items);
            Recycler.SetLayoutManager(linearLayoutManager);
            Recycler.SetAdapter(adapter);
            adapter.ItemClick += Adapter_ItemClick;

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("Query")
                .OrderBy("TimeStamp", false)
                .AddSnapshotListener((value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var query = new QueriesModel();
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    query = dc.Document.ToObject<QueriesModel>();
                                    query.Uid = dc.Document.Id;
                                    Items.Add(query);
                                    break;
                                case DocumentChangeType.Modified:
                                    query = dc.Document.ToObject<QueriesModel>();
                                    query.Uid = dc.Document.Id;
                                    Items[dc.OldIndex] = query;
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });


        }

        private void Adapter_ItemClick(object sender, QuerySendersAdapterClickEventArgs e)
        {
            QueryRepliesFragment frag = new QueryRepliesFragment(Items[e.Position].Uid);
            var ft = ChildFragmentManager.BeginTransaction();
            frag.Show(ft, null);
        }
    }
}