using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spettro.Models
{
    public struct FrequencyAmplitude //Change to a record, someday...
    {
        public FrequencyAmplitude(Frequency frequency, Amplitude amplitude)
        {
            Frequency = frequency;
            Amplitude = amplitude;
        }

        public Frequency Frequency { get; }
        public Amplitude Amplitude { get; }
    }
}
