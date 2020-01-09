using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Application.CryptoCurrencies.Queries.GetCryptoCurrencies;
using Application.Exceptions;
using Application.ExchangeRates.Queries.GetExchangeRatesByCryptoCurrencyId;
using Application.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]

    public class CryptoConversionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CryptoConversionController> _logger;

        public CryptoConversionController(IMediator mediator, ILogger<CryptoConversionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetCryptoCurrencies()
        {
            var ccMV = await GetInitialCryptoCurrencyViewModel();
            if (ccMV != null)
            {
                return View("Index", ccMV);
            }
            else { return View(); }
        }

        /// <summary>
        /// Since coinmarket cap's free tier api only allows for single conversion per request, this controller sets up 5 different request uri's for getting all conversions that the assignment requires, at once. 
        /// </summary>
        /// <param name="ccVm"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetExchangeRates(CryptoCurrencyViewModel ccVm)
        {
            List<string> conversionCurrencies = new List<string> { "USD", "EUR", "BRL", "GBP", "AUD" };
            List<string> requestMessages = new List<string>();
            
            try
            {
                foreach (var currency in conversionCurrencies)
                {
                    var queryParams = new Dictionary<string, string>()
                    {
                        {"id", ccVm.CryptoCurrencySymbolId },
                        {"convert", currency }
                    };
                    var url = QueryHelpers.AddQueryString("cryptocurrency/quotes/latest", queryParams);
                    requestMessages.Add(url);
                }
                
                Response<Dictionary<string, CryptoCurrencyDetailDTO>> response = await _mediator.Send(new GetExchangeRatesByCryptoCurrencyIdQuery { HttpClientName = "coinMarketCap", RequestMessagesUris = requestMessages });
                ccVm = CryptoCurrencyViewModel.Create(response);
                
            }
            catch(CryptoConvertCoreException exc)
            {
                ccVm.ErrorMessage = exc.Message.Replace("'", "\\'");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Unexpected Error GetExchangeRates Action");
                ccVm.ErrorMessage =exc.Message.Replace("'", "\\'");
            }
            ccVm.SymbolSelectList = GetInitialCryptoCurrencyViewModel().Result.SymbolSelectList;
            return View("Index", ccVm);


        }

        private async Task<CryptoCurrencyViewModel> GetInitialCryptoCurrencyViewModel()
        {
            CryptoCurrencyViewModel ccVm = new CryptoCurrencyViewModel();
            try
            {
                Response<List<CryptoCurrencyDTO>> response = await _mediator.Send(new GetCryptoCurrenciesQuery { RequestUri = "cryptocurrency/map", HttpClientName = "coinMarketCap"});
                var symbolList = response.Data.Select(r => new
                {
                    Id = r.Id,
                    Description = $"{r.Symbol} - {r.Name}"
                }).ToList();
                ccVm.SymbolSelectList = new SelectList(symbolList, "Id", "Description");
                return ccVm;
            }
             catch(CryptoConvertCoreException exc)
            {
                ccVm.ErrorMessage = exc.Message.Replace("'", "\\'");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Unexpected Error GetExchangeRates Action");
                ccVm.ErrorMessage =exc.Message.Replace("'", "\\'");
            }
            return ccVm;
        }
    }
}