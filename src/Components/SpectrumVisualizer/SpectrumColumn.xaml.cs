using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpectrumAnalyzer.Components
{


    /// <summary>
    /// Logica di interazione per SpectrumColumn.xaml
    /// </summary>
    public partial class SpectrumColumn : UserControl
    {
        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register("Frequency", typeof(string), typeof(SpectrumColumn),
            new PropertyMetadata(""));

        public string Frequency
        {
            get { return (string)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        public SpectrumColumn()
        {
            InitializeComponent();
        }
    }
}
