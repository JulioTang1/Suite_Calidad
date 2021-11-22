using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace APP.Activities
{
    [Activity(Label = "PrecipitacionesActivity", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class PrecipitacionesActivity : AppCompatActivity
    {
        int idFinca;
        int activo;
        LinearLayout padrePrecipitaciones;
        ImageView volverRecorridos;
        Button guardarPrecipitaciones;
        RelativeLayout fecha;
        TextView fechaText;
        DateTime FechaActual;
        string[] listaFechas;

        //Bandera para saber que input invoco al teclado
        public string indicador = "";
        TecladoFragment tecladofragment;
        RelativeLayout eventPrecipitacion1, eventPrecipitacion2, eventPrecipitacion3, eventPrecipitacion4, eventPrecipitacion5, eventPrecipitacion6, eventPrecipitacion7;
        public TextView textPrecipitacion1, textPrecipitacion2, textPrecipitacion3, textPrecipitacion4, textPrecipitacion5, textPrecipitacion6, textPrecipitacion7;
        TextView fechaPrecipitacion1, fechaPrecipitacion2, fechaPrecipitacion3, fechaPrecipitacion4, fechaPrecipitacion5, fechaPrecipitacion6, fechaPrecipitacion7;
        TextView acumulado;

        ObservableCollection<float> precipitaciones;

        Drawable background, backgroundOriginal;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idFinca = Intent.GetIntExtra("idFinca", 0);
            activo = Intent.GetIntExtra("activo", 0);

            SetContentView(Resource.Layout.Precipitaciones);

            padrePrecipitaciones = (LinearLayout)FindViewById(Resource.Id.padrePrecipitaciones);

            background = this.GetDrawable(Resource.Drawable.roundededgesCeleste);
            backgroundOriginal = this.GetDrawable(Resource.Drawable.roundededges);

            FechaActual = DateTime.Now;
            fechaText = (TextView)FindViewById(Resource.Id.fechaText);
            fechaText.Text = FechaActual.ToString("dd/MM/yyyy");
            fecha = (RelativeLayout)FindViewById(Resource.Id.fecha);
            fecha.Click += Fecha_Click;

            //Se elimina el boton si la finca esta deshabilitada
            if (activo == 1)
            {
                guardarPrecipitaciones = (Button)FindViewById(Resource.Id.guardarPrecipitaciones);
                guardarPrecipitaciones.Click += GuardarPrecipitaciones_Click;
                padrePrecipitaciones.RemoveViewAt(5);
            }
            else
            {
                padrePrecipitaciones.RemoveViewAt(6);
            }

            volverRecorridos = (ImageView)FindViewById(Resource.Id.volverRecorridos);
            volverRecorridos.Click += VolverRecorridos_Click;

            eventPrecipitacion1 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion1);
            eventPrecipitacion2 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion2);
            eventPrecipitacion3 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion3);
            eventPrecipitacion4 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion4);
            eventPrecipitacion5 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion5);
            eventPrecipitacion6 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion6);
            eventPrecipitacion7 = (RelativeLayout)FindViewById(Resource.Id.eventPrecipitacion7);

            textPrecipitacion1 = (TextView)FindViewById(Resource.Id.textPrecipitacion1);
            textPrecipitacion2 = (TextView)FindViewById(Resource.Id.textPrecipitacion2);
            textPrecipitacion3 = (TextView)FindViewById(Resource.Id.textPrecipitacion3);
            textPrecipitacion4 = (TextView)FindViewById(Resource.Id.textPrecipitacion4);
            textPrecipitacion5 = (TextView)FindViewById(Resource.Id.textPrecipitacion5);
            textPrecipitacion6 = (TextView)FindViewById(Resource.Id.textPrecipitacion6);
            textPrecipitacion7 = (TextView)FindViewById(Resource.Id.textPrecipitacion7);

            fechaPrecipitacion1 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion1);
            fechaPrecipitacion2 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion2);
            fechaPrecipitacion3 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion3);
            fechaPrecipitacion4 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion4);
            fechaPrecipitacion5 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion5);
            fechaPrecipitacion6 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion6);
            fechaPrecipitacion7 = (TextView)FindViewById(Resource.Id.fechaPrecipitacion7);

            acumulado = (TextView)FindViewById(Resource.Id.acumulado);

            ChangeDate();
        }

        private void EventPrecipitacion1_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion1";
            DesplegarTeclado("Lunes");
        }

        private void EventPrecipitacion2_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion2";
            DesplegarTeclado("Martes");
        }

        private void EventPrecipitacion3_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion3";
            DesplegarTeclado("Miércoles");
        }

        private void EventPrecipitacion4_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion4";
            DesplegarTeclado("Jueves");
        }

        private void EventPrecipitacion5_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion5";
            DesplegarTeclado("Viernes");
        }

        private void EventPrecipitacion6_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion6";
            DesplegarTeclado("Sábado");
        }

        private void EventPrecipitacion7_Click(object sender, EventArgs e)
        {
            //Bandera indicando modificacion de input temperatura maxima
            indicador = "precipitacion7";
            DesplegarTeclado("Domingo");
        }

        public void DesplegarTeclado(string nombre)
        {
            tecladofragment = new TecladoFragment("PrecipitacionesActivity", nombre);
            var trans = SupportFragmentManager.BeginTransaction();
            tecladofragment.Show(trans, "Teclado");
        }

        public void Tecladofragment_OnValueRegistered(float valor)
        {
            //Asigna valor del teclado al input de la variable correspondiente
            if (indicador == "precipitacion1")
            {
                textPrecipitacion1.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[0] = float.Parse(textPrecipitacion1.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion2")
            {
                textPrecipitacion2.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[1] = float.Parse(textPrecipitacion2.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion3")
            {
                textPrecipitacion3.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[2] = float.Parse(textPrecipitacion3.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion4")
            {
                textPrecipitacion4.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[3] = float.Parse(textPrecipitacion4.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion5")
            {
                textPrecipitacion5.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[4] = float.Parse(textPrecipitacion5.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion6")
            {
                textPrecipitacion6.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[5] = float.Parse(textPrecipitacion6.Text, CultureInfo.InvariantCulture);
            }
            else if (indicador == "precipitacion7")
            {
                textPrecipitacion7.Text = valor.ToString(CultureInfo.InvariantCulture);
                precipitaciones[6] = float.Parse(textPrecipitacion7.Text, CultureInfo.InvariantCulture);
            }
            else { }

            //se actualiza el total
            float sum = 0;
            for (int i = 0; i < 7; i++)
            {
                sum += precipitaciones[i];
            }
            acumulado.Text = String.Concat(sum.ToString(CultureInfo.InvariantCulture), "mm");

            //Cierra fragmento del teclado
            tecladofragment.Dismiss();
        }

        public void Tecladofragment_OnBackTeclado()
        {
            //Cierra fragmento del teclado
            tecladofragment.Dismiss();
        }

        private void Fecha_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time) { fechaText.Text = time.ToString("dd/MM/yyyy"); }, DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")));
            frag.Show(FragmentManager, DatePickerFragment.TAG);

            frag.ChangeDate += Frag_ChangeDate;
        }

        private void Frag_ChangeDate(object sender, EventArgs e)
        {
            ChangeDate();
        }

        private async void ChangeDate()
        {
            listaFechas = setRangoSemana(DateTime.Parse(fechaText.Text, CultureInfo.CreateSpecificCulture("de-DE")));

            CultureInfo ci = new CultureInfo("es-ES");
            TextInfo textInfo = ci.TextInfo;
            fechaPrecipitacion1.Text = String.Concat(listaFechas[0], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[0]).ToString("dddd", ci)));
            fechaPrecipitacion2.Text = String.Concat(listaFechas[1], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[1]).ToString("dddd", ci)));
            fechaPrecipitacion3.Text = String.Concat(listaFechas[2], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[2]).ToString("dddd", ci)));
            fechaPrecipitacion4.Text = String.Concat(listaFechas[3], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[3]).ToString("dddd", ci)));
            fechaPrecipitacion5.Text = String.Concat(listaFechas[4], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[4]).ToString("dddd", ci)));
            fechaPrecipitacion6.Text = String.Concat(listaFechas[5], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[5]).ToString("dddd", ci)));
            fechaPrecipitacion7.Text = String.Concat(listaFechas[6], " ", textInfo.ToTitleCase(DateTime.Parse(listaFechas[6]).ToString("dddd", ci)));

            precipitaciones = new ObservableCollection<float>();

            await DB.LoadPrecipitaciones(idFinca, listaFechas, precipitaciones);

            textPrecipitacion1.Text = precipitaciones[0].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion2.Text = precipitaciones[1].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion3.Text = precipitaciones[2].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion4.Text = precipitaciones[3].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion5.Text = precipitaciones[4].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion6.Text = precipitaciones[5].ToString(CultureInfo.InvariantCulture);
            textPrecipitacion7.Text = precipitaciones[6].ToString(CultureInfo.InvariantCulture);

            float sum = 0;
            for (int i = 0; i < 7; i++)
            {
                sum += precipitaciones[i];
            }
            acumulado.Text = String.Concat(sum.ToString(CultureInfo.InvariantCulture), "mm");

            //Se pinta la fecha actual
            eventPrecipitacion1.Background = DateTime.Parse(listaFechas[0].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion2.Background = DateTime.Parse(listaFechas[1].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion3.Background = DateTime.Parse(listaFechas[2].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion4.Background = DateTime.Parse(listaFechas[3].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion5.Background = DateTime.Parse(listaFechas[4].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion6.Background = DateTime.Parse(listaFechas[5].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;
            eventPrecipitacion7.Background = DateTime.Parse(listaFechas[6].ToString()).ToString("yyyy-MM-dd") == FechaActual.ToString("yyyy-MM-dd") ? background : backgroundOriginal;

            //Se apagan los eventos de las fechas
            eventPrecipitacion1.Click -= EventPrecipitacion1_Click;
            eventPrecipitacion2.Click -= EventPrecipitacion2_Click;
            eventPrecipitacion3.Click -= EventPrecipitacion3_Click;
            eventPrecipitacion4.Click -= EventPrecipitacion4_Click;
            eventPrecipitacion5.Click -= EventPrecipitacion5_Click;
            eventPrecipitacion6.Click -= EventPrecipitacion6_Click;
            eventPrecipitacion7.Click -= EventPrecipitacion7_Click;

            //Se encienden los eventos que esten antes de la fecha actual o en la fecha actual para fincas activas
            if(activo == 1)
            {
                if (DateTime.Parse(listaFechas[0].ToString()) <= FechaActual)
                {
                    eventPrecipitacion1.Click += EventPrecipitacion1_Click;
                }
                if (DateTime.Parse(listaFechas[1].ToString()) <= FechaActual)
                {
                    eventPrecipitacion2.Click += EventPrecipitacion2_Click;
                }
                if (DateTime.Parse(listaFechas[2].ToString()) <= FechaActual)
                {
                    eventPrecipitacion3.Click += EventPrecipitacion3_Click;
                }
                if (DateTime.Parse(listaFechas[3].ToString()) <= FechaActual)
                {
                    eventPrecipitacion4.Click += EventPrecipitacion4_Click;
                }
                if (DateTime.Parse(listaFechas[4].ToString()) <= FechaActual)
                {
                    eventPrecipitacion5.Click += EventPrecipitacion5_Click;
                }
                if (DateTime.Parse(listaFechas[5].ToString()) <= FechaActual)
                {
                    eventPrecipitacion6.Click += EventPrecipitacion6_Click;
                }
                if (DateTime.Parse(listaFechas[6].ToString()) <= FechaActual)
                {
                    eventPrecipitacion7.Click += EventPrecipitacion7_Click;
                }
            }
        }

        private void GuardarPrecipitaciones_Click(object sender, System.EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
            terminar.SetMessage("¿Está seguro que desea finalizar la toma de precipitaciones?");
            terminar.SetTitle("Finalizar precipitaciones");

            terminar.SetPositiveButton("Si", (alert, args) =>
            {
                GuardarBD();
            });

            terminar.SetNegativeButton("No", (alert, args) =>
            {

            });

            terminar.Show();
        }

        private async void GuardarBD()
        {
            await DB.SavePrecipitaciones(idFinca, listaFechas, precipitaciones);
            Finish();
        }

        public override void OnBackPressed()
        {
            VolverRecorridos();
        }

        private void VolverRecorridos_Click(object sender, System.EventArgs e)
        {
            if (activo == 1)
            {
                VolverRecorridos();
            }
            else
            {
                Finish();
            }
        }

        private void VolverRecorridos()
        {
            Android.Support.V7.App.AlertDialog.Builder cancelar = new Android.Support.V7.App.AlertDialog.Builder(this);
            cancelar.SetMessage("¿Desea guardar las precipitaciones antes de volver atrás?");
            cancelar.SetTitle("Volver atrás");

            cancelar.SetPositiveButton("Si", (alert, args) =>
            {
                GuardarBD();
            });

            cancelar.SetNegativeButton("No", (alert, args) =>
            {
                Finish();
            });

            cancelar.Show();
        }

        string[] setRangoSemana(DateTime fecha)
        {
            var noSemana = numeroSemana(fecha);
            //Validación para verificar si es la ultima semana del anio
            var semana = primerDíaSemana(noSemana == 52 ? fecha.AddYears(-1).Year : fecha.Year, noSemana, CultureInfo.CurrentCulture);
            string[] dias =
            {
                semana.AddDays(1).ToShortDateString(),
                semana.AddDays(2).ToShortDateString(),
                semana.AddDays(3).ToShortDateString(),
                semana.AddDays(4).ToShortDateString(),
                semana.AddDays(5).ToShortDateString(),
                semana.AddDays(6).ToShortDateString(),
                semana.AddDays(7).ToShortDateString()
            };
            return dias;
        }

        int numeroSemana(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        static DateTime primerDíaSemana(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
    }
}