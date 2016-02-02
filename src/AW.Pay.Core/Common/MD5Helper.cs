using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AW.Pay.Core
{
    public sealed class MD5Helper
    {
        public static string Sign(string prestr, string key, string _input_charset)
        {
            StringBuilder sb = new StringBuilder(32);
            prestr = prestr + key;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }
    }
}
