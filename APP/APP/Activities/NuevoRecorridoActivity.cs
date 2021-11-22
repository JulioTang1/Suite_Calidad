using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Widget;
using APP.Fragments;
using AppDemo.LocalLogic.Componentes;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class NuevoRecorridoActivity : AppCompatActivity
    {
        string opcion;

        ImageView volver;
        Button iniciarRecorrido;
        RelativeLayout selectDepartamento, selectMunicipio, selectFinca;
        BuscadorSelectorFragment buscadorSelector;
        int allFincas;
        TextView selectDepartamentoText, selectMunicipioText, selectFincaText;

        public string selector = "Departamentos";

        public int idDepartamento = 0;
        public int idMunicipio = 0;
        public int idFinca = 0;
        public int activo = 0;
        public string nombreFinca = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            opcion = Intent.GetStringExtra("opcion");

            SetContentView(Resource.Layout.NuevoRecorrido);

            volver = (ImageView)FindViewById(Resource.Id.volver);
            volver.Click += Volver_Click;

            iniciarRecorrido = (Button)FindViewById(Resource.Id.iniciarRecorrido);
            iniciarRecorrido.Click += IniciarRecorrido_Click;

            //Texto del boton de acuerdo a la opcion seleccionada
            if (opcion == "mostrar")
            {
                iniciarRecorrido.Text = "Mostrar Visitas";
                allFincas = 1;
            }
            else if (opcion == "crear")
            {
                iniciarRecorrido.Text = "Iniciar";
                allFincas = 0;
            }
            else if (opcion == "precipitaciones")
            {
                iniciarRecorrido.Text = "Registrar Precipitaciones";
                allFincas = 1;
            }
            else if (opcion == "consultarBioseguridad")
            {
                iniciarRecorrido.Text = "Chequeos Bioseguridad";
                allFincas = 1;
            }
            else if (opcion == "bioseguridad")
            {
                iniciarRecorrido.Text = "Registrar Bioseguridad";
                allFincas = 0;
            }
            else { }

            selectDepartamento = (RelativeLayout)FindViewById(Resource.Id.selectDepartamento);
            selectDepartamento.Click += SelectDepartamento_Click;

            selectMunicipio = (RelativeLayout)FindViewById(Resource.Id.selectMunicipio);
            selectMunicipio.Click += SelectMunicipio_Click;

            selectFinca = (RelativeLayout)FindViewById(Resource.Id.selectFinca);
            selectFinca.Click += SelectFinca_Click;

            selectDepartamentoText = (TextView)FindViewById(Resource.Id.selectDepartamentoText);
            selectMunicipioText = (TextView)FindViewById(Resource.Id.selectMunicipioText);
            selectFincaText = (TextView)FindViewById(Resource.Id.selectFincaText);

            //Se cargan los selectores con lo que el usuario los había dejado antes de cerrar la aplicación en medio de un recorrido
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            int idDepartamento = prefs.GetInt("idDepartamento", 0);
            string nombreDepartamento = prefs.GetString("nombreDepartamento", "");
            int idMunicipio = prefs.GetInt("idMunicipio", 0);
            string nombreMunicipio = prefs.GetString("nombreMunicipio", "");
            int idFinca = prefs.GetInt("idFinca", 0);
            string nombreFinca = prefs.GetString("nombreFinca", "");
            int idVisita = prefs.GetInt("idVisita", 0);
            if (idDepartamento != 0  && idVisita != 0)
            {
                selector = "Departamentos";
                optionSelected(idDepartamento, nombreDepartamento, 1);
            }
            if (idMunicipio != 0 && idVisita != 0)
            {
                selector = "Municipios";
                optionSelected(idMunicipio, nombreMunicipio, 1);
            }
            if (idFinca != 0 && idVisita != 0)
            {
                selector = "Fincas";
                optionSelected(idFinca, nombreFinca, 1);
                Intent intent = new Intent(this, typeof(EnfermedadesActivity));
                intent.PutExtra("idVisita", idVisita);
                StartActivity(intent);
            }
        }

        public void optionSelected(int id, string nombre, int activo) 
        {
            //Se limpia la sesion
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            if (selector == "Departamentos")
            {
                selectDepartamentoText.Text = nombre;
                selectMunicipioText.Text = "Municipio";
                selectFincaText.Text = "Finca";
                this.idDepartamento = id;
                idMunicipio = 0;
                idFinca = 0;

                editor.PutInt("idDepartamento", id);
                editor.PutString("nombreDepartamento", nombre);
            }
            else if (selector == "Municipios")
            {
                selectMunicipioText.Text = nombre;
                selectFincaText.Text = "Finca";
                this.idMunicipio = id;
                idFinca = 0;


                editor.PutInt("idMunicipio", id);
                editor.PutString("nombreMunicipio", nombre);
            }
            else if (selector == "Fincas")
            {
                selectFincaText.Text = nombre;
                this.nombreFinca = nombre;
                this.idFinca = id;
                this.activo = activo;

                editor.PutInt("idFinca", id);
                editor.PutString("nombreFinca", nombre);
            }
            else { }

            editor.Commit();

            if(buscadorSelector != null)
            {
                buscadorSelector.Dismiss();
            }
        }

        private void SelectDepartamento_Click(object sender, System.EventArgs e)
        {
            selector = "Departamentos";
            buscadorSelector = new BuscadorSelectorFragment("NuevoRecorridoActivity", allFincas);
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector Departamento");
        }

        private void SelectMunicipio_Click(object sender, System.EventArgs e)
        {
            selector = "Municipios";
            buscadorSelector = new BuscadorSelectorFragment("NuevoRecorridoActivity", allFincas);
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector Municipio");
        }

        private void SelectFinca_Click(object sender, System.EventArgs e)
        {
            selector = "Fincas";
            buscadorSelector = new BuscadorSelectorFragment("NuevoRecorridoActivity", allFincas);
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector Finca");
        }

        private void Volver_Click(object sender, System.EventArgs e)
        {
            Finish();
        }

        private void IniciarRecorrido_Click(object sender, System.EventArgs e)
        {
            if (idFinca == 0)
            {
                ToastFragment.ShowMakeText(this, "Por favor seleccione un finca");
            }
            else
            {
                //Despliega vista de acuerdo a la opcion seleccionada
                if (opcion == "mostrar")
                {
                    Intent intent = new Intent(this, typeof(RecorridoActivity));
                    intent.PutExtra("idFinca", idFinca);
                    intent.PutExtra("nombreFinca", nombreFinca);
                    intent.PutExtra("opcion", opcion);
                    StartActivity(intent);
                }
                else if (opcion == "crear")
                {
                    Intent intent = new Intent(this, typeof(IniciarRecorridoActivity));
                    intent.PutExtra("idFinca", idFinca);
                    intent.PutExtra("nombreFinca", nombreFinca);
                    StartActivity(intent);
                }
                else if (opcion == "precipitaciones")
                {
                    Intent intent = new Intent(this, typeof(PrecipitacionesActivity));
                    intent.PutExtra("idFinca", idFinca);
                    intent.PutExtra("activo", activo);
                    StartActivity(intent);
                }
                else if (opcion == "consultarBioseguridad")
                {
                    Intent intent = new Intent(this, typeof(RecorridoActivity));
                    intent.PutExtra("idFinca", idFinca);
                    intent.PutExtra("nombreFinca", nombreFinca);
                    intent.PutExtra("opcion", opcion);
                    StartActivity(intent);
                }
                else if(opcion == "bioseguridad")
                {
                    Intent intent = new Intent(this, typeof(BioseguridadActivity));
                    intent.PutExtra("opcion", "llenar");
                    intent.PutExtra("idFinca", idFinca);
                    StartActivity(intent);
                }
                else { }
            }
        }
    }
}