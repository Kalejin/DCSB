using DCSB.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DCSB.Converters
{
    public class DisplayOptionToEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayOption enable && parameter is DisplayOption thisDisplay)
            {
                if (enable != thisDisplay && enable != DisplayOption.Both)
                {
                    return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
