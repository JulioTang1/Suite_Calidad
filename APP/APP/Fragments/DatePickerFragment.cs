using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;

namespace APP.Fragments
{

    public class DatePickerFragment : DialogFragment,
                                     DatePickerDialog.IOnDateSetListener
    {
        public event EventHandler ChangeDate;

        public static DateTime currently = DateTime.Now;
        public static DateTime min;
        public static DateTime max;
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime _currently)
        {
            currently = _currently;
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime _currently, DateTime _min, DateTime _max)
        {
            currently = _currently;
            min = _min;
            max = _max;
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month - 1,
                                                           currently.Day);
            if (min != DateTime.Parse("0001-01-01"))
            {
                dialog.DatePicker.MinDate = (long)(min.Date - new DateTime(1970, 1, 1)).TotalMilliseconds + 1000 * 60 * 60 * 24 * 1;
                dialog.DatePicker.MaxDate = (long)(max.Date - new DateTime(1970, 1, 1)).TotalMilliseconds + 1000 * 60 * 60 * 24 * 1;
            }

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Log.Debug(TAG, selectedDate.ToString("dd/MM/yyyy"));
            _dateSelectedHandler(selectedDate);
            ChangeDate?.Invoke(this, new EventArgs());
        }
    }
}