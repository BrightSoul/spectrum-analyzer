using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SpectrumAnalyzer.Converters
{
    public class ValueToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int limit = System.Convert.ToInt32(parameter);
            int minimum = Math.Min(limit, 0);
            int maximum = Math.Max(limit, 0);

            double currentValue = Math.Min(maximum, Math.Max(minimum, System.Convert.ToDouble(value)));
            double percentage = ((currentValue - minimum) / (double)(maximum - minimum)) * 100;
            return percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
