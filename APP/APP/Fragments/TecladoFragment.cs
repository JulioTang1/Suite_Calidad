using Android.OS;
using Android.Views;
using Android.Widget;
using APP.Activities;
using System;
using System.Globalization;

namespace APP.Fragments
{
    public class TecladoFragment : Android.Support.V4.App.DialogFragment
    {
        string invocador, nombre;

        ImageView volverTeclado, borrarTeclado;
        TextView text, indicador;
        Button uno, dos, tres, cuatro, cinco, seis, siete, ocho, nueve, punto, cero, ok;

        public TecladoFragment(string invocador, string nombre)
        {
            this.invocador = invocador;
            this.nombre = nombre;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Teclado, container, false);

            //Instancia de elmentos del fragmento teclado
            volverTeclado = (ImageView)view.FindViewById(Resource.Id.volverTeclado);
            borrarTeclado = (ImageView)view.FindViewById(Resource.Id.borrarTeclado);
            text = (TextView)view.FindViewById(Resource.Id.text);
            indicador = (TextView)view.FindViewById(Resource.Id.indicador);
            uno = (Button)view.FindViewById(Resource.Id.uno);
            dos = (Button)view.FindViewById(Resource.Id.dos);
            tres = (Button)view.FindViewById(Resource.Id.tres);
            cuatro = (Button)view.FindViewById(Resource.Id.cuatro);
            cinco = (Button)view.FindViewById(Resource.Id.cinco);
            seis = (Button)view.FindViewById(Resource.Id.seis);
            siete = (Button)view.FindViewById(Resource.Id.siete);
            ocho = (Button)view.FindViewById(Resource.Id.ocho);
            nueve = (Button)view.FindViewById(Resource.Id.nueve);
            punto = (Button)view.FindViewById(Resource.Id.punto);
            cero = (Button)view.FindViewById(Resource.Id.cero);
            ok = (Button)view.FindViewById(Resource.Id.ok);

            //Eventos click sobre los botones del teclado
            volverTeclado.Click += VolverTeclado_Click;
            borrarTeclado.Click += BorrarTeclado_Click;
            borrarTeclado.LongClick += BorrarTeclado_LongClick;
            uno.Click += Uno_Click;
            dos.Click += Dos_Click;
            tres.Click += Tres_Click;
            cuatro.Click += Cuatro_Click;
            cinco.Click += Cinco_Click;
            seis.Click += Seis_Click;
            siete.Click += Siete_Click;
            ocho.Click += Ocho_Click;
            nueve.Click += Nueve_Click;
            punto.Click += Punto_Click;
            cero.Click += Cero_Click;
            ok.Click += Ok_Click;

