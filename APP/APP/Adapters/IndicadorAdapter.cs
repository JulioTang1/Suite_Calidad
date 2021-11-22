using Android.App;
using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APP.Adapters
{
    public class IndicadorAdapter : BaseAdapter<Indicador>
    {
        Activity mcontext;
        ObservableCollection<Indicador> indicador = new ObservableCollection<Indicador>();

        public IndicadorAdapter(Activity context, IList<Indicador> list)
        {
            this.mcontext = context;
            this.indicador = (ObservableCollection<Indicador>)list;
        }

        public override Indicador this[int position] => indicador[position];

        public override int Count => indicador.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            view = mcontext.LayoutInflater.Inflate(Resource.Layout.Indicador, null);

            view.FindViewById<TextView>(Resource.Id.nombreIndicador).Text = indicador[position].nombre.ToString();
            view.FindViewById<TextView>(Resource.Id.indicador).Text = indicador[position].indicador == 7 ? "" : indicador[position].valor.ToString();
            view.FindViewById<TextView>(Resource.Id.fechaIndicador).Text = indicador[position].fecha.ToString();

            if (indicador[position].boton == 1)
            {
                view.FindViewById<Button>(Resource.Id.botonIndicador).Visibility = Android.Views.ViewStates.Visible;
                view.FindViewById<Button>(Resource.Id.botonIndicador).Tag = position;
                view.FindViewById<Button>(Resource.Id.botonIndicador).Click += botonVisita_Click;
            }

            return view;
        }

        private void botonVisita_Click(object sender, System.EventArgs e)
        {
            ((VerIndicadoresActivity)this.mcontext).VerFotos(
                int.Parse(indicador[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                indicador[int.Parse(((View)sender).Tag.ToString())].valor.ToString(),
                int.Parse(indicador[int.Parse(((View)sender).Tag.ToString())].indicador.ToString())
            );
        }
    }
}