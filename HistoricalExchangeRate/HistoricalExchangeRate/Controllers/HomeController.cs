using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HistoricalExchangeRate.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace HistoricalExchangeRate.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index(FormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // geting parameters from route

                var dates = model.Dates.Split(',');

                var ExchangeRateModel = new FormViewModel();

                List<DateTime> Listdates = dates.Select(date => DateTime.Parse(date)).ToList();

                using (var client = new HttpClient())
                {
                    try
                    {

                        var rCurrency = model.Currency;
                        var bCurrency = model.BaseCurrency;

                        //getting data from api
                        var apiUri = "/history?start_at=" + Listdates.Min().ToString("yyyy-MM-dd") + "&end_at=" + Listdates.Max().ToString("yyyy-MM-dd") + "&symbols=" + rCurrency + "&base=" + bCurrency;

                        client.BaseAddress = new Uri("https://api.exchangeratesapi.io");
                        var response = await client.GetAsync(apiUri);
                        response.EnsureSuccessStatusCode();

                        var stringResult = await response.Content.ReadAsStringAsync();

                        var exchangeRates = JsonConvert.DeserializeObject<ExchangeRate>(stringResult);

                        var rates = exchangeRates.Rates.Where(x => Listdates.Contains(x.Key.Date));

                        var onlyRates = rates.Select(x => new Rates
                        {
                            Cyrrency = x.Value.Keys.First().ToString(),
                            Rate = x.Value.Values.First(),
                            Date = x.Key
                        });

                        var max = onlyRates.Select(x => x.Rate).Max();
                        var min = onlyRates.Select(x => x.Rate).Min();

                        model.Result = (new ExchangeRateResult
                        {
                            MaximumExchangeRate = "A max rate of " + max + " on " + onlyRates.First(x => x.Rate == max).Date.ToString("yyyy-MM-dd"),
                            MinimumExchangeRate = "A min rate of " + min + " on " + onlyRates.First(x => x.Rate == min).Date.ToString("yyyy-MM-dd"),
                            AverageRate = "An average rate of " + onlyRates.Select(x => x.Rate).Average().ToString()
                        });


                    }
                    catch (HttpRequestException httpRequestException)
                    {
                        return BadRequest($"Error getting exchangeRates from exchangerates api: {httpRequestException.Message}");
                    }
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
