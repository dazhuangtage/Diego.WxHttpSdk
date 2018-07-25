using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace Diego.WxHttpSdk.Command
{
    /// <summary>
    /// 微信Http协议帮助类
    /// </summary>
    public static class HttpWeChatHelp
    {
        const string UuidExStr = "window.QRLogin.code = ([\\d]+?); window.QRLogin.uuid = \"([\\s\\S]+?)\";";

        /// <summary>
        /// 提取字符串中的状态码以及UUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool GetUuid(string str, out (string code, string uuid) resut)
        {
            resut = default((string code, string uuid));
            if (Regex.IsMatch(str, UuidExStr)) {
                var matchResult = Regex.Match(str,UuidExStr);
                resut =(matchResult.Groups[1].Value,matchResult.Groups[2].Value);
                return true;
            }
            return false;
        }
    }
}
