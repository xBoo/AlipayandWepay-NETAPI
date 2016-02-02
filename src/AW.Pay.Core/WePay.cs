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
using System.Web;
using AW.Pay.Core.Model;

namespace AW.Pay.Core
{
    public class WePay : IWePay
    {
        public string BuildWePay(string orderNo, string productName, int totalFee, string customerIP, EnumWePayTradeType tradeType)
        {
            return this.UnifiedOrder(orderNo, productName, totalFee, customerIP, tradeType);
        }

        public bool VerifyNotify(HttpRequestBase request, out WepayReturnModel model)
        {
            bool verifyResult = false;
            model = new WepayReturnModel();

            string requestXml = GetRequestXmlData(request);
            var dic = FromXml(requestXml);

            string returnCode = GetValueFromDic<string>(dic, "return_code");

            if (!string.IsNullOrEmpty(returnCode) && returnCode == "SUCCESS")//通讯成功
            {
                bool result = WePayNotifyValidation(dic);
                if (result)
                {
                    string transactionid = GetValueFromDic<string>(dic, "transaction_id");

                    if (!string.IsNullOrEmpty(transactionid))
                    {
                        string queryXml = BuildQueryRequest(transactionid, dic);
                        string queryResult = HTTPHelper.Post(WepayConfig.WEPAY_ORDERQUERY_URL, queryXml);
                        var queryReturnDic = FromXml(queryResult);

                        if (ValidatonQueryResult(queryReturnDic))//查询成功
                        {
                            verifyResult = true;
                            model.OutTradeNo = GetValueFromDic<string>(dic, "out_trade_no");
                            model.TotalFee = GetValueFromDic<decimal>(dic, "total_fee") / 100;
                            model.TradeNo = transactionid;
                            model.TradeStatus = GetValueFromDic<string>(dic, "result_code");
                            model.ReturnXml = BuildReturnXml("OK", "成功");
                        }
                        else
                            model.ReturnXml = BuildReturnXml("FAIL", "订单查询失败");
                    }
                    else
                        model.ReturnXml = BuildReturnXml("FAIL", "支付结果中微信订单号不存在");
                }
                else
                    model.ReturnXml = BuildReturnXml("FAIL", "签名失败");
            }
            else
            {
                string returnmsg;
                dic.TryGetValue("return_msg", out returnmsg);
                throw new Exception("异步通知错误：" + returnmsg);
            }

            return verifyResult;
        }




