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

        public static implicit operator DateTime(Time time)
        {
            return time.ZoneTime;
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
    }
}
