using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using APP.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace APP.Helpers
{
    internal class NotificationHelper : INotification
    {
        private static Context context = global::Android.App.Application.Context;

        public Notification ReturnNotif(string foregroundChannelId, string servicio)
        {
            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle(servicio + " activa")
                .SetSmallIcon(Resource.Drawable.IconoApp)
                .SetOngoing(true);

            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
                notificationChannel.Importance = NotificationImportance.High;
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });

                var notifManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notifManager != null)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }
    }
}