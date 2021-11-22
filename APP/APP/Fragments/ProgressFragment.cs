using Android.OS;
using Android.Views;
using Android.Widget;

namespace APP.Fragments
{
    public class ProgressFragment : Android.Support.V4.App.DialogFragment
    {
        string status;

        public ProgressFragment(string _status)
        {
            status = _status;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView statusTextView = (TextView)view.FindViewById(Resource.Id.progressStatus);
            statusTextView.Text = status;
            return view;
        }
    }
}