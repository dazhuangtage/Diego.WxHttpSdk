using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diego.WxHttpSdk.Extends;
using Diego.WxHttpSdk.Command;
using Diego.WxHttpSdk.Exceptions;
using Diego.WxHttpSdk.SdkEventArgs;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Diego.WxHttpSdk.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diego.WxHttpSdk
{
    public class HttpWeChat : IDisposable
    {
        #region Member
        #region Public Member
        public HttpClient HttpClient { private set; get; }
        public AuthorizationInfo AuthorizationInfo { get; }
        public WeChatData WeChatData { get; set; }
        public string DeviceID { get; set; }

        #endregion

        #region Private Member
        internal SyncKey SyncKey = new SyncKey();
        private CancellationTokenSource m_cancellationToken = new CancellationTokenSource();
        private WeChatMessageMonitor m_weChatMessageMonitor;
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// 二维码发生变化事件
        /// </summary>
        public event EventHandler<QrCodeChangEventArgs> QRCodeChangeEvent;

        /// <summary>
        /// 二维码状态发生改变事件
        /// </summary>
        public event EventHandler<QrCodeStatusChangEventArgs> QRCodeStatusChangeEvent;

        /// <summary>
        /// 初始化数据之前
        /// </summary>
        public event EventHandler<EventArgs> InitDatabeforeEvent;

        /// <summary>
        /// 初始化数据完成
        /// </summary>
        public event EventHandler<EventArgs> InitDatacompleteEvent;

        /// <summary>
        /// 消息检查事件
        /// </summary>
        public event EventHandler<SyncCheckEventArgs> SyncCheckEvent;

        /// <summary>
        /// 收到新消息事件
        /// </summary>
        public event EventHandler<WebwxsyncEventArgs> WebwxsyncEvent;
        #endregion

        public HttpWeChat()
        {
            HttpClient = new HttpClient();
            AuthorizationInfo = new AuthorizationInfo(this);
        }


        public void AddMemberToGroup(UpdateMemberToGroup update)
        {
            var model = new
            {
                BaseRequest = new BaseRequest
                {
                    Uin = AuthorizationInfo.Uin,
                    Sid = AuthorizationInfo.Sid,
                    Skey = AuthorizationInfo.Skey,
                    DeviceID = DeviceID
                },
                update.ChatRoomName,
                AddMemberList=update.MemberList
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = HttpClient.PostAsync(WeChatHttpProUrl.AddToGroupApiUrl, content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
        }

        public void Dispose()
        {
            this.m_cancellationToken.Cancel();
            this.InitDatabeforeEvent = null;
            this.InitDatacompleteEvent = null;
            this.QRCodeChangeEvent = null;
            this.QRCodeStatusChangeEvent = null;
            this.SyncCheckEvent = null;
            this.WebwxsyncEvent = null;
        }

        public void Login()
        {
            var getUuidStr = HttpClient.GetStringAsync(string.Format(WeChatHttpProUrl.GetUUIdUrl, DateTime.Now.ToUnixJs())).Result;
            if (HttpWeChatHelp.GetUuid(getUuidStr, out (string code, string uuid) uuid) && uuid.code.Equals("200"))
            {
                var code = string.Empty;  //微信返回的二维码状态
                try
                {
                    var getQrcodeBytes = HttpClient.GetByteArrayAsync(string.Format(WeChatHttpProUrl.GetQrCodeUrl, uuid.uuid)).Result;
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    QRCodeChangeEvent?.Invoke(this, new QrCodeChangEventArgs(uuid.uuid, getQrcodeBytes));
                    while (true)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var statusStr = HttpClient.GetStringAsync(string.Format(WeChatHttpProUrl.GetQrStatusUrl, uuid.uuid)).Result;
                        if (Regex.IsMatch(statusStr, "window.code=([\\d]+)"))
                        {
                            code = Regex.Match(statusStr, "window.code=([\\d]+)").Groups[1].Value;
                            if (Enum.TryParse(code, out EQrCodeStatus status))
                            {
                                QRCodeStatusChangeEvent?.Invoke(this, new QrCodeStatusChangEventArgs(uuid.uuid, status));
                                if (code == "200")
                                {
                                    string redirect_uri = RegexHelper.GetValue("(?<=window.redirect_uri=\").*?(?=\")", statusStr);
                                    if (UrlHelp.ParseUrl(redirect_uri, out string baseUrl, out NameValueCollection nvc))
                                    {
                                        AuthorizationInfo.Init(nvc["ticket"], nvc["uuid"], nvc["lang"], nvc["scan"]);
                                        WeChatData = new WeChatData(this);
                                        InitDatabeforeEvent?.Invoke(this, new EventArgs());
                                        WeChatData.InitData();
                                        InitDatacompleteEvent?.Invoke(this, new EventArgs());
                                        //初始化消息监听
                                        m_weChatMessageMonitor = new WeChatMessageMonitor(this, this.m_cancellationToken.Token);
                                        m_weChatMessageMonitor.SyncCheck();
                                    }
                                    else
                                    {
                                        throw new AnalyzeLoginParameterException(statusStr, null);
                                    }
                                }
                                if (code == "408" || code == "200") break;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    if (code != "200")
                        this.QRCodeStatusChangeEvent?.Invoke(this, new QrCodeStatusChangEventArgs(uuid.uuid, EQrCodeStatus.TimeOut));
                }
            }
            else
            {
                throw new UUIDStrInvalidException(getUuidStr, null);
            }
        }

        public void RemoveMemberToGroup(UpdateMemberToGroup update)
        {
            var model = new
            {
                BaseRequest = new BaseRequest
                {
                    Uin = AuthorizationInfo.Uin,
                    Sid = AuthorizationInfo.Sid,
                    Skey = AuthorizationInfo.Skey,
                    DeviceID = DeviceID
                },
                update.ChatRoomName,
                DelMemberList = update.MemberList
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = HttpClient.PostAsync(WeChatHttpProUrl.RemoveGroupApiUrl, content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
        }

        public SendMessageResult SendMessage(SendMessage message)
        {
            var model = new
            {
                BaseRequest = new BaseRequest
                {
                    Uin = AuthorizationInfo.Uin,
                    Sid = AuthorizationInfo.Sid,
                    Skey = AuthorizationInfo.Skey,
                    DeviceID = DeviceID
                },
                Msg =new {
                    Type = 1,
                    message.Content,
                    message.FromUserName,
                    message.ToUserName,
                    message.LocalID,
                    message.ClientMsgId
                },
                Scene = 0
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = HttpClient.PostAsync(GetSendMessageApiUrl(), content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
            var jobj = (JObject)JsonConvert.DeserializeObject(postResult);
            SendMessageResult sendMessageResult = new SendMessageResult();
            sendMessageResult.Status = jobj["BaseResponse"]["Ret"].ToString() == "0";
            sendMessageResult.ErrorMsg= jobj["BaseResponse"]["ErrMsg"].ToString();
            sendMessageResult.MsgId =Convert.ToInt64(jobj["MsgID"].ToString());
            return sendMessageResult;
        }

        private string GetSendMessageApiUrl()
        {
            return string.Format(WeChatHttpProUrl.SendMessageUrl, System.Web.HttpUtility.UrlEncode(this.AuthorizationInfo.Pass_ticket));

        }

        public void SignOut()
        {
            var signOutUrl=string.Format(WeChatHttpProUrl.LoginOutApiUrl, System.Web.HttpUtility.UrlEncode(this.AuthorizationInfo.Skey));
            var model = new
            {
                sid=AuthorizationInfo.Sid,
                uin=AuthorizationInfo.Uin
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = HttpClient.PostAsync(signOutUrl, content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
            this.m_cancellationToken.Cancel();
        }

        public void UpdateRemarke(UpdateRemarke updateRemarke)
        {
            var updateRemarkeUrl = string.Format(WeChatHttpProUrl.UpdateRemarkNameApiUrl, System.Web.HttpUtility.UrlEncode(this.AuthorizationInfo.Pass_ticket));
            var model = new
            {
                BaseRequest = new BaseRequest
                {
                    Uin = AuthorizationInfo.Uin,
                    Sid = AuthorizationInfo.Sid,
                    Skey = AuthorizationInfo.Skey,
                    DeviceID = DeviceID
                },
                CmdId=2,
                updateRemarke.RemarkName,
                updateRemarke.UserName
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postResult = HttpClient.PostAsync(updateRemarkeUrl, content, CancellationToken.None).Result.Content.ReadAsStringAsync().Result;
        }


        /// <summary>
        /// 触发消息检查更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="syncCheckEventArgs"></param>
        internal void TriggerSyncCheckEvent(object sender, SyncCheckEventArgs syncCheckEventArgs)
        {
            this.SyncCheckEvent?.Invoke(sender, syncCheckEventArgs);
        }


        /// <summary>
        /// 触发收到新消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="syncCheckEventArgs"></param>
        internal void TriggerWebwxsyncEvent(object sender, WebwxsyncEventArgs webwxsyncEventArgs)
        {
            this.WebwxsyncEvent?.Invoke(sender, webwxsyncEventArgs);
        }
    }
}
