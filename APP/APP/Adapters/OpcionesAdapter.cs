using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APP.Adapters
{
    class OpcionesAdapter : BaseAdapter<Selectores>
    {
        string invocador;
        FragmentActivity mcontext;
        IList<Selectores> list;
        String dataString = "";
        ObservableCollection<Selectores> listData = new ObservableCollection<Selectores>();

        public OpcionesAdapter(FragmentActivity context, IList<Selectores> list, EditText inputFilter, string invocador)
        {            
            this.mcontext = context;
            this.list = list;
            this.listData = (ObservableCollection<Selectores>)list;
            this.invocador = invocador;

            inputFilter.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                if (e.Text.ToString() == "")
                {
                    dataString = "";
                    listData = (ObservableCollection<Selectores>)list;
                              
                    this.NotifyDataSetChanged();
                }
                else
                {
                    dataString = e.Text.ToString();
                    listData = new ObservableCollection<Selectores>();

                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i].nombre.ToLower().Contains(dataString.ToLower()))
                        {
                            listData.Add(list[i]);
                        }
                    }

                    this.NotifyDataSetInvalidated();
                    this.NotifyDataSetChanged();
                }
            };
        }

        public override Selectores this[int position] => listData[position];

        public override int Count => listData.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            view = mcontext.LayoutInflater.Inflate(Resource.Layout.OpcionesSelector, null);

            view.FindViewById<TextView>(Resource.Id.largeText).Text = listData[position].nombre.ToString();

            view.Tag = position;
            view.Click += View_Click;

            return view;
        }

        private void View_Click(object sender, EventArgs e)
        {
            if (this.invocador == "NuevoRecorridoActivity")
            {
                ((NuevoRecorridoActivity)this.mcontext).optionSelected(
                    int.Parse(listData[int.Parse(((View)sender).Tag.ToString())].id_server.ToString()),
                    listData[int.Parse(((View)sender).Tag.ToString())].nombre.ToString(),
                    int.Parse(listData[int.Parse(((View)sender).Tag.ToString())].activo.ToString())
                );
            }
            else if (this.invocador == "IndicadoresActivity")
            {
                ((IndicadoresActivity)this.mcontext).optionSelected(
                    int.Parse(listData[int.Parse(((View)sender).Tag.ToString())].id_server.ToString()),
                    listData[int.Parse(((View)sender).Tag.ToString())].nombre.ToString()
                );
            }
            else if (this.invocador == "EdadesActivity")
            {
                ((EdadesActivity)this.mcontext).optionSelected(
                    int.Parse(listData[int.Parse(((View)sender).Tag.ToString())].id_server.ToString())
                );
            }
            else { }
        }
    }
}