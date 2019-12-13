using Application.ExchangeRates.Queries.GetExchangeRatesByCryptoCurrencyId;
using Application.Response;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class CryptoCurrencyViewModel
    {

        public SelectList SymbolSelectList{ get; set; }

        public string CryptoCurrencySymbolId { get; set; }

        public CurrencyRateViewModel CurrencyRates { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>
        /// Mapping of Response<Dictionary<string, CryptoCurrencyDetailDTO> to CryptoCurrencyViewModel
        /// </summary>
        public static Expression<Func<Response<Dictionary<string, CryptoCurrencyDetailDTO>>, CryptoCurrencyViewModel>> Projection
        {
            get
            {
                return currency => new CryptoCurrencyViewModel
                {
                    CurrencyRates = currency.Data.Select(p => new CurrencyRateViewModel
                    {
                        CryptoCurrencyName = p.Value.Name,
                        CryptoCurrencySymbol = p.Value.Symbol,
                        Quotes = p.Value.Quote.Select(q => new QuoteViewModel{ 
                            Currency = q.Key,
                            Quote = q.Value.Price.Value.ToString("#.##"),
                            MarketCapValue = q.Value.MarketCap.Value.ToString("#,##"),
                            LastUpdated = q.Value.LastUpdated.Value.ToLocalTime().DateTime.ToString("G")
                        }).ToList(),
                    }).SingleOrDefault()
                };
            }
        }

        public static CryptoCurrencyViewModel Create(Response<Dictionary<string, CryptoCurrencyDetailDTO>> response)
        {
            return Projection.Compile().Invoke(response);
        }
    }

   
}
