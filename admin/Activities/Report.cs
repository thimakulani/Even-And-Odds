using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Google.Android.Material.Button;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using iTextSharp.text;
using admin.Models;
using admin.Common;
using admin.AppData;
using admin.Adapters;

namespace admin.Activities
{
    [Activity(Label = "Report", MainLauncher =false)]
    public class Report : AppCompatActivity, IPermissionListener
    {
        private MaterialButton BtnGenerate;
        public ProgressBar LoadingProgressBar;
        private MaterialButton RequestsType;
        private ImageView imgBack;
        // private string FileName;
        string documentName = "Delivery Report " + DateTime.Now.ToString("dddd dd MMM") + ".pdf";

        private RecyclerView RecyclerHistory;
        private List<DelivaryModal> delivariesList = new List<DelivaryModal>();
        private List<DelivaryModal> ReportData = new List<DelivaryModal>();
       

        //permissions
        readonly string[] permission = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationId = 0;


        protected  override  void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_report);

            BtnGenerate = FindViewById<MaterialButton>(Resource.Id.BtnGenerate);
            RecyclerHistory = FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);
            LoadingProgressBar = FindViewById<ProgressBar>(Resource.Id.LoadingProgressBar);
            RequestsType = FindViewById<MaterialButton>(Resource.Id.RequestsType);
            imgBack = FindViewById<ImageView>(Resource.Id.ImgCloseHistory);
            BtnGenerate.Click += BtnGenerate_Click;
            RequestsType.Click += RequestsType_Click;
           // CheckPermission();
            imgBack.Click += ImgBack_Click;
            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.WriteExternalStorage)
                .WithListener(this)
                .Check();
            //get data
            Common.PrintReport.WriteFileToStorage(this, "Lato_Black.ttf");

        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void RequestsType_Click(object sender, EventArgs e)
        {
            Android.Widget.PopupMenu popupMenu = new Android.Widget.PopupMenu(this, RequestsType);
            popupMenu.Menu.Add(IMenu.None, 1, 1, "All-Requests");
            popupMenu.Menu.Add(IMenu.None, 1, 1, "Monthly");
            popupMenu.Menu.Add(IMenu.None, 2, 2, "Weekly");
            popupMenu.Menu.Add(IMenu.None, 3, 3, "Daily"); 
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, Android.Widget.PopupMenu.MenuItemClickEventArgs e)
        {
            
            RequestsType.Text = e.Item.TitleFormatted.ToString();
            if (e.Item.TitleFormatted.ToString() == "All-Requests")
            {
                ReportData = delivariesList;
                SetRecycler(ReportData);

            }
            if (e.Item.TitleFormatted.ToString() == "Weekly")
            {
                DateTime today = DateTime.Today;
                int currDayWeek = (int)today.DayOfWeek;
                DateTime sunday = today.AddDays(-currDayWeek);
                DateTime monday = sunday.AddDays(1);
                if(currDayWeek == 0)
                {
                    monday = monday.AddDays(-7);
                }
                var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
                ReportData = (from data in delivariesList
                              where
                              data.RequestTime.Contains(dates[0].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[1].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[2].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[3].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[4].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[5].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[6].Date.ToString("dd MMMMM yyyy"))
                              select data).ToList<DelivaryModal>();
                SetRecycler(ReportData);
            }
            if (e.Item.TitleFormatted.ToString() == "Daily")
            {
                ReportData = (from data in delivariesList
                              where
                              
                              data.RequestTime.Contains(DateTime.Now.ToString("dddd, dd MMMM yyyy,"))
                              select data).ToList<DelivaryModal>();
                SetRecycler(ReportData);
            }
            if (e.Item.TitleFormatted.ToString() == "Monthly")
            {
                ReportData = (from data in delivariesList
                              where
                              data.RequestTime.Contains(DateTime.Now.ToString("MMMM yyyy"))
                              select data).ToList<DelivaryModal>();
                SetRecycler(ReportData);
            }
            //Toast.MakeText(this, ReportData.Count.ToString(), ToastLength.Long).Show();
        }
        RequestAdapter adapter;
        private void SetRecycler(List<DelivaryModal> reportData)
        {
            LinearLayoutManager linearLayout = new LinearLayoutManager(this);
            adapter = new RequestAdapter(ref reportData);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);
            
        }




        private async void BtnGenerate_Click(object sender, EventArgs e)
        {

            //CreatePDFFile(Common.PrintReport.GetAppPath(this) + documentName);
            string dname = Resource.String.app_name + " " + DateTime.Now.ToString("dddd dd MMM HH mm ss tt") + ".pdf";
            if (new Java.IO.File(dname).Exists())
            {
                new Java.IO.File(dname).Delete();
            }
            try
            {
                BtnGenerate.Text = "Please wait...";
                BtnGenerate.Enabled = false;
                //FileStream fs = new FileStream(documentName, FileMode.Create);
                ReportModel reportModel = new ReportModel(ReportData);
                await Task.Run(() =>
                {
                    reportModel.GenerateInvoice(new FileStream(PrintReport.GetAppPath(this) + dname, FileMode.Create, FileAccess.Write), this);
                });
                Common.PrintReport.WriteFileToStorage(this, dname);
                PrintManager printManager = (PrintManager)this.GetSystemService(Context.PrintService);
                Common.PrintReportPDFAdapter _adapter = new Common.PrintReportPDFAdapter(this, Common.PrintReport.GetAppPath(this)
                        + dname, dname);
                printManager.Print("Document", _adapter, new PrintAttributes.Builder().Build());
                BtnGenerate.Enabled = true;
                BtnGenerate.Text = "Generate Report";
            }
            catch (FileNotFoundException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            catch (DocumentException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            catch (IOException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "Must accept this permission", ToastLength.Long).Show();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            Toast.MakeText(this, "permission granted", ToastLength.Long).Show();

        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            

        }
        private bool CheckPermission()
        {
            bool permisionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                //               permisionGranted = true;
                RequestPermissions(permission, requestLocationId);
            }
            else
            {
                return true;
            }
            return permisionGranted;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] gra0ntResults)
        {
            if (gra0ntResults[0] == (int)Permission.Granted)
            {
                Toast.MakeText(this, "Permission was granted", ToastLength.Long).Show();
               
            }
            else
            {
                Toast.MakeText(this, "Permission was denied", ToastLength.Long).Show();
            }
        }

               
            
        
    }
}