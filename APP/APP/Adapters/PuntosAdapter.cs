using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APP.Adapters
{
    public class PuntosAdapter : BaseAdapter<PuntoLectura>
    {
        EnfermedadesActivity mcontext;
        ObservableCollection<PuntoLectura> puntoLectura = new ObservableCollection<PuntoLectura>();

        public PuntosAdapter(EnfermedadesActivity context, IList<PuntoLectura> list)
        {
            this.mcontext = context;
            this.puntoLectura = (ObservableCollection<PuntoLectura>)list;
        }

        public override PuntoLectura this[int position] => puntoLectura[position];

        public override int Count => puntoLectura.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            view = mcontext.LayoutInflater.Inflate(Resource.Layout.Puntos, null);
            view.FindViewById<TextView>(Resource.Id.puntoLectura).Text = puntoLectura[position].punto.ToString();
            view.FindViewById<TextView>(Resource.Id.fechaPuntoLectura).Text = puntoLectura[position].fecha.ToString();
            view.FindViewById<Button>(Resource.Id.eliminar).Tag = position;
            view.FindViewById<Button>(Resource.Id.eliminar).Click += eliminar_Click;
            return view;
        }

        private void eliminar_Click(object sender, System.EventArgs e)
        {
            ((EnfermedadesActivity)this.mcontext).EliminarPunto(
                int.Parse(puntoLectura[int.Parse(((View)sender).Tag.ToString())].id.ToString())
            );
        }
    }
}