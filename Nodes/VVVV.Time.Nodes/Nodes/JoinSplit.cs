using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Time.Core;
using VVVV.Core.Logging;
using VVVV.Packs.Time;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.Time.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "Gregorian", Category = "Time", Version = "Split", Help = "Gregorian style time splitting.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SplitTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Output("TimeStamp")]
        public ISpread<double> FTimeStamp;

        [Output("Millisecond")]
        public ISpread<int> FMilli;

        [Output("Second")]
        public ISpread<int> FSecond;

        [Output("Minute")]
        public ISpread<int> FMinute;

        [Output("Hour")]
        public ISpread<int> FHour;

        [Output("DayOfWeek")]
        public ISpread<int> FDayOfWeek;

        [Output("Day")]
        public ISpread<int> FDay;

        [Output("Month")]
        public ISpread<int> FMonth;

        [Output("Year")]
        public ISpread<int> FYear;

        [Output("IsDaylightSavingTime")]
        public ISpread<bool> FIsDaylightSavingTime;

        [Output("Timezone")]
        public ISpread<string> FTimezone;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FTimeStamp.SliceCount = SpreadMax;
            FMilli.SliceCount = FSecond.SliceCount = FMinute.SliceCount = FHour.SliceCount = SpreadMax;
            FDayOfWeek.SliceCount = FDay.SliceCount = FMonth.SliceCount = FYear.SliceCount = SpreadMax;
            FIsDaylightSavingTime.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    FTimeStamp[i] = FInput[i].TimeInOriginalZone.Subtract(DateTime.MinValue).TotalDays;
                    FMilli[i] = FInput[i].TimeInOriginalZone.Millisecond;
                    FSecond[i] = FInput[i].TimeInOriginalZone.Second;
                    FMinute[i] = FInput[i].TimeInOriginalZone.Minute;
                    FHour[i] = FInput[i].TimeInOriginalZone.Hour;
                    FDayOfWeek[i] = (int)FInput[i].TimeInOriginalZone.DayOfWeek;
                    FDay[i] = FInput[i].TimeInOriginalZone.Day;
                    FMonth[i] = FInput[i].TimeInOriginalZone.Month;
                    FYear[i] = FInput[i].TimeInOriginalZone.Year;
                    FIsDaylightSavingTime[i] = FInput[i].TimeInOriginalZone.IsDaylightSavingTime();
                    FTimezone[i] = FInput[i].TimeZone.Id;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FTimeStamp[i] =
                        FMilli[i] =
                        FSecond[i] =
                        FMinute[i] =
                        FHour[i] = FDayOfWeek[i] = FDay[i] = FMonth[i] = FYear[i] = 0;
                    FIsDaylightSavingTime[i] = false;
                    FTimezone[i] = "";
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "Gregorian", Category = "Time", Version = "Join", Help = "Create time in a gregorian way.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class JoinTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Millisecond", DefaultValue = 0, MinValue = 0, MaxValue = 999)]
        public ISpread<int> FMilli;

        [Input("Second", DefaultValue = 0, MinValue = 0, MaxValue = 59)]
        public ISpread<int> FSecond;

        [Input("Minute", DefaultValue = 0, MinValue = 0, MaxValue = 59)]
        public ISpread<int> FMinute;

        [Input("Hour", DefaultValue = 0, MinValue = 0, MaxValue = 23)]
        public ISpread<int> FHour;

        [Input("Day", DefaultValue = 1, MinValue = 1, MaxValue = 31)]
        public ISpread<int> FDay;

        [Input("Month", DefaultValue = 1, MinValue = 1, MaxValue = 12)]
        public ISpread<int> FMonth;

        [Input("Year", DefaultValue = 1, MinValue = 1, MaxValue = 9999)]
        public ISpread<int> FYear;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimezone;

        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<bool> FSuccess;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        [ImportingConstructor]
        public JoinTimeNode()
        {
            var timezones = new List<string>();
            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones()) timezones.Add(z.Id);
            EnumManager.UpdateEnum("TimezoneEnum", "UTC", timezones.ToArray());
        }

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FSuccess.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    var dt = new DateTime(FYear[i], FMonth[i], FDay[i], FHour[i], FMinute[i], FSecond[i], FMilli[i]);
                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(FTimezone[i]);
                    var dtwz = new DateTimeWithZone(dt, tz);
                    FOutput[i] = dtwz;
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.Utc);
                    FSuccess[i] = false;
                }

            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "ChangeTimezone", Category = "Time", Help = "Changes time from the current timezone to a new timezone. This also converts the time!", Tags = "Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class UpdateTimezoneNode : IPluginEvaluate
    {
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimezone;

        #region fields & pins
        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        #endregion fields & pins

        [ImportingConstructor]
        public UpdateTimezoneNode()
        {
            var timezones = new List<string>();
            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones()) timezones.Add(z.Id);
            EnumManager.UpdateEnum("TimezoneEnum", "UTC", timezones.ToArray());
        }

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(FTimezone[i]);
                var dtwz = new DateTimeWithZone(DateTime.SpecifyKind(FInput[i].TimeInSpecificZone(tz), DateTimeKind.Unspecified), tz);
                FOutput[i] = dtwz;
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "TimeSpan", Category = "Time", Version = "Join", Help = "Creates a timespan", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class JoinTimespanNode : IPluginEvaluate
    {
        #region fields & pins

        [Input("Day")]
        public ISpread<int> FDay;

        [Input("Hour")]
        public ISpread<int> FHour;

        [Input("Minute")]
        public ISpread<int> FMinute;

        [Input("Second")]
        public ISpread<int> FSecond;

        [Input("Millisecond")]
        public ISpread<int> FMilli;

        [Output("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FTimeSpan.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                FTimeSpan[i] = new TimeSpan(FDay[i], FHour[i], FMinute[i], FSecond[i], FMilli[i]);
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "TimeSpan", Category = "Time", Version = "Split", Help = "Splits a given timespan", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class SplitTimespanNode : IPluginEvaluate
    {
        #region fields & pins

        [Input("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Day")]
        public ISpread<int> FDay;

        [Output("Hour")]
        public ISpread<int> FHour;

        [Output("Minute")]
        public ISpread<int> FMinute;

        [Output("Second")]
        public ISpread<int> FSecond;

        [Output("Millisecond")]
        public ISpread<int> FMilli;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FDay.SliceCount = FHour.SliceCount = FMinute.SliceCount = FSecond.SliceCount = FMilli.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                FDay[i] = FTimeSpan[i].Days;
                FHour[i] = FTimeSpan[i].Hours;
                FMinute[i] = FTimeSpan[i].Minutes;
                FSecond[i] = FTimeSpan[i].Seconds;
                FMilli[i] = FTimeSpan[i].Milliseconds;
            }
        }
    }

}