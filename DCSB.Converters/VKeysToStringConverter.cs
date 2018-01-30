using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using DCSB.Utils;

namespace DCSB.Converters
{
    public class VKeysToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<VKey> keys)
            {
                return string.Join(" + ", keys);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
