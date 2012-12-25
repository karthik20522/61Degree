using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using _61Degree.Models;
using _61Degree.DALC;

namespace _61Degree.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        NewsItemDALC _newsItemDALC;

        public HomeController()
        {
            _newsItemDALC = new NewsItemDALC();
        }

        [OutputCache(Duration=60)]
        public async Task<ActionResult> Index()
        {
            var maxDays = Utils.IsMobileDevice(Request.UserAgent) ? 1 : 3;

            var newsItems = await Task.Factory.StartNew<List<NewsItem>>(() =>
            {
                return _newsItemDALC.GetNewsItems(take: 800, days: maxDays);
            });

            var groupedItems = (from i in newsItems
                    let dt = new DateTime(i.publishedDate)
                    group i by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                    select new NewsView { 
                        Key = new DateTime(g.Key.y, g.Key.m, g.Key.d).ToString("D"), 
                        Values = g.Select(s=> s).ToList() 
                    }).ToList();

            //do background fetch - no need to wait. 
            //the news wouldn't be most recent but that's fine by me!
            Task.Factory.StartNew(() => { GetLatest(); });

            return View("Index", groupedItems);
        }

        private void GetLatest()
        {
            FetchNewsItems("/*yourfeedsource*/");
        }

        private void FetchNewsItems(string publisher)
        {
            try
            {
                var newsData = Utils.GetData(publisher).Result;
                var list = JsonConvert.DeserializeObject<List<NewsItem>>(newsData);

                _newsItemDALC.AddNewsItems(list);
            }
            catch { }
        }
    }
}
