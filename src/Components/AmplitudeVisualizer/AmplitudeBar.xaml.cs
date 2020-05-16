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
    /// Logica di interazione per AmplitudeBar.xaml
    /// </summary>
    public partial class AmplitudeBar : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(float), typeof(AmplitudeBar)
        );
        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public AmplitudeBar()
        {
            InitializeComponent();
        }
    }
}
