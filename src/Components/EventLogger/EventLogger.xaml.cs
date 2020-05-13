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
    /// Logica di interazione per EventLogger.xaml
    /// </summary>
    public partial class EventLogger : UserControl
    {
        public EventLogger()
        {
            this.DataContext = new { Events = new[] {
                new { Instant = "00:01:10", Frequency = "20Hz", Value = "20dB" },
                new { Instant = "00:02:22", Frequency = "1kHz", Value = "18dB" },
                new { Instant = "00:03:15", Frequency = "10kHz", Value = "19dB" },
                }
            };
            InitializeComponent();
        }
    }
}
