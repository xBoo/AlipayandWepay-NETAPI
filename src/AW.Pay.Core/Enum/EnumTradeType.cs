using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core.Enum
{
    public enum EnumTradeType
    {
        /// <summary>
        /// 公众号支付
        /// </summary>
        JSAPI = 0,
        /// <summary>
        /// 原生扫码支付
        /// </summary>
        NATIVE,
        /// <summary>
        /// app支付
        /// </summary>
        APP,
        /// <summary>
        /// wap支付（浏览器调用微信app，目前腾讯还未对外开放）
        /// </summary>
        WAP
    }
}
