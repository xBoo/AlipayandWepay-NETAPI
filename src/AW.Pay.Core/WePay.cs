using AW.Pay.Core.Enum;
using AW.Pay.Core.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AW.Pay.Core
{
    public class WePay : IWePay
    {

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        private string UnifiedOrder(string orderNo, string productName, int totalFee, string customerIP, EnumTradeType tradeType)
        {
            string requestXml = this.BuildRequest(orderNo, productName, totalFee, customerIP, tradeType);
            string resultXml = HTTPHelper.Post(WepayConfig.WEPAY_PAY_URL, requestXml);

            var dic = FromXml(resultXml);

            string returnCode = "";
            dic.TryGetValue("return_code", out returnCode);

            if (returnCode == "SUCCESS")
            {
                if (tradeType == EnumTradeType.APP)
                {
                    var prepay_id = GetValueFromDic<string>(dic, "prepay_id");
                    if (!string.IsNullOrEmpty(prepay_id))
                        return BuildAppPay(prepay_id);
                    else
                        throw new Exception("支付错误:" + GetValueFromDic<string>(dic, "err_code_des"));
                }
                else if (tradeType == EnumTradeType.NATIVE)
                {
                    string codeUrl = "";
                    dic.TryGetValue("code_url", out codeUrl);
                    if (string.IsNullOrEmpty(codeUrl))
                        return codeUrl;
                    else
                        throw new Exception("未找到对应的二维码链接");
                }
                else
                    throw new Exception("JSAPI & WAP 未实现");
            }
            else
                throw new Exception("后台统一下单失败");
        }

        private string BuildRequest(string orderNo, string productName, int totalFee, string customerIP, EnumTradeType tradeType)
        {
            SortedDictionary<string, string> dicParam = CreateParam(orderNo, productName, totalFee, customerIP, tradeType);

            string signString = CreateURLParamString(dicParam);
            string sign = MD5Helper.Sign(signString, tradeType == EnumTradeType.APP ? WepayConfig.WEPAY_APP_KEY : WepayConfig.WEPAY_WEB_KEY, WepayConfig.WEPAY_CHARTSET).ToUpper();
            dicParam.Add("sign", sign);

            return BuildForm(dicParam);
        }

        private static SortedDictionary<string, string> CreateParam(string orderNo, string productName, decimal totalFee, string customerIP, EnumTradeType tradeType)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("appid", tradeType == EnumTradeType.APP ? WepayConfig.WEPAY_APP_APPID : WepayConfig.WEPAY_WEB_APPID);//账号ID
            dic.Add("mch_id", tradeType == EnumTradeType.APP ? WepayConfig.WEPAY_APP_MCH_ID : WepayConfig.WEPAY_WEB_MCH_ID);//商户号
            dic.Add("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            dic.Add("body", productName);//商品描述
            dic.Add("out_trade_no", orderNo);//商户订单号
            dic.Add("total_fee", totalFee.ToString());//总金额
            dic.Add("spbill_create_ip", customerIP);//终端IP
            dic.Add("notify_url", tradeType == EnumTradeType.APP ? WepayConfig.WEPAY_APP_NOTIFY_URL : WepayConfig.WEPAY_WEB_NOTIFY_URL);//通知地址
            dic.Add("trade_type", tradeType.ToString());//交易类型

            return dic;
        }

        private static string CreateURLParamString(SortedDictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray.OrderBy(o => o.Key))
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }

        private static string BuildForm(SortedDictionary<string, string> dicParam)
        {
            StringBuilder sbXML = new StringBuilder();
            sbXML.Append("<xml>");
            foreach (KeyValuePair<string, string> temp in dicParam)
            {
                sbXML.Append("<" + temp.Key + ">" + temp.Value + "</" + temp.Key + ">");
            }

            sbXML.Append("</xml>");
            return sbXML.ToString();
        }

        private static SortedDictionary<string, string> FromXml(string xml)
        {
            SortedDictionary<string, string> sortDic = new SortedDictionary<string, string>();
            if (string.IsNullOrEmpty(xml))
            {
                throw new Exception("将空的xml串转换为WxPayData不合法!");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;

                if (!sortDic.ContainsKey(xe.Name))
                    sortDic.Add(xe.Name, xe.InnerText);
            }
            return sortDic;
        }

        private static T GetValueFromDic<T>(SortedDictionary<string, string> dic, string key)
        {
            string val;
            dic.TryGetValue(key, out val);

            T returnVal = default(T);
            if (val != null)
                returnVal = (T)Convert.ChangeType(val, typeof(T));

            return returnVal;
        }

        private static string BuildAppPay(string prepayid)
        {
            var dicParam = CreateWapAndAppPayParam(prepayid);
            string signString = CreateURLParamString(dicParam);
            string sign = MD5Helper.Sign(signString, WepayConfig.WEPAY_APP_KEY, WepayConfig.WEPAY_CHARTSET).ToUpper();
            dicParam.Add("sign", sign);

            return JsonConvert.SerializeObject(
                new
                {
                    appid = dicParam["appid"],
                    partnerid = dicParam["partnerid"],
                    prepayid = dicParam["prepayid"],
                    package = dicParam["package"],
                    noncestr = dicParam["noncestr"],
                    timestamp = dicParam["timestamp"],
                    sign = dicParam["sign"]
                });
        }

        private static SortedDictionary<string, string> CreateWapAndAppPayParam(string prepayId)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("appid", WepayConfig.WEPAY_APP_APPID);//公众账号ID
            dic.Add("partnerid", WepayConfig.WEPAY_APP_MCH_ID);//商户号
            dic.Add("prepayid", prepayId);//预支付交易会话ID
            dic.Add("package", "Sign=WXPay");//扩展字段
            dic.Add("noncestr", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            dic.Add("timestamp", (Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds)).ToString());//时间戳

            return dic;
        }
    }
}
