using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Test.Infrastructure
{
    public class FakeGetCryptoCurrenciesHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _fakeResponse;

        public FakeGetCryptoCurrenciesHttpMessageHandler(HttpResponseMessage responseMessage)
        {
            _fakeResponse = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_fakeResponse);
        }
    }
}
