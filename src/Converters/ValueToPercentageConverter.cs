using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SpectrumAnalyzer.Converters
{
    public class ValueToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var slider = parameter as Slider;
            double minimum = slider.Minimum;
            double maximum = slider.Maximum;

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
