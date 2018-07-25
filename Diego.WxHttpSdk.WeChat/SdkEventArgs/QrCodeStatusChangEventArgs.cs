using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.SdkEventArgs
{
    public class QrCodeStatusChangEventArgs : EventArgs
    {
        public string Uuid { get; set; }
        public EQrCodeStatus EQrCodeStatus { get; set; }
        public QrCodeStatusChangEventArgs(string uuid, EQrCodeStatus eQrCodeStatus)
        {
            this.Uuid = uuid;
            this.EQrCodeStatus = eQrCodeStatus;
        }
    }

    /// <summary>
    /// 二维码状态
    /// </summary>
    public enum EQrCodeStatus
    {
        /// <summary>
        /// 登陆超时
        /// </summary>
        TimeOut = 408,
        /// <summary>
        /// 扫描成功
        /// </summary>
        ScanSucess = 201,
        /// <summary>
        /// 登陆成功
        /// </summary>
        LoginSucess = 200,
    }
}
