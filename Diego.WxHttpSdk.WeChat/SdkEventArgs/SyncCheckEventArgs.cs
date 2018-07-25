using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.SdkEventArgs
{
    public  class SyncCheckEventArgs : EventArgs
    {
        public int Retcode { get; set; }

        public int Selector { get; set; }
    }
}
