using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core.Model
{
    public class WepayReturnModel
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 交易订单号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 交易金额（单位元）
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public string TradeStatus { get; set; }

        /// <summary>
        /// 返回xml
        /// </summary>
        public string ReturnXml { get; set; }
    }
}
