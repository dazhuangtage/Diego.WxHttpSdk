using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Diego.WxHttpSdk.Extends;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using Diego.WxHttpSdk.Model;

namespace Diego.WxHttpSdk
{
    /// <summary>
    /// 微信数据,一个HttpWeChat对应一个
    /// 维护对应微信号的内存数据
    /// 包括:好友,群组等
    /// </summary>
    public class WeChatData
    {
        private HttpWeChat m_HttpWeChat;
        private BaseRequest baseRequest;
        private Random R;
        public WeChatData(HttpWeChat httpWeChat)
        {
            this.m_HttpWeChat = httpWeChat;
            R = new Random();
            httpWeChat.DeviceID = "e" + R.Next(10000000, 99999999).ToString() + R.Next(1000000, 9999999).ToString();
            baseRequest = new BaseRequest
            {
                DeviceID = httpWeChat.DeviceID,
                Sid = httpWeChat.AuthorizationInfo.Sid,
                Skey = httpWeChat.AuthorizationInfo.Skey,
                Uin = httpWeChat.AuthorizationInfo.Uin
            };
        }

        /// <summary>
        /// 当前登陆成员信息
        /// </summary>
        public WeChatMember CurrentLoginMemberInfo { get; set; }

        /// <summary>
        /// 当前用户的所有好友(包括群组公众号)
        /// </summary>
        public List<WeChatMember> AllMember { get; set; }

        /// <summary>
        ///  获取当前微信用户的所有好友
        /// </summary>
        public List<WeChatMember> Friends { get; set; }

        /// <summary>
        /// 获取当前微信用户的所有群组信息
        /// </summary>
        public List<WeChatMember> Group { get; set; }

        /// <summary>
        /// 获取当前微信用户的所有公众号信息
        /// </summary>
        public List<WeChatMember> PublicAccount { get; set; }

        /// <summary>
        /// 最近联系好友
        /// </summary>
        public List<WeChatMember> RecentContact { get; set; }

      

        /// <summary>
        /// 初始化内存数据
        /// </summary>
        public void InitData()
        {
            void initData()
            {
                //初始化自己的信息
                var initDataResultUrl = string.Format(WeChatHttpProUrl.InitDataPostUrl, DateTime.Now.ToUnix(), m_HttpWeChat.AuthorizationInfo.Pass_ticket);
                StringContent content = new StringContent(JsonConvert.SerializeObject(new { BaseRequest = this.baseRequest }), Encoding.UTF8, "application/json");
                var initDataResponseMessage = m_HttpWeChat.HttpClient.PostAsync(initDataResultUrl, content).Result;
                var initDataStr = initDataResponseMessage.Content.ReadAsStringAsync().Result;
                var initDataJob = JsonConvert.DeserializeObject<JObject>(initDataStr);
                //赋值SyncKey
                m_HttpWeChat.SyncKey.GetSyncKey(initDataStr);//.Keys = JsonConvert.DeserializeObject<List<SyncKeyItem>>(initDataJob["SyncKey"]["List"].ToString());
                //赋值最近联系人
                RecentContact = JsonConvert.DeserializeObject<List<WeChatMember>>(initDataJob["ContactList"].ToString());
                //赋值当前用户信息
                CurrentLoginMemberInfo = JsonConvert.DeserializeObject<WeChatMember>(initDataJob["User"].ToString());
            }

            void initAllMember()
            {
                //抓取用户所有的好友列表
                var initMemberUrl = string.Format(WeChatHttpProUrl.InitMemberGetUrl, DateTime.Now.ToUnix());
                var allMemberDataStr = m_HttpWeChat.HttpClient.GetStringAsync(initMemberUrl).Result;
                var allJsonData = JsonConvert.DeserializeObject<JObject>(allMemberDataStr);
                //赋值所有好友数据
                AllMember = JsonConvert.DeserializeObject<List<WeChatMember>>(allJsonData["MemberList"].ToString())??new List<WeChatMember>();
                //赋值好友数据,不包括群组 公众号之类的
                Friends = AllMember.Where(c =>c.UserName.StartsWith("@")&&c.VerifyFlag==0).ToList();
                Group = AllMember.Where(c =>  c.UserName.StartsWith("@@")).ToList();
                PublicAccount = AllMember.Where(c => c.UserName.StartsWith("@") && c.VerifyFlag != 0).ToList();
            }
            Task.WaitAll(Task.Run(action: initData), Task.Run(action: initAllMember));
        }
    }
}
