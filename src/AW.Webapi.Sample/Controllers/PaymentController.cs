using AW.Pay.Core.Common;
using AW.Pay.Core.Enum;
using AW.Webapi.Sample.Models.PayModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AW.Webapi.Sample.Controllers
{
    public class PaymentController : Controller
    {
        public static HttpClient _client = new HttpClient();
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AliPayDemo()
        {
            return View();
        }

        public async Task<string> AliPayAsync(AliPayReqParam payInfo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8115/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // HTTP POST
                var gizmo = new AliPayReqParam { OrderNo = payInfo.OrderNo, Subject = payInfo.Subject, TotalAmount = payInfo.TotalAmount, Type = 0 };
                var response = await client.PostAsJsonAsync("api/Payment/CreateAliPayRequestParam", gizmo);
                var resultValue =await response.Content.ReadAsAsync<BizResult<string>>();
                if (resultValue.Code == EnumBizCode.Failed)
                {
                    return string.Empty;
                }
                return resultValue.ReturnObject;
            }
        }

        public ActionResult WePayDemo()
        {
            return View();
        }

        public async Task<ActionResult> WePayAsync(WePayReqParam payInfo)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8115/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // HTTP POST
                var gizmo = new WePayReqParam { OrderNo = payInfo.OrderNo, ProductName = payInfo.ProductName, CustomerIp="127.0.0.1", TotalFee = payInfo.TotalFee, TradeType = EnumWePayTradeType.NATIVE};
                var response = await client.PostAsJsonAsync("api/Payment/CreateWePayRequestParam", gizmo);
                var resultValue = await response.Content.ReadAsAsync<BizResult<string>>();
                if (resultValue.Code == EnumBizCode.Failed)
                {
                    throw new Exception("微信支付失败");
                }
                ViewBag.OrderId = payInfo.OrderNo;
                ViewBag.PayAmount = payInfo.TotalFee;
                ViewBag.PayUrl = resultValue.ReturnObject;
                return View("WePay");
            }
        }
    }
}