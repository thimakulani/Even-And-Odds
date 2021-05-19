using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Google.Android.Material.Button;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using admin.Adapters;
using admin.FirebaseHelper;
using admin.Models;
using FirebaseAdmin.Auth;
using Xamarin.Essentials;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;

namespace admin.Activities
{
    [Activity(Label = "SuperUser", MainLauncher = false)]
    public class SuperUser : Activity, IOnSuccessListener, IDialogInterfaceOnDismissListener
    {


        private readonly List<AppUsers> items = new List<AppUsers>();
        private readonly List<AppUsers> UseritemsList = new List<AppUsers>();
        //private List<AppUsers> adminList = new List<AppUsers>();

        // private RecyclerView RecyclerUserList;
        private AdminAdapter adapter;
        private MaterialButton txtCreateSuperUser;


        /*views*/
        private RelativeLayout root_superuser_layout;
        private TextInputEditText InputName;
        private TextInputEditText InputEmail;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private AndroidX.AppCompat.Widget.SearchView InputSearchUser;
        

        private MaterialButton BtnSubmitReg;
        //private CheckBox Terms;
        private ImageView ImgBack;
        private RecyclerView recyclerUsersList;



        /*Driver registration dialog*/
        /*Dialog*/
        private AlertDialog RegistrationDialog;
        private AlertDialog.Builder dialogBuilder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_super_users);


