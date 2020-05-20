using Spettro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Spettro.Services.Input
{
    public interface IInput
    {
        IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(InputOption option, CancellationToken token);
        InputOption[] GetInputOptions();
        int Priority { get; }
        FrameworkElement CreateConfigUI(string optionName);
    }
}
