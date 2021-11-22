using Android.Content;
using Android.Preferences;
using System;
using System.Threading.Tasks;

namespace APP.Helpers
{
    public class Location
    {
        private Context context = global::Android.App.Application.Context;

        public int idVisita { get; set; }

        public bool serviceRunning { get; set; }

        public Location(int idVisita)
        {
            this.idVisita = idVisita;
            serviceRunning = false;
        }

        public void setRunningStateLocationService(bool isRunning)
        {
            if (isRunning)
            {
                serviceRunning = true;
            }
            else
            {
                serviceRunning = false;
            }
        }

        public bool getRunningStateLocationService()
        {
            bool locationServiceIsRunning;
            if (serviceRunning)
            {
                locationServiceIsRunning = serviceRunning;
            }
            else
            {
                locationServiceIsRunning = false;
            }
            return locationServiceIsRunning;
        }

        public async void Run()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(getRunningStateLocationService());

                await LocationChanges.StartListening(idVisita);

                while (getRunningStateLocationService())
                {
                    await Task.Delay(1000);
                    await LocationChanges.SaveLocation();
                }

                await LocationChanges.StopListening();
            }
            catch (Exception ex)
            {
                //Se lee el id del usuario
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                int idUsuario = prefs.GetInt("idUsuario", 0);
                //si el gps esta apagado llega aqui
                await LocationChanges.StopListening();
                //Guarda en base de datos el error
                await DB.ErrorService(idUsuario, 2, ex.Message);                
            }
        }
    }
}