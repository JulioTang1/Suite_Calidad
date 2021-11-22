using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using APP.Adapters;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class RecorridoActivity : AppCompatActivity
    {
        RelativeLayout fecha, fechaFin;
        TextView fechaText, fechaFinText, textFinca;
        DateTime FechaActual, FechaFinRango;
        int idFinca;
        string nombreFinca;
        string opcion;
        ObservableCollection<Visitas> visitas;
        ListView listVisitas;
        ImageView volverRecorridos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idFinca = Intent.GetIntExtra("idFinca", 0);
            nombreFinca = Intent.GetStringExtra("nombreFinca");
            opcion = Intent.GetStringExtra("opcion");

            SetContentView(Resource.Layout.Recorridos);

            textFinca = (TextView)FindViewById(Resource.Id.textFinca);
            textFinca.Text = nombreFinca;

            FechaActual = DateTime.Now.AddDays(-7);
            fechaText = (TextView)FindViewById(Resource.Id.fechaText);
            fechaText.Text = FechaActual.ToString("dd/MM/yyyy");

            fecha = (RelativeLayout)FindViewById(Resource.Id.fecha);
            fecha.Click += Fecha_Click;

            FechaFinRango = DateTime.Now;
            fechaFinText = (TextView)FindViewById(Resource.Id.fechaFinText);
            fechaFinText.Text = FechaFinRango.ToString("dd/MM/yyyy");

            fechaFin = (RelativeLayout)FindViewById(Resource.Id.fechaFin);
            fechaFin.Click += FechaFin_Click; ;

            listVisitas = (ListView)FindViewById(Resource.Id.listVisitas);
            ChangeDate();

            volverRecorridos = (ImageView)FindViewById(Resource.Id.volverRecorridos);
            volverRecorridos.Click += VolverRecorridos_Click;
        }

        private void VolverRecorridos_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Fecha_Click(object sender, System.EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time) { fechaText.Text = time.ToString("dd/MM/yyyy"); }, DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")), new DateTime(1970, 1, 1), DateTime.Parse(fechaFinText.Text, CultureInfo.CreateSpecificCulture("de-DE"))); 
            frag.Show(FragmentManager, DatePickerFragment.TAG);

            frag.ChangeDate += Frag_ChangeDate;
        }

        private void FechaFin_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time) { fechaFinText.Text = time.ToString("dd/MM/yyyy"); }, DateTime.Parse(fechaFinText.Text, CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")), DateTime.Now.Date);
            frag.Show(FragmentManager, DatePickerFragment.TAG);

            frag.ChangeDate += Frag_ChangeDate;
        }

        private  void Frag_ChangeDate(object sender, EventArgs e)
        {
            ChangeDate();
        }

        private async void ChangeDate()
        {
            if (opcion == "mostrar")
            {
                visitas = new ObservableCollection<Visitas>();
                await DB.CountRecorridos(idFinca, DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")).ToString("yyyy-MM-dd"), DateTime.Parse(fechaFinText.Text, CultureInfo.CreateSpecificCulture("de-DE")).ToString("yyyy-MM-dd"), visitas);
                listVisitas.Adapter = new VisitasAdapter(this, visitas, "RecorridoActivity", 0);
            }
            else if (opcion == "consultarBioseguridad")
            {
                visitas = new ObservableCollection<Visitas>();
                await DB.CountBioseguridad(idFinca, DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")).ToString("yyyy-MM-dd"), DateTime.Parse(fechaFinText.Text, CultureInfo.CreateSpecificCulture("de-DE")).ToString("yyyy-MM-dd"), visitas);
                listVisitas.Adapter = new VisitasAdapter(this, visitas, "RecorridoActivity", 1);
            }
            else { }
        }

        public void VisitaSelected(int idVisita, string visita, string fecha)
        {
            DateTime fechaVisita = DateTime.Parse(fecha);

            Intent intent = new Intent(this, typeof(LecturasActivity));
            intent.PutExtra("idVisita", idVisita);
            intent.PutExtra("idFinca", idFinca);
            intent.PutExtra("nombreFinca", nombreFinca);
            intent.PutExtra("visita", visita);
            intent.PutExtra("fecha", fechaVisita.ToString("dd/MM/yyyy"));
            StartActivity(intent);
        }

        public void ChequeoSelected(int idVisita)
        {
            Intent intent = new Intent(this, typeof(BioseguridadActivity));
            intent.PutExtra("opcion", "ver");
            intent.PutExtra("idVisita", idVisita);
            StartActivity(intent);
        }

        public void RutaSelected(int idVisita, string visita, string fecha)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            int idVisita_procesamiento = prefs.GetInt("idVisita_procesamiento", 0);
            if (idVisita_procesamiento == idVisita)
            {
                ToastFragment.ShowMakeText(this, "El recorrido se está procesando, por favor inténtelo de nuevo en unos minutos");
            }
            else
            {
                Intent intent = new Intent(this, typeof(MapsActivity));
                intent.PutExtra("idVisita", idVisita);
                intent.PutExtra("nombreFinca", nombreFinca);
                intent.PutExtra("visita", visita);
                intent.PutExtra("fecha", fecha);
                intent.PutExtra("activity", "RecorridoActivity");
                StartActivity(intent);
            }
        }
    }
}