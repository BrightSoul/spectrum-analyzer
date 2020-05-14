using Nito.AsyncEx;
using SpectrumAnalyzer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logica di interazione per SpectrumGrid.xaml
    /// </summary>
    public partial class AmplitudeGrid : UserControl
    {
        public static readonly DependencyProperty FrequencyAmplitudesProperty =
    DependencyProperty.Register(
        "FrequencyAmplitudes", typeof(FrequencyAmplitude[]), typeof(AmplitudeGrid), new PropertyMetadata(Callback)
);
        public FrequencyAmplitude[] FrequencyAmplitudes
        {
            get { return (FrequencyAmplitude[])GetValue(FrequencyAmplitudesProperty); }
            set { SetValue(FrequencyAmplitudesProperty, value); }
        }

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (underlyingQueue.Count == 0)
            {
                renderingQueue.Add(e.NewValue as FrequencyAmplitude[]);
            }
            else
            {
                Debug.WriteLine($"{nameof(AmplitudeGrid)} skipped a frame");
            }
        }

        private static ConcurrentQueue<FrequencyAmplitude[]> underlyingQueue = new ConcurrentQueue<FrequencyAmplitude[]>();
        private static AsyncCollection<FrequencyAmplitude[]> renderingQueue = new AsyncCollection<FrequencyAmplitude[]>(underlyingQueue);


        public AmplitudeGrid()
        {
            InitializeComponent();
        }
    }
}
