using SpectrumAnalyzer.Helpers;
using SpectrumAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SpectrumAnalyzer.ViewModels
{
    public partial class MainViewModel
    {
        public RelayCommand<Frequency> SelectFrequencyCommand => new RelayCommand<Frequency>(SelectFrequency);
        public RelayCommand ExitCommand => new RelayCommand(Exit);

        private void SelectFrequency(Frequency frequency)
        {
            SelectedFrequency = Frequencies[0];
            //SelectedFrequency = frequency;
        }
        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
