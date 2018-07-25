using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.SdkEventArgs
{
    public class WebwxsyncEventArgs : EventArgs
    {
        public Model.WeChatMessage WeChatMessage { get; set; }
        public WebwxsyncEventArgs(Model.WeChatMessage weChatMessage)
        {
            this.WeChatMessage = weChatMessage;
        }
    }
}
