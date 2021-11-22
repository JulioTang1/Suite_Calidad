using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;
using APP.Fragments;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;
using System.Collections.ObjectModel;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        ObservableCollection<int> maestros;

        TextInputLayout usuarioText, contraseñaText;
        Button loginButton;
        ProgressFragment progressDialogue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);

            usuarioText = (TextInputLayout)FindViewById(Resource.Id.usuarioText);
            contraseñaText = (TextInputLayout)FindViewById(Resource.Id.contraseñaText);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);

            loginButton.Click += LoginButton_Click;

            //Se lee el id del usuario
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            int idUsuario = prefs.GetInt("idUsuario", 0);

            //Si ya se encuentra logeado entra automaticamente
            if (idUsuario != 0)
            {
                CloseProgressDialogue();
                StartActivity(typeof(MainActivity));
                Finish();
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            //Validacion de existencia de datos maestros para poder ingresar a la app
            maestros = new ObservableCollection<int>();
            await DB.SincronizacionMaestros(maestros);
            if (maestros[0] != 0)
            {
                string usuario, contraseña;
                usuario = usuarioText.EditText.Text;
                contraseña = contraseñaText.EditText.Text;

                if (usuario.Length == 0)
                {
                    ToastFragment.ShowMakeText(this, "Por favor complete el campo usuario");
                    return;
                }
                else if (contraseña.Length == 0)
                {
                    ToastFragment.ShowMakeText(this, "Por favor complete el campo contraseña");
                    return;
                }

                await DB.Login(usuario, contraseña);

                //Se lee el id del usuario
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                int idUsuario = prefs.GetInt("idUsuario", 0);

                if (idUsuario != 0)
                {
                    StartActivity(typeof(MainActivity));
                    Finish();
                }
                else
                {
                    ToastFragment.ShowMakeText(this, "El usuario o la contraseña es incorrecta");
                }
            }
            else
            {
                ToastFragment.ShowMakeText(this, "La sincronización no ha sido exitosa, por favor reinicie la aplicación");
            }
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