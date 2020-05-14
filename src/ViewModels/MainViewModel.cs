using SpectrumAnalyzer.Helpers;
using SpectrumAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace SpectrumAnalyzer.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Frequencies = new ObservableCollection<Frequency>(
                new []
                {
                    new Frequency(20),
                    new Frequency(40),
                    new Frequency(60),
                    new Frequency(100)
                });
        }
        public ObservableCollection<Frequency> Frequencies { get; }

        private Frequency selectedFrequency;
        public Frequency SelectedFrequency
        {
            get => selectedFrequency;
            private set => Set(ref selectedFrequency, value);
        }


        #region INotifyPropertyChanged implementation and helpers

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }
            backingField = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
