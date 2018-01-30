using System;
using System.Globalization;
using System.Windows.Data;

namespace DCSB.Converters
{
    public class StringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string format && values[1] is object data)
            {
                try
                {
                    return string.Format(format, data);
                }
                catch (FormatException)
                {
                    return "Invalid format";
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
