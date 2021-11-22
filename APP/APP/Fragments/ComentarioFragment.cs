using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using APP.Activities;

namespace APP.Fragments
{
    public class ComentarioFragment : Android.Support.V4.App.DialogFragment
    {
        string invocador, titulo, contenido;
        TextInputLayout text;
        Button guardar;

        public ComentarioFragment(string invocador, string titulo, string contenido)
        {
            this.invocador = invocador;
            this.titulo = titulo;
            this.contenido = contenido;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Comentario, container, false);
            text = (TextInputLayout)view.FindViewById(Resource.Id.text);
            guardar = (Button)view.FindViewById(Resource.Id.guardar);
            guardar.Click += Guardar_Click;

            text.EditText.Text = contenido;
            text.Hint = titulo;

            return view;
        }

        private void Guardar_Click(object sender, System.EventArgs e)
        {
            if (invocador == "BioseguridadActivity")
            {
                ((BioseguridadActivity)Activity).ComentarioFragment(text.EditText.Text, titulo);
            }
            else if (invocador == "IndicadoresActivity")
            {
                ((IndicadoresActivity)Activity).ComentarioFragment(text.EditText.Text, titulo);
            }
        }
    }
}