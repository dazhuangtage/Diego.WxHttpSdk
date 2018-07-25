using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    /// <summary>
    /// 微信消息
    /// </summary>
    public class WeChatMessage
    {
        /// <summary>
        /// 新增消息数量
        /// </summary>
        public int AddMsgCount { get; set; }
        /// <summary>
        /// 新增好友数量
        /// </summary>
        public int ModContactCount { get; set; }
        /// <summary>
        /// 删除好友数量
        /// </summary>
        public int DelContactCoun { get; set; }

        //微信消息列表（这玩意会返回此次登陆后微信的历史聊天记录有多条需要过滤）
        public List<Message> AddMsgList { get; set; }
        //新增好友
        public List<WeChatMember> ModContactList { get; set; }
        //删除好友
        public List<WeChatMember> DelContactList { get; set; }
    }
}
