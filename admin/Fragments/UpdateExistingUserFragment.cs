using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Even_Odds_Delivary.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Even_Odds_Delivary.Fragments
{
    public class UpdateExistingUserFragment : DialogFragment
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
        private AppUsers appUsers = new AppUsers();

        public UpdateExistingUserFragment(AppUsers appUsers)
        {
            this.appUsers = appUsers;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
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
            InputEmail.Enabled = false;
            FabClose.Click += FabClose_Click;
            BtnSubmitReg.Click += BtnSubmitReg_Click;
            BtnType.Click += BtnType_Click;
            SetInputs();



        }

        private void SetInputs()
        {
            InputName.Text = appUsers.Name;
            InputSurname.Text = appUsers.Surname;
            InputPhone.Text = appUsers.PhoneNumber;
            InputEmail.Text = appUsers.Email;
            InputMake.Text = appUsers.Make;
            InputRegNo.Text = appUsers.RegNo;
            InputColor.Text = appUsers.Color;
            if (!string.IsNullOrEmpty(appUsers.Type))
            {
                BtnType.Text = appUsers.Type;
            }
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
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private void BtnSubmitReg_Click(object sender, EventArgs e)
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
                BtnType.RequestFocus();
                BtnType.Error = "required";
                return;
            }
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(appUsers.KeyId).Child("UserType").SetValue("Driver");
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(appUsers.KeyId).Child("Make").SetValue(InputMake.Text);
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(appUsers.KeyId).Child("RegNo").SetValue(InputRegNo.Text);
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(appUsers.KeyId).Child("Color").SetValue(InputColor.Text);
            FirebaseDatabase.Instance.GetReference("AppUsers").Child(appUsers.KeyId).Child("Type").SetValue(BtnType.Text);

            AndroidHUD.AndHUD.Shared.ShowSuccess(context, "Successfully created a driver", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
            Dismiss();
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
            Dispose();
        }
    }
}