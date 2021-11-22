using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Java.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CamaraActivity : AppCompatActivity
    {

        int indexPath = 0;
        String[] paths = { "", "", "" };

        //Variables para el formulario de bioseguridad
        int aspecto;
        int position;

        ImageView im1;
        ImageView im2;
        ImageView im3;

        bool procesandoImagen = false;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            paths = Intent.GetStringArrayExtra("paths");

            aspecto = Intent.GetIntExtra("aspecto", 0);
            position = Intent.GetIntExtra("position", 0);

            SetContentView(Resource.Layout.Camara);

            ImageView captureCamera = FindViewById<ImageView>(Resource.Id.botonCaptura);
            ImageView galeriaCapture = FindViewById<ImageView>(Resource.Id.seleccionarGaleria);
            ImageView eliminarCapture = FindViewById<ImageView>(Resource.Id.eliminarFoto);
            ImageView volverCamara = FindViewById<ImageView>(Resource.Id.volverCamara);

            volverCamara.Click += VolverCamara_Click;

            selectedIm(indexPath);

            captureCamera.Click += CaptureCamera_Click;
            galeriaCapture.Click += GaleriaCapture_Click;
            eliminarCapture.Click += EliminarCapture_Click;

            im1 = FindViewById<ImageView>(Resource.Id.im1);
            im2 = FindViewById<ImageView>(Resource.Id.im2);
            im3 = FindViewById<ImageView>(Resource.Id.im3);

            im1.Click += Im1_Click;
            im2.Click += Im2_Click;
            im3.Click += Im3_Click;

            //Carga imagenes ya tomadas anteriormente
            LoadImages();
        }

        private async void CaptureCamera_Click(object sender, EventArgs e)
        {
            procesandoImagen = true;
            im1.Click -= Im1_Click;
            im2.Click -= Im2_Click;
            im3.Click -= Im3_Click;

            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                Toast.MakeText(this, "Camara no disponible", ToastLength.Long).Show();
                return;
            }

            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Name = milliseconds.ToString() + ".png",
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            });

            if (file == null)
            {
                im1.Click += Im1_Click;
                im2.Click += Im2_Click;
                im3.Click += Im3_Click;
                procesandoImagen = false;
                
                return;
            }

            //Borra foto previa si existe
            if (paths[indexPath].Length > 0)
            {
                System.IO.File.Delete(paths[indexPath]);
            }

            loadImageFromUrl(file.Path);
            loadImageFromUrlPreviews(indexPath, file.Path);

            paths[indexPath] = file.Path;
            file.Dispose();

            im1.Click += Im1_Click;
            im2.Click += Im2_Click;
            im3.Click += Im3_Click;
            procesandoImagen = false;
        }

        private async void GaleriaCapture_Click(object sender, EventArgs e)
        {
            procesandoImagen = true;
            im1.Click -= Im1_Click;
            im2.Click -= Im2_Click;
            im3.Click -= Im3_Click;

            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                Toast.MakeText(this, "Camara no disponible", ToastLength.Long).Show();
                return;
            }

            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            });

            if (file == null)
            {
                im1.Click += Im1_Click;
                im2.Click += Im2_Click;
                im3.Click += Im3_Click;
                procesandoImagen = false;

                return;
            }

            //Borra foto previa si existe
            if (paths[indexPath].Length > 0)
            {
                System.IO.File.Delete(paths[indexPath]);
            }

            //Se mueve de carpeta temporal a oficial
            System.IO.File.Move(file.Path, String.Concat(file.Path.Substring(0, file.Path.LastIndexOf("temp/")), milliseconds.ToString(), file.Path.Substring(file.Path.LastIndexOf("."))));
            string newPath = String.Concat(file.Path.Substring(0, file.Path.LastIndexOf("temp/")), milliseconds.ToString(), file.Path.Substring(file.Path.LastIndexOf(".")));

            loadImageFromUrl(newPath);
            loadImageFromUrlPreviews(indexPath, newPath);

            paths[indexPath] = newPath;
            file.Dispose();

            im1.Click += Im1_Click;
            im2.Click += Im2_Click;
            im3.Click += Im3_Click;
            procesandoImagen = false;
        }

        private void EliminarCapture_Click(object sender, EventArgs e)
        {
            procesandoImagen = true;
            im1.Click -= Im1_Click;
            im2.Click -= Im2_Click;
            im3.Click -= Im3_Click;

            File img = new File(paths[indexPath]);
            img.Delete();

            paths[indexPath] = "";

            ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
            im.SetImageResource(Resource.Drawable.vacio);
            loadImageFromUrlPreviews(indexPath, paths[indexPath]);

            im1.Click += Im1_Click;
            im2.Click += Im2_Click;
            im3.Click += Im3_Click;
            procesandoImagen = false;
        }

        private void Im1_Click(object sender, EventArgs e)
        {
            indexPath = 0;
            loadImageFromUrl(paths[indexPath]);
            selectedIm(indexPath);
        }

        private void Im2_Click(object sender, EventArgs e)
        {
            indexPath = 1;
            loadImageFromUrl(paths[indexPath]);
            selectedIm(indexPath);
        }

        private void Im3_Click(object sender, EventArgs e)
        {
            indexPath = 2;
            loadImageFromUrl(paths[indexPath]);
            selectedIm(indexPath);
        }

        private void VolverCamara_Click(object sender, EventArgs e)
        {
            VolverCamara();
        }

        public override void OnBackPressed()
        {
            VolverCamara();
        }

        private void VolverCamara()
        {
            if (!procesandoImagen)
            {
                Intent resultIntent = new Intent();
                resultIntent.PutExtra("paths", paths);
                resultIntent.PutExtra("aspecto", aspecto);
                resultIntent.PutExtra("position", position);
                SetResult(Result.Ok, resultIntent);
                Finish();
            }
        }

        public void LoadImages()
        {
            //Carga miniaturas
            loadImageFromUrlPreviews(0, paths[0]);
            loadImageFromUrlPreviews(1, paths[1]);
            loadImageFromUrlPreviews(2, paths[2]);

            //Carga imagen principal
            loadImageFromUrl(paths[0]);
        }

        public void loadImageFromUrl(String imgFile)
        {
            File img = new File(imgFile);
            if (img.Exists())
            {
                Bitmap myBitmap = BitmapFactory.DecodeFile(img.AbsolutePath);
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageBitmap(myBitmap);
            }
            else
            {
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageResource(Resource.Drawable.vacio);
            }
        }

        public void loadImageFromUrlPreviews(int indexPath, String imgFile)
        {
            ImageView im;

            if (indexPath == 0)
            {
                im = FindViewById<ImageView>(Resource.Id.im1);
            }
            else if (indexPath == 1)
            {
                im = FindViewById<ImageView>(Resource.Id.im2);
            }
            else
            {
                im = FindViewById<ImageView>(Resource.Id.im3);
            }

            File img = new File(imgFile);
            if (img.Exists())
            {
                Bitmap myBitmap = BitmapFactory.DecodeFile(img.AbsolutePath);
                im.SetImageBitmap(myBitmap);
            }
            else
            {
                im.SetImageResource(Resource.Drawable.vacio);
            }
        }

        public void selectedIm(int indexPath)
        {
            ImageView im1 = FindViewById<ImageView>(Resource.Id.im1);
            ImageView im2 = FindViewById<ImageView>(Resource.Id.im2);
            ImageView im3 = FindViewById<ImageView>(Resource.Id.im3);

            im1.SetPadding(0, 0, 0, 0);
            im2.SetPadding(0, 0, 0, 0);
            im3.SetPadding(0, 0, 0, 0);

            if (indexPath == 0)
            {
                im1.SetBackgroundColor(Color.Blue);
                im1.SetPadding(5, 5, 5, 5);
            }
            else if (indexPath == 1)
            {
                im2.SetBackgroundColor(Color.Blue);
                im2.SetPadding(5, 5, 5, 5);
            }
            else
            {
                im3.SetBackgroundColor(Color.Blue);
                im3.SetPadding(5, 5, 5, 5);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}