using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;
using System.Globalization;
using Xamarin.Essentials;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class IndicadoresActivity : AppCompatActivity
    {
        ProgressFragment progressDialogue;
        ComentarioFragment comentarioFragment;
        ImageView volverIndicadores;
        LinearLayout IndicadoresLinear;
        int idCaptura, idEdad, idTipo;
        public string indicador;//PARA LA CALCULADORA
        RelativeLayout eventIndicador1, eventIndicador2, eventIndicador3, eventIndicador4;
        TextView nombreIndicador1, nombreIndicador2, nombreIndicador3, nombreIndicador4;
        public TextView textIndicador1, textIndicador2, textIndicador3, textIndicador4;
        ImageView imagenIndicador1, imagenIndicador2, imagenIndicador3, imagenIndicador4;
        RelativeLayout nombreIndicador2Interno, nombreIndicador3Interno;

        //Campos tipo texto
        TextInputLayout textArea1, textArea2, textArea3;

        RelativeLayout selector1, selector2, selector3, selector4;
        TextView selector1Text, selector2Text, selector3Text, selector4Text;
        TextView selector1Title, selector2Title, selector3Title, selector4Title;
        ImageView imagenSelector1, imagenSelector2, imagenSelector3, imagenSelector4;
        BuscadorSelectorFragment buscadorSelector;
        public string selector;//PARA EL SELECTOR
        float idSelect1 = 0, idSelect2 = 0, idSelect3 = 0, idSelect4 = 0;
        int error;

        TecladoFragment tecladoFragment;
        Button guardarIndicadores;
        Indicadores[] ListaIndicadores;

        //PARA LA CAMARA
        string camara;
        String[] paths1 = { "", "", "" };
        String[] paths2 = { "", "", "" };

        //INDICADOR DE FOTOS
        int indicadorFotos = -1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            idCaptura = Intent.GetIntExtra("idCaptura", 0);
            idEdad = Intent.GetIntExtra("idEdad", 0);
            idTipo = Intent.GetIntExtra("idTipo", 0);

            SetContentView(Resource.Layout.Indicadores);

            IndicadoresLinear = (LinearLayout)FindViewById(Resource.Id.IndicadoresLinear);

            volverIndicadores = (ImageView)FindViewById(Resource.Id.volverIndicadores);
            volverIndicadores.Click += VolverIndicadores_Click;

            //INPUTS
            eventIndicador1 = (RelativeLayout)FindViewById(Resource.Id.eventIndicador1);
            eventIndicador2 = (RelativeLayout)FindViewById(Resource.Id.eventIndicador2);
            eventIndicador3 = (RelativeLayout)FindViewById(Resource.Id.eventIndicador3);
            eventIndicador4 = (RelativeLayout)FindViewById(Resource.Id.eventIndicador4);

            nombreIndicador1 = (TextView)FindViewById(Resource.Id.nombreIndicador1);
            nombreIndicador2 = (TextView)FindViewById(Resource.Id.nombreIndicador2);
            nombreIndicador3 = (TextView)FindViewById(Resource.Id.nombreIndicador3);
            nombreIndicador4 = (TextView)FindViewById(Resource.Id.nombreIndicador4);

            nombreIndicador2Interno = (RelativeLayout)FindViewById(Resource.Id.nombreIndicador2Interno);
            nombreIndicador3Interno = (RelativeLayout)FindViewById(Resource.Id.nombreIndicador3Interno);

            textIndicador1 = (TextView)FindViewById(Resource.Id.textIndicador1);
            textIndicador2 = (TextView)FindViewById(Resource.Id.textIndicador2);
            textIndicador3 = (TextView)FindViewById(Resource.Id.textIndicador3);
            textIndicador4 = (TextView)FindViewById(Resource.Id.textIndicador4);

            imagenIndicador1 = (ImageView)FindViewById(Resource.Id.imagenIndicador1);
            imagenIndicador2 = (ImageView)FindViewById(Resource.Id.imagenIndicador2);
            imagenIndicador3 = (ImageView)FindViewById(Resource.Id.imagenIndicador3);
            imagenIndicador4 = (ImageView)FindViewById(Resource.Id.imagenIndicador4);

            //SELECTORES
            selector1 = (RelativeLayout)FindViewById(Resource.Id.selector1);
            selector2 = (RelativeLayout)FindViewById(Resource.Id.selector2);
            selector3 = (RelativeLayout)FindViewById(Resource.Id.selector3);
            selector4 = (RelativeLayout)FindViewById(Resource.Id.selector4);

            selector1Text = (TextView)FindViewById(Resource.Id.selector1Text);
            selector2Text = (TextView)FindViewById(Resource.Id.selector2Text);
            selector3Text = (TextView)FindViewById(Resource.Id.selector3Text);
            selector4Text = (TextView)FindViewById(Resource.Id.selector4Text);

            selector1Title = (TextView)FindViewById(Resource.Id.selector1Title);
            selector2Title = (TextView)FindViewById(Resource.Id.selector2Title);
            selector3Title = (TextView)FindViewById(Resource.Id.selector3Title);
            selector4Title = (TextView)FindViewById(Resource.Id.selector4Title);

            imagenSelector1 = (ImageView)FindViewById(Resource.Id.imagenSelector1);
            imagenSelector2 = (ImageView)FindViewById(Resource.Id.imagenSelector2);
            imagenSelector3 = (ImageView)FindViewById(Resource.Id.imagenSelector3);
            imagenSelector4 = (ImageView)FindViewById(Resource.Id.imagenSelector4);

            //Campos tipo texto
            textArea1 = (TextInputLayout)FindViewById(Resource.Id.textArea1);
            textArea2 = (TextInputLayout)FindViewById(Resource.Id.textArea2);
            textArea3 = (TextInputLayout)FindViewById(Resource.Id.textArea3);

            CreateIndicadores();

            guardarIndicadores = (Button)FindViewById(Resource.Id.guardarIndicadores);
            guardarIndicadores.Click += GuardarIndicadores_Click;
        }

        public override void OnBackPressed()
        {
            VolverIndicadores();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (idEdad == 1 || idEdad == 2 || idEdad == 6)//10 SEMANAS, 7 SEMANAS Y PLANTA JOVEN
            {
                indicadorFotos = data.GetIntExtra("index", -1);
                if (indicadorFotos != -1)
                {
                    imagenIndicador3.SetImageResource(Resource.Drawable.camara_verde);
                }
            }
            else if (idEdad == 3)//0 SEMANAS
            {
                indicadorFotos = data.GetIntExtra("index", -1);
                if (indicadorFotos != -1)
                {
                    imagenIndicador2.SetImageResource(Resource.Drawable.camara_verde);
                }
            }
            else
            {

                String[] paths = data.GetStringArrayExtra("paths");

                int countPaths = 0;

                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        countPaths++;
                    }
                }

                if (camara == "uno")
                {
                    paths1 = paths;

                    //Todos los espacios de imagenes estan llenos
                    if (countPaths == 3)
                    {
                        imagenIndicador1.SetImageResource(Resource.Drawable.camara_verde);
                    }
                    //Dos espacios de imagenes estan llenos
                    else if (countPaths == 2)
                    {
                        imagenIndicador1.SetImageResource(Resource.Drawable.camara_naranja);
                    }
                    //Un espacio de imagen esta lleno
                    else if (countPaths == 1)
                    {
                        imagenIndicador1.SetImageResource(Resource.Drawable.camara_rojo);
                    }
                    //No hay imagenes puestas
                    else
                    {
                        imagenIndicador1.SetImageResource(Resource.Drawable.camara_azul);
                    }
                }
                else if (camara == "dos")
                {
                    paths2 = paths;

                    //Todos los espacios de imagenes estan llenos
                    if (countPaths == 3)
                    {
                        imagenSelector2.SetImageResource(Resource.Drawable.camara_verde);
                    }
                    //Dos espacios de imagenes estan llenos
                    else if (countPaths == 2)
                    {
                        imagenSelector2.SetImageResource(Resource.Drawable.camara_naranja);
                    }
                    //Un espacio de imagen esta lleno
                    else if (countPaths == 1)
                    {
                        imagenSelector2.SetImageResource(Resource.Drawable.camara_rojo);
                    }
                    //No hay imagenes puestas
                    else
                    {
                        imagenIndicador1.SetImageResource(Resource.Drawable.camara_azul);
                    }
                }
                else { }
            }
        }

        private void VolverIndicadores_Click(object sender, System.EventArgs e)
        {
            VolverIndicadores();
        }

        private void VolverIndicadores()
        {
            Android.Support.V7.App.AlertDialog.Builder cancelar = new Android.Support.V7.App.AlertDialog.Builder(this);
            cancelar.SetMessage("¿Desea guardar los indicadores antes de volver atrás?");
            cancelar.SetTitle("Volver atrás");

            cancelar.SetPositiveButton("Si", (alert, args) =>
            {
                ValidacionesInidicadores();

                if (error == 0)
                {
                    GuardarBD();
                }
            });

            cancelar.SetNegativeButton("No", (alert, args) =>
            {
                NoGuardar();
            });

            cancelar.Show();
        }

        private async void NoGuardar()
        {
            if (idTipo != 1)
            {
                await DB.DeletePunto(idCaptura);
            }
            
            Finish();
        }

        private void CreateIndicadores()
        {
            //INDICADORES PARA EDADES
            if (idEdad == 1 || idEdad == 2)//10 Y 7 SEMANAS
            {
                //SOLO EXISTEN 3 INDICADORES BORRO LOS DEMAS
                IndicadoresLinear.RemoveViewAt(13);
                IndicadoresLinear.RemoveViewAt(12);
                IndicadoresLinear.RemoveViewAt(11);
                IndicadoresLinear.RemoveViewAt(10);
                IndicadoresLinear.RemoveViewAt(9);
                IndicadoresLinear.RemoveViewAt(8);
                IndicadoresLinear.RemoveViewAt(7);
                IndicadoresLinear.RemoveViewAt(6);
                IndicadoresLinear.RemoveViewAt(5);
                IndicadoresLinear.RemoveViewAt(1);
                IndicadoresLinear.RemoveViewAt(0);

                //SE RENOMBRAN LOS INDICADORES
                nombreIndicador1.Text = "HF";
                nombreIndicador2.Text = "YLS";
                nombreIndicador3.Text = "CF";

                //SIN LAYOUT INTERNO
                nombreIndicador3Interno.Visibility = Android.Views.ViewStates.Invisible;

                //SIN CAMARAS
                imagenIndicador1.Visibility = Android.Views.ViewStates.Invisible;
                imagenIndicador2.Visibility = Android.Views.ViewStates.Invisible;

                //SE ASIGNAN LOS EVENTOS PARA DESPLEGAR TECLADOS
                eventIndicador1.Click += EventIndicador1_Click;
                eventIndicador2.Click += EventIndicador2_Click;

                //EVENTO DE CAMARAS
                imagenIndicador3.Click += ImagenIndicador3_Click;

                //COMENTARIO FRAGMENT
                textArea3.EditText.Touch += EditText_Touch;
            }
            else if (idEdad == 3)//0 SEMANAS
            {
                //SOLO EXISTEN 4 INDICADORES BORRO LOS DEMAS
                IndicadoresLinear.RemoveViewAt(13);
                IndicadoresLinear.RemoveViewAt(12);
                IndicadoresLinear.RemoveViewAt(11);
                IndicadoresLinear.RemoveViewAt(10);
                IndicadoresLinear.RemoveViewAt(9);
                IndicadoresLinear.RemoveViewAt(8);
                IndicadoresLinear.RemoveViewAt(7);
                IndicadoresLinear.RemoveViewAt(6);
                IndicadoresLinear.RemoveViewAt(1);
                IndicadoresLinear.RemoveViewAt(0);

                //SE RENOMBRAN LOS INDICADORES
                nombreIndicador1.Text = "YLS";
                nombreIndicador2.Text = "CF";
                nombreIndicador3.Text = "TH";
                nombreIndicador4.Text = "YLI";

                //SIN LAYOUT INTERNO
                nombreIndicador2Interno.Visibility = Android.Views.ViewStates.Invisible;

                //SIN CAMARAS
                imagenIndicador1.Visibility = Android.Views.ViewStates.Invisible;
                imagenIndicador3.Visibility = Android.Views.ViewStates.Invisible;
                imagenIndicador4.Visibility = Android.Views.ViewStates.Invisible;

                //SE ASIGNAN LOS EVENTOS PARA DESPLEGAR TECLADOS
                eventIndicador1.Click += EventIndicador1_Click;
                eventIndicador3.Click += EventIndicador3_Click;
                eventIndicador4.Click += EventIndicador4_Click;

                //EVENTO DE CAMARAS
                imagenIndicador2.Click += ImagenIndicador2_Click;

                //COMENTARIO FRAGMENT
                textArea3.EditText.Touch += EditText_Touch;
            }
            else if (idEdad == 6)//PLANTA JOVEN
            {
                //SOLO EXISTEN 2 INDICADORES BORRO LOS DEMAS
                IndicadoresLinear.RemoveViewAt(11);
                IndicadoresLinear.RemoveViewAt(8);
                IndicadoresLinear.RemoveViewAt(5);
                IndicadoresLinear.RemoveViewAt(1);
                IndicadoresLinear.RemoveViewAt(0);

                //SE RENOMBRAN LOS INDICADORES
                nombreIndicador1.Text = "TH";
                nombreIndicador2.Text = "EFA";
                nombreIndicador3.Text = "CF";
                selector2Text.Text = "H2";
                selector2Title.Text = "H2";
                selector3Text.Text = "H3";
                selector3Title.Text = "H3";
                selector4Text.Text = "H4";
                selector4Title.Text = "H4";

                //SIN LAYOUT INTERNO
                nombreIndicador3Interno.Visibility = Android.Views.ViewStates.Invisible;

                //SIN CAMARAS
                imagenIndicador1.Visibility = Android.Views.ViewStates.Invisible;
                imagenIndicador2.Visibility = Android.Views.ViewStates.Invisible;
                imagenSelector2.Visibility = Android.Views.ViewStates.Invisible;
                imagenSelector3.Visibility = Android.Views.ViewStates.Invisible;
                imagenSelector4.Visibility = Android.Views.ViewStates.Invisible;

                //SE ASIGNAN LOS EVENTOS PARA DESPLEGAR TECLADOS
                eventIndicador1.Click += EventIndicador1_Click;
                eventIndicador2.Click += EventIndicador2_Click;

                // SE ASIGNAN LOS EVENTOS PARA SELECTORES
                selector2.Click += Selector2_Click;
                selector3.Click += Selector3_Click;
                selector4.Click += Selector4_Click;

                //EVENTO DE CAMARAS
                imagenIndicador3.Click += ImagenIndicador3_Click;

                //COMENTARIO FRAGMENT
                textArea3.EditText.Touch += EditText_Touch;
            }
            else
            {
                if (idTipo == 2)//ENFERMEDADES VASCULARES
                {
                    //SOLO EXISTEN 3 INDICADORES BORRO LOS DEMAS
                    IndicadoresLinear.RemoveViewAt(14);
                    IndicadoresLinear.RemoveViewAt(13);
                    IndicadoresLinear.RemoveViewAt(12);
                    IndicadoresLinear.RemoveViewAt(11);
                    IndicadoresLinear.RemoveViewAt(8);
                    IndicadoresLinear.RemoveViewAt(5);
                    IndicadoresLinear.RemoveViewAt(4);
                    IndicadoresLinear.RemoveViewAt(3);
                    IndicadoresLinear.RemoveViewAt(2);

                    //SE RENOMBRAN LOS INDICADORES
                    selector1Text.Text = "Fusarium";
                    selector1Title.Text = "Fusarium";
                    selector2Text.Text = "Moko";
                    selector2Title.Text = "Moko";
                    selector3Text.Text = "Erwinia";
                    selector3Title.Text = "Erwinia";

                    //SIN CAMARAS
                    imagenSelector1.Visibility = Android.Views.ViewStates.Invisible;
                    imagenSelector2.Visibility = Android.Views.ViewStates.Invisible;
                    imagenSelector3.Visibility = Android.Views.ViewStates.Invisible;

                    //SE ASIGNAN LOS EVENTOS PARA DESPLEGAR TECLADOS
                    selector1.Click += Selector1_Click;
                    selector2.Click += Selector2_Click;
                    selector3.Click += Selector3_Click;
                }
                else if (idTipo == 3)//CONDICIONES CULTURALES
                {
                    //SOLO EXISTEN 3 INDICADORES BORRO LOS DEMAS
                    IndicadoresLinear.RemoveViewAt(14);
                    IndicadoresLinear.RemoveViewAt(13);
                    IndicadoresLinear.RemoveViewAt(12);
                    IndicadoresLinear.RemoveViewAt(5);
                    IndicadoresLinear.RemoveViewAt(4);
                    IndicadoresLinear.RemoveViewAt(3);
                    IndicadoresLinear.RemoveViewAt(1);
                    IndicadoresLinear.RemoveViewAt(0);

                    //SE RENOMBRAN LOS INDICADORES
                    nombreIndicador1.Text = "NF [cm]";
                    selector2Text.Text = "FIT";
                    selector2Title.Text = "FIT";
                    selector3Text.Text = "RTI";
                    selector3Title.Text = "RTI";

                    //SIN CAMARAS
                    imagenSelector3.Visibility = Android.Views.ViewStates.Invisible;

                    //SE ASIGNAN LOS EVENTOS PARA DESPLEGAR TECLADOS
                    eventIndicador1.Click += EventIndicador1_Click;
                    selector2.Click += Selector2_Click;
                    selector3.Click += Selector3_Click;

                    //EVENTO DE CAMARAS
                    imagenIndicador1.Click += ImagenIndicador1_Click;
                    imagenSelector2.Click += ImagenSelector2_Click;

                    //COMENTARIO FRAGMENT
                    textArea1.EditText.Touch += EditText_Touch1;
                    textArea2.EditText.Touch += EditText_Touch2;
                }
            }
        }

        private void EditText_Touch1(object sender, View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("IndicadoresActivity", "Comentario FIT", textArea1.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        private void EditText_Touch2(object sender, View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("IndicadoresActivity", "Comentario RTI", textArea2.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        private void EditText_Touch(object sender, Android.Views.View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("IndicadoresActivity", "Lote", textArea3.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        public void ComentarioFragment(string comentario, string titulo)
        {
            if (titulo == "Lote")
            {
                textArea3.EditText.Text = comentario;
            }
            else if (titulo == "Comentario FIT") 
            {
                textArea1.EditText.Text = comentario;
            }
            else if (titulo == "Comentario RTI")
            {
                textArea2.EditText.Text = comentario;
            }

            comentarioFragment.Dismiss();
        }

        private void ImagenIndicador1_Click(object sender, EventArgs e)
        {
            camara = "uno";

            Intent intent = new Intent(this, typeof(CamaraActivity));
            intent.PutExtra("paths", paths1);
            StartActivityForResult(intent, 0);

        }

        private void ImagenIndicador2_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(IndicadorFotoActivity));
            intent.PutExtra("index", indicadorFotos);
            intent.PutExtra("idEdad", idEdad);
            StartActivityForResult(intent, 0);
        }

        private void ImagenIndicador3_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(IndicadorFotoActivity));
            intent.PutExtra("index", indicadorFotos);
            intent.PutExtra("idEdad", idEdad);
            StartActivityForResult(intent, 0);
        }

        private void ImagenSelector2_Click(object sender, EventArgs e)
        {
            camara = "dos";

            Intent intent = new Intent(this, typeof(CamaraActivity));
            intent.PutExtra("paths", paths2);
            StartActivityForResult(intent, 0);
        }

        private void EventIndicador1_Click(object sender, System.EventArgs e)
        {
            indicador = "Indicador 1";
            DesplegarTeclado(nombreIndicador1.Text);
        }

        private void EventIndicador2_Click(object sender, System.EventArgs e)
        {
            indicador = "Indicador 2";
            DesplegarTeclado(nombreIndicador2.Text);
        }

        private void EventIndicador3_Click(object sender, System.EventArgs e)
        {
            indicador = "Indicador 3";
            DesplegarTeclado(nombreIndicador3.Text);
        }

        private void EventIndicador4_Click(object sender, System.EventArgs e)
        {
            indicador = "Indicador 4";
            DesplegarTeclado(nombreIndicador4.Text);
        }

        private void Selector1_Click(object sender, System.EventArgs e)
        {
            selector = "Fusarium";
            buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector Fusarium");
        }

        private void Selector2_Click(object sender, System.EventArgs e)
        {
            if (idEdad == 7 && idTipo == 2)
            {
                selector = "Moko";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector Moko");
            }
            else if (idEdad == 7 && idTipo == 3)
            {
                selector = "FIT";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector FIT");
            }
            else if (idEdad == 6 && idTipo == 1)
            {
                selector = "H2";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector H2");
            }
        }

        private void Selector3_Click(object sender, System.EventArgs e)
        {
            if (idEdad == 7 && idTipo == 2)
            {
                selector = "Erwinia";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector Erwinia");
            }
            else if (idEdad == 7 && idTipo == 3)
            {
                selector = "RTI";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector RTI");
            }
            else if (idEdad == 6 && idTipo == 1)
            {
                selector = "H3";
                buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
                var trans = SupportFragmentManager.BeginTransaction();
                buscadorSelector.Show(trans, "Selector H3");
            }
        }

        private void Selector4_Click(object sender, EventArgs e)
        {
            selector = "H4";
            buscadorSelector = new BuscadorSelectorFragment("IndicadoresActivity");
            var trans = SupportFragmentManager.BeginTransaction();
            buscadorSelector.Show(trans, "Selector H4");
        }

        public void DesplegarTeclado(string nombre)
        {
            tecladoFragment = new TecladoFragment("IndicadoresActivity", nombre);
            var trans = SupportFragmentManager.BeginTransaction();
            tecladoFragment.Show(trans, "Teclado");
        }

        public void Tecladofragment_OnBackTeclado()
        {
            //Cierra fragmento del teclado
            tecladoFragment.Dismiss();
        }

        public void Tecladofragment_OnValueRegistered(float valor)
        {
            //Asigna valor del teclado al input de la variable correspondiente
            if (indicador == "Indicador 1")
            {
                textIndicador1.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "Indicador 2")
            {
                textIndicador2.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "Indicador 3")
            {
                textIndicador3.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else if (indicador == "Indicador 4")
            {
                textIndicador4.Text = valor.ToString(CultureInfo.InvariantCulture);
            }
            else { }

            //Cierra fragmento del teclado
            tecladoFragment.Dismiss();
        }

        private void GuardarIndicadores_Click(object sender, System.EventArgs e)
        {
            ValidacionesInidicadores();

            if (error == 0)
            {
                Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
                terminar.SetMessage("¿Está seguro que desea finalizar la captura de indicadores?");
                terminar.SetTitle("Finalizar captura");

                terminar.SetPositiveButton("Si", (alert, args) =>
                {
                    GuardarBD();
                });

                terminar.SetNegativeButton("No", (alert, args) =>
                {

                });

                terminar.Show();
            }
        }

        //VERIFICA QUE EL GPS ESTE ENCENDIDO
        public static bool isGPSProvider(Context context)
        {
            LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
            return lm.IsProviderEnabled(LocationManager.GpsProvider);
        }

        private void ValidacionesInidicadores()
        {
            error = 0;

            if (!isGPSProvider(this))
            {
                ToastFragment.ShowMakeText(this, "Es necesario que active el GPS para poder continuar");
                error = 1;
            }

            //INDICADORES PARA EDADES
            if (idEdad == 1 || idEdad == 2)//10 Y 7 SEMANAS
            {
                ListaIndicadores = new Indicadores[4];
                ListaIndicadores[0] = new Indicadores(1, float.Parse(textIndicador1.Text, CultureInfo.InvariantCulture));//HF
                ListaIndicadores[1] = new Indicadores(2, float.Parse(textIndicador2.Text, CultureInfo.InvariantCulture));//YLS
                ListaIndicadores[2] = new Indicadores(7, indicadorFotos + 1);//CF
                ListaIndicadores[3] = new Indicadores(51, textArea3.EditText.Text);//LOTE
            }
            else if (idEdad == 3)//0 SEMANAS
            {
                ListaIndicadores = new Indicadores[5];
                ListaIndicadores[0] = new Indicadores(2, float.Parse(textIndicador1.Text, CultureInfo.InvariantCulture));//YLS
                ListaIndicadores[1] = new Indicadores(7, indicadorFotos + 1);//CF
                ListaIndicadores[2] = new Indicadores(4, float.Parse(textIndicador3.Text, CultureInfo.InvariantCulture));//TH
                ListaIndicadores[3] = new Indicadores(5, float.Parse(textIndicador4.Text, CultureInfo.InvariantCulture));//YLI
                ListaIndicadores[4] = new Indicadores(51, textArea3.EditText.Text);//LOTE
            }
            else if (idEdad == 6)//PLANTA JOVEN
            {
                ListaIndicadores = new Indicadores[7];
                ListaIndicadores[0] = new Indicadores(4, float.Parse(textIndicador1.Text, CultureInfo.InvariantCulture));//TH
                ListaIndicadores[1] = new Indicadores(8, float.Parse(textIndicador2.Text, CultureInfo.InvariantCulture));//EFA
                ListaIndicadores[2] = new Indicadores(7, indicadorFotos + 1);//CF
                ListaIndicadores[3] = new Indicadores(48, idSelect2);//H2
                ListaIndicadores[4] = new Indicadores(49, idSelect3);//H3
                ListaIndicadores[5] = new Indicadores(50, idSelect4);//H4
                ListaIndicadores[6] = new Indicadores(51, textArea3.EditText.Text);//LOTE
            }
            else
            {
                if (idTipo == 2)//ENFERMEDADES VASCULARES
                {
                    if (idSelect1 == 0)
                    {
                        error = 1;
                        ToastFragment.ShowMakeText(this, "Por favor seleccione un indicador fusarium");
                    }
                    else if (idSelect2 == 0)
                    {
                        error = 1;
                        ToastFragment.ShowMakeText(this, "Por favor seleccione un indicador moko");
                    }
                    else if (idSelect3 == 0)
                    {
                        error = 1;
                        ToastFragment.ShowMakeText(this, "Por favor seleccione un indicador erwinia");
                    }
                    else
                    {
                        ListaIndicadores = new Indicadores[3];
                        ListaIndicadores[0] = new Indicadores(9, idSelect1);//FUSARIUM
                        ListaIndicadores[1] = new Indicadores(10, idSelect2);//MOKO
                        ListaIndicadores[2] = new Indicadores(11, idSelect3);//ERWINIA
                    }
                }
                else if (idTipo == 3)//CONDICIONES CULTURALES
                {
                    ListaIndicadores = new Indicadores[5];
                    ListaIndicadores[0] = new Indicadores(12, float.Parse(textIndicador1.Text, CultureInfo.InvariantCulture), paths1);//NF
                    ListaIndicadores[1] = new Indicadores(13, idSelect2, paths2);//FIT
                    ListaIndicadores[2] = new Indicadores(14, idSelect3);//RTI
                    ListaIndicadores[3] = new Indicadores(52, textArea1.EditText.Text);//COMENTARIO FIT
                    ListaIndicadores[4] = new Indicadores(53, textArea2.EditText.Text);//COMENTARIO RTI
                }
            }
        }

        private async void GuardarBD()
        {
            ShowProgressDialogue("Por favor permanezca en esta ubicación ...");

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(30));
            var userLocation = await Geolocation.GetLocationAsync(request);

            //si es posible leer ubicacion, se guarda la ubicacion de donde se esta tomando la muestra, si no se guarda ubicacion en ceros
            if (userLocation != null)
            {
                await DB.SavePlantaIndicadores(idCaptura, idEdad, userLocation.Latitude.ToString(CultureInfo.InvariantCulture), userLocation.Longitude.ToString(CultureInfo.InvariantCulture), ListaIndicadores);
            }
            else
            {
                await DB.SavePlantaIndicadores(idCaptura, idEdad, "0", "0", ListaIndicadores);
            }

            CloseProgressDialogue();
            Finish();
        }

        public void optionSelected(int id, string nombre)
        {
            if (selector == "H2")
            {
                selector2Text.Text = nombre;
                idSelect2 = id;
            }
            else if (selector == "H3")
            {
                selector3Text.Text = nombre;
                idSelect3 = id;
            }
            if (selector == "H4")
            {
                selector4Text.Text = nombre;
                idSelect4 = id;
            }
            else if (selector == "Fusarium")
            {
                selector1Text.Text = nombre;
                idSelect1 = id;
            }
            else if (selector == "Moko")
            {
                selector2Text.Text = nombre;
                idSelect2 = id;
            }
            else if (selector == "Erwinia")
            {
                selector3Text.Text = nombre;
                idSelect3 = id;
            }
            else if (selector == "FIT")
            {
                selector2Text.Text = nombre;
                idSelect2 = id;
            }
            else if (selector == "RTI")
            {
                selector3Text.Text = nombre;
                idSelect3 = id;
            }
            else { }

            buscadorSelector.Dismiss();
        }

        public void ShowProgressDialogue(string status)
        {

            progressDialogue = new ProgressFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "Progress");
        }

        public void CloseProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.DismissAllowingStateLoss();
                progressDialogue = null;
            }
        }
    }
}