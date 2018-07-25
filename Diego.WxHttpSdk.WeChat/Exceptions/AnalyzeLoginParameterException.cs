using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Exceptions
{
    /// <summary>
    /// 解析登陆参数失败
    /// </summary>
    public class AnalyzeLoginParameterException : Exception
    {
        public AnalyzeLoginParameterException(string message, Exception innerException) : base("解析登陆参数失败,当前返回值是:"+ message, innerException)
        {
        }
    }
}
