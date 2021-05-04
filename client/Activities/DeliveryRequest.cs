using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using client.AppData;
using client.Classes;
using client.MapsHelper;
using Firebase.Auth;
using Firebase.Database;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Places;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace client.Activities
{
    [Activity(Label = "DeliveryRequest")]
    public class DeliveryRequest : AppCompatActivity, IOnMapReadyCallback, IValueEventListener, 
        IOnSuccessListener, IOnFailureListener, IDialogInterfaceOnCancelListener
    {
        //private LinearLayout PickUpDestinationLayout;
        //
        //pascel dialog
        /*Dialog*/
        private Android.App.AlertDialog PascelDialog;
        private Android.App.AlertDialog.Builder dialogBuilder;
        //dialog views
        private ImageView ImgClosePascelDialog;
        private ImageView ImgMyLocation;
        private FloatingActionButton FabHome;
        private FloatingActionButton FabRestart;

        private CoordinatorLayout DeliveryRootLayout;
        //

        /*layouts*/
        RelativeLayout layoutPickup;
        RelativeLayout layoutDestination;

        /*txtViews*/
        TextView TxtPickup;
        TextView TxtDestination;
        //*************************************************

        private LinearLayout bottomSheetLayout;
        private BottomSheetBehavior bottomSheet;
        //   private Button ;
        private MaterialButton BtnOpenBottomSheet;
        private RadioButton RdbDestinationLocation;
        private RadioButton RdbPickUpLocation;
        private ImageView ImgCenterMarker;

        //bottom sheet views
        private MaterialButton BtnContinueRequest;
        private TextView TxtPrice;
        private TextView TxtDistance;
        private TextView TxtDuration;
        /*Map*/
        GoogleMap googleMap;
        //trip details
        LatLng pickupLocationLatLng;
        LatLng destinationLocationLatLng;
        //
        readonly string[] permission = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationId = 0;
        LocationRequest locationRequest;
        FusedLocationProviderClient locationClient;
        Android.Locations.Location lastLocation;
        static readonly int UPDATE_INTERVAL = 5;
        static readonly int UPDATE_FASTEST_INTERVAL = 5;
        static readonly int DISPLACEMENT = 3;

        private LocationCallBackHelper locationCallBack;

        /********************* Marker**************************/
        // private MarkerOptions markerOptions = new MarkerOptions();
        /*helpers*/
        //  MapFunctionHelper mapHelper;


        //flag
        int addressRequest = 1;
        bool PickupAddressFromSearch = false;
        bool DestinationAddressFromSearch = false;


        //user keyId
        private string keyId;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;
            SetContentView(Resource.Layout.activity_delivery);
            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            keyId = FirebaseAuth.Instance.CurrentUser.Uid;
            //


            ConnectViews();

            //test
            BtnOpenBottomSheet.Visibility = ViewStates.Visible;
            var mapFragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);
            CheckGps();
            try
            {
                if (CheckPermission())
                {
                    CreateLocationRequest();
                    GetLocation();
                    StartLocationUpdate();
                }
                if (!PlacesApi.IsInitialized)
                {
                    PlacesApi.Initialize(this, GetString(Resource.String.mapKey));
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

        }

       
        private void ConnectViews()
        {

            TxtPickup = FindViewById<TextView>(Resource.Id.TxtPickUpLocation);
            TxtDestination = FindViewById<TextView>(Resource.Id.TxtDestinationLocation);

            RdbPickUpLocation = FindViewById<RadioButton>(Resource.Id.RdbPickUpLocation);
            RdbDestinationLocation = FindViewById<RadioButton>(Resource.Id.RdbDestinationLocation);
            ImgMyLocation = FindViewById<ImageView>(Resource.Id.ImgMyLocation);
           // PickUpDestinationLayout = FindViewById<LinearLayout>(Resource.Id.PickUpDestinationLayout);

            layoutPickup = FindViewById<RelativeLayout>(Resource.Id.RelativePickUpLocation);
            layoutDestination = FindViewById<RelativeLayout>(Resource.Id.RelativeDestinationLocation);
            ImgCenterMarker = FindViewById<ImageView>(Resource.Id.ImgCenterMarker);

            //
            bottomSheetLayout = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            //    BtnFavLocation = FindViewById<Button>(Resource.Id.BtnFavLocation);
            BtnOpenBottomSheet = FindViewById<MaterialButton>(Resource.Id.BtnOpenBottomSheet);
            /*bottom sheet views*/
            bottomSheet = BottomSheetBehavior.From(bottomSheetLayout);
            BtnContinueRequest = bottomSheetLayout.FindViewById<MaterialButton>(Resource.Id.BtnContinueRequest);
            TxtPrice = bottomSheetLayout.FindViewById<TextView>(Resource.Id.TxtPrice);
            TxtDistance = bottomSheetLayout.FindViewById<TextView>(Resource.Id.TxtDistance);
            TxtDuration = bottomSheetLayout.FindViewById<TextView>(Resource.Id.TxtDuration);

            //
            DeliveryRootLayout = FindViewById<CoordinatorLayout>(Resource.Id.DeliveryRootLayout);
            FabHome = FindViewById<FloatingActionButton>(Resource.Id.FabHome);
            FabRestart = FindViewById<FloatingActionButton>(Resource.Id.FabRestart);

            //click
            //BtnFavLocation.Click += BtnFavLocation_Click;
            BtnOpenBottomSheet.Click += BtnOpenBottomSheet_Click;

            //radio btns
            RdbDestinationLocation.Click += RdbDestinationLocation_Click;
            RdbPickUpLocation.Click += RdbPickUpLocation_Click;

            //bottom sheet
            BtnContinueRequest.Click += BtnContinueRequest_Click;
            /*click*/
            layoutPickup.Click += LayoutPickup_Click;
            layoutDestination.Click += LayoutDestination_Click;

            ImgMyLocation.Click += ImgMyLocation_Click;
            FabHome.Click += FabHome_Click;
            FabRestart.Click += FabRestart_Click;

            PriceData priceData = new PriceData();
            priceData.GetPrice();
            priceData.PriceHandle += PriceData_PriceHandle;

        }
        private void FabRestart_Click(object sender, EventArgs e)
        {
            base.Recreate();
        }
        private double InitialPrice;
        private double AfterInitial;
        private void PriceData_PriceHandle(object sender, PriceData.PriceEventHandler e)
        {
            InitialPrice = e.InitialPrice;
            AfterInitial = e.AfrterPrice;
        }

        private void FabHome_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("Log", "Other");
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
            Finish();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }


        private void ImgMyLocation_Click(object sender, EventArgs e)
        {
            if (lastLocation != null)
            {
                LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 17));
            }

        }

        private void RdbPickUpLocation_Click(object sender, EventArgs e)
        {
            addressRequest = 1;
            RdbPickUpLocation.Checked = true;
            RdbDestinationLocation.Checked = false;
            PickupAddressFromSearch = false;
            DestinationAddressFromSearch = false;
            ImgCenterMarker.SetColorFilter(Color.Red);
        }

        private void RdbDestinationLocation_Click(object sender, EventArgs e)
        {
            PickupAddressFromSearch = false;
            DestinationAddressFromSearch = false;
            RdbPickUpLocation.Checked = false;
            addressRequest = 2;
            RdbDestinationLocation.Checked = true;
            ImgCenterMarker.SetColorFilter(Color.DarkGreen);
        }

       
        private void BtnContinueRequest_Click(object sender, EventArgs e)
        {
            OpenPascelDetails();
            DeliveryRootLayout.Alpha = 0.5f;
        }
        private string trip_distance;
        private string trip_price;
        private async void BtnOpenBottomSheet_Click(object sender, EventArgs e)
        {

            if (pickupLocationLatLng != null && destinationLocationLatLng != null)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                if (Xamarin.Essentials.NetworkAccess.Internet == Xamarin.Essentials.Connectivity.NetworkAccess)
                {
                    try
                    {


                        BtnOpenBottomSheet.Text = "Please wait...";
                        BtnOpenBottomSheet.Enabled = false;
                        googleMap.Clear();
                        string json;
                        json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatLng, destinationLocationLatLng);
                        if (!string.IsNullOrEmpty(json))
                        {
                            mapHelper.DrawTripOnMap(json);
                            TxtDuration.Text = mapHelper.durationstring;
                            TxtDistance.Text = mapHelper.distanceString;
                            trip_distance = mapHelper.distanceString;

                            TxtPrice.Text = "R" + mapHelper.EstimateFares(InitialPrice, AfterInitial).ToString("0.##");
                            trip_price = mapHelper.EstimateFares(InitialPrice, AfterInitial).ToString("0.##");
                            bottomSheet.State = BottomSheetBehavior.StateExpanded;

                            ImgCenterMarker.Visibility = ViewStates.Gone;

                        }
                        BtnOpenBottomSheet.Text = "Done...";
                        await System.Threading.Tasks.Task.Delay(2000);
                        BtnOpenBottomSheet.Text = "Continue";
                        BtnOpenBottomSheet.Enabled = true;
                    }
                    catch (Exception)
                    {
                        BtnOpenBottomSheet.Text = "Please wait...";
                        BtnOpenBottomSheet.Enabled = false;



                        Xamarin.Essentials.Location locationA = new Xamarin.Essentials.Location(pickupLocationLatLng.Latitude, pickupLocationLatLng.Longitude);
                        Xamarin.Essentials.Location locationB = new Xamarin.Essentials.Location(destinationLocationLatLng.Latitude, destinationLocationLatLng.Longitude);

                        var dstnc = Xamarin.Essentials.Location.CalculateDistance(locationA, locationB, Xamarin.Essentials.DistanceUnits.Kilometers);
                        //Toast.MakeText(this, dstnc.ToString(), ToastLength.Long).Show();
                        TxtDistance.Text = dstnc.ToString("0.##") + "Km";
                        trip_distance = dstnc.ToString("0.##") + "Km";
                        double fares;
                        if (dstnc <= 5)
                        {
                            fares = 25.00;
                        }
                        else
                        {
                            fares = ((dstnc - 5) * 7) + 25;
                        }

                        TxtPrice.Text = "R" + fares.ToString("0.##");
                        trip_price = "R" + fares.ToString("0.##");
                        TxtDuration.Visibility = ViewStates.Gone;
                        bottomSheet.State = BottomSheetBehavior.StateExpanded;


                        BtnOpenBottomSheet.Text = "Done...";
                        await System.Threading.Tasks.Task.Delay(2000);
                        BtnOpenBottomSheet.Text = "Continue";

                        BtnOpenBottomSheet.Enabled = true;
                    }
                }
                else
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage("Network error");
                    alert.SetNegativeButton("Ok", delegate
                    {
                        alert.Dispose();
                    });

                    alert.Show();
                }




            }
            else
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Warning");
                alert.SetMessage("Please select a pickup location and a destination");
                alert.SetNegativeButton("Ok", delegate
                {
                    alert.Dispose();
                });

                alert.Show();
            }
        }



        private EditText InputName;
        private EditText InputSurname;
        //  private EditText InputEmail;
        private EditText InputContactNo;
        private EditText InputAltNumber;
        private EditText InputItemType;
        private EditText InputPickUpLocation;
        private EditText InputDestinationLocation;
        private EditText InputPersonName;
        private EditText InputPersonContact;
        private MaterialButton BtnSubmitDeliveryRequest;
        private MaterialButton BtnDatePick;
        private MaterialButton BtnTimePick;
        private RadioButton RdbCash;
        private RadioButton RdbOnline;
        //private TextView txtPaymentMethod;

        private void OpenPascelDetails()
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            dialogBuilder = new Android.App.AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.activity_pascel_details, null);
            InputName = view.FindViewById<EditText>(Resource.Id.InputPacelFullnames);
            InputSurname = view.FindViewById<EditText>(Resource.Id.InputPacelSurname);
            InputContactNo = view.FindViewById<EditText>(Resource.Id.InputPacelContact);
            InputAltNumber = view.FindViewById<EditText>(Resource.Id.InputPacelAltContact);
            InputItemType = view.FindViewById<EditText>(Resource.Id.InputPacelItemType);
            InputPickUpLocation = view.FindViewById<EditText>(Resource.Id.InputPacelPickupLocation);
            InputDestinationLocation = view.FindViewById<EditText>(Resource.Id.InputPacelDestination);
            InputPersonName = view.FindViewById<EditText>(Resource.Id.InputPacelPersonN);
            InputPersonContact = view.FindViewById<EditText>(Resource.Id.InputPacelPersonContacts);
            BtnSubmitDeliveryRequest = view.FindViewById<MaterialButton>(Resource.Id.BtnSubmitDeliveryRequest);
            RdbCash = view.FindViewById<RadioButton>(Resource.Id.RdbCash);
            RdbOnline = view.FindViewById<RadioButton>(Resource.Id.RdbOnline);
            //txtPaymentMethod = view.FindViewById<TextView>(Resource.Id.txtPaymentMethod);
            //buttons
            BtnDatePick = view.FindViewById<MaterialButton>(Resource.Id.BtnPicupDate);
            BtnTimePick = view.FindViewById<MaterialButton>(Resource.Id.BtnPicupTime);
            ImgClosePascelDialog = view.FindViewById<ImageView>(Resource.Id.ImgClosePascelDialog);

            if (mapHelper.distance <= 5)
            {
                RdbCash.Visibility = ViewStates.Gone;
                RdbOnline.Visibility = ViewStates.Gone;
                //txtPaymentMethod.Text = "Free Delivary";
            }
            RetriveUserInfo();

            //BtnTimePick.Click += BtnTimePick_Click;
            //BtnDatePick.Click += BtnDatePick_Click;
            BtnSubmitDeliveryRequest.Click += BtnSubmitDeliveryRequest_Click;


            ImgClosePascelDialog.Click += ImgClosePascelDialog_Click;

            dialogBuilder.SetView(view);
            dialogBuilder.SetCancelable(true);
            dialogBuilder.SetOnCancelListener(this);
            PascelDialog = dialogBuilder.Create();
            PascelDialog.Show();

        }

       
        private void BtnTimePick_Click(object sender, EventArgs e)
        {
            TimePickerFragment timePicker = TimePickerFragment.NewInstance(
                delegate (DateTime time)
                {
                    BtnTimePick.Text = time.ToShortTimeString();
                });
            timePicker.Show(SupportFragmentManager, TimePickerFragment.TAG);
        }

        private void BtnSubmitDeliveryRequest_Click(object sender, EventArgs e)
        {

            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Confirm");
            alert.SetMessage("Press continue to proceed with delivery request");
            alert.SetNegativeButton("Cancel", delegate
            {
                alert.Dispose();
            });
            alert.SetPositiveButton("Continue", delegate
            {
                RequestDelevary();
                alert.Dispose();
            });
            alert.Show();
        }

        private void RequestDelevary()
        {
            if (string.IsNullOrEmpty(InputPersonName.Text) && string.IsNullOrWhiteSpace(InputPersonName.Text))
            {
                //Toast.MakeText(this, "Please provide the name of the person you delivering to", ToastLength.Long).Show();
                InputPersonName.Error = "Name could not be empty";
                InputPersonName.RequestFocus();
                return;
            }
            if (string.IsNullOrEmpty(InputPersonContact.Text) && string.IsNullOrWhiteSpace(InputPersonContact.Text))
            {
                //Toast.MakeText(this, "Please provide the contact numbers of the person you delivering to", ToastLength.Long).Show();
                InputPersonContact.Error = "Please provide phone number";
                InputPersonContact.RequestFocus();

                return;
            }
            if (string.IsNullOrEmpty(InputItemType.Text))
            {
                InputItemType.Error = "Item description";
                InputItemType.RequestFocus();
                return;
            }
            //if (BtnDatePick.Text == "Pickup Date")
            //{
            //    Toast.MakeText(this, "Please pick a date", ToastLength.Long).Show();
            //    BtnDatePick.RequestFocus();
            //    return;
            //}
            //if (BtnTimePick.Text == "Pickup Time")
            //{
            //    Toast.MakeText(this, "Please pick time", ToastLength.Long).Show();
            //    BtnDatePick.RequestFocus();
            //    return;
            //}
            string PaymentType;
            if (RdbOnline.Checked)
            {
                PaymentType = "Online";
            }
            else
            {
                PaymentType = "Cash";
            }

            string RequestTime = DateTime.Now.ToString("dddd, dd MMMM yyyy, HH:mm tt");
            HashMap user = new HashMap();
            user.Put("Name", InputName.Text);
            user.Put("Surname", InputName.Text);
            user.Put("ContactNo", InputContactNo.Text);
            user.Put("UserId", keyId);
            user.Put("AlteContactNo", InputAltNumber.Text);


            //HashMap driver = new HashMap();
            //driver.Put("DriverName", "Waiting");
            //driver.Put("DriverId", "Waiting");

            HashMap location = new HashMap();

            location.Put("PickupAddress", InputPickUpLocation.Text);
            location.Put("PickupLat", pickupLocationLatLng.Latitude.ToString());
            location.Put("PickupLong", pickupLocationLatLng.Longitude.ToString());
            location.Put("DestinationAddress", InputDestinationLocation.Text);
            location.Put("DestinationLatitude", destinationLocationLatLng.Latitude.ToString());
            location.Put("DestinationLongitude", destinationLocationLatLng.Longitude.ToString());



            HashMap data = new HashMap();


           // data.Put("Driver", driver);
            data.Put("Locations", location);
            data.Put("User", user);
            data.Put("ItemType", InputItemType.Text);
            //data.Put("Date", BtnDatePick.Text);
            //data.Put("Time", BtnTimePick.Text);
            data.Put("PersonName", InputPersonName.Text);
            data.Put("PersonContact", InputPersonContact.Text);
            data.Put("PaymentType", PaymentType);
            data.Put("Price", trip_price);
            data.Put("Distance", trip_distance);
            data.Put("RequestTime", RequestTime);
            data.Put("Status", "Waiting");

            DatabaseReference dbRef = FirebaseDatabase.Instance.GetReference("DelivaryRequest").Push();
            dbRef.SetValue(data)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }
        
        private void BtnDatePick_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                if (DateTime.Now.Date > time)
                {
                    Toast.MakeText(this, $"{time.ToLongDateString()} Not a valid date{DateTime.Now.Date}", ToastLength.Long).Show();
                }
                else
                {
                    BtnDatePick.Text = time.ToLongDateString();
                }

            });
            frag.Show(SupportFragmentManager, DatePickerFragment.TAG);
        }
        private void LayoutPickup_Click(object sender, EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>
            {
                Place.Field.Id,
                Place.Field.Name,
                Place.Field.LatLng,
                Place.Field.Address
            };
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("ZA")
                .Build(this);

            StartActivityForResult(intent, 1);
        }
        private void LayoutDestination_Click(object sender, EventArgs e)
        {
            List<Place.Field> fields = new List<Place.Field>
            {
                Place.Field.Id,
                Place.Field.Name,
                Place.Field.LatLng,
                Place.Field.Address
            };
            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("ZA")
                .Build(this);

            StartActivityForResult(intent, 2);
        }
        private void ImgClosePascelDialog_Click(object sender, EventArgs e)
        {
            PascelDialog.Dismiss();

            DeliveryRootLayout.Alpha = 1f;
        }
        MapFunctionHelper mapHelper;
        public void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            this.googleMap.CameraIdle += GoogleMap_CameraIdle;
            string mapKey = Resources.GetString(Resource.String.mapKey);


            mapHelper = new MapFunctionHelper(mapKey, this.googleMap);
        }
        private async void GoogleMap_CameraIdle(object sender, EventArgs e)
        {


            try
            {
                if (addressRequest == 1)
                {
                    pickupLocationLatLng = googleMap.CameraPosition.Target;
                    if (!double.IsNaN(pickupLocationLatLng.Latitude))
                    {
                        TxtPickup.Text = await mapHelper.FindCordinateAddress(pickupLocationLatLng);

                    }

                }
                if (addressRequest == 2)
                {
                    if (!double.IsNaN(destinationLocationLatLng.Latitude))
                    {
                        destinationLocationLatLng = googleMap.CameraPosition.Target;
                        TxtDestination.Text = await mapHelper.FindCordinateAddress(destinationLocationLatLng); 
                    }
                }

            }
            catch (Exception)
            {
                if (addressRequest == 1)
                {
                    pickupLocationLatLng = googleMap.CameraPosition.Target;
                    if (pickupLocationLatLng != null)
                    {
                        TxtPickup.Text = await GetAddress(pickupLocationLatLng.Latitude, pickupLocationLatLng.Longitude);

                    }

                }
                if (addressRequest == 2)
                {
                    destinationLocationLatLng = googleMap.CameraPosition.Target;
                    TxtDestination.Text = await GetAddress(destinationLocationLatLng.Latitude, destinationLocationLatLng.Longitude);
                }

            }

        }

        /***
         * Enable gps
         * **/
        private void CheckGps()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                builder.SetMessage("Enable location");
                builder.SetNegativeButton("Cancel", delegate
                {
                    builder.Dispose();
                });
                builder.SetPositiveButton("Settings", delegate
                {

                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                });
                builder.Show();

            }

        }
        private bool CheckPermission()
        {
            bool permisionGranted = false;
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
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
                CreateLocationRequest();
                GetLocation();
                StartLocationUpdate();
            }
            else
            {
                Toast.MakeText(this, "Permission was denied", ToastLength.Long).Show();
            }
        }
        private void CreateLocationRequest()
        {
            locationRequest = new LocationRequest();
            locationRequest.SetInterval(UPDATE_INTERVAL);
            locationRequest.SetFastestInterval(UPDATE_FASTEST_INTERVAL);
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = LocationServices.GetFusedLocationProviderClient(this);
            locationCallBack = new LocationCallBackHelper();
            locationCallBack.CurrentLocation += LocationCallBack_CurrentLocation; ;
        }

        private void LocationCallBack_CurrentLocation(object sender, LocationCallBackHelper.OnLocationCapturedEventArgs e)
        {
            lastLocation = e.location;
            LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
            if (!RdbDestinationLocation.Checked && !RdbPickUpLocation.Checked && !PickupAddressFromSearch && !DestinationAddressFromSearch)
            {
                //lastLocation = e.location;
                //LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 17));
            }

        }
        private async void GetLocation()
        {
            if (!CheckPermission())
            {
                return;
            }
            lastLocation = await locationClient.GetLastLocationAsync();
            if (!double.IsNaN(lastLocation.Latitude))
            {
                LatLng pos = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
                googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(pos, 13));
                ////loading to database


                //TxtPickup.Text =  address.Result[0].Thoroughfare.Append(" ") + " " + address.Result[0].SubAdminArea;
            }
        }
        private void StartLocationUpdate()
        {
            if (CheckPermission())
            {
                locationClient.RequestLocationUpdates(locationRequest, locationCallBack, null);
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                    addressRequest = 0;
                    PickupAddressFromSearch = true;
                    DestinationAddressFromSearch = false;
                    RdbDestinationLocation.Checked = false;
                    RdbPickUpLocation.Checked = false;
                    var place = Autocomplete.GetPlaceFromIntent(data);
                    TxtPickup.Text = place.Name;
                    pickupLocationLatLng = place.LatLng;
                    googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    ImgCenterMarker.SetColorFilter(Color.Red);

                }
            }
            if (requestCode == 2)
            {
                if (resultCode == Result.Ok)
                {
                    addressRequest = 0;
                    PickupAddressFromSearch = false;
                    DestinationAddressFromSearch = true;
                    RdbDestinationLocation.Checked = false;
                    RdbPickUpLocation.Checked = false;
                    var place = Autocomplete.GetPlaceFromIntent(data);
                    destinationLocationLatLng = place.LatLng;
                    TxtDestination.Text = place.Name.ToString();
                    googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    ImgCenterMarker.SetColorFilter(Color.DarkGreen);
                    //Toast.MakeText(this, place.LatLng.Latitude.ToString(), ToastLength.Long).Show();

                }
            }
        }
        private void RetriveUserInfo()
        {
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(keyId)
                .AddValueEventListener(this);
            InputPickUpLocation.Text = TxtPickup.Text;
            InputDestinationLocation.Text = TxtDestination.Text;
        }
        public void OnCancelled(DatabaseError error)
        {

        }
        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                InputName.Text = snapshot.Child("Name").Value.ToString();
                InputContactNo.Text = snapshot.Child("Phone").Value.ToString();
                InputSurname.Text = snapshot.Child("Surname").Value.ToString();
                //InputEmail.Text = snapshot.Child("Email").Value.ToString();
                if (snapshot.Child("AltPhone").Exists())
                    InputAltNumber.Text = snapshot.Child("AltPhone").Value.ToString();// != "null" ? snapshot.Child("AltPhone").Value.ToString() : "";
            }

        }
        Android.App.AlertDialog SuccessDialog;
        Android.App.AlertDialog.Builder SuccessDialogBuilder;
        private TextView txtSuccessDialogPickupLocation;
        private TextView txtSuccessDialogDestination;
        private TextView txtSuccessDialogTime;
        private TextView txtSuccessDialogDate;
        private TextView txtSuccessDialogDeliverTo;

        private MaterialButton BtnSuccessDialogOk;
        private void SuccessPopUpDialog()
        {
            SuccessDialogBuilder = new Android.App.AlertDialog.Builder(this);

            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.success_dialog, null);

            txtSuccessDialogPickupLocation = view.FindViewById<TextView>(Resource.Id.txtSuccessDialogPickupLocation);
            txtSuccessDialogDestination = view.FindViewById<TextView>(Resource.Id.txtSuccessDialogDestination);
            txtSuccessDialogTime = view.FindViewById<TextView>(Resource.Id.txtSuccessDialogTime);
            txtSuccessDialogDate = view.FindViewById<TextView>(Resource.Id.txtSuccessDialogDate);
            txtSuccessDialogDeliverTo = view.FindViewById<TextView>(Resource.Id.txtSuccessDialogDeliverTo);
            BtnSuccessDialogOk = view.FindViewById<MaterialButton>(Resource.Id.BtnSuccessDialogOk);

            txtSuccessDialogPickupLocation.Text = InputPickUpLocation.Text;
            txtSuccessDialogDestination.Text = InputDestinationLocation.Text;
            txtSuccessDialogDate.Text = BtnDatePick.Text;
            txtSuccessDialogTime.Text = BtnTimePick.Text;
            txtSuccessDialogDeliverTo.Text = InputPersonName.Text;




            BtnSuccessDialogOk.Click += BtnSuccessDialogOk_Click;


            SuccessDialogBuilder.SetView(view);
            SuccessDialogBuilder.SetCancelable(false);
            SuccessDialog = SuccessDialogBuilder.Create();
            SuccessDialog.Show();
        }
        
        private void BtnSuccessDialogOk_Click(object sender, EventArgs e)
        {
            SuccessDialog.Dismiss();
            PascelDialog.Dismiss();

            DeliveryRootLayout.Alpha = 1f;

            

            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("Log", "History");
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
            Finish();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            SuccessPopUpDialog();

        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Error");
            alert.SetMessage(e.Message);
            alert.SetNeutralButton("Ok", delegate
            {
                alert.Dispose();
            });
            alert.Show();
        }

        public void OnCancel(IDialogInterface dialog)
        {
            DeliveryRootLayout.Alpha = 1f;
        }

        private async Task<string> GetAddress(double lat, double lon)
        {


            Geocoder geocode = new Geocoder(this);

            var address = await geocode.GetFromLocationAsync(lat, lon, 1);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(address[0].SubThoroughfare) && !string.IsNullOrWhiteSpace(address[0].SubThoroughfare))
            {

                s.Append(address[0].SubThoroughfare);
            }
            if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
            {
                if (!string.IsNullOrEmpty(address[0].SubThoroughfare) && !string.IsNullOrWhiteSpace(address[0].SubThoroughfare))
                {
                    s.Append(", ");
                }
                s.Append(address[0].Thoroughfare);
            }
            if (!string.IsNullOrEmpty(address[0].SubLocality) && !string.IsNullOrWhiteSpace(address[0].SubLocality))
            {
                if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
                {
                    s.Append(", ");
                }

                s.Append(address[0].SubLocality);
            }
            if (!string.IsNullOrEmpty(address[0].Locality) && !string.IsNullOrWhiteSpace(address[0].Locality))
            {
                if (!string.IsNullOrEmpty(address[0].SubLocality) && !string.IsNullOrWhiteSpace(address[0].SubLocality))
                {
                    s.Append(", ");
                }
                s.Append(address[0].Locality);
            }
            if (!string.IsNullOrEmpty(address[0].SubAdminArea) && !string.IsNullOrWhiteSpace(address[0].SubAdminArea))
            {
                if (!string.IsNullOrEmpty(address[0].Thoroughfare) && !string.IsNullOrWhiteSpace(address[0].Thoroughfare))
                {
                    s.Append(", ");
                }
                s.Append(address[0].SubAdminArea);
            }
           
           
            return s.ToString();


        }
    }
}
