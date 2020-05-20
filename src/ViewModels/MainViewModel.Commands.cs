using Spettro.Helpers;
using Spettro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Spettro.ViewModels
{
    public partial class MainViewModel
    {
        public RelayCommand<int> SelectFrequencyCommand => new RelayCommand<int>(SelectFrequency);
        public RelayCommand ResetEventLogCommand => new RelayCommand(ResetEventLog);
        public RelayCommand ExitCommand => new RelayCommand(Exit);
        public RelayCommand ShowConfigurationForSelectedInputOptionCommand => new RelayCommand(ShowConfigurationForSelectedInputOption);

    }
}
