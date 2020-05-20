using Spettro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logica di interazione per EventLogger.xaml
    /// </summary>
    public partial class EventLogger : UserControl
    {
        public EventLogger()
        {
            InitializeComponent();
            EventGrid.Loaded += EventGrid_Loaded;
        }

        private void EventGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var source = (sender as ListView).ItemsSource as ObservableCollection<Event>;
            if (source == null)
            {
                return;
            }
            source.CollectionChanged += Source_CollectionChanged;
        }

        private DateTime lastNotification = DateTime.MinValue;
        private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null || e.NewItems.Count == 0)
            {
                return;
            }
            SpeechTextbox.Text = "Errore " + (e.NewItems[0] as Event).Frequency;
            if (DateTime.Now.Subtract(lastNotification).TotalSeconds >= 2)
            {
                lastNotification = DateTime.Now;
                var peer = FrameworkElementAutomationPeer.FromElement(SpeechTextbox);
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }
        }
    }
}
