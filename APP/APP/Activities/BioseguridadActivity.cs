using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using APP.Adapters;
using APP.Fragments;
using APP.Helpers;
using Com.Github.Aakira.Expandablelayout;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Essentials;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]    
    public class BioseguridadActivity : AppCompatActivity
    {
        ScrollView scroll;
        ProgressFragment progressDialogue;
        string opcion;
        int idFinca;
        int idVisita;

        RelativeLayout aspecto1, aspecto2, aspecto3, aspecto4;
        ExpandableRelativeLayout aspecto1Expandable, aspecto2Expandable, aspecto3Expandable, aspecto4Expandable;
        ListView listChequeos1, listChequeos2, listChequeos3, listChequeos4;
        TextInputLayout observaciones, recomendaciones, responsable;
        Button guardarBioseguridad;
        ImageView volverBioseguridad;
        ComentarioFragment comentarioFragment;

        //PARA LA CAMARA
        ImageView camara;
        int id_array_indicador;        

        //Arreglo que almacena el resultado de los chequeos
        Indicadores[] ListaIndicadores = {
            new Indicadores(15, 3, new String[3]),
            new Indicadores(16, 3, new String[3]),
            new Indicadores(17, 3, new String[3]),
            new Indicadores(18, 3, new String[3]),
            new Indicadores(19, 3, new String[3]),
            new Indicadores(20, 3, new String[3]),
            new Indicadores(21, 3, new String[3]),
            new Indicadores(22, 3, new String[3]),
            new Indicadores(23, 3, new String[3]),
            new Indicadores(24, 3, new String[3]),
            new Indicadores(25, 3, new String[3]),
            new Indicadores(26, 3, new String[3]),
            new Indicadores(27, 3, new String[3]),
            new Indicadores(28, 3, new String[3]),
            new Indicadores(29, 3, new String[3]),
            new Indicadores(30, 3, new String[3]),
            new Indicadores(31, 3, new String[3]),
            new Indicadores(32, 3, new String[3]),
            new Indicadores(33, 3, new String[3]),
            new Indicadores(34, 3, new String[3]),
            new Indicadores(35, 3, new String[3]),
            new Indicadores(36, 3, new String[3]),
            new Indicadores(37, 3, new String[3]),
            new Indicadores(38, 3, new String[3]),
            new Indicadores(39, 3, new String[3]),
            new Indicadores(40, 3, new String[3]),
            new Indicadores(41, 3, new String[3]),
            new Indicadores(42, 3, new String[3]),
            new Indicadores(43, 3, new String[3]),
            new Indicadores(44, 3, new String[3])
        };

        //Arreglo que almacena el resultado de los campos de texto
        ObservableCollection<String[]> ListaTextObservable;
        String[] ListaText = { "", "", "" };

        //Visualizacion de fotos tomadas en chequeos ya realizados
        ObservableCollection<String[]> pathsObj;
        String[] paths = { "", "", "" };

        //Cuestionarios
        ObservableCollection<ControlesBioseguridad> chequeos1;
        ObservableCollection<ControlesBioseguridad> chequeos2;
        ObservableCollection<ControlesBioseguridad> chequeos3;
        ObservableCollection<ControlesBioseguridad> chequeos4;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Bioseguridad);

            opcion = Intent.GetStringExtra("opcion");
            if (opcion == "llenar")
            {
                idFinca = Intent.GetIntExtra("idFinca", 0);
            }
            else if (opcion == "ver")
            {
                idVisita = Intent.GetIntExtra("idVisita", 0);
            }
            else { }            

            //Scroll
            scroll = (ScrollView)FindViewById(Resource.Id.scroll);

            //Botones del desplegable
            aspecto1 = (RelativeLayout)FindViewById(Resource.Id.aspecto1);
            aspecto2 = (RelativeLayout)FindViewById(Resource.Id.aspecto2);
            aspecto3 = (RelativeLayout)FindViewById(Resource.Id.aspecto3);
            aspecto4 = (RelativeLayout)FindViewById(Resource.Id.aspecto4);

            aspecto1.Click += Aspecto1_Click;
            aspecto2.Click += Aspecto2_Click;
            aspecto3.Click += Aspecto3_Click;
            aspecto4.Click += Aspecto4_Click;

            //Desplegables y contenido
            aspecto1Expandable = (ExpandableRelativeLayout)FindViewById(Resource.Id.aspecto1Expandable);
            aspecto2Expandable = (ExpandableRelativeLayout)FindViewById(Resource.Id.aspecto2Expandable);
            aspecto3Expandable = (ExpandableRelativeLayout)FindViewById(Resource.Id.aspecto3Expandable);
            aspecto4Expandable = (ExpandableRelativeLayout)FindViewById(Resource.Id.aspecto4Expandable);
            listChequeos1 = (ListView)FindViewById(Resource.Id.listChequeos1);
            listChequeos2 = (ListView)FindViewById(Resource.Id.listChequeos2);
            listChequeos3 = (ListView)FindViewById(Resource.Id.listChequeos3);
            listChequeos4 = (ListView)FindViewById(Resource.Id.listChequeos4);

            //Campos tipo texto
            observaciones = (TextInputLayout)FindViewById(Resource.Id.observaciones);
            recomendaciones = (TextInputLayout)FindViewById(Resource.Id.recomendaciones);
            responsable = (TextInputLayout)FindViewById(Resource.Id.responsable);

            //Boton para guardar cuestionario
            guardarBioseguridad = (Button)FindViewById(Resource.Id.guardarBioseguridad);
            guardarBioseguridad.Click += GuardarBioseguridad_Click;
            if (opcion == "ver")
            {
                ((LinearLayout)scroll.GetChildAt(0)).RemoveViewAt(7);
            }

            //Boton para volver atras en el cuestionario
            volverBioseguridad = (ImageView)FindViewById(Resource.Id.volverBioseguridad);
            volverBioseguridad.Click += VolverBioseguridad_Click;

            //Carga cuestionarios y cierra desplegables
            SetUpChequeos();                        
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            String[] paths = data.GetStringArrayExtra("paths");
            int aspecto = data.GetIntExtra("aspecto", 0);
            int position = data.GetIntExtra("position", 0);

            int countPaths = 0;

            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Length > 0)
                {
                    countPaths++;
                }
            }

            ListaIndicadores[id_array_indicador].paths = paths;

            //Todos los espacios de imagenes estan llenos
            if (countPaths == 3)
            {
                camara.SetImageResource(Resource.Drawable.camara_verde);
            }
            //Dos espacios de imagenes estan llenos
            else if (countPaths == 2)
            {
                camara.SetImageResource(Resource.Drawable.camara_naranja);
            }
            //Un espacio de imagen esta lleno
            else if (countPaths == 1)
            {
                camara.SetImageResource(Resource.Drawable.camara_rojo);
            }
            //No hay imagenes puestas
            else
            {
                camara.SetImageResource(Resource.Drawable.camara_azul);
            }

            //Se modifica adapter
            if (aspecto == 1)
            {
                chequeos1[position].camara = countPaths;
            }
            else if (aspecto == 2)
            {
                chequeos2[position].camara = countPaths;
            }
            else if (aspecto == 3)
            {
                chequeos3[position].camara = countPaths;
            }
            else if (aspecto == 4)
            {
                chequeos4[position].camara = countPaths;
            }
            else { }
        }

        public override void OnBackPressed()
        {
            VolverBioseguridad();
        }

        private void VolverBioseguridad_Click(object sender, EventArgs e)
        {
            VolverBioseguridad();
        }

        private void VolverBioseguridad()
        {
            if (opcion == "llenar")
            {
                Android.Support.V7.App.AlertDialog.Builder cancelar = new Android.Support.V7.App.AlertDialog.Builder(this);
                cancelar.SetMessage("¿Desea guardar el chequeo de bioseguridad antes de volver atrás?");
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
            else if (opcion == "ver")
            {
                Finish();
            }
            else { }
        }

        private async void SetUpChequeos()
        {
            //Cuestionario 1
            chequeos1 = new ObservableCollection<ControlesBioseguridad>();
            chequeos1.Add(new ControlesBioseguridad(15, "1. Hay zona de bioseguridad en la entrada de la finca con áreas limpias y sucias delimitadas y piso de fácil lavado", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(16, "2. La zona de bioseguridad tiene pediluvios para lavado(Cepillo) y con cubierta para desinfección de calzado", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(17, "3. La zona de bioseguridad tiene botas de caucho de diferente tallas (trabajadores y visitantes)", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(18, "4. La zona de bioseguridad tiene disponible overoles desechables para visitantes nivel de riesgo 1", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(19, "5. La zona de bioseguridad tiene lava manos con agua, jabón y toallas desechables", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(20, "6. La zona de bioseguridad tiene espacio de almacenamiento de producto desinfectante", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(21, "7. La zona de bioseguridad tiene hojas de seguridad y fichas técnicas del producto desinfectante", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(22, "8. La zona de bioseguridad tiene un área  de confinación de residuos solidos y líquidos", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(23, "9. Existe plataforma para lavado y desinfección de vehículos con superficie (concreto o gravilla)", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(24, "10. La finca esta cercada y/o cuenta con canales perimetrales para evitar la entrada de personas o animales", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(25, "11. Existen parqueaderos externos de vehículos, con concreto o gravilla antes de entrar a la finca", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(26, "12. El acceso a la finca esta aislado de las zonas productivas de la finca y con superficie (concreto o gravilla)", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(27, "13. Existen pediluvios activos de paso obligado en las áreas de acceso a los lotes", 3, 0, 1));
            chequeos1.Add(new ControlesBioseguridad(28, "14. Los accesos alternos tienen pediluvios de paso obligado con amonios cuaternarios al 1%", 3, 0, 1));

            //Cuestionario 2
            chequeos2 = new ObservableCollection<ControlesBioseguridad>();
            chequeos2.Add(new ControlesBioseguridad(29, "1. El personal que ingresa a finca deja su calzado personal en el área de bioseguridad", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(30, "2. Se realiza lavado y desinfección de botas de caucho en la entrada y salida de la finca", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(31, "3. Se realiza lavado y desinfección de vehículos que ingresan y salen de la finca", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(32, "4. Se dispone adecuadamente los residuos que se generan en finca (cosecha, solidos y líquidos)", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(33, "5. No se permite la entrada de herramientas personales , ni salidas de herramientas propias de la finca", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(34, "6. Se realiza desinfección de herramientas planta a planta con desinfectantes recomendados", 3, 0, 2));
            chequeos2.Add(new ControlesBioseguridad(35, "7. Empaca la fruta en cajas de primer uso y no tiene contacto con el suelo", 3, 0, 2));

            //Cuestionario 3
            chequeos3 = new ObservableCollection<ControlesBioseguridad>();
            chequeos3.Add(new ControlesBioseguridad(36, "1. La mezcla desinfectante es: DDAC (120 gr/lt) y/o Cloruro de benzalconio (100 gr/lt). Al 1% de concentración", 3, 0, 3));
            chequeos3.Add(new ControlesBioseguridad(37, "2. El calzado se sumerge 15 cm y su exposición al producto desinfectante es mayor a 40 segundos", 3, 0, 3));
            chequeos3.Add(new ControlesBioseguridad(38, "3. La solución desinfectante esta libre de componentes orgánicos ( cascarilla de arroz, etc…)", 3, 0, 3));
            chequeos3.Add(new ControlesBioseguridad(39, "4. Hay registros actualizados de chequeo de concentración de la solución desinfectante", 3, 0, 3));

            //Cuestionario 4
            chequeos4 = new ObservableCollection<ControlesBioseguridad>();
            chequeos4.Add(new ControlesBioseguridad(40, "1. Existen avisos de FocR4T y señalización para todas las áreas de Bioseguridad", 3, 0, 4));
            chequeos4.Add(new ControlesBioseguridad(41, "2. La finca socializa folleto de FocR4T a visitantes", 3, 0, 4));
            chequeos4.Add(new ControlesBioseguridad(42, "3. La Finca lleva registro de ingreso y salida de visitantes, vehículos y maquinaria", 3, 0, 4));
            chequeos4.Add(new ControlesBioseguridad(43, "4. Hay evidencia de capacitación de operarios sobre enfermedades vasculares", 3, 0, 4));
            chequeos4.Add(new ControlesBioseguridad(44, "5. Se lleva organizada la carpeta de bioseguridad (Chequeos, Visitas Bioseguridad, Reg. de siembras, capacitaciones, Reg. de exportación, f. técnicas, hojas seg, etc.)", 3, 0, 4));

            //Trae informacion de formulario diligenciado previamente
            if (opcion == "ver")
            {
                ListaTextObservable = new ObservableCollection<String[]>();
                await DB.BringChequeos(idVisita, chequeos1, chequeos2, chequeos3, chequeos4, ListaTextObservable);
                ListaText = ListaTextObservable[0];

                //Llenado de campos de texto y configuracion para solo visualizar
                observaciones.EditText.Text = ListaText[0];
                recomendaciones.EditText.Text = ListaText[1];
                responsable.EditText.Text = ListaText[2];

                //Se desactiva el input
                observaciones.EditText.Enabled = false;
                recomendaciones.EditText.Enabled = false;
                responsable.EditText.Enabled = false;
            }
            else 
            {
                observaciones.EditText.Touch += EditText_Touch;
                recomendaciones.EditText.Touch += EditText_Touch1;
                responsable.EditText.Touch += EditText_Touch2;
            }

            //Carga en interfaz los cuestionarios
            listChequeos1.Adapter = new ChequeoAdapter(this, chequeos1, opcion);
            listChequeos2.Adapter = new ChequeoAdapter(this, chequeos2, opcion);
            listChequeos3.Adapter = new ChequeoAdapter(this, chequeos3, opcion);
            listChequeos4.Adapter = new ChequeoAdapter(this, chequeos4, opcion);

            //Cierra desplegables
            aspecto1Expandable.Collapse();
            aspecto2Expandable.Collapse();
            aspecto3Expandable.Collapse();
            aspecto4Expandable.Collapse();
        }

        private void EditText_Touch(object sender, View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("BioseguridadActivity", "Observaciones", observaciones.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        private void EditText_Touch1(object sender, View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("BioseguridadActivity", "Recomendaciones", recomendaciones.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        private void EditText_Touch2(object sender, View.TouchEventArgs e)
        {
            //Entra solo si no va a usar el scroll
            if (e.Event.Action == MotionEventActions.Up)
            {
                comentarioFragment = new ComentarioFragment("BioseguridadActivity", "Responsable", responsable.EditText.Text);
                var trans = SupportFragmentManager.BeginTransaction();
                comentarioFragment.Show(trans, "Comentario");
            }
        }

        public void ComentarioFragment(string comentario, string titulo)
        {
            if (titulo == "Observaciones")
            {
                observaciones.EditText.Text = comentario;
            }
            else if (titulo == "Recomendaciones")
            {
                recomendaciones.EditText.Text = comentario;
            }
            else if (titulo == "Responsable")
            {
                responsable.EditText.Text = comentario;
            }
            else { }

            comentarioFragment.Dismiss();
        }

        private void Aspecto1_Click(object sender, System.EventArgs e)
        {
            //Cierra los desplegables no deseados
            aspecto2Expandable.Collapse();
            aspecto3Expandable.Collapse();
            aspecto4Expandable.Collapse();

            //Toggle el desplegable deseado
            aspecto1Expandable.Toggle();

            //Desplaza scroll a la parte superior del formulario
            scroll.ScrollTo(0, 0);
        }

        private void Aspecto2_Click(object sender, System.EventArgs e)
        {
            //Cierra los desplegables no deseados
            aspecto1Expandable.Collapse();
            aspecto3Expandable.Collapse();
            aspecto4Expandable.Collapse();

            //Toggle el desplegable deseado
            aspecto2Expandable.Toggle();

            //Desplaza scroll a la parte superior del formulario
            scroll.ScrollTo(0, 0);            
        }

        private void Aspecto3_Click(object sender, System.EventArgs e)
        {
            //Cierra los desplegables no deseados
            aspecto1Expandable.Collapse();
            aspecto2Expandable.Collapse();
            aspecto4Expandable.Collapse();

            //Toggle el desplegable deseado
            aspecto3Expandable.Toggle();

            //Desplaza scroll a la parte superior del formulario
            scroll.ScrollTo(0, 0);            
        }

        private void Aspecto4_Click(object sender, System.EventArgs e)
        {
            //Cierra los desplegables no deseados
            aspecto1Expandable.Collapse();
            aspecto2Expandable.Collapse();
            aspecto3Expandable.Collapse();

            //Toggle el desplegable deseado
            aspecto4Expandable.Toggle();

            //Desplaza scroll a la parte superior del formulario
            scroll.ScrollTo(0, 0);
        }

        public void SetChequeo(int id, int valor, int aspecto, int position)
        {
            //Se modifica adapter
            if (aspecto == 1)
            {
                chequeos1[position].respuesta = valor;
            }
            else if (aspecto == 2)
            {
                chequeos2[position].respuesta = valor;
            }
            else if (aspecto == 3)
            {
                chequeos3[position].respuesta = valor;
            }
            else if (aspecto == 4)
            {
                chequeos4[position].respuesta = valor;
            }
            else { }

            //Se modifica indicadores para base de datos
            ListaIndicadores[id - 15].valor = valor;
        }

        public void InvokeCamera(int id, View icono, int aspecto, int position)
        {
            //Almacena el icono que fue pulsado y la posicion en el arreglo de resultados
            camara = (ImageView)icono;
            id_array_indicador = id - 15;

            //Elimina campos NULL en el arreglo de rutas para las fotos
            ListaIndicadores[id - 15].paths[0] = ListaIndicadores[id - 15].paths[0] == null ? "" : ListaIndicadores[id - 15].paths[0];
            ListaIndicadores[id - 15].paths[1] = ListaIndicadores[id - 15].paths[1] == null ? "" : ListaIndicadores[id - 15].paths[1];
            ListaIndicadores[id - 15].paths[2] = ListaIndicadores[id - 15].paths[2] == null ? "" : ListaIndicadores[id - 15].paths[2];

            //Llamado a la actividad de la camara
            Intent intent = new Intent(this, typeof(CamaraActivity));
            intent.PutExtra("paths", ListaIndicadores[id - 15].paths);
            intent.PutExtra("aspecto", aspecto);
            intent.PutExtra("position", position);
            StartActivityForResult(intent, 0);
        }

        public async void VerFotos(int id)
        {
            pathsObj = new ObservableCollection<String[]>();
            await DB.BringFotoBioseguridad(idVisita, id, pathsObj);
            paths = pathsObj[0];
            Intent intent = new Intent(this, typeof(FotosActivity));
            intent.PutExtra("paths", paths);
            StartActivity(intent);
        }

        private void GuardarBioseguridad_Click(object sender, System.EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
            terminar.SetMessage("¿Está seguro que desea finalizar el chequeo de bioseguridad?");
            terminar.SetTitle("Finalizar chequeo de bioseguridad");

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
            ShowProgressDialogue("Por favor permanezca en esta ubicación ...");

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(30));
            var userLocation = await Geolocation.GetLocationAsync(request);

            //Configuracion de campos de texto para guardar en base de datos
            ListaText[0] = observaciones.EditText.Text;
            ListaText[1] = recomendaciones.EditText.Text;
            ListaText[2] = responsable.EditText.Text;

            //si es posible leer ubicacion, se guarda la ubicacion de donde se esta tomando la muestra, si no se guarda ubicacion en ceros
            if (userLocation != null)
            {
                await DB.SaveBioseguridad(idFinca, userLocation.Latitude.ToString(CultureInfo.InvariantCulture), userLocation.Longitude.ToString(CultureInfo.InvariantCulture), ListaIndicadores, ListaText);
            }
            else
            {
                await DB.SaveBioseguridad(idFinca, "0", "0", ListaIndicadores, ListaText);
            }

            CloseProgressDialogue();
            Finish();
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