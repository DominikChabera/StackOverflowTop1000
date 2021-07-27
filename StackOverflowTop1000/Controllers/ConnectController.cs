using Newtonsoft.Json;
using StackOverflowTop1000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace StackOverflowTop1000.Controllers
{
    public class ConnectController : Controller
    {
        private readonly string _key = "QMlckro*2cRmCebTl7Yjig((";
        private readonly int _pagesize = 100;
        private readonly int _page = 10;
        private List<string> _resultJson = new List<string>();
        private int _totalCount;

        public ActionResult Index()
        {
            try
            {
                var tag = ConvertJsonToObject();
                CalculateTheTotalCountOfTheTags(tag);
                CalculatePercentPopularityOfTag(tag);
                return View(tag);

            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }



        private void Get1000TopTag()
        {
            for (int i = 1; i <= _page; i++) // Ograniczenie API to 100 tagów na stronę wiec muszę mieć 10 różnych stron pobranych.
            {
                string url = "/2.3/tags?key=" + _key + "&site=stackoverflow&pagesize=" + _pagesize + "&order=desc&sort=popular&filter=default&page=" + i;
                HttpClientHandler handler = new HttpClientHandler();
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using (var Client = new HttpClient(handler))
                {
                    Client.BaseAddress = new Uri("https://api.stackexchange.com/");
                    //HTTP GET
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jason"));
                    var response = Client.GetAsync(url);
                    response.Wait();
                    var result = response.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        _resultJson.Add(result.Content.ReadAsStringAsync().Result);
                    }
                    Thread.Sleep(2000); // Zatrzymuje program na 2 sek. Co pętla, aby nie obciążać za bardzo serwera i aby uniknąć rozłączenie w wyniku ograniczeń narzuconych przez API.
                }
            }
        }

        private List<TagList> ConvertJsonToObject() // deserializuje dane w formacie Json do formatu List<TagList> aby móc przeprowadzać operacje na obiektach
        {
            try
            {
                var tagLit = new List<TagList>();
                foreach (var item in _resultJson)
                {
                    tagLit.Add(JsonConvert.DeserializeObject<TagList>(item));
                }
                return tagLit;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException();
            }
            catch(ArgumentNullException)
            {
                throw new ArgumentNullException();
            }

        }
        private void CalculateTheTotalCountOfTheTags(List<TagList> tags) // Zliczam całkowitą liczbę użycia wszystkich tagów, aby móc obliczyć ich procent wykorzystywania w danej próbce danych.
        {
            foreach (var tag in tags)
            {
                _totalCount += tag.Items.Sum(s => s.count);
            }
        }

        private void CalculatePercentPopularityOfTag(List<TagList> tags) // Oblicza jak popularny jest dany tag mnożę razy 100
        {                                                                // gdyż normalnie po podsumowaniu wszystkich pojedynczych wyników pokazywało jako 1.00 a nie 100
            foreach (var tag in tags)                                    // więc przesunąłem przecinek, aby liczba wydawała się bardziej przystępna, bo nie każdy uzna 1.00 za 100%.
            {
                tag.Items.ForEach(c => { c.popularityPercent = ((decimal)c.count / (decimal)_totalCount) * 100; });
            }
        }
    }
}