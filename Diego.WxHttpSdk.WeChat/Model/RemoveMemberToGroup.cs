using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    /// <summary>
    /// 从讨论组中移除好友
    /// </summary>
    public class UpdateMemberToGroup
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public string ChatRoomName { get; set; }

        /// <summary>
        /// 好友UserName
        /// </summary>
        public string[] MemberList { get; set; }
    }
}
