using System;

//[20180221]Create by Simon
namespace MP_Moudule
{
    public static class UNIXTime
    {

        public static DateTime ToDatetime(long unixTime)
        { return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime; }

        public static DateTime ToDatetime(long unixTime, int gmt)
        { return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime.AddHours(gmt); }

        public static long DatetimeToUnix(DateTime datetime)
        { return ((DateTimeOffset)datetime).ToUnixTimeSeconds(); }


        public static long DatetimeToUnix(DateTime datetime, int gmt)
        { return ((DateTimeOffset)datetime.AddHours(-gmt)).ToUnixTimeSeconds(); }

        public static long GetUnixNow()
        { return DatetimeToUnix(DateTime.Now); }
    }
}
