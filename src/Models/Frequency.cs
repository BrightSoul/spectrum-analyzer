using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumAnalyzer.Models
{
    public class Frequency
    {
        public Frequency(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override string ToString()
        {
            if (Value >= 1000)
            {
                return $"{Value/(decimal)1000}kHz";
            }
            return $"{Value}Hz";
        }
    }
}
