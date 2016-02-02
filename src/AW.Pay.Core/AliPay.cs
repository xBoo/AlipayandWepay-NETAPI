using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AW.Pay.Core.Interface;
using System.Web;
using System.Security.Cryptography;
using AW.Pay.Core.Model;
using System.Collections.Specialized;

namespace AW.Pay.Core
{
    public class AliPay : IAlipay
    {
        public string BuildAliPay(string orderNo, string subject, decimal payAmount, EnumAliPayTradeType tradeType)
        {
            return this.BuildAliPay(orderNo, subject, payAmount, tradeType);
        }

        public bool VerifyReturnURL(HttpRequestBase request, out AliPayReturnModel model)
        {
            var requestVal = request.QueryString;
            return Verify(request, requestVal, out model);
        }

        public bool VerfyNotify(HttpRequestBase request, out AliPayReturnModel model)
        {
            var requestVal = request.Form;
            return Verify(request, requestVal, out model);
        }

        #region private method

        private bool Verify(HttpRequestBase request, NameValueCollection requestVal, out AliPayReturnModel model)
        {
            bool result = false;
            SortedDictionary<string, string> sortedDic = new SortedDictionary<string, string>();
            foreach (var item in requestVal.AllKeys)
            {
                if (item.ToLower() != "sign" && item.ToLower() != "sign_type" && !string.IsNullOrEmpty(item))
                    sortedDic.Add(item, requestVal[item]);
            }

            string requestSign = requestVal["sign"];
            string requestSigntype = requestVal["sign_type"];
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
                result = RSAFromPkcs8.verify(param, requestSign, AlipayConfig.ALIPay_RSA_ALI_PUBLICKEY, "utf-8");

            string responseText = GetResponseTxt(requestVal["notify_id"]);

            bool resultVal = result && responseText == "true";
            if (resultVal)
            {
                model = new AliPayReturnModel()
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

        private string BuildRequest(string orderNo, string subject, decimal totalAmt, EnumAliPayTradeType aliPayType)
        {
            var signType = aliPayType == EnumAliPayTradeType.APP ? EnumSignType.RSA : EnumSignType.MD5;

            SortedDictionary<string, string> dicParam = CreateParam(orderNo, subject, totalAmt, aliPayType);
            string urlParam = CreateURLParamString(dicParam, aliPayType);

            string sign = BuildRequestsign(urlParam, signType);
            dicParam.Add("sign_type", signType.ToString());

            if (aliPayType == EnumAliPayTradeType.APP)
            {
                //APP支付URL字段须进行URL编码，具体出处参看官方文档
                dicParam.Add("sign", HttpUtility.UrlEncode(sign, Encoding.UTF8));
                return urlParam + "&sign=\"" + sign + "\"&sign_type=\"" + signType.ToString() + "\"";
            }
            else
            {
                dicParam.Add("sign", sign);
                return BuildForm(dicParam);
            }
        }

        private string BuildForm(SortedDictionary<string, string> dicParam)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + AlipayConfig.ALIPay_URL + "_input_charset=" + AlipayConfig.CHARTSET + "' method='get'>");

            foreach (KeyValuePair<string, string> temp in dicParam)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            sbHtml.Append("<input type='submit' value='确认' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");
            return sbHtml.ToString();
        }

        private SortedDictionary<string, string> CreateParam(string orderNo, string subject, decimal totalAmt, EnumAliPayTradeType aliPayType)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            #region BASEPARAM

            string service = aliPayType == EnumAliPayTradeType.Website ? AlipayConfig.ALIPay_WEB_SERVICE
                            : aliPayType == EnumAliPayTradeType.Wap ? AlipayConfig.ALIPay_WAP_SERVICE
                            : aliPayType == EnumAliPayTradeType.APP ? AlipayConfig.ALIPay_MOBILE_SERVICE
                            : "";

            dic.Add("service", service);
            dic.Add("partner", AlipayConfig.ALI_PARTER);
            dic.Add("_input_charset", AlipayConfig.CHARTSET);
            dic.Add("notify_url", AlipayConfig.ALIPay_NotifyURL);

            //dic.Add("sign_type", SIGNTYPE); 
            #endregion

            #region BIZPARAM
            dic.Add("out_trade_no", orderNo);
            dic.Add("subject", subject);
            dic.Add("payment_type", AlipayConfig.PAYMENT_TYPE);
            dic.Add("total_fee", totalAmt.ToString("F2"));
            //dic.Add("seller_email", ALI_SELLEREMAIL);
            dic.Add("seller_id", AlipayConfig.ALI_SELLERID);
            //dic.Add("anti_phishing_key", anti_phishing_key);//防钓鱼时间戳,如果已申请开通防钓鱼证，则此字段必填。
            //dic.Add("exter_invoke_ip", exter_invoke_ip);//客户端 IP ,如果商户申请后台开通防钓鱼 IP地址检查选项，此字段必填，校验用。 
            #endregion

            if (aliPayType == EnumAliPayTradeType.APP)
                dic.Add("body", subject + "购买");

            return dic;
        }

        private string CreateURLParamString(SortedDictionary<string, string> dicArray, EnumAliPayTradeType type = EnumAliPayTradeType.Website)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray.OrderBy(o => o.Key))
            {
                if (type == EnumAliPayTradeType.APP)
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
                mysign = MD5Helper.Sign(urlParam, AlipayConfig.ALI_KEY, AlipayConfig.CHARTSET);
            else if (signType == EnumSignType.RSA)
                mysign = RSASign(urlParam, AlipayConfig.ALIPay_RSA_PRIVATEKEY, AlipayConfig.CHARTSET);

            return mysign;
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
            string veryfy_url = AlipayConfig.ALI_HTTPS_VERYFY_URL + "&partner=" + AlipayConfig.ALI_PARTER + "&notify_id=" + notify_id;
            string response = HTTPHelper.Get(veryfy_url, 120000);
            return response;
        }


        #endregion
    }
}
