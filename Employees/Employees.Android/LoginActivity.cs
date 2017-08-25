using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Switch = Android.Support.V7.Widget.SwitchCompat;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Employees.Models;
using Employees.Services;
using Android.Content;

namespace Employees.Droid
{
    [Activity(Label = "Empleados", MainLauncher = true, Icon = "@drawable/Icon")]
    public class LoginActivity : AppCompatActivity
    {
        #region Widget
        Toolbar toolbar;
        Android.Support.Design.Widget.TextInputLayout usernameLayout;
        Android.Support.Design.Widget.TextInputEditText editTextUsername;
        Android.Support.Design.Widget.TextInputLayout passwordLayout;
        Android.Support.Design.Widget.TextInputEditText editTextPassword;
        Android.App.ProgressDialog progress;
        Button buttonLogin;
        Switch switchRememberme;
        View rootView;
        #endregion

        #region Attributes
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);
            
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            usernameLayout = FindViewById<Android.Support.Design.Widget.TextInputLayout>(Resource.Id.usernameLayout);
            editTextUsername = FindViewById<TextInputEditText>(Resource.Id.editTextUsername);
            passwordLayout = FindViewById<Android.Support.Design.Widget.TextInputLayout>(Resource.Id.passwordLayout);
            editTextPassword = FindViewById<TextInputEditText>(Resource.Id.editTextPassword);
            buttonLogin = FindViewById<Button>(Resource.Id.ButtonLogin);
            rootView = FindViewById<View>(Android.Resource.Id.Content);
            switchRememberme = FindViewById<Switch>(Resource.Id.switchRememberme);

            apiService = new ApiService();
            dialogService = new DialogService();

            //Toolbar will now take on defaul actionbar characteristics
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Empleados";

            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetHomeButtonEnabled(true);
            //SupportActionBar.SetIcon(Resource.Id.icon);

            buttonLogin.Click += OnLogin;
        }

        async void OnLogin(object sender, EventArgs e)
        {
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Verificando Datos...");
            progress.SetCancelable(false);
            progress.Show();

            bool isValid = ValidateEmail() && ValidatePassword();
            if (!isValid)
            {
                progress.Dismiss();
                Snackbar.Make(rootView, Resource.String.fillRequired, Snackbar.LengthLong)
                        .Show();
                return;
            }
            //else
            //{
            usernameLayout.ErrorEnabled = false;
            passwordLayout.ErrorEnabled = false;

            //var checkConnetion = await apiService.CheckConnection();
            //if (!checkConnetion.IsSuccess)
            //{
            //    //progressBarActivityIndicator.Visibility = ViewStates.Invisible;
            //    progress.Dismiss();
            //    //buttonLogin.Enabled = true;
            //    dialogService.ShowMessage(this, "Error", checkConnetion.Message);
            //    Snackbar.Make(rootView, "Problemas de conexión", Snackbar.LengthLong)
            //            .Show();
            //    return;
            //}

            var urlAPI = Resources.GetString(Resource.String.URLAPI);

            var token = await apiService.GetToken(
                urlAPI,
                editTextUsername.Text,
                editTextPassword.Text);

            if (token == null)
            {
                progress.Dismiss();
                //buttonLogin.Enabled = true;
                dialogService.ShowMessage(this, "Error", "El email o la contraseña es incorrecto.");
                editTextPassword.Text = null;
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                progress.Dismiss();
                //buttonLogin.Enabled = true;
                dialogService.ShowMessage(this, "Error", token.ErrorDescription);
                editTextPassword.Text = null;
                return;
            }

            var response = await apiService.GetEmployeeByEmailOrCode(
                                            urlAPI,
                                            "/api",
                                            "/Employees/GetGetEmployeeByEmailOrCode",
                                            token.TokenType,
                                            token.AccessToken,
                                            token.UserName);

            if (!response.IsSuccess)
            {
                progress.Dismiss();
                //buttonLogin.Enabled = true;
                dialogService.ShowMessage(this, "Error", "Problema con el usuario, contacte a Pandian.");
                return;
            }

            var employee = (Employee)response.Result;
            employee.AccessToken = token.AccessToken;
            employee.IsRemembered = switchRememberme.Checked;
            employee.Password = editTextPassword.Text;
            employee.TokenExpires = token.Expires;
            employee.TokenType = token.TokenType;

            progress.Dismiss();

            var intent = new Intent(this, typeof(EmployeesActivity));
            intent.PutExtra("AccessToken", employee.AccessToken);
            intent.PutExtra("TokenType", employee.TokenType);
            intent.PutExtra("EmployeeId", employee.EmployeeId);
            intent.PutExtra("FullName", employee.FullName);

            StartActivity(intent);
            //buttonLogin.Enabled = true;

            //it isn't Necesary
            //Snackbar.Make(rootView, Resource.String.fillRight, Snackbar.LengthLong)
            //            .Show();
            //}
        }

        bool ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(editTextPassword.Text))
            {
                passwordLayout.ErrorEnabled = true;
                passwordLayout.Error = GetString(Resource.String.passwordRequired);
                return false;
            }
            else
            {
                passwordLayout.ErrorEnabled = false;
                return true;
            }
        }

        bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(editTextUsername.Text))
            {
                usernameLayout.ErrorEnabled = true;
                usernameLayout.Error = GetString(Resource.String.usernameRequired);
                return false;
            }
            else
            {
                usernameLayout.ErrorEnabled = false;
                return true;
            }
        } 
        #endregion
        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    if (item.ItemId == Android.Resource.Id.Home)
        //        Finish();

        //    return base.OnOptionsItemSelected(item);
        //}
    }
}