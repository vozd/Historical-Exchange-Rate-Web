using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HistoricalExchangeRate.Models
{
    public class ExchangeRateResult
    {
        public string MaximumExchangeRate { get; set; }
        public string MinimumExchangeRate { get; set; }
        public string AverageRate { get; set; }
    }
}
