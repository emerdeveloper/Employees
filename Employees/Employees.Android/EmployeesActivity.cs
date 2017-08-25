using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Employees.Models;
using Employees.Services;
using System.Collections.Generic;
using System.Linq;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Employees.Droid
{
    [Activity(Label = "Empleados", MainLauncher = false, Icon = "@drawable/Icon")]
    public class EmployeesActivity : AppCompatActivity
    {

        #region Attributes
        string accessToken;
        string tokenType;
        string employeeId;
        string fullName;
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Widgets
        Toolbar toolbar;
        TextView textViewFullName;
        ListView listViewEmployees;
        ProgressBar progressBarActivityIndicator;
        #endregion

        #region Methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Employees);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            textViewFullName = FindViewById<TextView>(Resource.Id.textViewFullName);
            listViewEmployees = FindViewById<ListView>(Resource.Id.listViewEmployees);
            progressBarActivityIndicator = FindViewById<ProgressBar>(Resource.Id.progressBarActivityIndicator);

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Empleados";

            progressBarActivityIndicator.Visibility = ViewStates.Invisible;

            apiService = new ApiService();
            dialogService = new DialogService();

            //Get Data of After Activity
            accessToken = Intent.GetStringExtra("AccessToken");
            tokenType = Intent.GetStringExtra("TokenType");
            employeeId = Intent.GetStringExtra("EmployeeId");
            fullName = Intent.GetStringExtra("FullName");

            textViewFullName.Text = fullName;

            LoadEmployees();
        }

        async void LoadEmployees()
        {
            progressBarActivityIndicator.Visibility = ViewStates.Visible;

            List<Employee> employees = null;

            var dataEmployees = (EmployeeFragment)this
                .FragmentManager
                .FindFragmentByTag("DataEmployees");

            if (dataEmployees == null)
            {
                var urlAPI = Resources.GetString(Resource.String.URLAPI);

                var response = await apiService.GetList<Employee>(
                    urlAPI,
                    "/api",
                    "/Employees",
                    tokenType,
                    accessToken);

                if (!response.IsSuccess)
                {
                    progressBarActivityIndicator.Visibility = ViewStates.Invisible;
                    dialogService.ShowMessage(this, "Error", response.Message);
                    return;
                }

                employees = ((List<Employee>)response.Result)
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList();

                dataEmployees = new EmployeeFragment();
                var fragmentTransaction = this.FragmentManager.BeginTransaction();
                fragmentTransaction.Add(dataEmployees, "DataEmployees");
                fragmentTransaction.Commit();
                dataEmployees.employees = employees;
            }
            else
            {
                employees = dataEmployees.employees;
            }

            listViewEmployees.Adapter = new EmployeesAdapter(
                this,
                employees,
                Resource.Layout.EmployeeItem,
                Resource.Id.imageFullPicture,
                Resource.Id.textViewFullName,
                Resource.Id.textViewEmail,
                Resource.Id.textViewEmployeeCode,
                Resource.Id.textViewPhone,
                Resource.Id.textViewAddress);

            progressBarActivityIndicator.Visibility = ViewStates.Invisible;
        } 
        #endregion
    }
}