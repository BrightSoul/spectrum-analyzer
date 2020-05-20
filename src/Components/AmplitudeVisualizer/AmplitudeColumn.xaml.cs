using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spettro.Components
{


    /// <summary>
    /// Logica di interazione per SpectrumColumn.xaml
    /// </summary>
    public partial class AmplitudeColumn : UserControl
    {
        public AmplitudeColumn()
        {
            InitializeComponent();
            FrequencySlider.ValueChanged += FrequencySlider_ValueChanged;
        }

        private void FrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(CurrentValue);
            peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}
