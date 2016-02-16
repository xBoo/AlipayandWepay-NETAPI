using AW.Pay.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AW.Webapi.Sample.Models.PayModel
{
    public class WePayReqParam
    {
        public string OrderNo { get; set; }
        public string ProductName { get; set; }
        public int TotalFee { get; set; }
        public string CustomerIp { get; set; }
        public EnumWePayTradeType TradeType { get; set; }
    }
}