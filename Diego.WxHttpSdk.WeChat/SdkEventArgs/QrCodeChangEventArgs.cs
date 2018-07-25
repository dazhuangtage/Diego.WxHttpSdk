using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.SdkEventArgs
{

    /// <summary>
    /// 二维码发生变化事件数据类型
    /// </summary>
    public class QrCodeChangEventArgs : EventArgs
    {
        /// <summary>
        /// 二维码数据流
        /// </summary>
        public byte[] QrCodeBytes { get; set; }

        /// <summary>
        /// 二维码唯一标识
        /// </summary>
        public string Uuid { get; set; }

        public QrCodeChangEventArgs(string uuid,byte[] bytes)
        {
            this.QrCodeBytes = bytes;
            this.Uuid = uuid;
        }
    }
}
