using Application.CryptoCurrencies.Queries.GetCryptoCurrencies;
using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Application.CryptoCurrencies.Queries.GetCryptoCurrencies
{
    public class GetCryptoCurrenciesQuery : IRequest<Response<List<CryptoCurrencyDTO>>>
    {
        //public HttpRequestMessage RequestMessage { get; set; }
        //public HttpClient Client { get; set; }

        public string HttpClientName { get; set; }
        public string RequestUri { get; set; }
    }
}
