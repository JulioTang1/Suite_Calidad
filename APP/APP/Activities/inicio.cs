using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Icu.Util;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using APP.Helpers;
using System;
using System.Collections.ObjectModel;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/Mytheme.Splash", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class inicio : AppCompatActivity
    {
        ObservableCollection<int> maestros;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            await DB.CreateDatabase();

            //Se hace una sincronizaion de maestros cuando la tabla de usuarios este vacia
            //Se guarda infromacion en intent de sincronizacion para evitar enviar varios services seguidos

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            maestros = new ObservableCollection<int>();
            await DB.SincronizacionMaestros(maestros);
            if (maestros[0] == 0)
            {
                string intentSincronizacionMaestros = prefs.GetString("intentSincronizacionMaestros", "");
                if (intentSincronizacionMaestros == "")
                {
                    editor.PutString("intentSincronizacionMaestros", "inicio");
                    editor.Commit();

                    //LLAMA AL SERVICIO DE SINCRONIZACION
                    Intent sincronizacion = new Intent(this, typeof(SincronizacionService));

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
                    editor.PutString("intentSincronizacionMaestros", stringIntnt);
                    editor.Commit();
                }
            }

            //Alarma para realizar sincronizaciones periodicas
            var intent = new Intent(this, typeof(ReceptorAlarma));
            var source = PendingIntent.GetBroadcast(this, 0, intent, 0);
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.SetRepeating(AlarmType.RtcWakeup, 0, 1000 * 60 * 60 * 1, source);

            StartActivity(typeof(LoginActivity));
            Finish();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var intent = new Intent(this, typeof(ReceptorAlarma));
            var source = PendingIntent.GetBroadcast(this, 0, intent, 0);
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.SetRepeating(AlarmType.RtcWakeup, 0, 1000 * 60 * 60 * 1, source);
        }
    }
}