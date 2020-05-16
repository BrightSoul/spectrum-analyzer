using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Services.Aggregate
{
    public interface IResampler
    {
        IEnumerable<FrequencyAmplitude> ResampleFrequencyAmplitudes(FrequencyAmplitude[] frequencyAmplitudes, Frequency[] desiredOutputFrequencies);
    }
}
