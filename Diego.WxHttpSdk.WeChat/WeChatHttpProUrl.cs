using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk
{
    /// <summary>
    /// 微信Http协议Url
    /// </summary>
    internal class WeChatHttpProUrl
    {
        /// <summary>
        /// 获取登陆参数的Post地址
        /// </summary>
        public static string GetUUIdUrl => "https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_={0}";
        /// <summary>
        /// 获取二维码地址
        /// </summary>
        public static string GetQrCodeUrl => "https://login.weixin.qq.com/qrcode/{0}";

        /// <summary>
        /// 获取二维码状态Url
        /// </summary>
        public static string GetQrStatusUrl => "https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r=-965104146&_=1512793338177";

        /// <summary>
        /// 获取登录参数Url
        /// </summary>
        public static string GetAuthorizationUrl => "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxnewloginpage?";


        /// <summary>
        /// 初始化个人信息的API地址
        /// </summary>

        public const string InitDataPostUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={0}&pass_ticket={1}";
        /// <summary>
        /// 获取所有好友列表API地址
        /// </summary>
        public const string InitMemberGetUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?r={0}";


        /// <summary>
        /// 开启微信状态通知API接口
        /// </summary>
        public const string WxStatusNotifyUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxstatusnotify?pass_ticket={0}";


        /// <summary>
        /// 发送消息API接口
        /// </summary>
        public const string SendMessageUrl = "http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={0}";


        /// <summary>
        /// 退出登陆微信API接口
        /// </summary>
        public const string LoginOutApiUrl = "http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxlogout?redirect=0&type=0&skey={0}";

        /// <summary>
        /// 修改备注API地址
        /// </summary>
        public const string UpdateRemarkNameApiUrl = "http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxoplog?lang=zh_CN&pass_ticket={0}";

        /// <summary>
        /// 添加好友到群组
        /// </summary>

        public const string AddToGroupApiUrl = "http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=addmember";


        /// <summary>
        /// 从群组中移除好友
        /// </summary>
        public const string RemoveGroupApiUrl = "http://wx.qq.com/cgi-bin/mmwebwx-bin/webwxupdatechatroom?fun=delmember";
    }
}
