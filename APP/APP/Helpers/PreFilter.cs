using Android.Content;
using Android.Preferences;
using System.Threading.Tasks;

namespace APP.Helpers
{
    public class PreFilter
    {
        private Context context = global::Android.App.Application.Context;
        public bool serviceRunning { get; set; }

        public PreFilter()
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

            await Task.Run(async () => {
                Filter filter = new Filter();
                await filter.Filtrar();
            });

            while (getRunningStateLocationService())
            {
                await Task.Delay(1000);
            };
        }
    }
}