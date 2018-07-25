using System;
using System.Collections.Generic;
using System.Text;

namespace Diego.WxHttpSdk.Extends
{
    public static class DatetimeExtend
    {
        /// <summary>
        /// C#时间转时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToUnix(this DateTime time)
        {

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
        }


        /// <summary>
        /// 时间戳转C#时间
        /// </summary>
        /// <param name="unix"></param>
        /// <returns></returns>
        public static DateTime ToDatetime(this long unix)
        {
            long unixTimeStamp = 1478162177;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }

        /// <summary>
        /// C#时间转Js时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToUnixJs(this DateTime time)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
        }


        /// <summary>
        /// 时间戳转C#时间
        /// </summary>
        /// <param name="unix"></param>
        /// <returns></returns>
        public static DateTime ToDatetimeByJs(this long unix)
        {
            long jsTimeStamp = 1478169023479;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
            return dt;
        }
    }
}
