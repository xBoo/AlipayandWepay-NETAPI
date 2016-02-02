using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core.Model
{
    public class WepayReturnModel
    {
        public string OutTradeNo { get; set; }

        public string TradeNo { get; set; }

        public decimal TotalFee { get; set; }

        public string TradeStatus { get; set; }

        public string ReturnXml { get; set; }
    }
}
