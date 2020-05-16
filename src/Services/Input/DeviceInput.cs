using NAudio.Wave;
using Nito.AsyncEx;
using SpectrumAnalyzer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public class DeviceInput : IInput
    {
        public int Priority => 10;

        private readonly AsyncCollection<Sample[]> sampleQueue = new AsyncCollection<Sample[]>(new ConcurrentQueue<Sample[]>());
        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(InputOption option, [EnumeratorCancellation] CancellationToken token = default)
        {
            var options = GetInputNames();
            option.SamplingFrequency = 44100;
            var channels = 1;

            using var waveIn = new WaveIn();
            waveIn.DeviceNumber = Array.IndexOf(options, option.OptionName);
            waveIn.DataAvailable += ProcessBuffer;
            waveIn.WaveFormat = new WaveFormat(option.SamplingFrequency, channels);
            waveIn.BufferMilliseconds = 1000 / 20;
            waveIn.StartRecording();

            while (await sampleQueue.OutputAvailableAsync(token))
            {
                var samples = await sampleQueue.TakeAsync(token);
                yield return samples;
            }
        }

        private void ProcessBuffer(object sender, WaveInEventArgs args)
        {
            Sample[] values = new Sample[args.Buffer.Length / 2];
            for (int i = 0; i < args.BytesRecorded; i += 2)
            {
                //short v = (short)((args.Buffer[i + 1] << 8) | args.Buffer[i + 0]);
                short v = BitConverter.ToInt16(args.Buffer, i);
                values[i / 2] = new Sample(new[] { (float) v });
            }
            sampleQueue.Add(values);
        }

        public InputOption[] GetInputOptions()
        {
            return GetInputNames().Select(name => new InputOption(name, this)).ToArray();
        }

        private string[] GetInputNames()
        {
            return Enumerable.Range(0, WaveIn.DeviceCount).Select(i => WaveIn.GetCapabilities(i)).Select(c => c.ProductName).ToArray();
        }

        public FrameworkElement CreateConfigUI(string label)
        {
            return new Button() { Content = label };
        }

        private float[] Get16BitMono(byte[] buffer, int offset) {
            //return new float[] { BitConverter.ToInt16(buffer, offset) / (float) 32768 };
            return new float[] { BitConverter.ToInt16(buffer, offset) };
        }
    }
}
