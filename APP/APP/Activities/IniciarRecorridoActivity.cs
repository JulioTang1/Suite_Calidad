using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class IniciarRecorridoActivity : AppCompatActivity
    {
        ImageView volverIniciarRecorrido;
        TextView textFinca, textoPrecipitacion;
        Android.Widget.RelativeLayout precipitacion, temperaturaMinima, temperatura, temperaturaMaxima, humedad;
        Android.Widget.Button recorridoFinca;
        TecladoFragment tecladofragment;
        public TextView indicadorPrecipitacion, indicadorTemperaturaMinima, indicadorTemperatura, indicadorTemperaturaMaxima, indicadorHumedad;
        int idFinca;
        string nombreFinca;
        ObservableCollection<int> idVisita;
        ObservableCollection<string> sectorFinca;
        ObservableCollection<string> precipitacionObj;

        //Bandera para saber que input invoco al teclado
        public string indicador = "";        

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.IniciarRecorrido);

            idFinca = Intent.GetIntExtra("idFinca", 0);
            nombreFinca = Intent.GetStringExtra("nombreFinca");

            //Se consulta el sector
            sectorFinca = new ObservableCollection<string>();
            await DB.LoadSector(idFinca, sectorFinca);

            //Se consulta la precipitacion
            precipitacionObj = new ObservableCollection<string>();
            await DB.LoadPrecipitacion(idFinca, precipitacionObj); //Con fecha (actual y id_finca)

            textoPrecipitacion = (TextView)FindViewById(Resource.Id.textoPrecipitacion);
            textoPrecipitacion.Text = String.Concat(DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"),  " ",
                CultureInfo.InvariantCulture.TextInfo.ToTitleCase(DateTime.Now.AddDays(-1).ToString("dddd", new CultureInfo("es-ES")).Substring(0, 3)),
                " Precipitación");

            volverIniciarRecorrido = (ImageView)FindViewById(Resource.Id.volverIniciarRecorrido);
            precipitacion = (Android.Widget.RelativeLayout)FindViewById(Resource.Id.precipitacion);
            temperaturaMinima = (Android.Widget.RelativeLayout)FindViewById(Resource.Id.temperaturaMinima);
            temperatura = (Android.Widget.RelativeLayout)FindViewById(Resource.Id.temperatura);
            temperaturaMaxima = (Android.Widget.RelativeLayout)FindViewById(Resource.Id.temperaturaMaxima);
            humedad = (Android.Widget.RelativeLayout)FindViewById(Resource.Id.humedad);
            recorridoFinca = (Android.Widget.Button)FindViewById(Resource.Id.recorridoFinca);
            indicadorPrecipitacion = (TextView)FindViewById(Resource.Id.indicadorPrecipitacion);
            indicadorPrecipitacion.Text = precipitacionObj.Count > 0 ? float.Parse(precipitacionObj[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) : "0";
            indicadorTemperaturaMinima = (TextView)FindViewById(Resource.Id.indicadorTemperaturaMinima);
            indicadorTemperatura = (TextView)FindViewById(Resource.Id.indicadorTemperatura);
            indicadorTemperaturaMaxima = (TextView)FindViewById(Resource.Id.indicadorTemperaturaMaxima);
            indicadorHumedad = (TextView)FindViewById(Resource.Id.indicadorHumedad);
            textFinca = (TextView)FindViewById(Resource.Id.textFinca);
            textFinca.Text = nombreFinca + " Sector " + sectorFinca[0];

            volverIniciarRecorrido.Click += VolverIniciarRecorrido_Click;
            precipitacion.Click += Precipitacion_Click;
            temperaturaMinima.Click += TemperaturaMinima_Click;
            temperatura.Click += Temperatura_Click;
            temperaturaMaxima.Click += TemperaturaMaxima_Click;
            humedad.Click += Humedad_Click;
            recorridoFinca.Click += RecorridoFinca_Click;
        }

        //VERIFICA QUE EL GPS ESTE ENCENDIDO
        public static bool isGPSProvider(Context context)
        {
            LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
            return lm.IsProviderEnabled(LocationManager.GpsProvider);
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

        private async void RecorridoFinca_Click(object sender, System.EventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            string intentFilter = prefs.GetString("intentFilter", "");

            //GUARDAR SOLO PRECIPITACION Y TEMPERATURA ACTUAL
            if (indicadorTemperatura.Text == "0")
            {
                ToastFragment.ShowMakeText(this, "El campo temperatura actual debe ser diferente de 0");
            }
            else if (!isGPSProvider(this))
            {
                ToastFragment.ShowMakeText(this, "Es necesario que active el GPS para poder continuar");
            }
            else if (isMyServiceRunning(typeof(FilterService)) && intentFilter != "")
            {
                ToastFragment.ShowMakeText(this, "No puede acceder a un nuevo recorrido debido a que se esta depurando el recorrido anterior");
            }
            else
            {
                //Se crea registro de visita en base de datos local
                idVisita = new ObservableCollection<int>();
                await DB.SaveVisit(
                    idFinca,
                    float.Parse(indicadorPrecipitacion.Text, CultureInfo.InvariantCulture),
                    float.Parse(indicadorTemperaturaMinima.Text, CultureInfo.InvariantCulture),
                    float.Parse(indicadorTemperatura.Text, CultureInfo.InvariantCulture),
                    float.Parse(indicadorTemperaturaMaxima.Text, CultureInfo.InvariantCulture),
                    float.Parse(indicadorHumedad.Text, CultureInfo.InvariantCulture),
                    idVisita);

                Intent intent = new Intent(this, typeof(EnfermedadesActivity));
                intent.PutExtra("idVisita", idVisita[0]);
                StartActivity(intent);

                Finish();
            }
        }

        private void VolverIniciarRecorrido_Click(object sender, System.EventArgs e)
        {
            Finish();
        }

        private void Precipitacion_Click(object sender, System.EventArgs e)
        {
            //Bandera indicando modificacion de input precipitacion
            indicador = "precipitacion";
            DesplegarTeclado("Precipitación");            
        }

        private void TemperaturaMinima_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura minima
            indicador = "temperaturaMinima";
            DesplegarTeclado("Temperatura Mínima");
        }

        private void Temperatura_Click(object sender, System.EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura
            indicador = "temperatura";
            DesplegarTeclado("Temperatura Actual");
        }

        private void TemperaturaMaxima_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "temperaturaMaxima";
            DesplegarTeclado("Temperatura Máxima");
        }

        private void Humedad_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input humedad
            indicador = "humedad";
            DesplegarTeclado("Humedad Relativa");
        }

        public void DesplegarTeclado(string nombre)
        {
            tecladofragment = new TecladoFragment("IniciarRecorridoActivity", nombre);
            var trans = SupportFragmentManager.BeginTransaction();
            tecladofragment.Show(trans, "Teclado");
        }

        public void Tecladofragment_OnBackTeclado()
        {
            //Cierra fragmento del teclado
            tecladofragment.Dismiss();
        }

        public void Tecladofragment_OnValueRegistered(float valor)
        {
            //Asigna valor del teclado al input de la variable correspondiente
            if (indicador == "precipitacion")
            {
                indicadorPrecipitacion.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "temperaturaMinima")
            {
                indicadorTemperaturaMinima.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "temperatura")
            {
                indicadorTemperatura.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "temperaturaMaxima")
            {
                indicadorTemperaturaMaxima.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "humedad")
            {
                indicadorHumedad.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else { }

            //Cierra fragmento del teclado
            tecladofragment.Dismiss();
        }
    }
}