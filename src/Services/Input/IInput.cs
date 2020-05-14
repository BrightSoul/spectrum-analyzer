using SpectrumAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SpectrumAnalyzer.Services.Input
{
    public interface IInput
    {
        IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(string option, CancellationToken token);
        InputOption[] GetInputOptions();
        int Priority { get; }
        FrameworkElement CreateConfigUI(string optionName);
    }
}
