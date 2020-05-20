using NAudio.Dmo;
using NAudio.Wave;
using NAudio.Wave.Asio;
using Nito.AsyncEx;
using Spettro.Models;
using Spettro.Models.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Spettro.Services.Input
{
    public class AsioInput : IInput
    {
        const int samplesBatchSize = 2048;

        private readonly AsyncCollection<Sample[]> sampleQueue = new AsyncCollection<Sample[]>(new ConcurrentQueue<Sample[]>());
        private float[] values;

        public int Priority => 20;

        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(InputOption option, [EnumeratorCancellation] CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(option.OptionName))
            {
                throw new ConfigurationNeededException();
            }           
            using var asio = new AsioOut(option.OptionName);
            option.SamplingFrequency = 44100;
            var wavprov = new BufferedWaveProvider(new WaveFormat(option.SamplingFrequency, 1));
            values = new float[samplesBatchSize];
            asio.AudioAvailable += HandleBuffer;
            //asio.InitRecordAndPlayback(wavprov, 1, 44100);
            asio.InitRecordAndPlayback(null, 1, option.SamplingFrequency);
            asio.Play();

            while (await sampleQueue.OutputAvailableAsync(token))
            {
                var samples = await sampleQueue.TakeAsync(token);
                yield return samples;
            }
            asio.AudioAvailable -= HandleBuffer;
        }

        private void HandleBuffer(object sender, AsioAudioAvailableEventArgs e)
        {
            /*byte[] buf = new byte[e.SamplesPerBuffer];
            for (int i = 0; i < e.InputBuffers.Length; i++)
            {
                //Marshal.Copy(e.InputBuffers[i], e.OutputBuffers, 0, e.InputBuffers.Length);
                //also tried to upper one but this way i also couldn't hear anything
                Marshal.Copy(e.InputBuffers[i], buf, 0, e.SamplesPerBuffer);
                Marshal.Copy(buf, 0, e.OutputBuffers[i], e.SamplesPerBuffer);
            }
            e.WrittenToOutputBuffers = true;*/


            var count = e.GetAsInterleavedSamples(values);
            //if (e.AsioSampleType == AsioSampleType.Int32LSB16)
            var samples = new Sample[count];
            for (var i = 0; i < count; i++)
            {
                samples[i] = new Sample(new[] { values[i] });
            }
            sampleQueue.Add(samples);
        }

        public InputOption[] GetInputOptions()
        {
            return AsioOut.GetDriverNames()
                          .Select(name => new InputOption(name, this, "ASIO"))
                          .ToArray();
            
        }

        public FrameworkElement CreateConfigUI(string driverName)
        {
            return new Button() { Content = driverName };
        }

    }
}
