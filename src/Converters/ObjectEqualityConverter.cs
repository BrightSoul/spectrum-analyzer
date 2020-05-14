using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SpectrumAnalyzer.Converters
{
    public class ObjectEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length <= 1)
            {
                return true;
            }
            bool equal = true;
            for (var i = 0; i < values.Length - 1; i++)
            {
                equal = equal && EqualityComparer<object>.Default.Equals(values[i], values[i+1]);
            }
            return equal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
