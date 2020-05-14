using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Models
{
    public struct FrequencyAmplitude //Change to a record, someday...
    {
        public FrequencyAmplitude(Frequency frequency, Amplitude amplitude)
        {
            Frequency = frequency;
            Intensity = amplitude;
        }

        public Frequency Frequency { get; }
        public Amplitude Intensity { get; }
    }
}
