﻿using SpectrumAnalyzer.Services.Input;
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
        public InputOption(string optionName, IInput owner)
        {
            OptionName = optionName;
            Owner = owner;
        }

        public string OptionName { get; }
        public IInput Owner { get; }
    }
}