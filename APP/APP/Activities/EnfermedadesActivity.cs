using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using APP.Adapters;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class EnfermedadesActivity : AppCompatActivity
    {
        ProgressFragment progressDialogue;
        Intent serviceIntent;
        bool InitGPSFlag = false;

        ImageView volverEnfermedades;
        int idVisita;
        ObservableCollection<PuntoLectura> puntoLectura;
        Android.Widget.ListView listPuntos;
        public Android.Widget.Button sigatoka1, sigatoka2, sigatoka3, sigatoka4, sigatoka5, sigatoka6, sigatoka7, sigatoka8, sigatoka9, sigatoka10, sigatoka11, sigatoka12, sigatoka13, sigatoka14, sigatoka15;
        public Android.Widget.Button enfermedadesVasculares, condicionesCulturales;
        ObservableCollection<int> idCaptura;
        int[] idCapturaSigatoka = new int[15];
        ObservableCollection<int> existPoint;
        Android.Widget.Button finalizarRecorrido;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            idVisita = Intent.GetIntExtra("idVisita", 0);

            SetContentView(Resource.Layout.Enfermedades);

            volverEnfermedades = (ImageView)FindViewById(Resource.Id.volverEnfermedades);
            volverEnfermedades.Click += VolverEnfermedades_Click;

            sigatoka1 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka1);
            sigatoka1.Click += Sigatoka1_Click;
            sigatoka2 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka2);
            sigatoka2.Click += Sigatoka2_Click;
            sigatoka3 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka3);
            sigatoka3.Click += Sigatoka3_Click;
            sigatoka4 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka4);
            sigatoka4.Click += Sigatoka4_Click;
            sigatoka5 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka5);
            sigatoka5.Click += Sigatoka5_Click;
            sigatoka6 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka6);
            sigatoka6.Click += Sigatoka6_Click;
            sigatoka7 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka7);
            sigatoka7.Click += Sigatoka7_Click;
            sigatoka8 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka8);
            sigatoka8.Click += Sigatoka8_Click;
            sigatoka9 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka9);
            sigatoka9.Click += Sigatoka9_Click;
            sigatoka10 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka10);
            sigatoka10.Click += Sigatoka10_Click;
            sigatoka11 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka11);
            sigatoka11.Click += Sigatoka11_Click;
            sigatoka12 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka12);
            sigatoka12.Click += Sigatoka12_Click;
            sigatoka13 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka13);
            sigatoka13.Click += Sigatoka13_Click;
            sigatoka14 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka14);
            sigatoka14.Click += Sigatoka14_Click;
            sigatoka15 = (Android.Widget.Button)FindViewById(Resource.Id.sigatoka15);
            sigatoka15.Click += Sigatoka15_Click;

            enfermedadesVasculares = (Android.Widget.Button)FindViewById(Resource.Id.enfermedadesVasculares);
            enfermedadesVasculares.Click += EnfermedadesVasculares_Click;
            condicionesCulturales = (Android.Widget.Button)FindViewById(Resource.Id.condicionesCulturales);
            condicionesCulturales.Click += CondicionesCulturales_Click;

            finalizarRecorrido = (Android.Widget.Button)FindViewById(Resource.Id.finalizarRecorrido);
            finalizarRecorrido.Click += FinalizarRecorrido_Click;

            listPuntos = (Android.Widget.ListView)FindViewById(Resource.Id.listPuntos);

            //Se ubica al usuario en la vista en la que cerro la app o inicia un nuevo recorrido de forma normal
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string Actividad = prefs.GetString("Actividad", "");
            if (Actividad == "EnfermedadesActivity")
            {
                //Se obtiene el arrglo de sigatokas
                string idCapturaSigatokaString = prefs.GetString("idCapturaSigatoka", "");
                JArray idCapturaSigatokaJ = JArray.Parse(idCapturaSigatokaString);
                for(var i = 0; i < 15; i++)
                {
                    idCapturaSigatoka[i] = int.Parse(idCapturaSigatokaJ[i].ToString());
                }
            }
            else if (Actividad == "SemanasActivity")
            {
                //Se obtiene el arrglo de sigatokas
                string idCapturaSigatokaString = prefs.GetString("idCapturaSigatoka", "");
                JArray idCapturaSigatokaJ = JArray.Parse(idCapturaSigatokaString);
                for (var i = 0; i < 15; i++)
                {
                    idCapturaSigatoka[i] = int.Parse(idCapturaSigatokaJ[i].ToString());
                }

                Intent intent = new Intent(this, typeof(SemanasActivity));
                int idCapt = prefs.GetInt("idCaptura", 0);
                intent.PutExtra("idCaptura", idCapt);
                StartActivity(intent);
            }
            else
            {
                InitGPSFlag = true;
                //Inicia servicio de GPS
                InitGPS();

                //Se guardan los id de sigatoka
                for (int i = 0; i < 15; i++)
                {
                    idCaptura = new ObservableCollection<int>();
                    await DB.SavePunto(idVisita, 1/*SIGATOKA*/, idCaptura, i);
                    idCapturaSigatoka[i] = idCaptura[0];
                }

                //Se enciende bandera para indicar que se esta capturando un recorrido
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutInt("idVisita", idVisita);
                editor.PutString("Actividad", "EnfermedadesActivity");
                editor.PutString("idCapturaSigatoka", JsonConvert.SerializeObject(idCapturaSigatoka, Formatting.None).ToString());
                editor.Commit();
            }
        }

        private bool isMyServiceRunning(System.Type cls)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);

            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }

        protected async override void OnResume()
        {
            base.OnResume();

            puntoLectura = new ObservableCollection<PuntoLectura>();
            await DB.CountEnfermedades(idVisita, puntoLectura);
            listPuntos.Adapter = new PuntosAdapter(this, puntoLectura);

            if ( !isMyServiceRunning(typeof(AndroidLocationService)) && !InitGPSFlag)
            {
                serviceIntent = new Intent(this, typeof(AndroidLocationService));
                serviceIntent.PutExtra("idVisita", idVisita);
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    StartForegroundService(serviceIntent);
                }
                else
                {
                    StartService(serviceIntent);
                }
            }
        }

        private async void InitGPS()
        {
            //Primer punto preciso del GPS
            ShowProgressDialogue("Iniciando GPS, por favor permanezca en esta ubicación ...");
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(30));
            var userLocation = await Geolocation.GetLocationAsync(request);
            string readLatitude = userLocation.Latitude.ToString(CultureInfo.InvariantCulture);
            string readLongitude = userLocation.Longitude.ToString(CultureInfo.InvariantCulture);
            await DB.SaveLocation(idVisita, readLatitude, readLongitude);
            CloseProgressDialogue();

            /*Servicio de GPS*/
            serviceIntent = new Intent(this, typeof(AndroidLocationService));
            serviceIntent.PutExtra("idVisita", idVisita);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                StartForegroundService(serviceIntent);
            }
            else
            {
                StartService(serviceIntent);
            }

            //Se guarda el ultimo intent del GPS
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            string stringIntnt = serviceIntent.ToURI();
            editor.PutString("intentGPS", stringIntnt);
            editor.Commit();
        }

        private async void Sigatoka1_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[0]);
            StartActivity(intent);
        }

        private async void Sigatoka2_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[1]);
            StartActivity(intent);
        }

        private async void Sigatoka3_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[2]);
            StartActivity(intent);
        }

        private async void Sigatoka4_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[3]);
            StartActivity(intent);
        }

        private async void Sigatoka5_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[4]);
            StartActivity(intent);
        }

        private async void Sigatoka6_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[5]);
            StartActivity(intent);
        }
        private async void Sigatoka7_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[6]);
            StartActivity(intent);
        }

        private async void Sigatoka8_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[7]);
            StartActivity(intent);
        }

        private async void Sigatoka9_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[8]);
            StartActivity(intent);
        }

        private async void Sigatoka10_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[9]);
            StartActivity(intent);
        }

        private async void Sigatoka11_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[10]);
            StartActivity(intent);
        }

        private async void Sigatoka12_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[11]);
            StartActivity(intent);
        }
        private async void Sigatoka13_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[12]);
            StartActivity(intent);
        }

        private async void Sigatoka14_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[13]);
            StartActivity(intent);
        }

        private async void Sigatoka15_Click(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SemanasActivity));
            intent.PutExtra("idCaptura", idCapturaSigatoka[14]);
            StartActivity(intent);
        }

        private async void EnfermedadesVasculares_Click(object sender, System.EventArgs e)
        {
            idCaptura = new ObservableCollection<int>();
            await DB.SavePunto(idVisita, 2/*VASCULARES*/, idCaptura);
            Intent intent = new Intent(this, typeof(IndicadoresActivity));
            intent.PutExtra("idCaptura", idCaptura[0]);
            intent.PutExtra("idEdad", 7);
            intent.PutExtra("idTipo", 2/*VASCULARES*/);
            StartActivity(intent);
        }

        private async void CondicionesCulturales_Click(object sender, System.EventArgs e)
        {
            idCaptura = new ObservableCollection<int>();
            await DB.SavePunto(idVisita, 3/*CULTURALES*/, idCaptura);
            Intent intent = new Intent(this, typeof(IndicadoresActivity));
            intent.PutExtra("idCaptura", idCaptura[0]);
            intent.PutExtra("idEdad", 7);
            intent.PutExtra("idTipo", 3/*CULTURALES*/);
            StartActivity(intent);
        }

        public override void OnBackPressed()
        {
            VolverEnfermedades();
        }

        private void VolverEnfermedades_Click(object sender, System.EventArgs e)
        {
            VolverEnfermedades();
        }

        private async void VolverEnfermedades()
        {
            existPoint = new ObservableCollection<int>();
            await DB.ExistPoint(idVisita, existPoint);

            if (existPoint[0] == 1)
            {
                Android.Support.V7.App.AlertDialog.Builder cancelar = new Android.Support.V7.App.AlertDialog.Builder(this);
                cancelar.SetMessage("¿Desea guardar los datos antes de volver atrás?");
                cancelar.SetTitle("Volver atrás");

                cancelar.SetPositiveButton("Si", (alert, args) =>
                {
                    Guardar();
                });

                cancelar.SetNegativeButton("No", (alert, args) =>
                {
                    NoGuardar();
                });

                cancelar.Show();
            }
            else
            {
                NoGuardar();
            }
        }

        private async void Guardar()
        {
            if (isGPSProvider(this))
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                if (serviceIntent == null)
                {
                    string intentGPS = prefs.GetString("intentGPS", "");
                    serviceIntent = Intent.GetIntent(intentGPS);
                }
                editor.PutString("intentGPS", "");
                editor.Commit();

                try
                {
                    StopService(serviceIntent);
                }
                catch
                {

                }

                ShowProgressDialogue("Finalizando GPS, por favor permanezca en esta ubicación ...");
                while (CrossGeolocator.Current.IsListening)
                {
                    await Task.Delay(1000);
                }
                CloseProgressDialogue();

                //Se procesa el recorrido
                //Se enciende bandera para indicar que se esta procesando el ultimo recorrido capturado
                editor.PutInt("idVisita_procesamiento", idVisita);
                editor.Commit();

                /*Servicio de Filtrado*/
                serviceIntent = new Intent(this, typeof(FilterService));
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    StartForegroundService(serviceIntent);
                }
                else
                {
                    StartService(serviceIntent);
                }

                //Se guarda el ultimo intent del filter
                string stringIntnt = serviceIntent.ToURI();
                editor.PutString("intentFilter", stringIntnt);

                //Se apaga bandera para indicar que se ha finalizado un recorrido [No se necesita el id usuario solo la actividad y la visita]
                editor.PutInt("idVisita", 0);
                editor.PutString("Actividad", "");
                editor.Commit();

                Finish();
            }
            else
            {
                ToastFragment.ShowMakeText(this, "Es necesario que active el GPS para poder continuar");
            }
        }

        private async void NoGuardar()
        {
            if (isGPSProvider(this))
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                if (serviceIntent == null)
                {
                    string intentGPS = prefs.GetString("intentGPS", "");
                    serviceIntent = Intent.GetIntent(intentGPS);
                }
                editor.PutString("intentGPS", "");
                editor.Commit();

                try
                {
                    StopService(serviceIntent);
                }
                catch
                {

                }

                ShowProgressDialogue("Finalizando GPS, por favor espere ...");
                while (CrossGeolocator.Current.IsListening)
                {
                    await Task.Delay(1000);
                }
                CloseProgressDialogue();

                await DB.DeleteVisit(idVisita);

                //Se apaga bandera para indicar que se ha finalizado un recorrido 
                editor.PutInt("idVisita", 0);
                editor.PutString("Actividad", "");
                editor.Commit();

                Finish();
            }
            else
            {
                ToastFragment.ShowMakeText(this, "Es necesario que active el GPS para poder continuar");
            }
        }

        private async void FinalizarRecorrido_Click(object sender, System.EventArgs e)
        {
            existPoint = new ObservableCollection<int>();
            await DB.ExistPoint(idVisita, existPoint);

            if (existPoint[0] == 1)
            {
                Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
                terminar.SetMessage("¿Está seguro que desea finalizar el recorrido?");
                terminar.SetTitle("Finalizar recorrido");

                terminar.SetPositiveButton("Si", (alert, args) =>
                {
                    Guardar();
                });

                terminar.SetNegativeButton("No", (alert, args) =>
                {

                });

                terminar.Show();
            }
            else
            {
                NoGuardar();
            }
        }

        public void ShowProgressDialogue(string status)
        {

            progressDialogue = new ProgressFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "Progress");
        }

        public void CloseProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.DismissAllowingStateLoss();
                progressDialogue = null;
            }
        }

        public void EliminarPunto(int id)
        {
            Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
            terminar.SetMessage("¿Está seguro que desea eliminar este punto captura?");
            terminar.SetTitle("Finalizar recorrido");

            terminar.SetPositiveButton("Si", async (alert, args) =>
            {
                await DB.DeleteEnfermedad(id);

                puntoLectura = new ObservableCollection<PuntoLectura>();
                await DB.CountEnfermedades(idVisita, puntoLectura);
                listPuntos.Adapter = new PuntosAdapter(this, puntoLectura);
            });

            terminar.SetNegativeButton("No", (alert, args) =>
            {

            });

            terminar.Show();
        }

        //VERIFICA QUE EL GPS ESTE ENCENDIDO
        public static bool isGPSProvider(Context context)
        {
            LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
            return lm.IsProviderEnabled(LocationManager.GpsProvider);
        }
    }
}