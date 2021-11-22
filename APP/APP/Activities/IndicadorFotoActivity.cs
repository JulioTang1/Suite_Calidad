using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using System;

namespace APP.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class IndicadorFotoActivity : AppCompatActivity
    {
        int indexPath = 0;

        ImageView volverIndicadorFotos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            indexPath = Intent.GetIntExtra("index", -1);
            SetContentView(Resource.Layout.IndicadorFoto);

            ImageView im1 = FindViewById<ImageView>(Resource.Id.im1);
            ImageView im2 = FindViewById<ImageView>(Resource.Id.im2);
            ImageView im3 = FindViewById<ImageView>(Resource.Id.im3);
            ImageView im4 = FindViewById<ImageView>(Resource.Id.im4);

            im1.Click += async (sender, args) =>
            {
                indexPath = 0;
                selectedIm(indexPath);
            };

            im2.Click += async (sender, args) =>
            {
                indexPath = 1;
                selectedIm(indexPath);
            };

            im3.Click += async (sender, args) =>
            {
                indexPath = 2;
                selectedIm(indexPath);
            };

            im4.Click += async (sender, args) =>
            {
                indexPath = 3;
                selectedIm(indexPath);
            };

            volverIndicadorFotos = FindViewById<ImageView>(Resource.Id.volverIndicadorFotos);
            volverIndicadorFotos.Click += VolverIndicadorFotos_Click;

            if(indexPath != -1)
            {
                selectedIm(indexPath);
            }

            LoadImages();
        }

        public void selectedIm(int indexPath)
        {
            ImageView im1 = FindViewById<ImageView>(Resource.Id.im1);
            ImageView im2 = FindViewById<ImageView>(Resource.Id.im2);
            ImageView im3 = FindViewById<ImageView>(Resource.Id.im3);
            ImageView im4 = FindViewById<ImageView>(Resource.Id.im4);

            im1.SetPadding(0, 0, 0, 0);
            im2.SetPadding(0, 0, 0, 0);
            im3.SetPadding(0, 0, 0, 0);
            im4.SetPadding(0, 0, 0, 0);

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
            else if (indexPath == 2)
            {
                im3.SetBackgroundColor(Color.Blue);
                im3.SetPadding(5, 5, 5, 5);
            }
            else
            {
                im4.SetBackgroundColor(Color.Blue);
                im4.SetPadding(5, 5, 5, 5);
            }
        }


        public void LoadImages()
        {
            ImageView im1 = FindViewById<ImageView>(Resource.Id.im1);
            ImageView im2 = FindViewById<ImageView>(Resource.Id.im2);
            ImageView im3 = FindViewById<ImageView>(Resource.Id.im3);
            ImageView im4 = FindViewById<ImageView>(Resource.Id.im4);

            im1.SetImageResource(Resource.Drawable.CF_ideal);
            im2.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento);
            im3.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento_2);
            im4.SetImageResource(Resource.Drawable.CF_Sintomas_Arrepollamiento_3);
        }


        private void VolverIndicadorFotos_Click(object sender, EventArgs e)
        {
            VolverIndicadorFotos();
        }

        public override void OnBackPressed()
        {
            VolverIndicadorFotos();
        }

        private void VolverIndicadorFotos()
        {
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("index", indexPath);
            SetResult(Result.Ok, resultIntent);
            Finish();
        }
    }
}