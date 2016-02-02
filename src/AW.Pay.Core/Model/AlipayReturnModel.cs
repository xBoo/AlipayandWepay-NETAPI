using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core.Model
{
    public class AlipayReturnModel
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
        /// 交易总金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public string TradeStatus { get; set; }
    }
}
