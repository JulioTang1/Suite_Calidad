using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APP.Adapters
{
    class ChequeoAdapter : BaseAdapter<ControlesBioseguridad>
    {        
        Activity mcontext;
        ObservableCollection<ControlesBioseguridad> chequeos = new ObservableCollection<ControlesBioseguridad>();
        string opcion;

        public ChequeoAdapter(Activity context, IList<ControlesBioseguridad> list, string opcion)
        {
            this.mcontext = context;
            this.chequeos = (ObservableCollection<ControlesBioseguridad>)list;
            this.opcion = opcion;
        }

        public override ControlesBioseguridad this[int position] => chequeos[position];

        public override int Count => chequeos.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            view = mcontext.LayoutInflater.Inflate(Resource.Layout.Chequeo, null);

            //Enunciado de la pregunta
            view.FindViewById<TextView>(Resource.Id.pregunta).Text = chequeos[position].pregunta.ToString();

            //Color de los botones
            Drawable background1 = mcontext.GetDrawable(Resource.Drawable.roundededgesleftAzul);
            Drawable background2 = mcontext.GetDrawable(Resource.Drawable.edgesAzul);
            Drawable background3 = mcontext.GetDrawable(Resource.Drawable.roundededgesrightAzul);

            if(chequeos[position].respuesta == 1)
            {
                view.FindViewById<RelativeLayout>(Resource.Id.si).Background = background1;
            }
            else if (chequeos[position].respuesta == 2)
            {
                view.FindViewById<RelativeLayout>(Resource.Id.no).Background = background2;
            }
            else if (chequeos[position].respuesta == 3)
            {
                view.FindViewById<RelativeLayout>(Resource.Id.na).Background = background3;
            }
            else { }

            //Eventos para botones de respuesta cuando se va a llenar un formulario nuevo
            if (opcion == "llenar")
            {
                view.FindViewById<RelativeLayout>(Resource.Id.si).Tag = position;
                view.FindViewById<RelativeLayout>(Resource.Id.si).Click += ChequeoAdapter_Click;
                view.FindViewById<RelativeLayout>(Resource.Id.no).Tag = position;
                view.FindViewById<RelativeLayout>(Resource.Id.no).Click += ChequeoAdapter_Click1;
                view.FindViewById<RelativeLayout>(Resource.Id.na).Tag = position;
                view.FindViewById<RelativeLayout>(Resource.Id.na).Click += ChequeoAdapter_Click2;
            }

            //Configuracion de camaras de acuerdo a las opciones "llenar" o "ver"
            if (opcion == "llenar")
            {
                //Color de la camara
                if (chequeos[position].camara == 1)
                {
                    view.FindViewById<ImageView>(Resource.Id.imagen).SetImageResource(Resource.Drawable.camara_rojo);
                }
                else if (chequeos[position].camara == 2)
                {
                    view.FindViewById<ImageView>(Resource.Id.imagen).SetImageResource(Resource.Drawable.camara_naranja);
                }
                else if (chequeos[position].camara == 3)
                {
                    view.FindViewById<ImageView>(Resource.Id.imagen).SetImageResource(Resource.Drawable.camara_verde);
                }
                else { }

                //Evento para la camara
                view.FindViewById<ImageView>(Resource.Id.imagen).Tag = position;
                view.FindViewById<ImageView>(Resource.Id.imagen).Click += ChequeoAdapter_Click3;
            }
            else if (opcion == "ver")
            {
                //Visualizacion de la camara si hay fotos registradas
                if (chequeos[position].camara == 0)
                {
                    view.FindViewById<ImageView>(Resource.Id.imagen).Visibility = Android.Views.ViewStates.Invisible;
                }
                else
                {
                    //Evento para la camara
                    view.FindViewById<ImageView>(Resource.Id.imagen).Tag = position;
                    view.FindViewById<ImageView>(Resource.Id.imagen).Click += ChequeoAdapter_Click4;
                }
            }
            else { }

            return view;
        }

        private void ChequeoAdapter_Click(object sender, System.EventArgs e)
        {
            //Variable para colorear los botones
            Drawable background;

            //Padre del elemento sobre el que se dio click
            View padre = (View)((View)sender).Parent;

            //Despinta todos los hijos del padre
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesleft);
            padre.FindViewById<RelativeLayout>(Resource.Id.si).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.edges);
            padre.FindViewById<RelativeLayout>(Resource.Id.no).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesright);
            padre.FindViewById<RelativeLayout>(Resource.Id.na).Background = background;

            //Sombrea el elemento clickeado
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesleftAzul);
            padre.FindViewById<RelativeLayout>(Resource.Id.si).Background = background;

            //Dispara metodo para llenar arreglo de los chequeos
            ((BioseguridadActivity)this.mcontext).SetChequeo(
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                1,
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].aspecto.ToString()),
                int.Parse(((View)sender).Tag.ToString())
            );            
        }

        private void ChequeoAdapter_Click1(object sender, System.EventArgs e)
        {
            //Variable para colorear los botones
            Drawable background;

            //Padre del elemento sobre el que se dio click
            View padre = (View)((View)sender).Parent;

            //Despinta todos los hijos del padre
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesleft);
            padre.FindViewById<RelativeLayout>(Resource.Id.si).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.edges);
            padre.FindViewById<RelativeLayout>(Resource.Id.no).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesright);
            padre.FindViewById<RelativeLayout>(Resource.Id.na).Background = background;

            //Sombrea el elemento clickeado
            background = mcontext.GetDrawable(Resource.Drawable.edgesAzul);
            padre.FindViewById<RelativeLayout>(Resource.Id.no).Background = background;

            //Dispara metodo para llenar arreglo de los chequeos
            ((BioseguridadActivity)this.mcontext).SetChequeo(
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                2,
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].aspecto.ToString()),
                int.Parse(((View)sender).Tag.ToString())
            );
        }

        private void ChequeoAdapter_Click2(object sender, System.EventArgs e)
        {
            //Variable para colorear los botones
            Drawable background;

            //Padre del elemento sobre el que se dio click
            View padre = (View)((View)sender).Parent;

            //Despinta todos los hijos del padre
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesleft);
            padre.FindViewById<RelativeLayout>(Resource.Id.si).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.edges);
            padre.FindViewById<RelativeLayout>(Resource.Id.no).Background = background;
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesright);
            padre.FindViewById<RelativeLayout>(Resource.Id.na).Background = background;

            //Sombrea el elemento clickeado
            background = mcontext.GetDrawable(Resource.Drawable.roundededgesrightAzul);
            padre.FindViewById<RelativeLayout>(Resource.Id.na).Background = background;

            //Dispara metodo para llenar arreglo de los chequeos
            ((BioseguridadActivity)this.mcontext).SetChequeo(
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                3,
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].aspecto.ToString()),
                int.Parse(((View)sender).Tag.ToString())
            );
        }

        private void ChequeoAdapter_Click3(object sender, System.EventArgs e)
        {
            //Dispara metodo para abrir la camara
            ((BioseguridadActivity)this.mcontext).InvokeCamera(
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].id.ToString()),
                (View)sender,
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].aspecto.ToString()),
                int.Parse(((View)sender).Tag.ToString())
            );            
        }

        private void ChequeoAdapter_Click4(object sender, System.EventArgs e)
        {
            ((BioseguridadActivity)this.mcontext).VerFotos(
                int.Parse(chequeos[int.Parse(((View)sender).Tag.ToString())].id.ToString())
            );
        }
    }
}