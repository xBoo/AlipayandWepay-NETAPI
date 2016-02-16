using AW.Pay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AW.Webapi.Sample.Models.PayModel
{
    public class AliPayReqParam
    {
        public string OrderNo { get; set; }
        public string Subject { get; set; }
        public decimal TotalAmount { get; set; }
        public EnumAliPayTradeType Type { get; set; }
    }
}