            ConnectViews();
        }

        private void ConnectViews()
        {

            root_superuser_layout = FindViewById<RelativeLayout>(Resource.Id.root_superuser_layout);

            //InputSearchUser.Visibility = ViewStates.Gone;

            ImgBack = FindViewById<ImageView>(Resource.Id.imgCloseSuperuser);

            //FabSearch = FindViewById<FloatingActionButton>(Resource.Id.FabSearchSuperUser);
            InputSearchUser = FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.InputSearchSuperUsers);
            InputSearchUser.Visibility = ViewStates.Gone;
            recyclerUsersList = FindViewById<RecyclerView>(Resource.Id.recyclerSuperUsersList);

            InputSearchUser.QueryTextChange += InputSearchUser_QueryTextChange1;
            txtCreateSuperUser = FindViewById<MaterialButton>(Resource.Id.txtCreateSuperUser);


            ImgBack.Click += ImgBack_Click;
           
            txtCreateSuperUser.Click += TxtCreateSuperUser_Click;

            SetUpRecycler(UseritemsList);

        }

        private void InputSearchUser_QueryTextChange1(object sender, AndroidX.AppCompat.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            var users = (from data in UseritemsList
                         where
                        data.Name.Contains(e.NewText) ||
                        data.Role.Contains(e.NewText) ||
                        data.Surname.Contains(e.NewText) ||
                        data.Email.Contains(e.NewText) ||
                        data.Phone.Contains(e.NewText)
                         select data).ToList<AppUsers>();

            SetUpRecycler(users);
        }


        private void TxtCreateSuperUser_Click(object sender, EventArgs e)
        {
            root_superuser_layout.Alpha = 0.5f;
            SuperUserRegistrationDialogHelper();
        }

        
        private void SuperUserRegistrationDialogHelper()
        {
            dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater layoutInflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = layoutInflater.Inflate(Resource.Layout.create_super_user_dialog, null);
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.SuperUserRegisterInputFirstName);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.SuperUserRegisterInputLastName);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.SuperUserRegisterInputPhoneNumber);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.SuperUserRegisterInputEmail);
            BtnSubmitReg = view.FindViewById<MaterialButton>(Resource.Id.SuperUserBtnRegisterDriver);
            BtnSubmitReg.Click += BtnSubmitReg_Click;
            FloatingActionButton FabCloseSuperRegDialog = view.FindViewById<FloatingActionButton>(Resource.Id.SuperUserFabCloseSuperUserRegDialog);
            FabCloseSuperRegDialog.Click += FabCloseSuperUserDialog_Click;
            dialogBuilder.SetView(view);

            dialogBuilder.SetCancelable(false);
            dialogBuilder.SetOnDismissListener(this);
            RegistrationDialog = dialogBuilder.Create();
            RegistrationDialog.Show();
        }

        private void FabCloseSuperUserDialog_Click(object sender, EventArgs e)
        {
            RegistrationDialog.Dismiss();
            root_superuser_layout.Alpha = 1f;

        }

        public void OnDismiss(IDialogInterface dialog)
        {
            root_superuser_layout.Alpha = 1f;

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        private void ImgBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        private void BtnSubmitReg_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputName.Text) && string.IsNullOrWhiteSpace(InputName.Text))
            {
                InputName.Error = "Please provide your name.";
                InputName.RequestFocus();
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text) && string.IsNullOrWhiteSpace(InputSurname.Text))
            {
                InputSurname.Error = "Please provide your last name";
                InputSurname.RequestFocus();
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text) && string.IsNullOrWhiteSpace(InputPhone.Text))
            {
                InputPhone.Error = "Please provide your phone numbers";
                InputPhone.RequestFocus();
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.Text = "Please provide your email address";
                InputEmail.RequestFocus();
                return;
            }
            foreach (var data in items)
            {
                if (data.Email == InputEmail.Text)
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Warning");
                    // builder.SetMessage("Reset password link has been sent to your email address");
                    builder.SetMessage("Email address: " + data.Email + " has already been registered" +
                        " to the system as a " + data.Role + ", Would you like to activate user as " +
                        "a administrator?");
                    builder.SetPositiveButton("Yes", delegate
                    {

                        //FirebaseDatabase.Instance.GetReference("AppUsers")
                        //.Child(data.Uid).Child(.Role").SetValue("Admin");
                        builder.Dispose();

                    });
                    builder.SetNegativeButton("No", delegate
                    {
                        builder.Dispose();
                        return;
                    });
                    builder.Show();
                    return;
                }
            }
            RegiterSuperUser(InputEmail.Text, InputPhone.Text);
        }

        private async void RegiterSuperUser(string email, string password)
        {

            var stream = Resources.Assets.Open("service_account.json");
            var _auth = FirebaseData.GetFirebaseAdminAuth(stream);
            

            UserRecordArgs user = new UserRecordArgs()
            {
                Email = email.Trim(),
                Password = password.Trim()
            };
            LoadingProgress();
            try
            {
                var results = await _auth.CreateUserAsync(user);
                RegisterInfor(results);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
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
            }

        }


        private void SetUpRecycler(List<AppUsers> users)
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerUsersList.SetLayoutManager(linearLayoutManager);
            adapter = new AdminAdapter(users);
            recyclerUsersList.SetAdapter(adapter);
            adapter.FabCallClick += Adapter_FabCallClick;
            adapter.FabEmailClick += Adapter_FabEmailClick;

        }
        private AlertDialog loading;
        private void LoadingProgress()
        {
            var loadingBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.loading_progress, null);
            loadingBuilder.SetView(view);
            loadingBuilder.SetCancelable(false);
            loading = loadingBuilder.Create();
            loading.Show();
        }


        private async void Adapter_FabEmailClick(object sender, AppUsersAdapterClickEventArgs e)
        {

            try
            {
                List<string> to = new List<string>
                {
                    items[e.Position].Email
                };// 
                var message = new EmailMessage
                {
                    Subject = "Even & Odds Team",
                    Body = "Even & Odds Team",
                    To = to,

                    //Cc = ,
                    //Bcc = bccRecipients            
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(fbsEx.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }

        private void Adapter_FabCallClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            try
            {
                PhoneDialer.Open(items[e.Position].Phone);
            }
            catch (ArgumentNullException anEx)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(anEx.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();

            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            
        }

        private void RegisterInfor(UserRecord results)
        {
            /*Register*/
            HUD($"Successfully registered a {InputName.Text} as administrator");
            RegistrationDialog.Dismiss();
        }
        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
    }
}