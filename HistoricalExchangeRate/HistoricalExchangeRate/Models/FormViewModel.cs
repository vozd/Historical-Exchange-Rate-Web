using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HistoricalExchangeRate.Models
{
    public class FormViewModel
    {
        [Required]
        public string Currency { get; set; }

        [Required]
        public string BaseCurrency { get; set; }

        [Required]
        public string Dates { get; set; }

        public ExchangeRateResult Result { get; set; }
    }
}
