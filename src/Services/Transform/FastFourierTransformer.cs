using NAudio.Dsp;
using Spettro.Models;
using Spettro.Services.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Spettro.Services.Transform
{
    public class FastFourierTransformer : ITransformer
    {
        //const int power = 11;
        const int power = 10;
        public async IAsyncEnumerable<FrequencyAmplitude[]> EnumerateFrequencyAmplitudesAsync(InputOption option, [EnumeratorCancellation] CancellationToken token)
        {
            var samplesCount = Convert.ToInt32(Math.Pow(2, power));
            //var complexSamples = new Complex[samplesCount];
            var complexSamples = new Complex[samplesCount];
            //var doubleSamples = new double[samplesCount];
            var position = 0;

            await foreach (var samples in option.Owner.EnumerateSamplesAsync(option, token))
            {
                int i;
                for (i = 0; i < samples.Length; i++)
                {
                    complexSamples[position] = new Complex {
                        X = (float) (samples[i].Values[0] * FastFourierTransform.HannWindow(position, 1024)),
                        Y = 0 //Just the real part
                    };
                    //doubleSamples[position] = samples[i].Values[0];
                    position++;
                    if (position >= samplesCount)
                    {
                        FastFourierTransform.FFT(true, (int) Math.Log(samplesCount, 2.0), complexSamples);

                        //var amplitudes = complexSamples.Take(complexSamples.Length / 2).Select(
                        var j = 0;
                        var actualSamplesCount = (complexSamples.Length / 2) + 1;
                        var amplitudes = complexSamples.Take(actualSamplesCount).Select(
                            c => new FrequencyAmplitude(
                                new Frequency(GetFrequency(j++, actualSamplesCount, option.SamplingFrequency)),
                                //new Amplitude(new float[] { Math.Abs(c.X + c.Y) })
                                //new Amplitude(new float[] { (float) Math.Log(Math.Abs(c.X + c.Y)) })
                                new Amplitude(new float[] { GetAmplitude(c) })
                                )
                            ).ToArray();

                        yield return amplitudes;
                        position = 0;
                    }
                }
                /*while (complexSamples.Count >= samplesCount)
                {
                    Array.Copy()
                    FastFourierTransform.FFT(true, power, )
                    yield return new FrequencyAmplitude[1] { new FrequencyAmplitude(new Frequency(11), new Amplitude(new float[0])) };
                }*/
            }
        }

        private int GetFrequency(int index, int length, int samplingFrequency)
        {
            var increment = length > 1 ? (samplingFrequency / 2) / (float) (length - 1) : 0;
            return Convert.ToInt32(Math.Floor(index * increment));
        }

        private float GetAmplitude(Complex value)
        {
            var magnitude = GetMagnitude(value);
            var amplitude = 20f * (float)Math.Log10(magnitude);
            if (float.IsNaN(amplitude))
            {
                amplitude = -100;
            }
            return amplitude;
            /*var amplitude = 20f * (float) Math.Log10(magnitude);
            //TODO: Calcola correttamente l'ampiezza
            return Math.Max(-30, Math.Min(0, amplitude));*/
        }

        private float GetMagnitude(Complex value)
        {
            float c = Math.Abs(value.X);
            float d = Math.Abs(value.Y);
            float result;
            if (c > d)
            {
                float r = d / c;
                result = (float) (c * Math.Sqrt(1.0 + r * r));
            }
            else if (d == 0.0)
            {
                result = c;  // c is either 0.0 or NaN
            }
            else
            {
                float r = c / d;
                result = (float) (d * Math.Sqrt(1.0 + r * r));
            }
            return result / 100f;
        }
    }
}