        #region private method
        /// <summary>
        /// 统一下单
        /// </summary>
        /// <returns></returns>
        private string UnifiedOrder(string orderNo, string productName, int totalFee, string customerIP, EnumWePayTradeType tradeType)
        {
            string requestXml = this.BuildRequest(orderNo, productName, totalFee, customerIP, tradeType);
            string resultXml = HTTPHelper.Post(WepayConfig.WEPAY_PAY_URL, requestXml);

            var dic = FromXml(resultXml);

            string returnCode = "";
            dic.TryGetValue("return_code", out returnCode);

            if (returnCode == "SUCCESS")
            {
                if (tradeType == EnumWePayTradeType.APP)
                {
                    var prepay_id = GetValueFromDic<string>(dic, "prepay_id");
                    if (!string.IsNullOrEmpty(prepay_id))
                        return BuildAppPay(prepay_id);
                    else
                        throw new Exception("支付错误:" + GetValueFromDic<string>(dic, "err_code_des"));
                }
                else if (tradeType == EnumWePayTradeType.NATIVE)
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

        private string BuildRequest(string orderNo, string productName, int totalFee, string customerIP, EnumWePayTradeType tradeType)
        {
            SortedDictionary<string, string> dicParam = CreateParam(orderNo, productName, totalFee, customerIP, tradeType);

            string signString = CreateURLParamString(dicParam);
            string sign = MD5Helper.Sign(signString, tradeType == EnumWePayTradeType.APP ? WepayConfig.WEPAY_APP_KEY : WepayConfig.WEPAY_WEB_KEY, WepayConfig.WEPAY_CHARTSET).ToUpper();
            dicParam.Add("sign", sign);

            return BuildForm(dicParam);
        }

        private static SortedDictionary<string, string> CreateParam(string orderNo, string productName, decimal totalFee, string customerIP, EnumWePayTradeType tradeType)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("appid", tradeType == EnumWePayTradeType.APP ? WepayConfig.WEPAY_APP_APPID : WepayConfig.WEPAY_WEB_APPID);//账号ID
            dic.Add("mch_id", tradeType == EnumWePayTradeType.APP ? WepayConfig.WEPAY_APP_MCH_ID : WepayConfig.WEPAY_WEB_MCH_ID);//商户号
            dic.Add("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            dic.Add("body", productName);//商品描述
            dic.Add("out_trade_no", orderNo);//商户订单号
            dic.Add("total_fee", totalFee.ToString());//总金额
            dic.Add("spbill_create_ip", customerIP);//终端IP
            dic.Add("notify_url", tradeType == EnumWePayTradeType.APP ? WepayConfig.WEPAY_APP_NOTIFY_URL : WepayConfig.WEPAY_WEB_NOTIFY_URL);//通知地址
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

        private string GetRequestXmlData(HttpRequestBase request)
        {
            System.IO.Stream stream = request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = stream.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            stream.Flush();
            stream.Close();
            stream.Dispose();

            return builder.ToString();
        }

        public static bool WePayNotifyValidation(SortedDictionary<string, string> dic)
        {
            string sign = GetValueFromDic<string>(dic, "sign");
            if (dic.ContainsKey("sign"))
            {
                dic.Remove("sign");
            }

            string tradeType = GetValueFromDic<string>(dic, "trade_type");
            string preString = CreateURLParamString(dic);
            string signString;

            if (!string.IsNullOrEmpty(tradeType) && tradeType == EnumWePayTradeType.APP.ToString())//app支付
                signString = MD5Helper.Sign(preString, WepayConfig.WEPAY_APP_KEY, WepayConfig.WEPAY_CHARTSET).ToUpper();
            else
                signString = MD5Helper.Sign(preString, WepayConfig.WEPAY_WEB_KEY, WepayConfig.WEPAY_CHARTSET).ToUpper();

            return signString == sign;
        }

        private static string BuildReturnXml(string code, string returnMsg)
        {
            return string.Format("<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", code, returnMsg);
        }

        public static string BuildQueryRequest(string transactionId, SortedDictionary<string, string> dic)
        {
            string tradeType = GetValueFromDic<string>(dic, "trade_type");
            bool isApp = tradeType == EnumWePayTradeType.APP.ToString();

            SortedDictionary<string, string> dicParam = CreateQueryParam(transactionId, isApp);
            string signString = CreateURLParamString(dicParam);
            string sign = MD5Helper.Sign(signString, isApp ? WepayConfig.WEPAY_APP_KEY : WepayConfig.WEPAY_WEB_KEY, "utf-8").ToUpper();
            dicParam.Add("sign", sign);

            return BuildForm(dicParam);
        }

        private static SortedDictionary<string, string> CreateQueryParam(string transactionId, bool isApp)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            dic.Add("appid", isApp ? WepayConfig.WEPAY_APP_APPID : WepayConfig.WEPAY_WEB_APPID);//公众账号ID
            dic.Add("mch_id", isApp ? WepayConfig.WEPAY_APP_MCH_ID : WepayConfig.WEPAY_WEB_MCH_ID);//商户号
            dic.Add("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            dic.Add("transaction_id", transactionId);//随机字符串
            return dic;
        }

        private static bool ValidatonQueryResult(SortedDictionary<string, string> dic)
        {
            bool result = false;

            if (dic.ContainsKey("return_code") && dic.ContainsKey("return_code"))
            {
                if (dic["return_code"].ToString() == "SUCCESS" && dic["result_code"].ToString() == "SUCCESS")
                    result = true;
            }

            if (!result)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in dic.Keys)
                {
                    sb.Append(item + ":" + dic[item] + "|");
                }
            }

            return result;
        }
        #endregion
    }
}
