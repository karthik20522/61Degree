using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using _61Degree.Models;

namespace _61Degree.DALC
{
    public class NewsItemDALC : IDisposable, INewsItemDALC
    {
        private MongoServer server;
        private MongoDatabase db;

        public NewsItemDALC()
        {
            var mServerAddress = System.Configuration.ConfigurationManager.AppSettings["Server"];
            var dbName = System.Configuration.ConfigurationManager.AppSettings["DBName"];

            server = new MongoClient(mServerAddress).GetServer();
            db = server.GetDatabase(dbName);
        }

        public NewsItemDALC(string mongoServer, string dbName)
        {
            server = new MongoClient(mongoServer).GetServer();
            db = server.GetDatabase(dbName);
        }

        public void Dispose()
        {
            //server.Disconnect();
        }       

        /// <summary>
        /// Get News Items ordered by published date of the article
        /// </summary>
        /// <param name="take"></param>
        /// <param name="page"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public List<NewsItem> GetNewsItems(int take = 1000, int page = 1, int days = 3)
        {
            var newsItems = new List<NewsItem>();

            using (server.RequestStart(db))
            {
                var newsDb = db.GetCollection<NewsItem>("newsitems");
                var query = Query.GTE("publishedDate", DateTime.Today.AddDays(-days).Ticks);

                var documentCursor = newsDb.Find(query)
                    .SetSortOrder(SortBy.Descending("publishedDate"))
                    .SetSkip((page - 1) * take)
                    .SetLimit(take);

                if(documentCursor.Count() > 0)
                    newsItems = documentCursor.ToList<NewsItem>();
            }

            return newsItems;
        }

        /// <summary>
        /// Get News items by Ids
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        public List<NewsItem> GetNewsItems(List<string> itemIds)
        {
            var newsItems = new List<NewsItem>();
            
            using (server.RequestStart(db))
            {
                var newsDb = db.GetCollection<NewsItem>("newsitems");
                var query = Query.In("_id", new BsonArray(itemIds));
                var documentCursor = newsDb.Find(query);

                if (documentCursor.Count() > 0)
                    newsItems = documentCursor.ToList<NewsItem>();
            }
            
            return newsItems;
        }

        /// <summary>
        /// Add new News Items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<NewsItem> AddNewsItems(List<NewsItem> items)
        {
            using (server.RequestStart(db))
            {
                var newsDb = db.GetCollection<NewsItem>("newsitems");
                items = items.Where(s => SearchNewsItemByURL(s.url).Count == 0).ToList();

                items.ForEach(item =>
                {
                    item._id = ObjectId.GenerateNewId().ToString();
                    item.publishedDate = DateTime.Parse(item.time).Ticks;
                    item.modifiedDate = DateTime.Now.Ticks;
                });

                if(items.Count > 0)
                    newsDb.InsertBatch(items);
            }

            return items;
        }

        /// <summary>
        /// Search news item by url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<NewsItem> SearchNewsItemByURL(string url)
        {
            var newsItems = new List<NewsItem>();

            using (server.RequestStart(db))
            {
                var newsDb = db.GetCollection<NewsItem>("newsitems");
                var query = Query.EQ("url", url);
                var documentCursor = newsDb.Find(query);

                if (documentCursor.Count() > 0)
                    newsItems = documentCursor.ToList<NewsItem>();
            }

            return newsItems;
        }

        /// <summary>
        /// Delete news items by Ids
        /// </summary>
        /// <param name="itemIds"></param>
        public void DeleteNewsItems(List<string> itemIds)
        {
            using (server.RequestStart(db))
            {
                var assetdb = db.GetCollection<NewsItem>("newsitems");
                var query = Query.In("_id", new BsonArray(itemIds));
                assetdb.Remove(query);
            }
        }

        /// <summary>
        /// Update News Items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<NewsItem> UpdateNewsItems(List<NewsItem> items)
        {
            using (server.RequestStart(db))
            {
                var assetdb = db.GetCollection<NewsItem>("newsitems");
                var ticks = DateTime.Now.Ticks;

                items.ForEach(item =>
                {
                    item.modifiedDate = ticks;
                    assetdb.Save(item.ToBsonDocument());
                });
            }

            return items;
        }

        /// <summary>
        /// ensure index
        /// </summary>
        public void EnsureIndexes()
        {
            using (server.RequestStart(db))
            {
                var newsDb = db.GetCollection<NewsItem>("newsitems");

                var keys = IndexKeys.Descending("publishedDate");
                var options = IndexOptions.SetName("publishedDate");
                newsDb.EnsureIndex(keys, options);

                keys = IndexKeys.Descending("url");
                options = IndexOptions.SetName("url");
                newsDb.EnsureIndex(keys, options);
            }
        }
    }
}