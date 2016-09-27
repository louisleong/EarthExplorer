using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Locations;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace EarthExplorer.Droid
{
    [Activity(Label = "EarthExplorer.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        int count = 1;
        List<PointOfInterest> Datasource;
        PointOfInterest currentPOI = new PointOfInterest();
        ManualResetEvent waitEvent = new ManualResetEvent(false);

        public void OnLocationChanged(Location location)
        {
            currentPOI.Name = "Exactly here!";
            currentPOI.Latitude = location.Latitude;
            currentPOI.Longitude = location.Longitude;

            waitEvent.Set();
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


        protected async override void OnCreate(Bundle bundle)
        {


            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            InitLocationManager();
            PointOfInterest.GetCurrentPOIAsync = async () =>
            {
                await Task.Run(() => waitEvent.WaitOne());
                return currentPOI;
            };

            ListView listView = FindViewById<ListView>(Resource.Id.listView1);

            listView.ItemClick += ListView_ItemClick;

            Datasource = await PointOfInterest.GetGlobalListAsync();
            listView.Adapter = new POIAdapter(this, Datasource);

        }
        private void InitLocationManager()
        {
            var locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            string locationProvider;
            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }

            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var poi = Datasource[(int)e.Id];

            var geoUri = Android.Net.Uri.Parse($"geo:{poi.Latitude},{poi.Longitude}");
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            StartActivity(mapIntent);
        }
    }
}


