using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class CurrencyRateViewModel
    {
        public DateTime LastUpdated { get; set; }
        public List<QuoteViewModel> Quotes { get; set; }
        public string CryptoCurrencyName { get; set; }
        public string CryptoCurrencySymbol { get; set; }
    }
}
