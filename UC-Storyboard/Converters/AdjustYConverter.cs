using System;
using System.Globalization;
using System.Windows.Data;

namespace UC_Storyboard.Converters
{
    //Class is used in case we want to make any collection-wide modifications to Y-Positions (e.g. adjust all District Y-Positions 2 pixels to the north)
    //This is a left-over from before when I was trying to make ItemControl template items snap-to-grid, but then moved to Canvas for pin-point precision, so currently it just returns.
    public class AdjustYConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is int intValue)
            {
                return intValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
