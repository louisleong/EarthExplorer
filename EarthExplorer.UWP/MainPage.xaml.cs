using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EarthExplorer.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var accessStatus = await Geolocator.RequestAccessAsync();

            PointOfInterest.GetCurrentPOIAsync = async () =>
                       {
                           var currentPOI = new PointOfInterest();
                           var geolocator = new Geolocator();

                           var position = await geolocator.GetGeopositionAsync();

                           currentPOI.Name = "Just where I am";
                           currentPOI.Latitude = position.Coordinate.Point.Position.Latitude;
                           currentPOI.Longitude = position.Coordinate.Point.Position.Longitude;

                           return currentPOI;
                       };

            POIList.ItemClick += POIList_ItemClick;

            var dataSource = await PointOfInterest.GetGlobalListAsync();

            POIList.ItemsSource = dataSource;
            MoveTo(dataSource[0]);

            Map.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads;
        }

        private void POIList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var poi = e.ClickedItem as PointOfInterest;
            MoveTo(poi);
        }

        private async void MoveTo(PointOfInterest poi)
        {
            var position = new BasicGeoposition();

            position.Latitude = poi.Latitude;
            position.Longitude = poi.Longitude;

            await Map.TrySetViewAsync(new Geopoint(position));
        }
    }
}
