using admin.Adapters;
using admin.Fragments;
using admin.Models;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;

namespace admin.Activities
{
    [Activity(Label = "RegisterDriver", MainLauncher = false)]
    public class RegisterDriver : AppCompatActivity
    {

        private readonly List<AppUsers> items = new List<AppUsers>();
        private readonly List<AppUsers> UseritemsList = new List<AppUsers>();
        // private RecyclerView RecyclerUserList;
        private AppUsersAdapter adapter;

        private MaterialButton txtCreateDriver;


        /*views*/

        private Android.Support.V7.Widget.SearchView InputSearchUser;

        private ImageView ImgBack;
        private RecyclerView recyclerUsersList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_registration);

            ConnectViews();
        }

        private void ConnectViews()
        {
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

            SetUpRecycler(UseritemsList);

        }

        private void InputSearchUser_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
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



        private void TxtCreateDriver_Click(object sender, EventArgs e)
        {

            DriverRegistrationDialog dlg = new DriverRegistrationDialog();
            dlg.Show(SupportFragmentManager.BeginTransaction(), "Driver Reg");
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
        private void SetUpRecycler(List<AppUsers> users)
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerUsersList.SetLayoutManager(linearLayoutManager);
            adapter = new AppUsersAdapter(users);
            recyclerUsersList.SetAdapter(adapter);
            adapter.FabCallClick += Adapter_FabCallClick;
            adapter.FabEmailClick += Adapter_FabEmailClick;
            adapter.CreateDriverClick += Adapter_CreateDriverClick;

        }

        private void Adapter_CreateDriverClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            if (items[e.Position].Role != "Driver")
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage("Are you sure you want to activate user: " + items[e.Position].Name + " " + items[e.Position].Surname + " as a driver?");
                builder.SetPositiveButton("Yes", delegate
                {

                    UpdateExistingUserFragment userFragment = new UpdateExistingUserFragment(items[e.Position])
                    {
                        Cancelable = false
                    };
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
                builder.SetMessage("Are you sure you want to deactivate driver: " + items[e.Position].Name + " " + items[e.Position].Surname + "?");
                builder.SetPositiveButton("Yes", async delegate
                 {
                     await CrossCloudFirestore
                         .Current
                         .Instance
                         .Collection("AppUsers")
                         .Document(items[e.Position].Uid)
                         .UpdateAsync("Role", "C");
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