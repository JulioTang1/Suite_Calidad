using Android.App;

namespace APP.Helpers
{
    public interface INotification
    {
        Notification ReturnNotif(string foregroundChannelId, string servicio);
    }
}