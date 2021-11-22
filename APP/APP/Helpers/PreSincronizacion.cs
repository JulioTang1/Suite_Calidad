using Android.Content;
using Android.Preferences;
using System.Threading.Tasks;

namespace APP.Helpers
{
    public class PreSincronizacion
    {
        private Context context = global::Android.App.Application.Context;
        public bool serviceRunning { get; set; }

        public PreSincronizacion()
        {
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
            System.Diagnostics.Debug.WriteLine(getRunningStateLocationService());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            string intentSincronizacionMaestros = prefs.GetString("intentSincronizacionMaestros", "");

            if (intentSincronizacionMaestros != "")
            {
                await Task.Run(async() => {
                    Sincronizacion sincronizacion = new Sincronizacion();
                    await sincronizacion.SincronizarMaestros();
                });
            }
            else
            {
                await Task.Run(async () => {
                    Sincronizacion sincronizacion = new Sincronizacion();
                    await sincronizacion.Sincronizar();
                });
            }

            while (getRunningStateLocationService())
            {
                await Task.Delay(1000);                
            };
        }
    }
}