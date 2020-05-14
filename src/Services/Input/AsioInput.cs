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
    public class AsioInput : IInput
    {
        const int samplesBatchSize = 20000;

        private readonly AsyncCollection<Sample[]> sampleQueue = new AsyncCollection<Sample[]>(new ConcurrentQueue<Sample[]>());
        private float[] values;

        public int Priority => 20;

        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(string driverName, [EnumeratorCancellation] CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(driverName))
            {
                throw new ConfigurationNeededException();
            }
            using var asio = new AsioOut(driverName);
            values = new float[samplesBatchSize * asio.NumberOfOutputChannels];
            asio.AudioAvailable += HandleBuffer;
            while (await sampleQueue.OutputAvailableAsync(token))
            {
                var samples = await sampleQueue.TakeAsync(token);
                yield return samples;
            }
            asio.AudioAvailable -= HandleBuffer;
        }

        private void HandleBuffer(object sender, AsioAudioAvailableEventArgs e)
        {
            var count = e.GetAsInterleavedSamples(values);
            var s = e.AsioSampleType;
            Debug.WriteLine($"{s}: {count}");
        }

        public InputOption[] GetInputOptions()
        {
            return AsioOut.GetDriverNames()
                          .Select(name => new InputOption(name, this))
                          .ToArray();
            
        }

        public FrameworkElement CreateConfigUI(string driverName)
        {
            return new Button() { Content = driverName };
        }

        private float[] Get16BitMono(byte[] buffer, int offset) {
            return new float[] { BitConverter.ToInt16(buffer, offset) / (float) 32768 };
        }
    }
}
