using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

namespace APP.Helpers
{
    [Service]
    public class AndroidLocationService : Service
    {
        private Location locShared;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            int idVisita = intent.GetIntExtra("idVisita", 0);

            bool isAlreadyStart;

            Notification notif = DependencyService.Get<INotification>().ReturnNotif("9001", "Geolocalización");
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

            try
            {
                locShared = new Location(idVisita);
                isAlreadyStart = locShared.getRunningStateLocationService();
                if (isAlreadyStart)
                {
                }
                else
                {
                    locShared.setRunningStateLocationService(true);
                    locShared.Run();
                }
            }
            catch (OperationCanceledException)
            {

            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            locShared.setRunningStateLocationService(false);
            base.OnDestroy();
        }
    }
}