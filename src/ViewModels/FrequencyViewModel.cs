using SpectrumAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.ViewModels
{

    public class FrequencyViewModel : BaseViewModel
    {
        public FrequencyViewModel(Frequency frequency)
        {
            Frequency = frequency;
        }

        public void UpdateAmplitude(Amplitude amplitude)
        {
            CurrentAmplitude = amplitude.Values[0];
        }

        public Frequency Frequency { get; }
        public int Value => Frequency.Value;

        private float currentAmplitude;
        public float CurrentAmplitude
        {
            get => currentAmplitude;
            set => Set(ref currentAmplitude, value);
        }

        private int threshold;
        public int Threshold
        {
            get => threshold;
            set => Set(ref threshold, value);
        }

        public DateTime LastNotified { get; set; } = DateTime.MinValue;

        public override string ToString()
        {
            return Frequency.ToString();
        }

    }
}