            //Carga de datos en pantalla segun actividad invocadora del fragmento
            text.Text = nombre; //nombre del indicador a registrar
            if (this.invocador == "IniciarRecorridoActivity")
            {
                //Pone valor previo del indicador en la pantalla del teclado
                if (((IniciarRecorridoActivity)Activity).indicador == "temperatura")
                {
                    indicador.Text = ((IniciarRecorridoActivity)Activity).indicadorTemperatura.Text;
                }
                else if (((IniciarRecorridoActivity)Activity).indicador == "precipitacion")
                {
                    indicador.Text = ((IniciarRecorridoActivity)Activity).indicadorPrecipitacion.Text;
                }
                else { }
            }
            else if (this.invocador == "IndicadoresActivity")
            {
                //Pone valor previo del indicador en la pantalla del teclado
                if (((IndicadoresActivity)Activity).indicador == "Indicador 1")
                {
                    indicador.Text = ((IndicadoresActivity)Activity).textIndicador1.Text;
                }
                else if (((IndicadoresActivity)Activity).indicador == "Indicador 2")
                {
                    indicador.Text = ((IndicadoresActivity)Activity).textIndicador2.Text;
                }
                else if (((IndicadoresActivity)Activity).indicador == "Indicador 3")
                {
                    indicador.Text = ((IndicadoresActivity)Activity).textIndicador3.Text;
                }
                else if (((IndicadoresActivity)Activity).indicador == "Indicador 4")
                {
                    indicador.Text = ((IndicadoresActivity)Activity).textIndicador4.Text;
                }
                else { }
            }
            else if(this.invocador == "PrecipitacionesActivity")
            {
                //Pone valor previo del indicador en la pantalla del teclado
                if (((PrecipitacionesActivity)Activity).indicador == "precipitacion1")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion1.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion2")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion2.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion3")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion3.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion4")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion4.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion5")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion5.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion6")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion6.Text;
                }
                else if (((PrecipitacionesActivity)Activity).indicador == "precipitacion7")
                {
                    indicador.Text = ((PrecipitacionesActivity)Activity).textPrecipitacion7.Text;
                }
                else { }
            }
            else { }

            return view;
        }

        private void BorrarTeclado_LongClick(object sender, View.LongClickEventArgs e)
        {
            //Reinicia valor en pantalla del indicador
            indicador.Text = "0";
        }

        private void BorrarTeclado_Click(object sender, EventArgs e)
        {
            //Valor cortado
            string valor_actual = indicador.Text.Substring(0, indicador.Text.Length - 1);

            //Actualiza valor en pantalla del indicador
            indicador.Text = valor_actual == "" ? "0" : valor_actual;
        }

        private void Uno_Click(object sender, EventArgs e)
        {
            EditIndicador("1");
        }

        private void Dos_Click(object sender, EventArgs e)
        {
            EditIndicador("2");
        }

        private void Tres_Click(object sender, EventArgs e)
        {
            EditIndicador("3");
        }

        private void Cuatro_Click(object sender, EventArgs e)
        {
            EditIndicador("4");
        }

        private void Cinco_Click(object sender, EventArgs e)
        {
            EditIndicador("5");
        }

        private void Seis_Click(object sender, EventArgs e)
        {
            EditIndicador("6");
        }

        private void Siete_Click(object sender, EventArgs e)
        {
            EditIndicador("7");
        }

        private void Ocho_Click(object sender, EventArgs e)
        {
            EditIndicador("8");
        }

        private void Nueve_Click(object sender, EventArgs e)
        {
            EditIndicador("9");
        }

        private void Punto_Click(object sender, EventArgs e)
        {
            EditIndicador(".");
        }

        private void Cero_Click(object sender, EventArgs e)
        {
            EditIndicador("0");
        }

        private void VolverTeclado_Click(object sender, EventArgs e)
        {
            //Llamado de funcion segun actividad invocadora del fragmento 
            if (this.invocador == "IniciarRecorridoActivity")
            {
                ((IniciarRecorridoActivity)Activity).Tecladofragment_OnBackTeclado();
            }
            else if (this.invocador == "IndicadoresActivity")
            {
                ((IndicadoresActivity)Activity).Tecladofragment_OnBackTeclado();
            }
            else if (this.invocador == "PrecipitacionesActivity")
            {
                ((PrecipitacionesActivity)Activity).Tecladofragment_OnBackTeclado();
            }
            else { }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            //Llamado de funcion segun actividad invocadora del fragmento 
            if (this.invocador == "IniciarRecorridoActivity")
            {
                ((IniciarRecorridoActivity)Activity).Tecladofragment_OnValueRegistered(float.Parse(indicador.Text, CultureInfo.InvariantCulture));
            }
            else if (this.invocador == "IndicadoresActivity")
            {
                ((IndicadoresActivity)Activity).Tecladofragment_OnValueRegistered(float.Parse(indicador.Text, CultureInfo.InvariantCulture));
            }
            else if (this.invocador == "PrecipitacionesActivity")
            {
                ((PrecipitacionesActivity)Activity).Tecladofragment_OnValueRegistered(float.Parse(indicador.Text, CultureInfo.InvariantCulture));
            }
            else { }
        }

        private void EditIndicador(string valor)
        {
            //Guarda valor actual del indicador (en caso de ser cero lo omite)
            string valor_actual = indicador.Text == "0" ? "" : indicador.Text;

            //Pone numeros antes del punto (3 numeros antes de decimal)
            if (valor != "." && !valor_actual.Contains(".") && valor_actual.Length < 3)
            {
                indicador.Text = valor_actual + valor;
            }

            //Pone numeros despues del punto
            if (valor != "." && valor_actual.Contains("."))
            {
                //(2 numeros decimales)
                if (valor_actual.Substring(valor_actual.IndexOf(".") + 1).Length < 2)
                {
                    indicador.Text = valor_actual + valor;
                }               
            }

            //Verificaciones para poner un punto
            if (valor == ".")
            {
                //Punto inicial
                if (valor_actual == "")
                {
                    indicador.Text = "0" + valor;
                }
                //Punto entre numeros
                else if (!valor_actual.Contains("."))
                {
                    indicador.Text = valor_actual + valor;
                }
                //No pone punto si no se cumplen las condiciones anteriores
                else { }

            }            
        }
    }
}