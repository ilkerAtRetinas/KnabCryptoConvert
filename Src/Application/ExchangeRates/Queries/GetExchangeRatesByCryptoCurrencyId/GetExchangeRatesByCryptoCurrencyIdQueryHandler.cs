using Application.Exceptions;
using Application.Response;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ExchangeRates.Queries.GetExchangeRatesByCryptoCurrencyId
{
    public class GetExchangeRatesByCryptoCurrencyIdQueryHandler : IRequestHandler<GetExchangeRatesByCryptoCurrencyIdQuery, Response<Dictionary<string, CryptoCurrencyDetailDTO>>>
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GetExchangeRatesByCryptoCurrencyIdQueryHandler> _logger;
        public GetExchangeRatesByCryptoCurrencyIdQueryHandler(IHttpClientFactory clientFactory, ILogger<GetExchangeRatesByCryptoCurrencyIdQueryHandler> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public async Task<Response<Dictionary<string, CryptoCurrencyDetailDTO>>> Handle(GetExchangeRatesByCryptoCurrencyIdQuery request, CancellationToken cancellationToken)
        {

            Response<Dictionary<string, CryptoCurrencyDetailDTO>> response = null;

            try
            {
                var client = _clientFactory.CreateClient(request.HttpClientName);
                foreach (var requestMessageUri in request.RequestMessagesUris)
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestMessageUri);
                    _logger.LogInformation("Api Request Sent For: " + requestMessageUri);
                    var responseMessage = await client.SendAsync(requestMessage, cancellationToken);
                    _logger.LogInformation("Api Response Received For: " + requestMessageUri);
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    if (response == null)
                    {
                        response = JsonConvert.DeserializeObject<Response<Dictionary<string, CryptoCurrencyDetailDTO>>>(content);
                        if (response.Status.ErrorCode != 0)
                        {
                            _logger.LogError("Api Error Code " + response.Status.ErrorCode + ": " + response.Status.ErrorMessage);
                            throw new CryptoConvertCoreException("Error " + response.Status.ErrorCode + " : " + response.Status.ErrorMessage);
                        }
                    }
                    else
                    {
                        var nextResponse = JsonConvert.DeserializeObject<Response<Dictionary<string, CryptoCurrencyDetailDTO>>>(content);
                        if (nextResponse.Status.ErrorCode != 0)
                        {
                            _logger.LogError("Api Error Code " + response.Status.ErrorCode + ": " + response.Status.ErrorMessage);
                            throw new CryptoConvertCoreException("Error " + response.Status.ErrorCode + " : " + response.Status.ErrorMessage);
                        }
                        var nextQuote = nextResponse.Data.FirstOrDefault().Value.Quote.FirstOrDefault();
                        response.Data.FirstOrDefault().Value.Quote.Add(nextQuote.Key, nextQuote.Value);
                    }
                }
            }
            catch (CryptoConvertCoreException exc)
            {
                throw exc;
            }
            catch (HttpRequestException exc)
            {
                _logger.LogError(exc, "No such host found.");
                throw new CryptoConvertCoreException("Couldn't reach to destionation host. Please make sure you have internet connection and the address you want to reach is correct.");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Unexpected Error");
                throw new CryptoConvertCoreException("Something bad happened.");
            }
            return response;
        }
    }
}
