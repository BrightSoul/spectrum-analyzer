using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Models
{
    public class Amplitude
    {
        public Amplitude(float[] values)
        {
            Values = values;
        }

        public float[] Values { get; }

        public override string ToString()
        {
            var output = new string[Values.Length];
            for (var i = 0; i < Values.Length; i++)
            {
                output[i] = Values[i].ToString("0.0'dB'");
            }
            return string.Join(",", output);
        }
    }
}
