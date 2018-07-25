using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Model
{
    public class BaseRequest
    {
        public long Uin { get; set; }

        public string Sid { get; set; }

        public string DeviceID { get; set; }

        public string Skey { get; set; }
    }
}
