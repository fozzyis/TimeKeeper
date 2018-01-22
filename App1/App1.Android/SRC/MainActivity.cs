using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using TimeKeeperShared;
using System;
using System.IO;
using Android.Content;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace TimeKeeper.Droid
{
    [Activity(Label = "TimeKeeper", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        int count = 1;
        bool timerRunning = false;
        public Employee loadEmployee;
        string packageName;
        string pathToSave;
        readonly string[] PermissionsStorage =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };

        public LinearLayout settingsLayout;

        const int RequestStorageId = 0;
        const string permission = Manifest.Permission.WriteExternalStorage;


        string documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path;   //ExternalStorageDirectory.Path;
        //string documentsPath = Android.OS.Environment.;
        const string EMPLOYEE_DATA_FILE = "userData.xml";

        TimeKeeperShared.TimeKeeper TKS = new TimeKeeperShared.TimeKeeper();
        EmployeeSettings employeeSettings = new EmployeeSettings();
        EmployeeDataManager tksXML = new EmployeeDataManager();
        


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            AppCenter.Start("5358b8a5-b78a-4851-9c5f-ecb5abe8ab1e",
                   typeof(Analytics), typeof(Crashes));

            packageName = PackageName;
            pathToSave = Path.Combine(documentsPath, packageName, EMPLOYEE_DATA_FILE);
            Console.WriteLine(pathToSave);
            settingsLayout = FindViewById<LinearLayout>(Resource.Id.SettingsLayout);

            Button button = FindViewById<Button>(Resource.Id.starttimer_button);
            Button button2 = FindViewById<Button>(Resource.Id.newtimer_button);
            Button options = FindViewById<Button>(Resource.Id.stoptimer_button);
            button.Click += StartTimer;
            button2.Click += GetStopTime;
            //options.Click += ShowOptions;
            //SetContentView(Resource.Layout.EmployeeSettings);
            //Button options = FindViewById<Button>(Resource.Id.stoptimer_button);
            options.Click += delegate
            {
                var filePath = pathToSave;
                var uri = Android.Net.Uri.Parse("file://" + filePath);
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] {
            "andrew.foster@broadspectrum.com",
            "fozzyis@gmail.com"
        });
                email.PutExtra(Android.Content.Intent.ExtraCc, new string[] {
            "andrew.foster1981@gmail.com"
        });
                email.PutExtra(Android.Content.Intent.ExtraSubject, "Hello Xamarin");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello Xamarin This is My Test Mail...!");
                email.PutExtra(Android.Content.Intent.ExtraStream, uri);
                Console.WriteLine(uri);
                email.SetType("message/rfc822");
                StartActivity(email);
            };

            var noEmployee = tksXML.CheckForSavedData(pathToSave, documentsPath, packageName);
            if (noEmployee == false)
            {
                LoadUserSettings();
            }
            else
            {
                RetrieveXML(this, null);

            }

        }

        public void LoadUserSettings()
        {
            
            SetContentView(Resource.Layout.EmployeeSettings);
            Button buttonXML = FindViewById<Button>(Resource.Id.save_button);
            buttonXML.Click += async (sender, e) => await TryGetStorageAsync();
            Button buttonRetrieve = FindViewById<Button>(Resource.Id.load_button);
            buttonRetrieve.Click += RetrieveXML;


        }
        /// <summary>
        /// Request permission code
        /// </summary>
        async Task TryGetStorageAsync()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await LoadFile();
                return;
            }

            await GetStoragePermissionAsync();
        }

        async Task GetStoragePermissionAsync()
        {
            //Check to see if any permission in our group is available, if one, then all are
            const string permission = Manifest.Permission.WriteExternalStorage;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                await LoadFile();
                return;
            }

            //need to request permission
            if (ShouldShowRequestPermissionRationale(permission))
            {
                //Explain to the user why we need to read the contacts
                Snackbar.Make(settingsLayout, "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                        .SetAction("OK", v => RequestPermissions(PermissionsStorage, RequestStorageId))
                        .Show();
                return;
            }
            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PermissionsStorage, RequestStorageId);
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestStorageId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            // var snack = Snackbar.Make(settingsLayout, "Location permission is available, getting lat/long.", Snackbar.LengthShort);
                            // snack.Show();

                            await LoadFile();
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                            //var snack = Snackbar.Make(settingsLayout, "Location permission is denied.", Snackbar.LengthShort);
                            //snack.Show();
                        }
                    }
                    break;
            }
        }

        async Task LoadFile()
        {
            RunXML(this, null);
        }
        /// <summary>
        /// Request permission code
        /// </summary>

        void SubmitTimesheet()
        {
            Button options = FindViewById<Button>(Resource.Id.stoptimer_button);
            options.Click += delegate
            {
                var filePath = pathToSave;
                var uri = Android.Net.Uri.Parse(filePath);
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] {
            "andrew.foster@broadspectrum.com",
            "fozzyis@gmail.com"
        });
                email.PutExtra(Android.Content.Intent.ExtraCc, new string[] {
            "andrew.foster1981@gmail.com"
        });
                email.PutExtra(Android.Content.Intent.ExtraSubject, "Hello Xamarin");
                email.PutExtra(Android.Content.Intent.ExtraText, "Hello Xamarin This is My Test Mail...!");
                email.PutExtra(Android.Content.Intent.ExtraStream, uri);
                email.SetType("message/rfc822");
                StartActivity(email);
            };
        }

        void StartTimer(object sender, EventArgs ea)
        {
            TKS.StartTimer();
            timerRunning = true;
            TextView startLabel = FindViewById<TextView>(Resource.Id.timeticker_text);
            startLabel.Text = TKS.GetStartTime().ToLongTimeString();
             
        }
        
        public void ShowOptions(object sender, EventArgs ea)
        {
            SetContentView(Resource.Layout.EmployeeSettings);
            Button buttonXML = FindViewById<Button>(Resource.Id.save_button);
            buttonXML.Click += RunXML;
            Button buttonRetrieve = FindViewById<Button>(Resource.Id.load_button);
            buttonRetrieve.Click += RetrieveXML;

            FindViewById<EditText>(Resource.Id.name_text).Text = employeeSettings.loadEmployee.employeeName;
            FindViewById<EditText>(Resource.Id.empID_text).Text = employeeSettings.loadEmployee.employeeID;
            FindViewById<EditText>(Resource.Id.site_text).Text = employeeSettings.loadEmployee.employeeSite;
            FindViewById<EditText>(Resource.Id.workgroup_text).Text = employeeSettings.loadEmployee.employeeWorkgroup;
            FindViewById<EditText>(Resource.Id.scheduler_text).Text = employeeSettings.loadEmployee.workgroupScheduler;
            FindViewById<EditText>(Resource.Id.schedemail_text).Text = employeeSettings.loadEmployee.schedulerEmail;
            FindViewById<EditText>(Resource.Id.teamleader_text).Text = employeeSettings.loadEmployee.workgroupTeamLeader;
            FindViewById<EditText>(Resource.Id.tlemail_text).Text = employeeSettings.loadEmployee.teamleaderEmail;
            FindViewById<EditText>(Resource.Id.lastedit_text).Text = employeeSettings.loadEmployee.lastTimeModified;
        }
        void GetStopTime(object sender, EventArgs ea)
        {
            TKS.GetStopTime();
            TextView stopLabel = FindViewById<TextView>(Resource.Id.timeticker_text);

            stopLabel.Text = TKS.GetTimeSheetEntry().ToString();
        }

        public void RunXML(object sender, EventArgs ea)
        {

            var addEmployee = new Employee();
            addEmployee.employeeName = FindViewById<EditText>(Resource.Id.name_text).Text;
            addEmployee.employeeID = FindViewById<EditText>(Resource.Id.empID_text).Text;
            addEmployee.employeeSite = FindViewById<EditText>(Resource.Id.site_text).Text;
            addEmployee.employeeWorkgroup = FindViewById<EditText>(Resource.Id.workgroup_text).Text;
            addEmployee.workgroupScheduler = FindViewById<EditText>(Resource.Id.scheduler_text).Text;
            addEmployee.schedulerEmail = FindViewById<EditText>(Resource.Id.schedemail_text).Text;
            addEmployee.workgroupTeamLeader = FindViewById<EditText>(Resource.Id.teamleader_text).Text;
            addEmployee.teamleaderEmail = FindViewById<EditText>(Resource.Id.tlemail_text).Text;
            addEmployee.lastTimeModified = DateTime.Now.ToLongDateString();

            var tksXML = new EmployeeDataManager();
            Console.WriteLine(documentsPath);

            Console.WriteLine(packageName);
            Directory.CreateDirectory(documentsPath + "/" + packageName);

            tksXML.SaveEmployee(addEmployee, pathToSave);
            
        }

        public void RetrieveXML(object sender, EventArgs ea)
        {
            var tksXML = new EmployeeDataManager();
            loadEmployee = tksXML.LoadEmployee(pathToSave);



            FindViewById<TextView>(Resource.Id.textView4).Text = "Name: " + loadEmployee.employeeName + ", Employee ID: " + loadEmployee.employeeID + ", Site: " + loadEmployee.employeeSite
                                                                    + ", Workgroup: " + loadEmployee.employeeWorkgroup + ", Scheduler: " + loadEmployee.workgroupScheduler + ", Scheduler Email: "
                                                                    + loadEmployee.schedulerEmail + ", Last Edit: " + loadEmployee.lastTimeModified;

        }



    }
}

