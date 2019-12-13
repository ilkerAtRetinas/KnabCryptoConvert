using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Application.Exceptions;

namespace Application.CryptoCurrencies.Queries.GetCryptoCurrencies
{
    
    public class GetCryptoCurrenciesQueryHandler : IRequestHandler<GetCryptoCurrenciesQuery, Response<List<CryptoCurrencyDTO>>>
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFacory;
        private readonly ILogger<GetCryptoCurrenciesQueryHandler> _logger;

        public GetCryptoCurrenciesQueryHandler(IMemoryCache cache, IHttpClientFactory clientFactory, ILogger<GetCryptoCurrenciesQueryHandler> logger)
        {
            _cache = cache;
            _clientFacory = clientFactory;
            _logger = logger;
        }
        /// <summary>
        /// Caches all crypto currencies with the first request to prevent additional api calls for further requests
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response<List<CryptoCurrencyDTO>>> Handle(GetCryptoCurrenciesQuery request, CancellationToken cancellationToken)

        {
            string content;
            Response<List<CryptoCurrencyDTO>> response;
            try
            {
               
                if (!_cache.TryGetValue("CuryptoCurrencies", out response))
                {
                    var client = _clientFacory.CreateClient(request.HttpClientName);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.RequestUri);
                    _logger.LogInformation("Api Request Sent");
                    var responseMessage = await client.SendAsync(requestMessage, cancellationToken);
                    _logger.LogInformation("Api Response Received");
                    content = await responseMessage.Content.ReadAsStringAsync();

                    //cache time set up for one day, this may be even longer, I don't think a new currency is being added so frequently 
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    response = JsonConvert.DeserializeObject<Response<List<CryptoCurrencyDTO>>>(content);
                    if(response.Status.ErrorCode != 0)
                    {
                        _logger.LogError("Api Error Code " + response.Status.ErrorCode + ": " + response.Status.ErrorMessage);
                        throw new CryptoConvertCoreException("Error " + response.Status.ErrorCode + " : " + response.Status.ErrorMessage);
                    }
                    _cache.Set("CuryptoCurrencies", response, cacheEntryOptions);
                }
                else
                {
                    _logger.LogInformation("Crypto Currencies received from cache");
                }
                
            }
            catch(CryptoConvertCoreException exc)
            {
                throw exc;
            }
            catch (HttpRequestException exc)
            {
                _logger.LogError(exc,"No such host found." );
                throw new CryptoConvertCoreException("Couldn't reach to destionation host. Please make sure you have internet connection and the address you want to reach is correct.");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc,"Unexpected Error in Application core");
                throw new CryptoConvertCoreException("Something bad happened.");
            }
            return response;
        }
    }
}
