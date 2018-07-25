using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Diego.WxHttpSdk.Extends;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Diego.WxHttpSdk.SdkEventArgs;
using System.Text.RegularExpressions;
using Diego.WxHttpSdk.Model;
namespace Diego.WxHttpSdk
{
    /// <summary>
    /// 微信消息监听
    /// 1:消息检查
    /// 2:获取最新的消息
    /// </summary>
    public class WeChatMessageMonitor
    {
        private CancellationToken m_token;
        private HttpWeChat m_httpWeChat;
        public WeChatMessageMonitor(HttpWeChat httpWeChat, CancellationToken token)
        {
            m_token = token;
            m_httpWeChat = httpWeChat;
        }


        /// <summary>
        /// 消息检查
        /// </summary>
        public void SyncCheck()
        {
            Task.Run(() =>
            {
                try
                {
                    WxStatusNotify();
                    while (true)
                    {
                        m_token.ThrowIfCancellationRequested();
                        string url = "https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=" + DateTime.Now.ToUnix() + "&skey=" + System.Web.HttpUtility.UrlEncode(m_httpWeChat.AuthorizationInfo.Skey) + "&sid=" + System.Web.HttpUtility.UrlEncode(m_httpWeChat.AuthorizationInfo.Sid) + "&uin=" + m_httpWeChat.AuthorizationInfo.Uin + "&deviceid=" + m_httpWeChat.DeviceID + "&synckey=" + System.Web.HttpUtility.UrlEncode(m_httpWeChat.SyncKey.List.Aggregate(string.Empty, (c, n) => c + "|" + n.Key + "_" + n.Val).TrimEnd('|').TrimStart('|')) + "&_=" + DateTime.Now.ToUnix();
                        var data = m_httpWeChat.HttpClient.GetStringAsync(url).Result;
                        SyncCheckEventArgs syncCheckEventArgs = new SyncCheckEventArgs();
                        if (Regex.IsMatch(data, "window.synccheck={retcode:\"([\\s\\S]+)\",selector:\"([\\s\\S]+)\"}"))
                        {
                            var matchREsult = Regex.Match(data, "window.synccheck={retcode:\"([\\s\\S]+)\",selector:\"([\\s\\S]+)\"}");
                            syncCheckEventArgs.Retcode = Convert.ToInt32(matchREsult.Groups[1].Value);
                            syncCheckEventArgs.Selector = Convert.ToInt32(matchREsult.Groups[2].Value);
                            m_httpWeChat.TriggerSyncCheckEvent(this, syncCheckEventArgs);
                        }
                        if (syncCheckEventArgs.Selector == 2)
                        {
                            WebwxSync();
                        }
                        Thread.Sleep(100);
                    }
                }
                catch (OperationCanceledException)
                {
                    
                }
            });
        }


        /// <summary>
        /// 获取最新消息
        /// </summary>
        private void WebwxSync() {

            var url = $"http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={this.m_httpWeChat.AuthorizationInfo.Sid}&skey={this.m_httpWeChat.AuthorizationInfo.Skey}&pass_ticket={this.m_httpWeChat.AuthorizationInfo.Pass_ticket}";
            var model = new
            {
                BaseRequest = new BaseRequest
                {
                    DeviceID = m_httpWeChat.DeviceID,
                    Sid = m_httpWeChat.AuthorizationInfo.Sid,
                    Skey = m_httpWeChat.AuthorizationInfo.Skey,
                    Uin = m_httpWeChat.AuthorizationInfo.Uin
                },
                m_httpWeChat.SyncKey,
                rr = -1064188881
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = m_httpWeChat.HttpClient.PostAsync(url, content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
            m_httpWeChat.SyncKey.GetSyncKey(postResult);
            var chatMessage = JsonConvert.DeserializeObject<WeChatMessage>(postResult);
            m_httpWeChat.TriggerWebwxsyncEvent(this, new WebwxsyncEventArgs(chatMessage));
        }


        /// <summary>
        /// 开始微信状态通知
        /// </summary>
        private void WxStatusNotify()
        {
            var model = new
            {
                BaseRequest = new Model.BaseRequest
                {
                    DeviceID = m_httpWeChat.DeviceID,
                    Sid = m_httpWeChat.AuthorizationInfo.Sid,
                    Skey = m_httpWeChat.AuthorizationInfo.Skey,
                    Uin = m_httpWeChat.AuthorizationInfo.Uin
                },
                Code = 3,
                FromUserName = m_httpWeChat.WeChatData.CurrentLoginMemberInfo.UserName,
                ToUserName = m_httpWeChat.WeChatData.CurrentLoginMemberInfo.UserName,
                ClientMsgId = DateTime.Now.ToUnix()
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = m_httpWeChat.HttpClient.PostAsync(string.Format(WeChatHttpProUrl.WxStatusNotifyUrl, m_httpWeChat.AuthorizationInfo.Pass_ticket), content, CancellationToken.None).Result.Content.ReadAsStringAsync();
        }
    }
}
