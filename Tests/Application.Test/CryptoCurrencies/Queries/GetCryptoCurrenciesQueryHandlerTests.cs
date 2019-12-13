using Application.CryptoCurrencies.Queries.GetCryptoCurrencies;
using Application.Exceptions;
using Application.Response;
using Application.Test.Infrastructure;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Test.CryptoCurrencies.Queries
{
    public class GetCryptoCurrenciesQueryHandlerTests
    {


        [Fact]
        public async Task TestGetCryptoServices_HttpOk()
        {
            var httpClientTestName = "test";
            var fakeCurrencies = new List<CryptoCurrencyDTO>
               {
                  new CryptoCurrencyDTO
                  {
                    Symbol = "ASymbol",
                    IsActive = true,
                    Id = 1,
                    Name = "ACoin",
                    Slug="ACoinSlug",
                    FirstHistoricalData =Convert.ToDateTime("01.01.2019"),
                    LastHistoricalData = Convert.ToDateTime("01.01.2018")

                  },
                  new CryptoCurrencyDTO
                  {
                    Symbol = "BSymbol",
                    IsActive = false,
                    Id = 2,
                    Name = "BCoin",
                    Slug="BCoinSlug",
                    FirstHistoricalData = Convert.ToDateTime("01.01.2017"),
                    LastHistoricalData = Convert.ToDateTime("01.01.2016")
                  }
               };

            var fakeStatus = new Status
            {
                CreditCount = 1,
                Elapsed = 5,
                ErrorCode = 0,
                ErrorMessage = string.Empty,
                Timestamp = Convert.ToDateTime("01.01.2015")

            };
            Response<List<CryptoCurrencyDTO>> fakeResponse = new Response<List<CryptoCurrencyDTO>>
            {
                Data = fakeCurrencies,
                Status = fakeStatus
            };
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var memoryCacheMock = Substitute.For<IMemoryCache>();
            var loggerMock = Substitute.For<ILogger<GetCryptoCurrenciesQueryHandler>>();
            var url = "http://correctUrl.com";
            var fakeHttpMessageHandler = new FakeGetCryptoCurrenciesHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponse), Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri(url)
            };
            httpClientFactoryMock.CreateClient(httpClientTestName).Returns(fakeHttpClient);

            var service = new GetCryptoCurrenciesQueryHandler(memoryCacheMock,httpClientFactoryMock, loggerMock);
            var result = await service.Handle(new GetCryptoCurrenciesQuery { HttpClientName = httpClientTestName, RequestUri ="/test" }, CancellationToken.None);

            result.Should().NotBeNull().And.BeOfType<Response<List<CryptoCurrencyDTO>>>();
            result.Data.Should().HaveCount(2).And
                .Contain(x => x.Symbol == "ASymbol").And
                .Contain(x => x.Slug == "ACoinSlug").And
                .Contain(x => x.Name == "ACoin").And
                .Contain(x => x.IsActive == true).And
                .Contain(x => x.Id == 1).And
                .Contain(x => x.FirstHistoricalData == Convert.ToDateTime("01.01.2019")).And
                .Contain(x => x.LastHistoricalData == Convert.ToDateTime("01.01.2018")).And
                .Contain(x => x.Symbol == "BSymbol").And
                .Contain(x => x.Slug == "BCoinSlug").And
                .Contain(x => x.Name == "BCoin").And
                .Contain(x => x.IsActive == false).And
                .Contain(x => x.Id == 2).And
                .Contain(x => x.FirstHistoricalData == Convert.ToDateTime("01.01.2017")).And
                .Contain(x => x.LastHistoricalData == Convert.ToDateTime("01.01.2016"));

            result.Status.Should().NotBeNull().And
                .BeOfType<Status>().And
                .BeEquivalentTo(fakeStatus);
        }

        [Fact]
        public async Task TestGetCryptoServices_BadRequest()
        {
            var httpClientTestName = "test";
            var fakeStatus = new Status
            {
                CreditCount = 1,
                Elapsed = 5,
                ErrorCode = 400,
                ErrorMessage = "Bad request message",
                Timestamp = DateTime.Now

            };
            Response<List<CryptoCurrencyDTO>> fakeResponse = new Response<List<CryptoCurrencyDTO>>
            {
                Data = null,
                Status = fakeStatus
            };


            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var memoryCacheMock = Substitute.For<IMemoryCache>();
            var loggerMock = Substitute.For<ILogger<GetCryptoCurrenciesQueryHandler>>();
            var url = "http://wrongUrl.com";
            var fakeHttpMessageHandler = new FakeGetCryptoCurrenciesHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonConvert.SerializeObject(fakeResponse), Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri(url)
            };
            httpClientFactoryMock.CreateClient(httpClientTestName).Returns(fakeHttpClient);

            var service = new GetCryptoCurrenciesQueryHandler(memoryCacheMock, httpClientFactoryMock, loggerMock);
            Func<Task> act = async () => await service.Handle(new GetCryptoCurrenciesQuery { HttpClientName = httpClientTestName, RequestUri = "/test" }, CancellationToken.None);
            await act.Should().ThrowExactlyAsync<CryptoConvertCoreException>();
        }
    }
}
