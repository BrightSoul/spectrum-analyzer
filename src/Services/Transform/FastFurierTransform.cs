using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Services.Transform
{
    public class FastFurierTransform : ITransformer
    {
        public async IAsyncEnumerable<FrequencyAmplitude[]> EnumerateFrequencyAmplitudesAsync(string optionName, IInput input, [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var samples in input.EnumerateSamplesAsync(optionName, token))
            {
                yield return new FrequencyAmplitude[1] { new FrequencyAmplitude(new Frequency(11), new Amplitude(new float[0])) };
            }
        }
    }
}
