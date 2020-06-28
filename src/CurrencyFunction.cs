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
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            
            var dollarBuySelector = "#edit-buy-wrapper .form-label-text";
            var valueDollarBuy = document.QuerySelectorAll(dollarBuySelector);
            var value = (valueDollarBuy.FirstOrDefault().TextContent).Split('=')[1];
            value = value.Substring(value.LastIndexOf("RD$") + 3).Trim();
            return new OkObjectResult(new {
                Currency = "USD",
                Rate = value,
                RateCurrency = "DOP"
            });
        }
    }
}
