using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System.Collections.ObjectModel;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SemanasActivity : Activity
    {
        Button edad1, edad2, edad3, edad6;
        Button guardarPunto;
        ImageView volverEnfermedades;
        int idCaptura;
        int diez, siete, cero, fija;
        ObservableCollection<string> plantasEdad;
        ObservableCollection<int> existPlanta;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idCaptura = Intent.GetIntExtra("idCaptura", 0);

            SetContentView(Resource.Layout.Semanas);

            edad1 = (Button)FindViewById(Resource.Id.edad1);
            edad1.Click += Edad1_Click;
            edad2 = (Button)FindViewById(Resource.Id.edad2);
            edad2.Click += Edad2_Click;
            edad3 = (Button)FindViewById(Resource.Id.edad3);
            edad3.Click += Edad3_Click;
            edad6 = (Button)FindViewById(Resource.Id.edad6);
            edad6.Click += Edad6_Click;

            guardarPunto = (Button)FindViewById(Resource.Id.guardarPunto);
            guardarPunto.Click += GuardarPunto_Click;

            volverEnfermedades = (ImageView)FindViewById(Resource.Id.volverEnfermedades);
            volverEnfermedades.Click += VolverEnfermedades_Click;

            //Se enciende bandera para indicar que se esta capturando un recorrido
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("Actividad", "SemanasActivity");
            editor.PutInt("idCaptura", idCaptura);
            editor.Commit();
        }

        protected async override void OnResume()
        {
            base.OnResume();

            plantasEdad = new ObservableCollection<string>();
            await DB.CountEdades(idCaptura, plantasEdad);

            edad1.Text = "10 Semanas: " + (int.Parse(plantasEdad[0]));
            edad2.Text = "7 Semanas: " + (int.Parse(plantasEdad[1]));
            edad3.Text = "0 Semanas: " + (int.Parse(plantasEdad[2]));
            edad6.Text = "Parcela Fija: " + (int.Parse(plantasEdad[3]));

            diez = int.Parse(plantasEdad[0]);
            siete = int.Parse(plantasEdad[1]);
            cero = int.Parse(plantasEdad[2]);
            fija = int.Parse(plantasEdad[3]);
        }

        private void Edad1_Click(object sender, System.EventArgs e)
        {
            if (diez == 1)
            {
                ToastFragment.ShowMakeText(this, "No se puede registrar mas de 1 planta.");
            }
            else
            {
                Intent intent = new Intent(this, typeof(IndicadoresActivity));
                intent.PutExtra("idCaptura", idCaptura);
                intent.PutExtra("idEdad", 1);
                intent.PutExtra("idTipo", 1/*SIGATOKA*/);
                StartActivity(intent);
            }
        }

        private void Edad2_Click(object sender, System.EventArgs e)
        {
            if (siete == 1)
            {
                ToastFragment.ShowMakeText(this, "No se puede registrar mas de 1 planta.");
            }
            else
            {
                Intent intent = new Intent(this, typeof(IndicadoresActivity));
                intent.PutExtra("idCaptura", idCaptura);
                intent.PutExtra("idEdad", 2);
                intent.PutExtra("idTipo", 1/*SIGATOKA*/);
                StartActivity(intent);
            }
        }

        private void Edad3_Click(object sender, System.EventArgs e)
        {
            if (cero == 1)
            {
                ToastFragment.ShowMakeText(this, "No se puede registrar mas de 1 planta.");
            }
            else
            {
                Intent intent = new Intent(this, typeof(IndicadoresActivity));
                intent.PutExtra("idCaptura", idCaptura);
                intent.PutExtra("idEdad", 3);
                intent.PutExtra("idTipo", 1/*SIGATOKA*/);
                StartActivity(intent);
            }
        }

        private void Edad6_Click(object sender, System.EventArgs e)
        {
            if (fija == 1)
            {
                ToastFragment.ShowMakeText(this, "No se puede registrar mas de 1 planta.");
            }
            else
            {
                Intent intent = new Intent(this, typeof(IndicadoresActivity));
                intent.PutExtra("idCaptura", idCaptura);
                intent.PutExtra("idEdad", 6);
                intent.PutExtra("idTipo", 1/*SIGATOKA*/);
                StartActivity(intent);
            }
        }

        private async void GuardarPunto_Click(object sender, System.EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
            terminar.SetMessage("¿Está seguro que desea finalizar el punto de lectura actual?");
            terminar.SetTitle("Finalizar punto lectura");

            terminar.SetPositiveButton("Si", async (alert, args) =>
            {
                //Se enciende bandera para indicar que se esta finalizando un punto de captura sigatoka
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("Actividad", "EnfermedadesActivity");
                editor.PutInt("idCaptura", 0);
                editor.Commit();

                Finish();
            });

            terminar.SetNegativeButton("No", (alert, args) =>
            {

            });

            terminar.Show();
        }

        public override void OnBackPressed()
        {
            VolverSemanas();
        }

        private void VolverEnfermedades_Click(object sender, System.EventArgs e)
        {
            VolverSemanas();
        }

        private async void VolverSemanas()
        {
            existPlanta = new ObservableCollection<int>();
            await DB.ExistPlanta(idCaptura, existPlanta);

            if (existPlanta[0] == 1)
            {
                Android.Support.V7.App.AlertDialog.Builder cancelar = new Android.Support.V7.App.AlertDialog.Builder(this);
                cancelar.SetMessage("¿Está seguro que desea finalizar el punto de lectura actual?");
                cancelar.SetTitle("Volver atrás");

                cancelar.SetPositiveButton("Si", (alert, args) =>
                {
                    //Se enciende bandera para indicar que se esta finalizando un punto de captura sigatoka
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString("Actividad", "EnfermedadesActivity");
                    editor.PutInt("idCaptura", 0);
                    editor.Commit();

                    Finish();
                });

                cancelar.SetNegativeButton("No", (alert, args) =>
                {

                });

                cancelar.Show();
            }
            else
            {
                NoGuardar();
            }
        }

        private async void NoGuardar()
        {
            //Se enciende bandera para indicar que se esta finalizando un punto de captura sigatoka
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("Actividad", "EnfermedadesActivity");
            editor.PutInt("idCaptura", 0);
            editor.Commit();

            Finish();
        }
    }
}