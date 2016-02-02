using AW.Pay.Core.Model;
using System.Web;

namespace AW.Pay.Core.Interface
{
    public interface IAlipay
    {
        /// <summary>
        /// 创建支付宝支付
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="subject">标题</param>
        /// <param name="payAmount">支付金额</param>
        /// <param name="tradeType">交易类型（网站支付、wap支付、APP支付）</param>
        /// <returns></returns>
        string BuildAliPay(string orderNo, string subject, decimal payAmount, EnumAliPayTradeType tradeType);

        /// <summary>
        /// 验证支付宝回调，并获取相关返回参数
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="model">当验证成功后，获取主要返回参数</param>
        /// <returns>验证结果</returns>
        bool VerifyReturnURL(HttpRequestBase request, out AlipayReturnModel model);

        /// <summary>
        /// 验证支付宝异步通知，并获取相关返回参数
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="model">当验证成功后，获取主要返回参数</param>
        /// <returns>验证结果</returns>
        bool VerfyNotify(HttpRequestBase request, out AlipayReturnModel model);
    }
}
