#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Nodes;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.Streams;
using VVVV.Core.Logging;

#endregion usings

namespace VVVV.Time
{

    #region PluginInfo
    [PluginInfo(Name = "CurrentTime", Category = "Time", Help = "Outputs the current time.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class CurrentTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Daylight Saving Time")]
        public ISpread<bool> FDaylightSavingTime;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FDaylightSavingTime.SliceCount = 1;
            var dtwz = new DateTimeWithZone(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), TimeZoneInfo.Local);
            FOutput[0] = dtwz;
            FDaylightSavingTime[0] = dtwz.TimeInOriginalZone.IsDaylightSavingTime();
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "AsTime", Category = "String", Help = "Parses a given string and outputs time in an arbitrary timezone.", Tags = "String, Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class AsTimeNodeString : IPluginEvaluate
    {
        #region fields & pins
        [Input("Input", CheckIfChanged = true)]
        public ISpread<string> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimeZone;

        [Input("Format", DefaultString = "yyyy'/'MM'/'dd-H:mm:ss")]
        public ISpread<string> FFormat;

        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        [ImportingConstructor]
        public AsTimeNodeString()
        {
            var timezones = new List<string>();
            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones()) timezones.Add(z.Id);
            EnumManager.UpdateEnum("TimezoneEnum", "UTC", timezones.ToArray());
        }

        public void Evaluate(int SpreadMax)
        {

            FOutput.SliceCount = FSuccess.SliceCount = SpreadMax;
            if (FInput.IsChanged)
            {
                for (int i = 0; i < SpreadMax; i++)
                {
                    try
                    {
                        var tz = TimeZoneInfo.FindSystemTimeZoneById(FTimeZone[i]);
                        DateTime dt = DateTime.ParseExact(FInput[i], FFormat[i], null);
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
    }

    #region PluginInfo
    [PluginInfo(Name = "AsTime", Category = "Value", Help = "Converts a double to time in a given timezone.", Tags = "Value, Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class AsTimeNodeValue : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<double> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimeZone;
    
        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        [ImportingConstructor]
        public AsTimeNodeValue()
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
                    var tz = TimeZoneInfo.FindSystemTimeZoneById(FTimeZone[i]);
                    DateTime dt = DateTime.FromOADate(FInput[i]);
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
    [PluginInfo(Name = "LocalTimezone", Category = "Time", Help = "Outputs your local timezone.", Tags = "Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class LocalTimezoneNode : IPluginEvaluate
    {
        #region fields & pins
        [Output("Timezone")]
        public ISpread<string> FOutput;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = 1;
            FOutput[0] = TimeZoneInfo.Local.Id;
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "AsString", Category = "Time", Help = "Gives the string representation of a Time object in a given format", Tags = "String", Author = "tmp")]
    #endregion PluginInfo
    public class AsStringNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("Format", DefaultString = "yyyy'/'MM'/'dd-H:mm:ss")]
        public ISpread<string> FFormat;

        [Output("Timestring")]
        public ISpread<string> FOutput;

        [Output("Timezone")]
        public ISpread<string> FTimezone;
        
        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FTimezone.SliceCount = SpreadMax;

            for (int i = 0; i < FInput.SliceCount; i++)
            {
                try
                {
                    FOutput[i] = FInput[i].TimeInOriginalZone.ToString(FFormat[i]);
                    FTimezone[i] = FInput[i].TimeZone.Id;    
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = "";
                    FTimezone[i] = "";    
                }
            }
        }
    }
    
    #region PluginInfo
    [PluginInfo(Name = "Gregorian", Category = "Time", Version = "Split", Help = "Gregorian style time splitting.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class GregorianSplitNode : IPluginEvaluate
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
                try{
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
    public class GregorianJoinNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Millisecond",DefaultValue = 0, MinValue = 0, MaxValue = 999)]
        public ISpread<int> FMilli;

        [Input("Second", DefaultValue = 0, MinValue = 0, MaxValue = 59)]
        public ISpread<int> FSecond;

        [Input("Minute", DefaultValue = 0, MinValue = 0, MaxValue = 59)]
        public ISpread<int> FMinute;

        [Input("Hour", DefaultValue = 0,MinValue = 0, MaxValue = 23)]
        public ISpread<int> FHour;

        [Input("Day", DefaultValue = 1, MinValue = 1, MaxValue = 31)]
        public ISpread<int> FDay;

        [Input("Month", DefaultValue = 1, MinValue = 1, MaxValue = 12)]
        public ISpread<int> FMonth;

        [Input("Year", DefaultValue = 1, MinValue = 1,MaxValue = 9999)]
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
        public GregorianJoinNode()
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
    public class ChangeTimezoneNode : IPluginEvaluate
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
        public ChangeTimezoneNode()
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
    public class TimeSpanJoinNode : IPluginEvaluate
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
    public class TimeSpanSplitNode : IPluginEvaluate
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

    #region PluginInfo
    [PluginInfo(Name = "Add", Category = "Time", Version = "TimeSpan", Help = "Adds a timespan to a given time", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class AddTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FSuccess.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    var dtwz = new DateTimeWithZone(FInput[i].TimeInOriginalZone.Add(FTimeSpan[i]), FInput[i].TimeZone);
                    FOutput[i] = dtwz;
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FSuccess[i] = false;
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "Subtract", Category = "Time", Version = "TimeSpan", Help = "Subtracts a timespan from a given time", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class SubtractTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FSuccess.SliceCount = SpreadMax;

            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    var dtwz = new DateTimeWithZone(FInput[i].TimeInOriginalZone.Subtract(FTimeSpan[i]), FInput[i].TimeZone);
                    FOutput[i] = dtwz;
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FSuccess[i] = false;
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "Subtract", Category = "Time", Help = "Subtracts times and creates a timespan.", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class SubtractTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time 1")]
        public ISpread<DateTimeWithZone> FInput1;

        [Input("Time 2")]
        public ISpread<DateTimeWithZone> FInput2;

        [Output("TimeSpan")]
        public ISpread<TimeSpan> FOutput;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    FOutput[i] = FInput1[i].UniversalTime - FInput2[i].UniversalTime;
                }
                catch (Exception e)
                {
                    FOutput[i] = new TimeSpan();
                    FLogger.Log(LogType.Debug, e.ToString());
                }
            }
        }
    }
   
    #region PluginInfo
    [PluginInfo(Name = "EQ", Category = "Time", Help = "Compares two times and returns 1 if they are equal", Tags = "TimeSpan", Author = "tmp")]
    #endregion PluginInfo
    public class EqualsDateTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time 1")]
        public ISpread<DateTimeWithZone> FInput1;

        [Input("Time 2")]
        public ISpread<DateTimeWithZone> FInput2;

        [Input("Epsilon (TimeSpan)")]
        public ISpread<TimeSpan> FEpsilon;

        [Output("Output")]
        public ISpread<bool> FOutput;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                DateTime epsilonHigh = FInput2[i].UniversalTime.Add(FEpsilon[i]);
                DateTime epsilonLow = FInput2[i].UniversalTime.Subtract(FEpsilon[i]);
                if (FInput1[i].UniversalTime.CompareTo(epsilonLow) >= 0 && FInput1[i].UniversalTime.CompareTo(epsilonHigh) <= 0)
                    FOutput[i] = true;
                else FOutput[i] = false;
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "GT", Category = "Time", Help = "Compares two times and returns 1 if time1 is gt time2.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class GtTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time 1")]
        public ISpread<DateTimeWithZone> FInput1;

        [Input("Time 2")]
        public ISpread<DateTimeWithZone> FInput2;

        [Output("Output")]
        public ISpread<bool> FOutput;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                if (FInput1[i].UniversalTime.CompareTo(FInput2[i].UniversalTime) > 0)
                    FOutput[i] = true;
                else FOutput[i] = false;
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "LT", Category = "Time", Help = "Compares two times and returns 1 if time1 is lt time2.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class LtTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time 1")]
        public ISpread<DateTimeWithZone> FInput1;

        [Input("Time 2")]
        public ISpread<DateTimeWithZone> FInput2;

        [Output("Output")]
        public ISpread<bool> FOutput;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                if (FInput1[i].UniversalTime.CompareTo(FInput2[i].UniversalTime) < 0)
                    FOutput[i] = true;
                else FOutput[i] = false;
            }
        }
    }

    public enum sortEnum
    {
        Ascending,
        Descending
    }

    #region PluginInfo
    [PluginInfo(Name = "Sort", Category = "Time", Help = "Sorts a spread of times. For that the internal UTC represantation is used.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SortDateTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("Input", DefaultEnumEntry = "Ascending")]
        public IDiffSpread<sortEnum> FEnum;

        [Output("Time")]
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Former Index")]
        public ISpread<int> FIndex;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FIndex.SliceCount = SpreadMax;

            var listData = new List<KeyValuePair<int, DateTimeWithZone>>();

            for (int i = 0; i < SpreadMax; i++)
            {
                listData.Add(new KeyValuePair<int, DateTimeWithZone>(i, FInput[i]));
            }
            if (Enum.GetName(typeof(sortEnum), FEnum[0]) == "Ascending")
            {
                listData.Sort((a, b) => a.Value.UniversalTime.CompareTo(b.Value.UniversalTime));
            }
            else
            {
                listData.Sort((a, b) => b.Value.UniversalTime.CompareTo(a.Value.UniversalTime));
            }
            for (int i = 0; i < SpreadMax; i++)
            {
                FOutput[i] = listData[i].Value;
                FIndex[i] = listData[i].Key;
            }
        }
    }

    [PluginInfo(Name = "Zip", Category = "Time", Help = "Zip time", Tags = "", Author = "tmp")]
    public class TimeZipNode : ZipNode<IInStream<DateTimeWithZone>>
    {
    }

    [PluginInfo(Name = "Unzip", Category = "Time", Help = "Unzip time", Tags = "", Author = "tmp")]
    public class TimeUnZipNode : UnzipNode<IInStream<DateTimeWithZone>>
    {
    }

    #region PluginInfo
    [PluginInfo(Name = "Select", Category = "Time", Help = "select the slices which form the new spread.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SelectTimeNode : IPluginEvaluate
    {
        #region fields & pins
        #pragma warning disable 649
        [Input("Input", CheckIfChanged = true)]
        ISpread<DateTimeWithZone> FInput;

        [Input("Select", DefaultValue = 1, MinValue = 0)]
        ISpread<int> FSelect;

        [Output("Output", AutoFlush = false)]
        ISpread<DateTimeWithZone> FOutput;

        [Output("Former Slice", AutoFlush = false)]
        ISpread<int> FFormer;
        #pragma warning restore
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = 0;
            FFormer.SliceCount = 0;
            if(FInput.IsChanged)
            {
                for (int i = 0; i < SpreadMax; i++)
                {
                    DateTimeWithZone output = FInput[i];

                    for (int j = 0; j < FSelect[i]; j++)
                    {
                        FOutput.Add(output);
                        FFormer.Add(i % FInput.SliceCount);
                    }
                }

                FOutput.Flush();
                FFormer.Flush();
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "Select", Category = "Time", Version = "Bin", Help = "select the slices which form the new spread.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SelectTimeBinNode : IPluginEvaluate
    {
        #region fields & pins
        #pragma warning disable 649
        [Input("Input", CheckIfChanged = true)]
        ISpread<ISpread<DateTimeWithZone>> FInput;

        [Input("Select", DefaultValue = 1, MinValue = 0)]
        ISpread<int> FSelect;

        [Output("Output", AutoFlush = false)]
        ISpread<ISpread<DateTimeWithZone>> FOutput;

        [Output("Former Slice", AutoFlush = false)]
        ISpread<int> FFormer;
        #pragma warning restore
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FInput.SliceCount;
            FFormer.SliceCount = 0;
            if (FInput.IsChanged)
            {
                for (int i = 0; i < FInput.SliceCount; i++)
                {
                    FOutput[i].SliceCount = 0;
                    for (int j = 0; j < FSelect[i]; j++)
                    {
                        FOutput[i].AddRange(FInput[i]);
                        FFormer.Add(i);
                    }
                }
                FOutput.Flush();
                FFormer.Flush();
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "S+H", Category = "Time", Help = "Sample and Hold - if set is 1 just passes the input through, but take a sample and hold it, as long as set is 0",
        Tags = "", Author = "tmp")]
    #endregion PluginInfo

    public class SHTimeNode : IPluginEvaluate
    {
        #region fields & pins
        #pragma warning disable 649
        [Input("Time", AutoValidate = false)]
        private ISpread<DateTimeWithZone> FInput;
        
        [Input("Set", IsBang = true)]
        private ISpread<bool> FSetIn;
        [Output("Time")]
        private ISpread<DateTimeWithZone> FOutput;
        #pragma warning restore
        #endregion fields & pins

        public void Evaluate(int spreadMax)
        {
            if (FSetIn[0])
            {
                FInput.Sync();
                FOutput.AssignFrom(FInput);
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "GetSlice", Category = "Time", Help = "gets all slices specified in the index input from the input spread",
        Tags = "Time", Author = "tmp")]
    #endregion PluginInfo

    public class GetSliceTimeNode : IPluginEvaluate
    {
        #region fields & pins
        #pragma warning disable 649
        [Input("Input", BinSize = 1)]
        IDiffSpread<ISpread<DateTimeWithZone>> FInput;

        [Input("Index", DefaultValue = 0)]
        ISpread<int> FIndex;

        [Output("Output", AutoFlush = false, BinVisibility = PinVisibility.OnlyInspector)]
        ISpread<ISpread<DateTimeWithZone>> FOutput;
        #pragma warning restore
        #endregion fields & pins

        public void Evaluate(int spreadMax)
        {
            spreadMax = FIndex.SliceCount;
            FOutput.SliceCount = spreadMax;

            for (int i = 0; i < spreadMax; i++)
            {
                FOutput[i] = FInput[FIndex[i]];
            }
            FOutput.Flush();
        }
    }

    [PluginInfo(Name = "DeleteSlice", Category = "Time", Help = "Deletes a slice from a Spread at the given index.", Tags = "", Author = "tmp")]
    public class TimeDeleteSliceNode : DeleteSlice<IInStream<DateTimeWithZone>>
    {
    }

    [PluginInfo(Name = "SetSlice", Category = "Time", Help = "Replace individual slices of the spread with the given input", Tags = "", Author = "tmp")]
    public class TimeGetSliceNode : SetSlice<IInStream<DateTimeWithZone>>
    {
    }

}
