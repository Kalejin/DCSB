using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace DCSB.Converters
{
    public class ConcatStringsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<string> strings && parameter is string separator)
            {
                return string.Join(separator, strings);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}