using Android.App;
using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APP.Adapters
{
    public class VisitasAdapter : BaseAdapter<Visitas>
    {
        Activity mcontext;
        ObservableCollection<Visitas> visitas = new ObservableCollection<Visitas>();
        string invocador;
        int banderaBioseguridad;

        public VisitasAdapter(Activity context, IList<Visitas> list, string invocador)
        {
            this.mcontext = context;
            this.visitas = (ObservableCollection<Visitas>)list;
            this.invocador = invocador;
        }

        public VisitasAdapter(Activity context, IList<Visitas> list, string invocador, int banderaBioseguridad)
        {
            this.mcontext = context;
            this.visitas = (ObservableCollection<Visitas>)list;
            this.invocador = invocador;
            this.banderaBioseguridad = banderaBioseguridad;
        }

        public override Visitas this[int position] => visitas[position];

        public override int Count => visitas.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            view = mcontext.LayoutInflater.Inflate(Resource.Layout.Visitas, null);

            view.FindViewById<TextView>(Resource.Id.visita).Text = visitas[position].nombre.ToString();
            view.FindViewById<TextView>(Resource.Id.fechaVisita).Text = visitas[position].fecha.ToString();

            if (invocador == "RecorridoActivity")
            {
                if (banderaBioseguridad == 0)
                {
                    view.FindViewById<Button>(Resource.Id.botonVisita).Text = "Ver Ruta";
                }
                else if (banderaBioseguridad == 1)
                {
                    view.FindViewById<Button>(Resource.Id.botonVisita).Visibility = Android.Views.ViewStates.Invisible;
                }
                else { }
            }
            else if(invocador == "LecturasActivity")
            {
                view.FindViewById<Button>(Resource.Id.botonVisita).Text = "Ir a Punto de Lectura";
            }
            else { }

            view.Tag = position;
            view.Click += View_Click;

            view.FindViewById<Button>(Resource.Id.botonVisita).Tag = position;
            view.FindViewById<Button>(Resource.Id.botonVisita).Click += botonVisita_Click;

            return view;
        }

        private void botonVisita_Click(object sender, System.EventArgs e)
        {
            if (invocador == "RecorridoActivity")
            {
                ((RecorridoActivity)this.mcontext).RutaSelected(
                    int.Parse(visitas[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                    visitas[int.Parse(((View)sender).Tag.ToString())].nombre.ToString(),
                    visitas[int.Parse(((View)sender).Tag.ToString())].fecha.ToString()
                );
            }
            else if (invocador == "LecturasActivity")
            {
                ((LecturasActivity)this.mcontext).RutaSelected(
                    int.Parse(visitas[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                    visitas[int.Parse(((View)sender).Tag.ToString())].fecha.ToString()
                );
            }
            else { }
        }

        private void View_Click(object sender, System.EventArgs e)
        {
            if (invocador == "RecorridoActivity")
            {
                if (banderaBioseguridad == 0)
                {
                    ((RecorridoActivity)this.mcontext).VisitaSelected(
                        int.Parse(visitas[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                        visitas[int.Parse(((View)sender).Tag.ToString())].nombre.ToString(),
                        ((View)sender).FindViewById<TextView>(Resource.Id.fechaVisita).Text
                    );
                }
                else if (banderaBioseguridad == 1)
                {
                    ((RecorridoActivity)this.mcontext).ChequeoSelected(
                        int.Parse(visitas[int.Parse(((View)sender).Tag.ToString())].id.ToString())
                    );
                }
                else { }
            }
            else if (invocador == "LecturasActivity")
            {
                ((LecturasActivity)this.mcontext).LecturaSelected(
                    int.Parse(visitas[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                    visitas[int.Parse(((View)sender).Tag.ToString())].nombre.ToString()
                );
            }
            else { }
        }
    }
}