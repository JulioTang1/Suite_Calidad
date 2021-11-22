using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Xamarin.Forms;

namespace APP.Helpers
{
    [Service]
    public class SincronizacionService : Service
    {
        private Context context = global::Android.App.Application.Context;
        private PreSincronizacion preSincronizacion;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 20000;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            bool isAlreadyStart;

            try
            {
                Notification notif = DependencyService.Get<INotification>().ReturnNotif("9002", "Sincronización");
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

                preSincronizacion = new PreSincronizacion();
                isAlreadyStart = preSincronizacion.getRunningStateLocationService();
                if (isAlreadyStart)
                {
                }
                else
                {
                    preSincronizacion.setRunningStateLocationService(true);
                    preSincronizacion.Run();
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
            preSincronizacion.setRunningStateLocationService(false);
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
                await DB.ErrorService(idUsuario, 1, ex.Message);

                //Baja bandera de service de sincronizacion
                editor.PutString("intentSincronizacion", "");
                editor.Commit();
            }
            catch (System.Exception e)
            {
                //Guarda en base de datos el error
                int idUsuario = prefs.GetInt("idUsuario", 0);
                editor.PutString("intentSincronizacion", "");
                await DB.ErrorService(idUsuario, 1, e.Message);
            }
        }
    }
}