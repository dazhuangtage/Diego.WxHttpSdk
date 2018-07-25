using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    /// <summary>
    /// 发送消息结果
    /// </summary>
    public class SendMessageResult
    {
        /// <summary>
        /// 表示发送成功还是失败
        /// </summary>
        public  bool Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 消息Id
        /// </summary>
        public long MsgId { get; set; }
    }
}
