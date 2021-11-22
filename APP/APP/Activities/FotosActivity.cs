using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.IO;
using System;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class FotosActivity : AppCompatActivity
    {
        ImageView volverFotos;
        LinearLayout menuFotos;
        int indexPath = 0;
        String[] paths = { "", "", "" };
        int indicador;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            paths = Intent.GetStringArrayExtra("paths");
            indicador = Intent.GetIntExtra("indicador", 0);
            SetContentView(Resource.Layout.Fotos);


            ImageView im1 = FindViewById<ImageView>(Resource.Id.im1);
            ImageView im2 = FindViewById<ImageView>(Resource.Id.im2);
            ImageView im3 = FindViewById<ImageView>(Resource.Id.im3);

            im1.Click += async (sender, args) =>
            {
                indexPath = 0;
                loadImageFromUrl(paths[indexPath]);
                selectedIm(indexPath);
            };

            im2.Click += async (sender, args) =>
            {
                indexPath = 1;
                loadImageFromUrl(paths[indexPath]);
                selectedIm(indexPath);
            };

            im3.Click += async (sender, args) =>
            {
                indexPath = 2;
                loadImageFromUrl(paths[indexPath]);
                selectedIm(indexPath);
            };

            selectedIm(indexPath);

            LoadImages();

            volverFotos = (ImageView)FindViewById(Resource.Id.volverFotos);
            volverFotos.Click += VolverFotos_Click;

            menuFotos = (LinearLayout)FindViewById(Resource.Id.menuFotos);
            if (indicador == 7)
            {
                menuFotos.RemoveViewAt(0);
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
            else if (indicador == 7 && paths[0] == "1.0")
            {
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageResource(Resource.Drawable.CF_ideal);
            }
            else if (indicador == 7 && paths[0] == "2.0")
            {
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento);
            }
            else if (indicador == 7 && paths[0] == "3.0")
            {
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento_2);
            }
            else if (indicador == 7 && paths[0] == "4.0")
            {
                ImageView im = FindViewById<ImageView>(Resource.Id.imageViewPanel);
                im.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento_3);
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

        private void VolverFotos_Click(object sender, System.EventArgs e)
        {
            Finish();
        }
    }
}