using Spettro.Models;
using Spettro.Services.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spettro.Services.Resample
{
    public class MaxResampler : IResampler
    {
        public IEnumerable<FrequencyAmplitude> ResampleFrequencyAmplitudes(FrequencyAmplitude[] frequencyAmplitudes, Frequency[] desiredOutputFrequencies)
        {
            int lowerBound = 0;
            for (var i = 0; i < desiredOutputFrequencies.Length; i++)
            {
                var frequency = desiredOutputFrequencies[i].Value;
                var upperBound = frequency + (i < desiredOutputFrequencies.Length - 1 ? (desiredOutputFrequencies[i + 1].Value - frequency) / 2 : 100000);

                var aggregatedAmplitudes = AggregateFrequencyAmplitudes(frequencyAmplitudes, lowerBound, upperBound);
                yield return new FrequencyAmplitude(desiredOutputFrequencies[i], new Amplitude(aggregatedAmplitudes));
                lowerBound = upperBound;
            }
        }

        private float[] AggregateFrequencyAmplitudes(FrequencyAmplitude[] frequencyAmplitudes, int lowerBound, int upperBound)
        {
            var relevantFrequencyAmplitudes = frequencyAmplitudes.Where(f => f.Frequency.Value >= lowerBound && f.Frequency.Value < upperBound).ToArray();
            if (relevantFrequencyAmplitudes.Length == 0)
            {
                return new float[] { 0f };
            }
            var maxAmplitude = relevantFrequencyAmplitudes.Average(f => f.Amplitude.Values[0]);
            return new float[] { maxAmplitude };
        }
    }
}
