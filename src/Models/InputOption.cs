using SpectrumAnalyzer.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpectrumAnalyzer.Models
{
    public class InputOption
    {
        public InputOption(string optionName, IInput owner, string category = null)
        {
            Category = category;
            OptionName = optionName;
            Owner = owner;
        }

        public string Category { get; }
        public string OptionName { get; }
        public IInput Owner { get; }
        public int SamplingFrequency { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Category))
            {
                return OptionName;
            }
            return $"[{Category}] {OptionName}";
        }
    }
}
