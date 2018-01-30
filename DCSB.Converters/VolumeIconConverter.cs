using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shapes;

namespace DCSB.Converters
{
    public class VolumeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? volume = value as double?;
            IList<Path> icons = parameter as IList<Path>;
            
            if (volume.HasValue && icons != null)
            {
                if (volume == 0)
                {
                    return icons[0];
                }

                int index = (int)(((volume - 1) / 100) * (icons.Count - 1)) + 1;
                return icons[index];
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
