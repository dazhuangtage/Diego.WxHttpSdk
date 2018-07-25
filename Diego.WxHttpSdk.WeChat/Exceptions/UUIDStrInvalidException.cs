using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Exceptions
{
    public class UUIDStrInvalidException : Exception
    {
        public UUIDStrInvalidException(string message, Exception innerException) : base("获取微信UUID字符串失败:"+message, innerException)
        {
        }
    }
}
