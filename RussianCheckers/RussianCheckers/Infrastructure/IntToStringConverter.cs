using System;
using System.Globalization;
using System.Windows.Data;

namespace RussianCheckers.Infrastructure
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            int.TryParse(value?.ToString(), out result);
            return result;
        }
    }
}