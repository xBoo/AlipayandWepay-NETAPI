using AW.Pay.Core.Enum;
using AW.Pay.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AW.Pay.Core.Interface
{
    public interface IWePay
    {
        /// <summary>
        /// 创建微信支付
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="productName">产品名称</param>
        /// <param name="totalFee">总金额，单位分</param>
        /// <param name="customerIP">调用IP</param>
        /// <param name="tradeType">交易类型（公众号支付、扫码支付、APP、WAP支付）</param>
        /// <returns>
        /// 扫码支付：返回支付URL
        /// APP支付：返回Json字符串，包含支付sdk支付参数
        /// 公众号支付&WAP支付：暂未实现
        /// </returns>
        string BuildWePay(string orderNo, string productName, int totalFee, string customerIP, EnumWePayTradeType tradeType);

        /// <summary>
        /// 微信支付异步通知验证
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="model">当验证成功后，获取主要返回参数</param>
        /// <returns>验证结果</returns>
        bool VerifyNotify(HttpRequestBase request, out WePayReturnModel model);
    }
}
