using System.Configuration;

namespace AW.Pay.Core
{
    public static class AlipayConfig
    {
        #region Alipay config
        public static string ALIPay_URL = ConfigurationManager.AppSettings["ALIPay_URL"];
        public static string ALIPay_NotifyURL = ConfigurationManager.AppSettings["ALIPay_NotifyURL"];
        public static string ALIPay_ErrorURL = ConfigurationManager.AppSettings["ALIPay_ErrorURL"];
        public static string ALI_PARTER = ConfigurationManager.AppSettings["ALI_PARTER"];
        public static string ALI_KEY = ConfigurationManager.AppSettings["ALI_KEY"];
        public static string ALI_ACCOUNT = ConfigurationManager.AppSettings["ALI_ACCOUNT"];
        public static string CHARTSET = "utf-8";//固定值
        public static string PAYMENT_TYPE = "1";//固定值
        public static string ALI_SELLERID = ConfigurationManager.AppSettings["ALI_SELLERID"];
        public static string ALI_SELLEREMAIL = ConfigurationManager.AppSettings["ALI_SELLEREMAIL"];
        public static string ALI_HTTPS_VERYFY_URL = ConfigurationManager.AppSettings["ALI_HTTPS_VERYFY_URL"];

        public static string ALIPay_WAP_SERVICE = ConfigurationManager.AppSettings["ALIPay_WAP_SERVICE"];
        public static string ALIPay_WEB_SERVICE = ConfigurationManager.AppSettings["ALIPay_WEB_SERVICE"];
        public static string ALIPay_MOBILE_SERVICE = ConfigurationManager.AppSettings["ALIPay_MOBILE_SERVICE"];

        public static string ALIPay_RSA_PUBLICKEY = ConfigurationManager.AppSettings["ALIPay_RSA_PUBLICKEY"];
        public static string ALIPay_RSA_PRIVATEKEY = ConfigurationManager.AppSettings["ALIPay_RSA_PRIVATEKEY"];
        public static string ALIPay_RSA_ALI_PUBLICKEY = ConfigurationManager.AppSettings["ALIPay_RSA_ALI_PUBLICKEY"];
        #endregion
    }

    public static class WepayConfig
    {
        public static string WEPAY_CHARTSET = "utf-8";
        public static string WEPAY_PAY_URL = ConfigurationManager.AppSettings["WEPAY_PAY_URL"];//统一下单URL
        public static string WEPAY_ORDERQUERY_URL = ConfigurationManager.AppSettings["WEPAY_ORDERQUERY_URL"];

        #region 微信开发者平台（APP支付）
        public static string WEPAY_APP_APPID = ConfigurationManager.AppSettings["WEPAY_MP_APPID"];
        public static string WEPAY_APP_MCH_ID = ConfigurationManager.AppSettings["WEPAY_MP_MCH_ID"];
        public static string WEPAY_APP_NOTIFY_URL = ConfigurationManager.AppSettings["WEPAY_MP_NOTIFY_URL"];
        public static string WEPAY_APP_URL = ConfigurationManager.AppSettings["WEPAY_MP_URL"];
        public static string WEPAY_APP_KEY = ConfigurationManager.AppSettings["WEPAY_APP_KEY"];


        #endregion

        #region 微信公众平台（扫码、公众号支付）
        public static string WEPAY_WEB_APPID = ConfigurationManager.AppSettings["WEPAY_WEB_APPID"];
        public static string WEPAY_WEB_MCH_ID = ConfigurationManager.AppSettings["WEPAY_WEB_MCH_ID"];
        public static string WEPAY_WEB_NOTIFY_URL = ConfigurationManager.AppSettings["WEPAY_WEB_NOTIFY_URL"];
        public static string WEPAY_WEB_URL = ConfigurationManager.AppSettings["WEPAY_WEB_URL"];
        public static string WEPAY_WEB_KEY = ConfigurationManager.AppSettings["WEPAY_WEB_KEY"];
        #endregion

    }
}
