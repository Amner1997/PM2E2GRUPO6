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

namespace PM2E2GRUPO6.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerMapa : ContentPage
    {
        public VerMapa(double latitud, double longitud, string descripcion)
        {
            InitializeComponent();

            var locator = CrossGeolocator.Current;
            bool isGpsEnabled = locator.IsGeolocationEnabled;

            if (isGpsEnabled)
            {
                // El GPS está activo
                var pin = new Pin
                {
                    Position = new Position(latitud, longitud),
                    Label = "Pin: " + descripcion                   
                };

           
                NavigateToBuilding(latitud, longitud);
                //mapa.Pins.Add(pin);

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
    }
}