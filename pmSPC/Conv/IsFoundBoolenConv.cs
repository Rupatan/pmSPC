using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace pmSPC.Conv
{
    public class IsFoundBoolenConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            var ret = val ? Visibility.Collapsed : Visibility.Visible;

            bool result = false;
            if (Boolean.TryParse(parameter?.ToString(), out result))
                ret = result ? (ret == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed) : ret;

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
