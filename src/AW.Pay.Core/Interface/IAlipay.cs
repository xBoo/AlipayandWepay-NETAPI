using AW.Pay.Core.Model;
using System.Web;

namespace AW.Pay.Core.Interface
{
    public interface IAlipay
    {
        /// <summary>
        /// 创建支付宝APP支付
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="subject">标题</param>
        /// <param name="payAmount">支付金额</param>
        /// <param name="signType">加密类型</param>
        /// <returns>支付宝支付参数</returns>
        string BuildMobilePay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.RSA);

        /// <summary>
        /// 创建支付宝Wap支付（支付宝暂未开发此方式）
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="subject">标题</param>
        /// <param name="payAmount">支付金额</param>
        /// <param name="signType">加密类型</param>
        /// <returns>支付宝支付参数</returns>
        string BuildWapPay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.MD5);

        /// <summary>
        /// 创建支付宝Web支付
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="subject">标题</param>
        /// <param name="payAmount">支付金额</param>
        /// <param name="signType">加密类型</param>
        /// <returns>支付宝支付参数</returns>
        string BuildWebPay(string orderNo, string subject, decimal payAmount, EnumSignType signType = EnumSignType.MD5);

        /// <summary>
        /// 验证支付宝回调，并获取相关返回参数
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="model">当验证成功后，获取主要返回参数</param>
        /// <returns>验证结果</returns>
        bool VerifyReturnUrl(HttpRequestBase request,out AlipayReturnModel model);
    }
}
