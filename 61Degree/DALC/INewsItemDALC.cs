using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _61Degree.Models;

namespace _61Degree.DALC
{
    interface INewsItemDALC
    {
        List<NewsItem> GetNewsItems(int take = 1000, int page = 1, int days = 3);
        List<NewsItem> GetNewsItems(List<string> itemIds);
        List<NewsItem> AddNewsItems(List<NewsItem> items);
        void DeleteNewsItems(List<string> itemIds);
        List<NewsItem> UpdateNewsItems(List<NewsItem> items);
    }
}