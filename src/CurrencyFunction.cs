using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Linq;
using AngleSharp.Html.Dom;
using System.Collections.Generic;

namespace Currency
{
    public static class CurrencyFunction
    {
        [FunctionName("CurrencyFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var config = Configuration.Default.WithDefaultLoader();
            var address = "https://www.banreservas.com/calculators/divisas";
            var bankName = "BanReservas";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            
            var dollarSellSelector = "#edit-sell-wrapper .form-label-text";
            var valueDollarBuy = document.QuerySelectorAll(dollarSellSelector);
            var value = (valueDollarBuy.FirstOrDefault().TextContent).Split('=')[1];
            value = value.Substring(value.LastIndexOf("RD$") + 3).Trim();

            return new OkObjectResult(new BankCurrencyRate {
                CurrencyRates = new List<CurrencyRate>() {
                    new CurrencyRate{
                        Currency = "USD",
                        Rate = double.Parse(value),
                        RateCurrency = "DOP"
                    }
                },
                BankName = bankName
            });
        }
    }

    public class BankCurrencyRate {
        public IEnumerable<CurrencyRate> CurrencyRates { get; set; }
        public string BankName { get; set; }
    }

    public class CurrencyRate
    {
        public string Currency { get; set; }
        public double Rate { get; set; }
        public string RateCurrency { get; set; }
        public string BankName { get; set; }
    }
}
