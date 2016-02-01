using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AW.Pay.Core.Interface;
using System.Web;
using System.Security.Cryptography;
using AW.Pay.Core.Model;

namespace AW.Pay.Core
{
    public class AliPay : IAlipay
    {
        public string BuildMobilePay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.RSA)
        {
            return this.BuildRequest(orderNo, subject, payAmount, EnumAliPayType.Mobile, signType);
        }

        public string BuildWapPay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.MD5)
        {
            return this.BuildRequest(orderNo, subject, payAmount, EnumAliPayType.Wap, signType);
        }

        public string BuildWebPay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.MD5)
        {
            return this.BuildRequest(orderNo, subject, payAmount, EnumAliPayType.Website, signType);
        }

        public bool VerifyReturnUrl(HttpRequestBase request, out AlipayReturnModel model)
        {
            var queryString = request.QueryString;

            bool result = false;
            SortedDictionary<string, string> sortedDic = new SortedDictionary<string, string>();
            foreach (var item in queryString.AllKeys)
            {
                if (item.ToLower() != "sign" && item.ToLower() != "sign_type" && !string.IsNullOrEmpty(item))
                    sortedDic.Add(item, queryString[item]);
            }

            string requestSign = queryString["sign"];
            string requestSigntype = queryString["sign_type"];
            string param = CreateURLParamString(sortedDic);

            EnumSignType signType = requestSigntype == "MD5" ? EnumSignType.MD5
                                    : requestSigntype == "RSA" ? EnumSignType.RSA
                                    : EnumSignType.MD5;

            if (signType == EnumSignType.MD5)
            {
                string sign = BuildRequestsign(param, signType);
                if (requestSign.Equals(sign))
                    result = true;
            }
            else
                result = RSAFromPkcs8.verify(param, requestSign, Config.ALIPay_RSA_ALI_PUBLICKEY, "utf-8");

            string responseText = GetResponseTxt(queryString["notify_id"]);

            bool resultVal = result && responseText == "true";
            if (resultVal)
            {
                model = new AlipayReturnModel()
                {
                    OutTradeNo = request.Form["out_trade_no"],
                    TradeNo = request.Form["trade_no"],
                    TradeStatus = request.Form["trade_status"]
                };

                decimal total_fee;
                decimal.TryParse(request.Form["total_fee"], out total_fee);
                model.TotalFee = total_fee;
            }
            else
                model = null;

            return resultVal;
        }

        #region private method
        private string BuildRequest(string orderNo, string subject, decimal totalAmt, EnumAliPayType aliPayType, EnumSignType signType = EnumSignType.MD5)
        {
            SortedDictionary<string, string> dicParam = CreateParam(orderNo, subject, totalAmt, aliPayType);
            string urlParam = CreateURLParamString(dicParam, aliPayType);
            string sign = HttpUtility.UrlEncode(BuildRequestsign(urlParam, signType), Encoding.UTF8);
            dicParam.Add("sign", sign);
            dicParam.Add("sign_type", signType.ToString());

            if (aliPayType == EnumAliPayType.Mobile)
            {
                return urlParam + "&sign=\"" + sign + "\"&sign_type=\"" + signType.ToString() + "\"";
            }
            else
                return BuildForm(dicParam);
        }

        private string BuildForm(SortedDictionary<string, string> dicParam)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + Config.ALIPay_URL + "_input_charset=" + Config.CHARTSET + "' method='get'>");

            foreach (KeyValuePair<string, string> temp in dicParam)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            sbHtml.Append("<input type='submit' value='确认' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");
            return sbHtml.ToString();
        }

        private SortedDictionary<string, string> CreateParam(string orderNo, string subject, decimal totalAmt, EnumAliPayType aliPayType)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            #region BASEPARAM

            string service = aliPayType == EnumAliPayType.Website ? Config.ALIPay_WEB_SERVICE
                            : aliPayType == EnumAliPayType.Wap ? Config.ALIPay_WAP_SERVICE
                            : aliPayType == EnumAliPayType.Mobile ? Config.ALIPay_MOBILE_SERVICE
                            : "";

            dic.Add("service", service);
            dic.Add("partner", Config.ALI_PARTER);
            dic.Add("_input_charset", Config.CHARTSET);
            dic.Add("notify_url", Config.ALIPay_NotifyURL);

            //dic.Add("sign_type", SIGNTYPE); 
            #endregion

            #region BIZPARAM
            dic.Add("out_trade_no", orderNo);
            dic.Add("subject", subject);
            dic.Add("payment_type", Config.PAYMENT_TYPE);
            dic.Add("total_fee", totalAmt.ToString("F2"));
            //dic.Add("seller_email", ALI_SELLEREMAIL);
            dic.Add("seller_id", Config.ALI_SELLERID);
            //dic.Add("anti_phishing_key", anti_phishing_key);//防钓鱼时间戳,如果已申请开通防钓鱼证，则此字段必填。
            //dic.Add("exter_invoke_ip", exter_invoke_ip);//客户端 IP ,如果商户申请后台开通防钓鱼 IP地址检查选项，此字段必填，校验用。 
            #endregion

            if (aliPayType == EnumAliPayType.Mobile)
                dic.Add("body", subject + "购买");

            return dic;
        }

        private string CreateURLParamString(SortedDictionary<string, string> dicArray, EnumAliPayType type = EnumAliPayType.Website)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray.OrderBy(o => o.Key))
            {
                if (type == EnumAliPayType.Mobile)
                    prestr.Append(temp.Key + "=\"" + temp.Value + "\"&");
                else
                    prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }

        private string BuildRequestsign(string urlParam, EnumSignType signType)
        {
            string mysign = "";

            if (signType == EnumSignType.MD5)
                mysign = MD5Sign(urlParam, Config.ALI_KEY, Config.CHARTSET);
            else if (signType == EnumSignType.RSA)
                mysign = RSASign(urlParam, Config.ALIPay_RSA_PRIVATEKEY, Config.CHARTSET);

            return mysign;
        }

        private string MD5Sign(string prestr, string key, string _input_charset)
        {
            StringBuilder sb = new StringBuilder(32);
            prestr = prestr + key;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        private string RSASign(string prestr, string privateKey, string input_charset)
        {
            try
            {
                return RSAFromPkcs8.sign(prestr, privateKey, input_charset);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string GetResponseTxt(string notify_id)
        {
            string veryfy_url = Config.ALI_HTTPS_VERYFY_URL + "&partner=" + Config.ALI_PARTER + "&notify_id=" + notify_id;
            string response = HTTPHelper.Get(veryfy_url, 120000);
            return response;
        }
        #endregion
    }
}
