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
    public class VerIndicadoresActivity : AppCompatActivity
    {
        ImageView volverIndicadores;
        ListView listIndicadores;
        int idLectura;
        string sigatoka;
        ObservableCollection<Indicador> indicador;
        ObservableCollection<String[]> pathsObj;
        String[] paths = { "", "", "" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idLectura = Intent.GetIntExtra("idLectura", 0);
            sigatoka = Intent.GetStringExtra("Sigatoka");

            SetContentView(Resource.Layout.VerIndicadores);

            volverIndicadores = (ImageView)FindViewById(Resource.Id.volverIndicadores);
            volverIndicadores.Click += VolverIndicadores_Click;

            listIndicadores = (ListView)FindViewById(Resource.Id.listIndicadores);

            ConsultaIndicadores();
        }

        private async void ConsultaIndicadores()
        {
            indicador = new ObservableCollection<Indicador>();
            if(sigatoka == "Si")
            {
                await DB.BringIndicadoresPlanta(idLectura, indicador);
            }
            else
            {
                await DB.BringIndicadores(idLectura, indicador);
            }
            listIndicadores.Adapter = new IndicadorAdapter(this, indicador);
        }

        public async void VerFotos(int id, string valor, int indicador)
        {
            if (indicador == 7)
            {
                paths[0] = valor;
                Intent intent = new Intent(this, typeof(FotosActivity));
                intent.PutExtra("paths", paths);
                intent.PutExtra("indicador", indicador);
                StartActivity(intent);
            }
            else
            {
                pathsObj = new ObservableCollection<String[]>();
                await DB.BringFoto(id, pathsObj);
                paths = pathsObj[0];
                Intent intent = new Intent(this, typeof(FotosActivity));
                intent.PutExtra("paths", paths);
                intent.PutExtra("indicador", indicador);
                StartActivity(intent);
            }
        }

        private void VolverIndicadores_Click(object sender, System.EventArgs e)
        {
            Finish();
        }
    }
}