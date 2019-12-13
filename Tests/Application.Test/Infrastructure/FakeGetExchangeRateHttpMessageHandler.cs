using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Test.Infrastructure
{
    public class FakeGetExchangeRateHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _fakeResponseX;
        private readonly HttpResponseMessage _fakeResponseY;

        public FakeGetExchangeRateHttpMessageHandler(HttpResponseMessage fakeResponseX, HttpResponseMessage fakeResponseY)
        {
            _fakeResponseX = fakeResponseX;
            _fakeResponseY = fakeResponseY;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.Query.ToString().Contains("currencyX"))
            {
                return await Task.FromResult(_fakeResponseX);
            }
            if (request.RequestUri.Query.ToString().Contains("currencyY"))
            {
                return await Task.FromResult(_fakeResponseY);
            }
            else
            {
                return null;
            }
            
        }
    }
}
