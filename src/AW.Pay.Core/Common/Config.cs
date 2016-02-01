using System.Configuration;

namespace AW.Pay.Core
{
    public static class Config
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
}
