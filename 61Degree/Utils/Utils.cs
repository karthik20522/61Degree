using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _61Degree
{
    public static class Utils
    {
        private static string[] mobileDevices = new string[] {"iphone","ppc", "windows ce","blackberry", "opera mini","mobile","palm", "portable","opera mobi" };

        public static bool IsMobileDevice(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;

            userAgent = userAgent.ToLower();
            return mobileDevices.Any(x => userAgent.Contains(x));
        }

        public static string ToUpper(string str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.Trim());
        }

        public static string RemoveSpecialCharacters(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string TrimSentence(string str, int length = 30)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (str.Length <= length)
                return str;
            else
                return str.Substring(0, length) + "...";
        }

        public static List<T> Randomize<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        public static string GetColor(string type)
        {
            if(string.IsNullOrEmpty(type))
                return "others";

            //news - B9BC8D
            //science and tech - E2640F
            //business - 7EC2B3
            //sports - C6DCAB
            //entertainment - 42749F
            //politics - BF381A
            //life - C84D5F
            //Opinion - E07628
            //others - BCD7D0

            if (type.Contains("news") || type.Contains("us") || type.Contains("uk") ||
                type.Contains("most-popular") || type.Contains("ushome") || type.Contains("crime"))
            {
                return "news";
            }

            if (type.Contains("technology") || type.Contains("science") || type.Contains("tech"))
            {
                return "science";
            }

            if (type.Contains("finance") || type.Contains("business") || type.Contains("money") || type.Contains("market"))
            {
                return "business";
            }

            if (type.Contains("sports") || type.Contains("nfl") || type.Contains("sport") || type.Contains("world-football") || type.Contains("mlb") ||
                type.Contains("nba") || type.Contains("football") || type.Contains("college-basketball") || type.Contains("college-football") || 
                type.Contains("ufc") || type.Contains("foxsoccer") || type.Contains("boxing") || type.Contains("nhl") || type.Contains("wwe"))
            {
                return "sports";
            }

            if (type.Contains("entertainment") || type.Contains("guilty-pleasures") || type.Contains("style") || type.Contains("celebrity-news") ||
                type.Contains("culture") || type.Contains("ents- tv-and-radio") || type.Contains("tv-and-radio") || type.Contains("movies"))
            {
                return "entertainment";
            }

            if (type.Contains("politics") || type.Contains("3am") || type.Contains("nation") || type.Contains("national"))
            {
                return "politics";
            }

            if (type.Contains("health") || type.Contains("life-style") || type.Contains("life") || type.Contains("arts") || type.Contains("travel"))
            {
                return "life";
            } 

            if (type.Contains("comment") || type.Contains("opinion"))
            {
                return "opinion";
            }

            return "others";
        }

        public static Task<string> GetData(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 20000;
            request.KeepAlive = false;

            Task<WebResponse> task = Task.Factory.FromAsync( request.BeginGetResponse, asyncResult => request.EndGetResponse(asyncResult), (object)null);
            return task.ContinueWith(t => 
                ReadStreamFromResponse(t.Result)
            );
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(responseStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}