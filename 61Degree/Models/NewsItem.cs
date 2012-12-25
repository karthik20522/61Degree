using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace _61Degree.Models
{
    public class NewsItem
    {
        [BsonId]
	    public string _id { get; set; }

        public string title { get; set; }
        public string url { get; set; }
        public string publisher { get; set; }
        public string category { get; set; }
        public string time { get; set; }        
        public List<Image> images { get; set; }

        public long publishedDate { get; set; }
        public long modifiedDate { get; set; }

        public bool Active { get; set; }
    }
}