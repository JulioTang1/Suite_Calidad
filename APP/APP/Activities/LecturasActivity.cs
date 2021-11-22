using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using APP.Adapters;
using APP.Helpers;
using System;
using System.Collections.ObjectModel;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LecturasActivity : AppCompatActivity
    {
        ImageView volverLecturas;
        TextView finca, visita, fecha, precipitacion, temperaturaMinima, temperatura, temperaturaMaxima, humedad;
        ListView listLecturas;
        int idVisita, idFinca;
        string nombreFinca, nombreVisita, nombreFecha;
        ObservableCollection<string> indicadores;
        ObservableCollection<Visitas> lecturas;
        ObservableCollection<string> sectorFinca;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idVisita = Intent.GetIntExtra("idVisita", 0);
            idFinca = Intent.GetIntExtra("idFinca", 0);
            nombreFinca = Intent.GetStringExtra("nombreFinca");
            nombreVisita = Intent.GetStringExtra("visita");
            nombreFecha = Intent.GetStringExtra("fecha");

            //Se consulta el sector
            sectorFinca = new ObservableCollection<string>();
            await DB.LoadSector(idFinca, sectorFinca);

            SetContentView(Resource.Layout.Lecturas);

            volverLecturas = (ImageView)FindViewById(Resource.Id.volverLecturas);
            volverLecturas.Click += VolverLecturas_Click;

            finca = (TextView)FindViewById(Resource.Id.finca);
            finca.Text = String.Concat(nombreFinca, ", ", sectorFinca[0]);

            visita = (TextView)FindViewById(Resource.Id.visita);
            visita.Text = nombreVisita;

            fecha = (TextView)FindViewById(Resource.Id.fecha);
            fecha.Text = nombreFecha;

            precipitacion = (TextView)FindViewById(Resource.Id.precipitacion);
            temperaturaMinima = (TextView)FindViewById(Resource.Id.temperaturaMinima);
            temperatura = (TextView)FindViewById(Resource.Id.temperatura);
            temperaturaMaxima = (TextView)FindViewById(Resource.Id.temperaturaMaxima);
            humedad = (TextView)FindViewById(Resource.Id.humedad);
            listLecturas = (ListView)FindViewById(Resource.Id.listLecturas);

            ConsultaIndicadores();
        }

        private async void ConsultaIndicadores()
        {
            indicadores = new ObservableCollection<string>();
            await DB.BringIndicadoresVisita(idVisita, indicadores);

            precipitacion.Text = "Precipitación: " + indicadores[0] + "mm/h";
            temperaturaMinima.Text = "Temperatura Mínima: " + indicadores[1] + "°C";
            temperatura.Text = "Temperatura Actual: " + indicadores[2] + "°C";
            temperaturaMaxima.Text = "Temperatura Máxima: " + indicadores[3] + "°C";
            humedad.Text = "Humedad Relativa: " + indicadores[4] + "%";

            lecturas = new ObservableCollection<Visitas>();
            await DB.CountLecturas(idVisita, lecturas);
            listLecturas.Adapter = new VisitasAdapter(this, lecturas, "LecturasActivity");
        }

        private void VolverLecturas_Click(object sender, System.EventArgs e)
        {
            Finish();
        }

        public void LecturaSelected(int idLectura, string enfermedad)
        {
            if (enfermedad == "Sigatoka - Parcela Fija")
            {
                Intent intent = new Intent(this, typeof(EdadesActivity));
                intent.PutExtra("idLectura", idLectura);
                StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(this, typeof(VerIndicadoresActivity));
                intent.PutExtra("idLectura", idLectura);
                intent.PutExtra("Sigatoka", "No");
                StartActivity(intent);
            }
        }

        public void RutaSelected(int idLectura, string fecha)
        {
            Intent intent = new Intent(this, typeof(MapsActivity));
            intent.PutExtra("idLectura", idLectura);
            intent.PutExtra("nombreFinca", nombreFinca);
            intent.PutExtra("visita", nombreVisita);
            intent.PutExtra("fecha", fecha);
            intent.PutExtra("activity", "LecturasActivity");
            StartActivity(intent);
        }
    }
}