using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    public class SendMessage
    {
        /// <summary>
        /// 要发送的消息（发送图片消息时该字段为MediaId）
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 自己的ID
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 好友的ID
        /// </summary>
        public string ToUserName { get; set; }
        /// <summary>
        /// 时间戳左移4位随后补上4位随机数
        /// </summary>
        public string  ClientMsgId{get;set;}
        /// <summary>
        /// 与clientMsgId相同
        /// </summary>
        public string LocalID { get { return ClientMsgId; }}
    }
}
