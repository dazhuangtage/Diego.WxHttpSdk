using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    public class Message
    {
        public string MsgId { get; set; }
        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public int MsgType { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }

        public int ImgStatus { get; set; }

        public int CreateTime { get; set; }

        public int VoiceLength { get; set; }

        public int PlayLength { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public string MediaId { get; set; }

        public string Url { get; set; }

        public int AppMsgType { get; set; }

        public int StatusNotifyCode { get; set; }

        public string StatusNotifyUserName { get; set; }

        public RecommendInfo RecommendInfo { get; set; }

        public int ForwardFlag { get; set; }

        public int HasProductId { get; set; }

        public string Ticket { get; set; }

        public int ImgHeight { get; set; }

        public int ImgWidth { get; set; }

        public int SubMsgType { get; set; }

        public string NewMsgId { get; set; }

        public string OriContent { get; set; }
    }


    public class RecommendInfo
    {
        //新好友的Username
        public string UserName { get; set; }
        //好友名
        public string NickName { get; set; }
        //加好友的时候说的话
        public string Content { get; set; }
        public string Ticket { get; set; }
        public string Signature { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sex { get; set; }
        public string VerifyFlag { get; set; }
        public string AttrStatus { get; set; }
        public string Alias { get; set; }
        public string RemarkName { get; set; }
        public string HeadImgUrl { get; set; }
        public string HeadImg { get; set; }
    }
}
