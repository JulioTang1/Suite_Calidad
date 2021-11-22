using Android.App;
using Android.Content;
using Android.Preferences;
using AppDemo.LocalLogic.Componentes;
using Plugin.Geolocator;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace APP.Helpers
{
    public static class LocationChanges
    {
        private static string _readLatitude = "0";
        private static string _readLongitude = "0";
        private static Context context = global::Android.App.Application.Context;
        public static int idVisita { get; set; }

        public static async Task StartListening(int _idVisita)
        {
            idVisita = _idVisita;

            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 0.1);
        }

        private static void Current_PositionError(object sender, Plugin.Geolocator.Abstractions.PositionErrorEventArgs e)
        {

        }

        public static async Task SaveLocation()
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
            var userLocation = await Geolocation.GetLocationAsync(request);

            if (userLocation != null)
            {
                _readLatitude = userLocation.Latitude.ToString(CultureInfo.InvariantCulture);
                _readLongitude = userLocation.Longitude.ToString(CultureInfo.InvariantCulture);
                await DB.SaveLocation(idVisita, _readLatitude, _readLongitude);
            }
            else
            {
                //Se enciende bandera de GPS solo cuando la app esta en uso
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutInt("GPS", 1);
                editor.Commit();
            }
        }

        public static async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();
        }
    }
}