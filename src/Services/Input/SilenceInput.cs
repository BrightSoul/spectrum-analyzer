using NAudio.Dmo;
using NAudio.Wave;
using Nito.AsyncEx;
using SpectrumAnalyzer.Models;
using SpectrumAnalyzer.Models.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SpectrumAnalyzer.Services.Input
{
    public class SilenceInput : IInput
    {
        const int samplesBatchSize = 44000;
        public int Priority => 0;

        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(string option, [EnumeratorCancellation] CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var samples = Enumerable.Repeat(0f, samplesBatchSize).Select(sample => new Sample(new[] { sample })).ToArray();
                yield return samples;
                await Task.Delay(1000, token);
            }
        }

        public InputOption[] GetInputOptions()
        {
            return new InputOption[] { new InputOption("Silenzio", this) };
        }

        public FrameworkElement CreateConfigUI(string option)
        {
            return new Button() { Content = option };
        }
    }
}
