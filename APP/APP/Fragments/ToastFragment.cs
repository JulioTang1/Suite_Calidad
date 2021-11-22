using Android.App;
using Android.Views;
using Android.Widget;
using APP;
using System;

namespace AppDemo.LocalLogic.Componentes
{
    class ToastFragment
    {
        public static void ShowMakeText(Activity context, String text)
        {
            View layout = context.LayoutInflater.Inflate(Resource.Layout.Toast, null);
            TextView textToast = layout.FindViewById<TextView>(Resource.Id.text);
            textToast.Text = text;

            Toast toast = new Toast(context);
            toast.SetGravity(GravityFlags.CenterHorizontal, 0, 0);
            toast.View = layout;

            toast.Duration = ToastLength.Long;

            toast.Show();
        }
    }
}