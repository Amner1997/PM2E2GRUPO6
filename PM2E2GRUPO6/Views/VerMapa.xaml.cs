using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Plugin.Media;
using Plugin.Geolocator;
using PM2E2GRUPO6.Models;

namespace PM2E2GRUPO6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerMapa : ContentPage
    {
        Sitios Site = null;
        public VerMapa(double latitud, double longitud, string descripcion, Sitios site)
        {
            Site = site;
            InitializeComponent();

            var locator = CrossGeolocator.Current;
            bool isGpsEnabled = locator.IsGeolocationEnabled;

            if (isGpsEnabled)
            {
                var pin = new Pin
                {
                    Position = new Position(latitud, longitud),
                    Label = "Pin: " + descripcion
                };

                mapa.Pins.Add(pin);
                //lugares 100 metros
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(100)));


            }
            else
            {
                // El GPS no está activo
                DisplayAlert("Error", "El GPS esta Inactivo por favor activelo", "OK");
            }

        }

        public async Task NavigateToBuilding(double latitud, double longitud)
        {
            var location = new Location(latitud, longitud);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
         
            await Xamarin.Essentials.Map.OpenAsync(location, options);
        }

        private async void btnNavegar_Clicked(object sender, EventArgs e)
        {
                var location = new Location(Site.Latitud, Site.Longitud);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Xamarin.Essentials.Map.OpenAsync(location, options);
           
        }
    }
}