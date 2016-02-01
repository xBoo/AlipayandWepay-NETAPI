using System;
using System.Collections.Generic;
using System.Linq;
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
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.ContentType = contentType;
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);//这里编码设置为utf8
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                result = sr.ReadToEnd().Trim();
            }
            catch (Exception e)
            {
                throw new Exception("POST请求错误" + e.ToString());
            }
            return result;
        }

        public static string Get(string url,int timeout, string contentType = "application/x-www-form-urlencoded")
        {
            string result = string.Empty;
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.ContentType = contentType;
                req.Method = "GET";
                req.Timeout = timeout;
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                result = sr.ReadToEnd().Trim();
            }
            catch (Exception e)
            {
                throw new Exception("GET请求错误" + e.ToString());
            }
            return result;
        }
    }
}
