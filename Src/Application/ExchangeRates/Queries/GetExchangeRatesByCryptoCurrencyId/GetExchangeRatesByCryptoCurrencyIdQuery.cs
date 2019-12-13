using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Application.ExchangeRates.Queries.GetExchangeRatesByCryptoCurrencyId
{
    public class GetExchangeRatesByCryptoCurrencyIdQuery : IRequest<Response<Dictionary<string, CryptoCurrencyDetailDTO>>>
    {
        public List<string> RequestMessagesUris { get; set; }
        public string HttpClientName { get; set; }
    }
}
