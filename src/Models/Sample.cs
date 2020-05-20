using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spettro.Models
{
    public struct Sample
    {
        public Sample(float[] values)
        {
            Values = values;
        }
        public float[] Values { get; }
        public int Channels => Values.Length;
    }
}
