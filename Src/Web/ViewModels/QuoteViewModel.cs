using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class QuoteViewModel
    {
        public string Currency { get; set; }
        public string Quote { get; set; }

        public string MarketCapValue { get; set; }

        public string LastUpdated { get; set; }
    }
}
