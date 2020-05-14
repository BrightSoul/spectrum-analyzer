using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Services.Transform
{
    public interface ITransformer
    {
        IAsyncEnumerable<FrequencyAmplitude[]> EnumerateFrequencyAmplitudesAsync(string optionName, IInput input, CancellationToken token);
    }
}
