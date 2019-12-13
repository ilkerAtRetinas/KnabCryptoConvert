using Application.Exceptions;
using Application.ExchangeRates.Queries.GetExchangeRatesByCryptoCurrencyId;
using Application.Response;
using Application.Test.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Test.ExchangeRates.Queries
{
    public class GetExchangeRatesByCryptoCurrencyIdQueryHandlerTests
    {
        [Fact]
        public async Task TestExchangeRates_HttpOk()
        {
            #region create fake data
            var httpClientTestName = "fakeService";
            var fakeQuoteXCurrency = new QuoteDTO
            {
                LastUpdated = Convert.ToDateTime("01.02.2019"),
                MarketCap = 500000,
                Price = 5,
                PercentChange1H = 1,
                PercentChange24H = 2,
                PercentChange7D = 1.25,
                Volume24H = 1500
            };

            var fakeQuoteYCurrency = new QuoteDTO
            {
                LastUpdated = Convert.ToDateTime("01.02.2019"),
                MarketCap = 500000,
                Price = 9,
                PercentChange1H = 1,
                PercentChange24H = 2,
                PercentChange7D = 1.25,
                Volume24H = 1500
            };


            var fakeQuoteX = new Dictionary<string, QuoteDTO>
            {
                { "XCurrency",  fakeQuoteXCurrency}
            };

            var fakeQuoteY = new Dictionary<string, QuoteDTO>
            {
                { "YCurrency",  fakeQuoteYCurrency}
            };
            CryptoCurrencyDetailDTO fakeCurrencyXDetails = new CryptoCurrencyDetailDTO
            {
                Id = 1,
                CirculatingSupply = 2,
                CmcRank = 3,
                DateAdded = Convert.ToDateTime("01.01.2019"),
                LastUpdated = Convert.ToDateTime("01.02.2019"),
                MaxSupply = 5000,
                Name = "ACoin",
                NumMarketPairs = 100,
                Slug = "ACoinSlug",
                Symbol = "ACoinSymbol",
                Tags = new List<string> { "tagA", "tagB" },
                TotalSupply = 4000,
                Quote = fakeQuoteX
            };

            CryptoCurrencyDetailDTO fakeCurrencyYDetails = new CryptoCurrencyDetailDTO
            {
                Id = 1,
                CirculatingSupply = 2,
                CmcRank = 3,
                DateAdded = Convert.ToDateTime("01.01.2019"),
                LastUpdated = Convert.ToDateTime("01.02.2019"),
                MaxSupply = 5000,
                Name = "ACoin",
                NumMarketPairs = 100,
                Slug = "ACoinSlug",
                Symbol = "ACoinSymbol",
                Tags = new List<string> { "tagA", "tagB" },
                TotalSupply = 4000,
                Quote = fakeQuoteY
            };

            var fakeStatus = new Status
            {
                CreditCount = 1,
                Elapsed = 5,
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Timestamp = Convert.ToDateTime("01.01.2015")

            };
            var fakeCurrencyXDetailData = new Dictionary<string, CryptoCurrencyDetailDTO>
            {
                { fakeCurrencyXDetails.Id.ToString(), fakeCurrencyXDetails }
            };

            var fakeCurrencyYDetailData = new Dictionary<string, CryptoCurrencyDetailDTO>
            {
                { fakeCurrencyYDetails.Id.ToString(), fakeCurrencyYDetails }
            };



            Response<Dictionary<string, CryptoCurrencyDetailDTO>> fakeResponseX = new Response<Dictionary<string, CryptoCurrencyDetailDTO>>
            {
                Data = fakeCurrencyXDetailData,
                Status = fakeStatus
            };

            Response<Dictionary<string, CryptoCurrencyDetailDTO>> fakeResponseY = new Response<Dictionary<string, CryptoCurrencyDetailDTO>>
            {
                Data = fakeCurrencyYDetailData,
                Status = fakeStatus
            };

            var urlX = "test/?convert=currencyX";
            var urlY = "test/?convert=currencyY";
            List<string> messageList = new List<string>
            {
                {urlX },
                { urlY }
            };
            var fakeHttpMessageHandler = new FakeGetExchangeRateHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponseX), Encoding.UTF8, "application/json")
            }, new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponseY), Encoding.UTF8, "application/json")
            }
            );
            #endregion

            //Setup fake httpclient and call
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var loggerMock = Substitute.For<ILogger<GetExchangeRatesByCryptoCurrencyIdQueryHandler>>();

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://correctAddress.com/")
            };
            httpClientFactoryMock.CreateClient(httpClientTestName).Returns(fakeHttpClient);



            var service = new GetExchangeRatesByCryptoCurrencyIdQueryHandler(httpClientFactoryMock, loggerMock);
            var result = await service.Handle(new GetExchangeRatesByCryptoCurrencyIdQuery { HttpClientName = httpClientTestName, RequestMessagesUris = messageList }, CancellationToken.None);

            result.Should().NotBeNull().And.BeOfType<Response<Dictionary<string, CryptoCurrencyDetailDTO>>>();
            result.Data.Should().HaveCount(1);

            result.Data.Values.FirstOrDefault().Quote.Should()
                .ContainKeys("XCurrency", "YCurrency");
                
        }

        [Fact]
        public async Task TestExchangeRates_BadRequest()
        {
            #region create fake data
            var httpClientTestName = "fakeService";
            var fakeStatus = new Status
            {
                CreditCount = 1,
                Elapsed = 5,
                ErrorCode = 400,
                ErrorMessage = "Bad request message",
                Timestamp = DateTime.Now

            };
            Response<Dictionary<string, CryptoCurrencyDetailDTO>> fakeResponse = new Response<Dictionary<string, CryptoCurrencyDetailDTO>>
            {
                Data = null,
                Status = fakeStatus
            };
            var urlX = "bad/?convert=currencyX";
            var urlY = "bad/?convert=currencyY";           
            List<string> messageList = new List<string>
            {
                { urlX },
                { urlY }
            };
            var fakeHttpMessageHandler = new FakeGetExchangeRateHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponse), Encoding.UTF8, "application/json")
            }, new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponse), Encoding.UTF8, "application/json")
            }
           );
            #endregion

            //Setup fake httpclient and call
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var loggerMock = Substitute.For<ILogger<GetExchangeRatesByCryptoCurrencyIdQueryHandler>>();
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://correctAddress.com/")
            };
            httpClientFactoryMock.CreateClient(httpClientTestName).Returns(fakeHttpClient);



            var service = new GetExchangeRatesByCryptoCurrencyIdQueryHandler(httpClientFactoryMock,loggerMock);

            Func<Task> act = async () => await service.Handle(new GetExchangeRatesByCryptoCurrencyIdQuery { HttpClientName = httpClientTestName, RequestMessagesUris = messageList }, CancellationToken.None);
            await act.Should().ThrowExactlyAsync<CryptoConvertCoreException>();
        }

    }
}
