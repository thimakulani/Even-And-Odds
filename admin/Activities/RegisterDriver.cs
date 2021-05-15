using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Button;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Even_Odds_Delivary.Adapters;
using Even_Odds_Delivary.AppData;
using Even_Odds_Delivary.FirebaseHelper;
using Even_Odds_Delivary.Fragments;
using Even_Odds_Delivary.Models;
using Firebase.Auth;
using Firebase.Database;
using FirebaseAdmin.Auth;
using Java.Util;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;

namespace Even_Odds_Delivary.Activities
{
    [Activity(Label = "RegisterDriver", MainLauncher = false)]
    public class RegisterDriver : AppCompatActivity, IOnFailureListener, IOnCompleteListener
    {
        
        private List<AppUsers> items = new List<AppUsers>();
        private List<AppUsers> UseritemsList = new List<AppUsers>();
        private AppUsersData userData = new AppUsersData();
       // private RecyclerView RecyclerUserList;
        private AppUsersAdapter adapter;

        private MaterialButton txtCreateDriver;


        /*views*/
        private RelativeLayout root_registration_layout;
        private EditText InputName;
        private EditText InputEmail;
        private EditText InputSurname;
        private EditText InputPhone;
        private Android.Support.V7.Widget.SearchView InputSearchUser;
        
        
        private MaterialButton BtnSubmitReg;
        //private CheckBox Terms;
        private ImageView ImgBack;
        private RecyclerView recyclerUsersList;



        /*Driver registration dialog*/
        /*Dialog*/
        //private AlertDialog RegistrationDialog;
        //private AlertDialog.Builder dialogBuilder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_registration);

            GetUserData();
            ConnectViews();
        }

        private void ConnectViews()
        {

            root_registration_layout = FindViewById<RelativeLayout>(Resource.Id.root_registration_layout);

            //InputSearchUser.Visibility = ViewStates.Gone;

            ImgBack = FindViewById<ImageView>(Resource.Id.imgCloseSignUp);

           // FabSearch = FindViewById<FloatingActionButton>(Resource.Id.FabRearch);
            InputSearchUser = FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.InputSearchUsers);
            InputSearchUser.Visibility = ViewStates.Visible;
            recyclerUsersList = FindViewById<RecyclerView>(Resource.Id.recyclerUsersList);

            InputSearchUser.QueryTextChange += InputSearchUser_QueryTextChange;
            txtCreateDriver = FindViewById<MaterialButton>(Resource.Id.txtCreateDriver);

            
            ImgBack.Click += ImgBack_Click;
           // FabSearch.Click += FabSearch_Click;
            txtCreateDriver.Click += TxtCreateDriver_Click;



        }

        private void InputSearchUser_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            var users = (from data in UseritemsList
                         where
                        data.Name.Contains(e.NewText) ||
                        data.UserType.Contains(e.NewText) ||
                        data.Surname.Contains(e.NewText) ||
                        data.Email.Contains(e.NewText) ||
                        data.PhoneNumber.Contains(e.NewText)
                         select data).ToList<AppUsers>();

            SetUpRecycler(users);
        }

      

        private void TxtCreateDriver_Click(object sender, EventArgs e)
        {
            
            DriverRegistrationDialog dlg = new DriverRegistrationDialog();
            dlg.Show(SupportFragmentManager.BeginTransaction(), "Driver Reg");
        }

        //private bool SearchOpen = false;
        //private void FabSearch_Click(object sender, EventArgs e)
        //{
        //    if(SearchOpen == false)
        //    {
        //        FabSearch.SetImageResource(Resource.Drawable.abc_ic_clear_material);
        //        InputSearchUser.Visibility = ViewStates.Visible;
        //        InputSearchUser.RequestFocus();
        //        SearchOpen = true;
        //        return;
        //    }
        //    if(SearchOpen == true)
        //    {
        //        FabSeaalphrch.SetImageResource(Resource.Drawable.abc_ic_search_api_material);
        //        InputSearchUser.Visibility = ViewStates.Gone;
        //        SearchOpen = false;
        //        return;
        //    }
        //}


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
            
        }


        private void GetUserData()
        {
            userData.GetUsers();
            userData.RetrivedData += UserData_RetrivedData;
        }

        private void UserData_RetrivedData(object sender, AppUsersData.UsersDataEventArgs e)
        {
            items = e.users_list;
            UseritemsList = e.users_list;
            SetUpRecycler(UseritemsList);
        }

        private void SetUpRecycler(List<AppUsers> users)
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerUsersList.SetLayoutManager(linearLayoutManager);
            adapter = new AppUsersAdapter(users, this);
            recyclerUsersList.SetAdapter(adapter);
            adapter.FabCallClick += Adapter_FabCallClick;
            adapter.FabEmailClick += Adapter_FabEmailClick;
            adapter.CreateDriverClick += Adapter_CreateDriverClick;

        }

        private void Adapter_CreateDriverClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            if(items[e.Position].UserType != "Driver")
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage("Are you sure you want to activate user: " + items[e.Position].Name + " " + items[e.Position].Surname + " as a driver?");
                builder.SetPositiveButton("Yes", delegate
                {

                    UpdateExistingUserFragment userFragment = new UpdateExistingUserFragment(items[e.Position]);
                    userFragment.Cancelable = false;
                    userFragment.Show(SupportFragmentManager.BeginTransaction(), "Update User");
                    builder.Dispose();
                    
                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage("Are you sure you want to deactivate driver: " + items[e.Position].Name + " " + items[e.Position].Surname + "?");
                builder.SetPositiveButton("Yes", delegate
                {

                    FirebaseDatabase.Instance.GetReference("AppUsers").Child(items[e.Position].KeyId).Child("UserType").SetValue("Client");
                    builder.Dispose();

                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();
                
            }
        }

        private async void Adapter_FabEmailClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            
            try
            {
                List<string> to = new List<string>();// 
                to.Add(items[e.Position].Email);
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
                PhoneDialer.Open(items[e.Position].PhoneNumber);
            }
            catch (ArgumentNullException anEx)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(anEx.Message);
                builder.SetNeutralButton("OK", delegate
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
                builder.SetNeutralButton("OK", delegate
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
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();

            }
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

        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            
        }

        public void OnComplete(Task task)
        {
            
        }

        
        //public void OnCancelled(DatabaseError error)
        //{

        //}

        //public void OnDataChange(DataSnapshot snapshot)
        //{
        //    if(snapshot.Value != null)
        //    {
        //        var child = snapshot.Children.ToEnumerable<DataSnapshot>();
        //        foreach(DataSnapshot data in child)
        //        {
        //            if (data.Child("Email").Value.ToString() == "thimakulani@gmail.com")
        //            {
        //                databaseReference.RemoveEventListener(this);
        //                return;
        //            }
        //        }
        //    }
        //}

    }
}