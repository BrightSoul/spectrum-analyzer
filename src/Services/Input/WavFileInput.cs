using NAudio.Wave;
using Spettro.Models;
using System;
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

namespace Spettro.Services.Input
{
    public class WavFileInput : IInput
    {
        const int samplesBatchSize = 2205;

        public int Priority => 10;

        public async IAsyncEnumerable<Sample[]> EnumerateSamplesAsync(InputOption option, [EnumeratorCancellation] CancellationToken token = default)
        {
            using var pcmStream = new WaveFileReader("G:\\test.wav");
            //using var pcmStream = WaveFormatConversionStream.CreatePcmStream(audio);
            option.SamplingFrequency = pcmStream.WaveFormat.SampleRate;
            var bytesCount = samplesBatchSize * pcmStream.BlockAlign;
            var buffer = new byte[bytesCount];
            //if pcmStream.BitsPerSample == 16 && pcmStream.Channels == 1
            Func<byte[], int, float[]> getValues = Get16BitMono;
            while (pcmStream.Position < pcmStream.Length)
            {
                int bytesRead = await pcmStream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0)
                {
                    continue;
                }

                var samples = new Sample[bytesRead / pcmStream.BlockAlign];
                for (var i = 0; i < samples.Length; i++)
                {
                    var values = getValues(buffer, i * pcmStream.BlockAlign);
                    samples[i] = new Sample(values);
                }
                yield return samples;
                await Task.Delay(50);
            }
        }

        public InputOption[] GetInputOptions()
        {
            return new InputOption[0]
            {
                //new InputOption("File wav", this)
            };
        }

        public FrameworkElement CreateConfigUI(string label)
        {
            return new Button() { Content = label };
        }

        private float[] Get16BitMono(byte[] buffer, int offset) {
            //return new float[] { BitConverter.ToInt16(buffer, offset) / (float) 32768 };
            short v = BitConverter.ToInt16(buffer, offset);
            return new float[] { (float) v };
        }
    }
}
