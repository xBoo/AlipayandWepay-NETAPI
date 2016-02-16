using AW.Pay.Core.Common;
using AW.Pay.Core.Interface;
using AW.Pay.Core.Model;
using AW.Webapi.Sample.Models.PayModel;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AW.Webapi.Sample.Areas.Api.Controllers
{
    public class PaymentController : ApiController
    {
        private readonly IAlipay _aliPay;
        private readonly IWePay _wePay;
        public PaymentController(IAlipay aliPay, IWePay wePay)
        {
            this._aliPay = aliPay;
            this._wePay = wePay;
        }
        /// <summary>
        /// 生成支付宝请求参数
        /// </summary>
        /// <param name="payInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public BizResult<string> CreateAliPayRequestParam(AliPayReqParam payInfo)
        {
            BizResult<string> biz = new BizResult<string>();
            biz.ReturnObject = this._aliPay.BuildAliPay(payInfo.OrderNo, payInfo.Subject, payInfo.TotalAmount, payInfo.Type); ;
            return biz;
        }

        /// <summary>
        /// 支付宝支付结果异步通知
        /// </summary>
        /// <returns>Form表单</returns>
        [HttpPost]
        public void AlipayNotify()
        {
            AliPayReturnModel payResult = new AliPayReturnModel();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context     
            HttpRequestBase request = context.Request;//定义传统request对象
            var result = this._aliPay.VerfyNotify(request,out payResult);
        }

        /// <summary>
        /// 生成支付宝请求参数
        /// </summary>
        /// <param name="payInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public BizResult<string> CreateWePayRequestParam(WePayReqParam payInfo)
        {
            BizResult<string> biz = new BizResult<string>();
            biz.ReturnObject = this._wePay.BuildWePay(payInfo.OrderNo,payInfo.ProductName,payInfo.TotalFee,payInfo.CustomerIp,payInfo.TradeType); ;
            return biz;
        }

        /// <summary>
        /// 微信支付支付结果异步通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public void WePayNotify()
        {
            WePayReturnModel payResult = new WePayReturnModel();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context     
            HttpRequestBase request = context.Request;//定义传统request对象
            var result = this._wePay.VerifyNotify(request, out payResult);
        }
    }
}
