using Android.App;
using Android.Content;
using Android.Preferences;

namespace APP.Helpers
{
    [BroadcastReceiver]
    class ReceptorAlarma : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Se llama metodo para iniciar servicio de sinconizar
            Sincronizar(context);

        }

        private void Sincronizar(Context context)
        {
            //Se guarda infromacion en intent de sincronizacion para evitar enviar varios services seguidos
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            int idUsuario = prefs.GetInt("idUsuario", 0);
            string intentSincronizacion = prefs.GetString("intentSincronizacion", "");

            if( !isMyServiceRunning(typeof(SincronizacionService), context) )
            {
                intentSincronizacion = "";
            }

            if (idUsuario != 0 && intentSincronizacion == "")
            {
                editor.PutString("intentSincronizacion", "inicio");
                editor.Commit();

                //LLAMA AL SERVICIO DE SINCRONIZACION
                Intent sincronizacion = new Intent(context, typeof(SincronizacionService));

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    Android.App.Application.Context.StartForegroundService(sincronizacion);
                }
                else
                {
                    Android.App.Application.Context.StartService(sincronizacion);
                }

                //Se guarda el ultimo intent de sincronizacion
                string stringIntnt = sincronizacion.ToURI();
                editor.PutString("intentSincronizacion", stringIntnt);
                editor.Commit();
            }
        }

        private bool isMyServiceRunning(System.Type cls, Context context)
        {
            ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);

            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsServiceRunning(System.Type ClassTypeof)
        {
            ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ShortClassName == ClassTypeof.ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
}