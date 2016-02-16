using AW.Pay.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core.Common
{
    /// <summary>
    /// HTTP请求返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BizResult<T>
    {
        private EnumBizCode _code;
        private string _message = "";
        private T _returnObject;

        /// <summary>
        /// 编码
        /// </summary>
        public EnumBizCode Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        public T ReturnObject
        {
            get
            {
                return _returnObject;
            }
            set
            {
                _returnObject = value;
            }
        }

        public BizResult(EnumBizCode resultCode = EnumBizCode.Success, string message = "操作成功")
        {
            this._code = resultCode;
            this._message = message;
        }
    }
}
