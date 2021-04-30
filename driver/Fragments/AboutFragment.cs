﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidAboutPage;

namespace driver.Fragments
{
    public class AboutFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            Element element = new Element();
            element.Title = "About";

            View about = new AboutPage(Application.Context)
                .IsRtl(false)
                .SetImage(Resource.Drawable.delivary_icon_2)
                .SetDescription("Even & Odds delivery")
                .AddItem(new Element() { Title = "Version 1.0.0" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("Thima")
               // .AddWebsite("")
                .AddPlayStore("Thima")
                .AddItem(CreateCopyright())
                .AddGroup("Developers")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("https//m.facebook.com/thima.sigauque")
                .AddGitHub("Thimakulani")
                .AddInstagram("instagram.com/thimasigauque")
                .Create();
            return about;
        }
        private Element CreateCopyright()
        {
            Element copy = new Element();
            string cr = $"Copyright {DateTime.Now.Year} by Thima Kulani";
            copy.Title = cr;
            copy.IconDrawable = Resource.Drawable.delivary_icon_2;
            return copy;
        }
    }
}