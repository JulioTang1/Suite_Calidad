using Android.OS;
using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Adapters;
using APP.Helpers;
using System.Collections.ObjectModel;

namespace APP.Fragments
{
    public class BuscadorSelectorFragment : Android.Support.V4.App.DialogFragment
    {
        string invocador;
        int allFincas;

        View view;
        ObservableCollection<Selectores> listaSelector;

        public BuscadorSelectorFragment(string invocador)
        {
            this.invocador = invocador;
        }

        public BuscadorSelectorFragment(string invocador, int allFincas)
        {
            this.invocador = invocador;
            this.allFincas = allFincas;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.BuscadorSelector, container, false);
            return view;
        }

        public async override void OnResume()
        {
            base.OnResume();

            listaSelector = new ObservableCollection<Selectores>();
            //Carga datos del selector segun actividad invocadora del fragmento 
            if (this.invocador == "NuevoRecorridoActivity")
            {
                if (((NuevoRecorridoActivity)Activity).selector == "Departamentos")
                {
                    await DB.LoadDepartamentos(listaSelector, allFincas);
                }
                else if (((NuevoRecorridoActivity)Activity).selector == "Municipios")
                {
                    await DB.LoadMunicipios(listaSelector, ((NuevoRecorridoActivity)Activity).idDepartamento, allFincas);
                }
                else if (((NuevoRecorridoActivity)Activity).selector == "Fincas")
                {
                    await DB.LoadFincas(listaSelector, ((NuevoRecorridoActivity)Activity).idMunicipio, allFincas);
                }
                else { }
            }
            else if (this.invocador == "IndicadoresActivity")
            {
                if (((IndicadoresActivity)Activity).selector == "H2" || ((IndicadoresActivity)Activity).selector == "H3" || ((IndicadoresActivity)Activity).selector == "H4")
                {
                    listaSelector.Add(new Selectores(0, "0"));
                    listaSelector.Add(new Selectores(2, "1-"));
                    listaSelector.Add(new Selectores(1, "1+"));
                    listaSelector.Add(new Selectores(4, "2-"));
                    listaSelector.Add(new Selectores(3, "2+"));
                    listaSelector.Add(new Selectores(6, "3-"));
                    listaSelector.Add(new Selectores(5, "3+"));
                }
                else if (((IndicadoresActivity)Activity).selector == "Fusarium"
                      || ((IndicadoresActivity)Activity).selector == "Moko"
                      || ((IndicadoresActivity)Activity).selector == "Erwinia")
                {
                    listaSelector.Add(new Selectores(4, "Sospechosa"));
                    listaSelector.Add(new Selectores(1, "Ausencia"));
                    listaSelector.Add(new Selectores(2, "Presencia tratada"));
                    listaSelector.Add(new Selectores(3, "Presencia sin tratar"));
                }
                else if (((IndicadoresActivity)Activity).selector == "FIT"
                      || ((IndicadoresActivity)Activity).selector == "RTI")
                {
                    listaSelector.Add(new Selectores(1, "Si"));
                    listaSelector.Add(new Selectores(2, "No"));
                }
                else { }
            }
            else if (this.invocador == "EdadesActivity")
            {
                if (((EdadesActivity)Activity).selector == "Edad1")
                {
                    await DB.LoadPlantasEdad(((EdadesActivity)Activity).idLectura, 1, listaSelector);
                }
                else if (((EdadesActivity)Activity).selector == "Edad2")
                {
                    await DB.LoadPlantasEdad(((EdadesActivity)Activity).idLectura, 2, listaSelector);
                }
                else if (((EdadesActivity)Activity).selector == "Edad3")
                {
                    await DB.LoadPlantasEdad(((EdadesActivity)Activity).idLectura, 3, listaSelector);
                }
                else if (((EdadesActivity)Activity).selector == "Edad6")
                {
                    await DB.LoadPlantasEdad(((EdadesActivity)Activity).idLectura, 6, listaSelector);
                }
                else { }
            }
            else { }

            ListView listView1 = view.FindViewById<ListView>(Resource.Id.customListPanel);
            EditText inputFilter = view.FindViewById<EditText>(Resource.Id.inputTextFilter);
            listView1.Adapter = new OpcionesAdapter(Activity, listaSelector, inputFilter, this.invocador);
        }
    }
}