using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Models
{
    public struct Sample
    {
        public Sample(float[] values)
        {
            Values = values;
        }
        float[] Values { get; }
        int Channels => Values.Length;
    }
}
