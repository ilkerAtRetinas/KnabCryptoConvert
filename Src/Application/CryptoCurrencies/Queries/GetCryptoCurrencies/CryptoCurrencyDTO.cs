using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CryptoCurrencies.Queries.GetCryptoCurrencies
{
    public class CryptoCurrencyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Slug { get; set; }
        public bool? IsActive { get; set; }
        public DateTimeOffset? FirstHistoricalData { get; set; }
        public DateTimeOffset? LastHistoricalData { get; set; }
    }
}
