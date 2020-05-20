using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spettro.Models
{
    public class Event
    {
        public TimeSpan Instant { get; set; }
        public string Frequency { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return $"{Instant.Minutes} minuti {Instant.Seconds} secondi {Frequency} {Value}";
        }
    }
}
