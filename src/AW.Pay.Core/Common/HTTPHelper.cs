using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core
{
    public static class HTTPHelper
    {
        public static string Post(string url, string content, string contentType = "application/x-www-form-urlencoded")
        {
            string result = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    var stringContent = new StringContent(content, Encoding.UTF8);
                    var response = client.PostAsync(url, stringContent).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw new Exception("POST请求错误" + e.ToString());
            }
            return result;
        }

        public static string Get(string url, int timeout, string contentType = "application/x-www-form-urlencoded")
        {
            string result = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 0, 0, timeout);
                    var response = client.GetAsync(url).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                };
            }
            catch (Exception e)
            {
                throw new Exception("GET请求错误" + e.ToString());
            }
            return result;
        }
    }
}
