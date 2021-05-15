using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.FirebaseHelper;
using Firebase.Database;
using FirebaseAdmin.Auth;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Even_Odds_Delivary.Fragments
{
    public class DriverRegistrationDialog : DialogFragment
    {
        private EditText InputName;
        private EditText InputEmail;
        private EditText InputSurname;
        private EditText InputPhone;
        private EditText InputColor;
        private EditText InputRegNo;
        private EditText InputMake;
        private MaterialButton BtnType;
        private MaterialButton BtnSubmitReg;
        private Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);

           View view = inflater.Inflate(Resource.Layout.create_driver_dialog, container, false);
            ConnectViews(view);
            return view;

        }

        private void ConnectViews(View view)
        {
            context = view.Context;
            InputName = view.FindViewById<EditText>(Resource.Id.RegisterInputFirstName);
            InputSurname = view.FindViewById<EditText>(Resource.Id.RegisterInputLastName);
            InputPhone = view.FindViewById<EditText>(Resource.Id.RegisterInputPhoneNumber);
            InputEmail = view.FindViewById<EditText>(Resource.Id.RegisterInputEmail);
            InputMake = view.FindViewById<EditText>(Resource.Id.RegisterInputMake);
            InputColor = view.FindViewById<EditText>(Resource.Id.RegisterInputColor);
            InputRegNo = view.FindViewById<EditText>(Resource.Id.RegisterInputRegNo);
            BtnType = view.FindViewById<MaterialButton>(Resource.Id.BtnRegisterCarType);
            BtnSubmitReg = view.FindViewById<MaterialButton>(Resource.Id.BtnRegisterDriver);
            FloatingActionButton FabClose = view.FindViewById<FloatingActionButton>(Resource.Id.FabCloseDirverRegDialog);
            FabClose.Click += FabClose_Click;
            BtnSubmitReg.Click += BtnSubmitReg_Click;
            BtnType.Click += BtnType_Click;
        }

        private void BtnType_Click(object sender, EventArgs e)
        {
            PopupMenu popupMenu = new PopupMenu(context, BtnType);
            popupMenu.Menu.Add(Menu.First, 1, 1, "Bike");
            popupMenu.Menu.Add(Menu.First, 1, 1, "Bakkie");
            popupMenu.Menu.Add(Menu.First, 1, 1, "Provate Car");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            BtnType.Text = e.Item.TitleFormatted.ToString();
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private async void BtnSubmitReg_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(InputName.Text) && string.IsNullOrWhiteSpace(InputName.Text))
            {
                //Toast.MakeText(this, "Please provide your name.", ToastLength.Long).Show();
                InputName.RequestFocus();
                InputName.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text) && string.IsNullOrWhiteSpace(InputSurname.Text))
            {
                InputSurname.RequestFocus();
                InputSurname.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text) && string.IsNullOrWhiteSpace(InputPhone.Text))
            {
                InputPhone.RequestFocus();
                InputPhone.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.RequestFocus();
                InputEmail.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputRegNo.Text) && string.IsNullOrWhiteSpace(InputRegNo.Text))
            {
                InputRegNo.RequestFocus();
                InputRegNo.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputMake.Text) && string.IsNullOrWhiteSpace(InputMake.Text))
            {
                InputMake.RequestFocus();
                InputMake.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputColor.Text) && string.IsNullOrWhiteSpace(InputColor.Text))
            {
                InputColor.RequestFocus();
                InputColor.Error = "required";
                return;
            }
            if (BtnType.Text == "Vehicle Type")
            {
                //Toast.MakeText(context, "Please provide your email address", ToastLength.Long).Show();
                BtnType.RequestFocus();
                BtnType.Error = "required";
                return;
            }
            var stream = Resources.Assets.Open("service_account.json");
            var _auth = FirebaseData.GetFirebaseAdminAuth(stream);

            UserRecordArgs user = new UserRecordArgs()
            {
                Email = InputEmail.Text.Trim(),
                Password = InputPhone.Text.Trim()
            };
            LoadingDialog loading = new LoadingDialog();
            loading.Cancelable = false;
            loading.Show(ChildFragmentManager.BeginTransaction(), "Loading");
            try
            {
                
                var results = await _auth.CreateUserAsync(user);
                RegisterDriverInfor(results.Uid);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            finally
            {
                loading.Dismiss();
                loading.Dispose();
                //loading.Show();
            }
        }
        private void RegisterDriverInfor(string uid)
        {
            HashMap data = new HashMap();
            data.Put("Name", InputName.Text);
            data.Put("Phone", InputPhone.Text);
            data.Put("Surname", InputSurname.Text);
            data.Put("Email", InputEmail.Text);
            data.Put("Type", BtnType.Text);
            data.Put("Make", InputMake.Text);
            data.Put("RegNo", InputRegNo.Text);
            data.Put("Color", InputColor.Text);
            var database = FirebaseDatabase.Instance.GetReference("AppUsers").Child(uid);
            database.SetValue(data);
            AndroidHUD.AndHUD.Shared.ShowSuccess(context, "Successfully registered a driver", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
            Dismiss();

        }
    }
}