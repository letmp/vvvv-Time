using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VVVV.Time
{
    public struct DateTimeWithZone
    {
        private readonly DateTime utcDateTime;
        private readonly TimeZoneInfo timeZone;

        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
            this.timeZone = timeZone;
        }

        public DateTime UniversalTime { get { return utcDateTime; } }

        public TimeZoneInfo TimeZone { get { return timeZone; } }

        public DateTime TimeInOriginalZone { get { return TimeZoneInfo.ConvertTime(utcDateTime, timeZone); } }
        public DateTime TimeInLocalZone { get { return TimeZoneInfo.ConvertTime(utcDateTime, TimeZoneInfo.Local); } }
        public DateTime TimeInSpecificZone(TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTime(utcDateTime, tz);
        }

    }
}
