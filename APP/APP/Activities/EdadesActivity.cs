using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using APP.Fragments;
using APP.Helpers;
using System.Collections.ObjectModel;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class EdadesActivity : AppCompatActivity
    {
        public int idLectura;
        public string selector;
        ImageView volverEdades;
        TextView edad1Text, edad2Text, edad3Text, edad6Text;
        RelativeLayout edad1, edad2, edad3, edad6;
        ObservableCollection<string> edades;
        BuscadorSelectorFragment buscadorSelector;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idLectura = Intent.GetIntExtra("idLectura", 0);

            SetContentView(Resource.Layout.Edades);

            volverEdades = (ImageView)FindViewById(Resource.Id.volverEdades);
            volverEdades.Click += VolverEdades_Click;

            edad1Text = (TextView)FindViewById(Resource.Id.edad1Text);
            edad2Text = (TextView)FindViewById(Resource.Id.edad2Text);
            edad3Text = (TextView)FindViewById(Resource.Id.edad3Text);
            edad6Text = (TextView)FindViewById(Resource.Id.edad6Text);

            edad1 = (RelativeLayout)FindViewById(Resource.Id.edad1);
            edad1.Click += Edad1_Click;
            edad2 = (RelativeLayout)FindViewById(Resource.Id.edad2);
            edad2.Click += Edad2_Click;
            edad3 = (RelativeLayout)FindViewById(Resource.Id.edad3);
            edad3.Click += Edad3_Click;
            edad6 = (RelativeLayout)FindViewById(Resource.Id.edad6);
            edad6.Click += Edad6_Click;

            FillSelectores();
        }

        private void Edad1_Click(object sender, System.EventArgs e)
        {
            selector = "Edad1";
            buscadorSelector = new BuscadorSelectorFragment("EdadesActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector 10 Semanas");
        }

        private void Edad2_Click(object sender, System.EventArgs e)
        {
            selector = "Edad2";
            buscadorSelector = new BuscadorSelectorFragment("EdadesActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector 7 Semanas");
        }

        private void Edad3_Click(object sender, System.EventArgs e)
        {
            selector = "Edad3";
            buscadorSelector = new BuscadorSelectorFragment("EdadesActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector 0 Semanas");
        }

        private void Edad6_Click(object sender, System.EventArgs e)
        {
            selector = "Edad6";
            buscadorSelector = new BuscadorSelectorFragment("EdadesActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector Planta Joven");
        }

        private async void FillSelectores()
        {
            edades = new ObservableCollection<string>();
            await DB.CountEdades(idLectura, edades);
            edad1Text.Text = "10 Semanas: " + edades[0];
            edad2Text.Text = "7 Semanas: " + edades[1];
            edad3Text.Text = "0 Semanas: " + edades[2];
            edad6Text.Text = "Parcela Fija: " + edades[3];
        }

        public void optionSelected(int id)
        {
            Intent intent = new Intent(this, typeof(VerIndicadoresActivity));
            intent.PutExtra("idLectura", id);
            intent.PutExtra("Sigatoka", "Si");
            StartActivity(intent);
            buscadorSelector.Dismiss();
        }

        private void VolverEdades_Click(object sender, System.EventArgs e)
        {
            Finish();
        }
    }
}