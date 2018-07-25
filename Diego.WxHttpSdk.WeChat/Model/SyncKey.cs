using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk
{
    public class SyncKey
    {
        public SyncKey()
        {
            this.List = new List<SyncKeyItem>();
        }

        public int Count { get { return List.Count; } }

        public List<SyncKeyItem> List { get; set; }


        public void GetSyncKey(string str)
        {
            
                var initDataJob = JsonConvert.DeserializeObject<JObject>(str);
                //赋值SyncKey
                List = JsonConvert.DeserializeObject<List<SyncKeyItem>>(initDataJob["SyncKey"]["List"].ToString());
        }

    }


    public struct SyncKeyItem
    {
        public string Key { get; set; }

        public string Val { get; set; }
    }
}
