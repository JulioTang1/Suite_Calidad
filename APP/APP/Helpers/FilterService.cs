using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Xamarin.Forms;

namespace APP.Helpers
{
    [Service]
    public class FilterService : Service
    {
        private Context context = global::Android.App.Application.Context;
        private PreFilter preFilter;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 30000;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            bool isAlreadyStart;

            try
            {
                Notification notif = DependencyService.Get<INotification>().ReturnNotif("9003", "Depuración de recorrido");
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

                preFilter = new PreFilter();
                isAlreadyStart = preFilter.getRunningStateLocationService();
                if (isAlreadyStart)
                {
                }
                else
                {
                    preFilter.setRunningStateLocationService(true);
                    preFilter.Run();
                }
            }
            catch (OperationCanceledException ex)
            {
                GuardarError(ex);
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            preFilter.setRunningStateLocationService(false);
            base.OnDestroy();
        }

        private async void GuardarError(OperationCanceledException ex)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            try
            {
                //Guarda en base de datos el error
                int idUsuario = prefs.GetInt("idUsuario", 0);
                await DB.ErrorService(idUsuario, 3, ex.Message);
            }
            catch (System.Exception e)
            {
                //Guarda en base de datos el error
                int idUsuario = prefs.GetInt("idUsuario", 0);
                await DB.ErrorService(idUsuario, 1, e.Message);
            }
        }
    }
}