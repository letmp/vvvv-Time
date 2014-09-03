using System;
using System.Runtime.Serialization;

namespace VVVV.Packs.Time
{
    [DataContract]
    public struct Time
    {
        [DataMember]
        private readonly DateTime utcDateTime;
        
        [DataMember]
        private readonly TimeZoneInfo timeZone;

        public Time(DateTime dateTime, TimeZoneInfo timeZone)
        {
            utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
            this.timeZone = timeZone;
        }

        public static Time JoinTime(int year, int month, int day, int hour, int minute, int second, int milli, string timezone)
        {
            var dt = new DateTime(year, month, day, hour, minute, second, milli);
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return new Time(dt, tz);
        }

        public static Time CurrentTime(){
            return new Time(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), TimeZoneInfo.Local);
        }

        public DateTime UniversalTime
        {
            get { return utcDateTime; }
        }

        public TimeZoneInfo TimeZone
        {
            get { return timeZone; }
        }

        public DateTime ZoneTime
        {
            get { return TimeZoneInfo.ConvertTime(utcDateTime, timeZone); }
        }

        public DateTime LocalTime
        {
            get { return TimeZoneInfo.ConvertTime(utcDateTime, TimeZoneInfo.Local); }
        }

        public DateTime OtherZoneTime(TimeZoneInfo otherZone)
        {
            return TimeZoneInfo.ConvertTime(utcDateTime, otherZone);
        }

        public static Time MinUTCTime()
        {
            return new Time(DateTime.MinValue, TimeZoneInfo.Utc);
        }

        public static double TimeStamp(Time time)
        {
            return time.ZoneTime.Subtract(DateTime.MinValue).TotalDays;
        }

        public static Time ChangeTimezone(Time time, string timezone)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return new Time(DateTime.SpecifyKind(time.OtherZoneTime(tz), DateTimeKind.Unspecified), tz);
        }

        public static Time StringAsTime(string timezonestring, string timestring, string timeformat){

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezonestring);
            DateTime dt = DateTime.ParseExact(timestring, timeformat, null);
            return new Time(dt, tz);
        }

        public static Time UnixStringAsTime(string timezonestring, string unixstring)
        {

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezonestring);
            DateTime dt = TimeFromUnixTimestamp(Convert.ToDouble(unixstring));
            return new Time(dt, tz);
        }
        
        private static DateTime TimeFromUnixTimestamp(double unixTimestamp)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1);
            double unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
            DateTime dtUnix = new DateTime(unixYear0.Ticks + (long)unixTimeStampInTicks);
            return dtUnix;
        }

        public static Time ValueAsTime(string timezone, double value)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            DateTime dt = DateTime.FromOADate(value);
            return new Time(dt, tz);
        }

        public static Time UnixValueAsTime(string timezone, int value)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            DateTime dt = TimeFromUnixTimestamp(value);
            return new Time(dt, tz);
        }

        private static DateTime TimeFromUnixTimestamp(int unixTimestamp)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1);
            long unixTimeStampInTicks = unixTimestamp * TimeSpan.TicksPerSecond;
            DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
            return dtUnix;
        }

        public static Time DecimalValueAsTime( string timezone, double value)
        {
            var dtwz = Time.CurrentTime();

            int year = dtwz.ZoneTime.Year;
            int month = dtwz.ZoneTime.Month;
            int day = dtwz.ZoneTime.Day;

            var dt = TimeFromDecimal(year, month, day, value);
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return new Time(dt, tz);
        }

        public static Time DecimalValueAsTimeAdvanced(string timezone,int year, int month, int day, double value)
        {
            var dtwz = Time.CurrentTime();
            var dt = TimeFromDecimal(year, month, day, value);
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return new Time(dt, tz);
        }

        private static DateTime TimeFromDecimal(int year, int month, int day, double dec)
        {
            double hour = Math.Floor(dec);
            double min = (dec - hour) * 60;
            double minute = Math.Floor(min);
            double sec = (min - minute) * 60;
            double second = Math.Floor(sec);
            double millis = Math.Floor((sec - second) * 1000); // 7 digits

            var dt = new DateTime(year, month, day, (int)hour, (int)minute, (int)second, (int)millis);

            return dt;
        }

        public static double UnixTimestampFromTime(Time time)
        {
           return UnixTimestampFromDateTime(time.LocalTime);
        }

        private static double UnixTimestampFromDateTime(DateTime date)
        {

            TimeSpan span = date - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return span.TotalSeconds;
        }

        public static bool operator >(Time t1, Time t2)
        {
            return (t1.UniversalTime.CompareTo(t2.UniversalTime) > 0);
        }

        public static bool operator <(Time t1, Time t2)
        {
            return (t1.UniversalTime.CompareTo(t2.UniversalTime) < 0);
        }
        
        public static TimeSpan operator -(Time t1, Time t2)
        {
            return (t1.UniversalTime - t2.UniversalTime);
        }

        public static Time operator +(Time t1, TimeSpan ts2)
        {
            return (new Time(t1.ZoneTime.Add(ts2), t1.TimeZone));
        }

        public static Time operator -(Time t1, TimeSpan ts2)
        {
            return (new Time(t1.ZoneTime.Subtract(ts2), t1.TimeZone));
        }

        public static implicit operator DateTime(Time time)
        {
            return time.UniversalTime;
        }

        /*public static implicit operator DateTime(Time time)
        {
            return time.ZoneTime;
        }*/

        public static implicit operator Time(DateTime time)
        {
            return new Time(DateTime.SpecifyKind(time, DateTimeKind.Unspecified), TimeZoneInfo.Local);
        }

        public override string ToString()
        {
            return this.LocalTime.ToString() + " " + this.TimeZone.DisplayName;
        }

    }
}
