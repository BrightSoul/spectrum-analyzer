using Spettro.Models;
using Spettro.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spettro.Services.Transform
{
    public interface ITransformer
    {
        IAsyncEnumerable<FrequencyAmplitude[]> EnumerateFrequencyAmplitudesAsync(InputOption option, CancellationToken token);
    }
}
