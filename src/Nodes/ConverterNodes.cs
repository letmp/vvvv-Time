using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.Packs.Time;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.Time.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "AsTime", Category = "String", Help = "Parses a given string and outputs time in an arbitrary timezone.", Tags = "String, Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class AsTimeStringNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Input", CheckIfChanged = true)]
        public ISpread<string> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimeZone;

        [Input("Format", DefaultString = "yyyy'/'MM'/'dd-H:mm:ss")]
        public ISpread<string> FFormat;

        [Output("Time")]
        public ISpread<Time> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        [ImportingConstructor]
        public AsTimeStringNode()
        {
            TimeZoneManager.Update();
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
                        var dtwz = new Time(dt, tz);
                        FOutput[i] = dtwz;
                        FSuccess[i] = true;
                    }
                    catch (Exception e)
                    {
                        FLogger.Log(LogType.Debug, e.ToString());
                        FOutput[i] = new Time(DateTime.MinValue, TimeZoneInfo.Utc);
                        FSuccess[i] = false;
                    }
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "AsTime", Category = "Value", Help = "Converts a double to time in a given timezone.", Tags = "Value, Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class AsTimeValueNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<double> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimeZone;

        [Output("Time")]
        public ISpread<Time> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        [ImportingConstructor]
        public AsTimeValueNode()
        {
            TimeZoneManager.Update();
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
                    var dtwz = new Time(dt, tz);
                    FOutput[i] = dtwz;
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = new Time(DateTime.MinValue, TimeZoneInfo.Utc);
                    FSuccess[i] = false;
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "AsString", Category = "Time", Help = "Gives the string representation of a Time object in a given format", Tags = "String", Author = "tmp")]
    #endregion PluginInfo
    public class AsStringTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<Time> FInput;

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
                    FOutput[i] = FInput[i].ZoneTime.ToString(FFormat[i]);
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
    [PluginInfo(Name = "AsString", Category = "Time", Version="TimeSpan", Help = "Gives the string representation of a TimeSpan object", Tags = "String", Author = "tmp")]
    #endregion PluginInfo
    public class AsStringTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("TimeSpan")]
        public ISpread<TimeSpan> FInput;

        [Output("TimeSpan String")]
        public ISpread<string> FOutput;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;

            for (int i = 0; i < FInput.SliceCount; i++)
            {
                try
                {
                    FOutput[i] = FInput[i].ToString();
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = "";
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "AsValue", Category = "Time", Version = "Decimal", Help = "Gives the decimal representation of a Time object", Tags = "", Author = "sebl")]
    #endregion PluginInfo
    public class AsValueDecimalTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<Time> FInput;

        [Output("Decimal")]
        public ISpread<double> FOutput;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FInput.SliceCount = SpreadMax;

            for (int i = 0; i < FInput.SliceCount; i++)
            {
                try
                {
//                  FOutput[i] = ( double )Convert.ToDecimal(TimeSpan.Parse(FInput[i].UniversalTime.ToString("HH:mm:ss")).TotalHours);
                    FOutput[i] = ( double )Convert.ToDecimal(TimeSpan.Parse(FInput[i].ZoneTime.ToString("HH:mm:ss")).TotalHours);
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = -1;
                }
            }
        }
    }


    #region PluginInfo
    [PluginInfo(Name = "AsValue", Category = "Time", Version = "Unix", Help = "Gives the Unnix Time Code (Value) of a Time object", Tags = "", Author = "sebl")]
    #endregion PluginInfo
    public class AsValueUnixTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<Time> FInput;

        [Output("Unix Time Stamp")]
        public ISpread<double> FOutput;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FInput.SliceCount = SpreadMax;

            for (int i = 0; i < FInput.SliceCount; i++)
            {
                try
                {
                    FOutput[i] = (double) UnixTimestampFromDateTime(FInput[i].ZoneTime);
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = -1;
                }
            }
        }

        public static double UnixTimestampFromDateTime(DateTime date)
        {
            TimeSpan span = date - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return span.TotalSeconds;
        }
    }


    #region PluginInfo
    [PluginInfo(Name = "AsString", Category = "Time", Version = "Unix", Help = "Gives the Unnix Time Code (String) of a Time object", Tags = "", Author = "sebl")]
    #endregion PluginInfo
    public class AsStringUnixTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<Time> FInput;

        [Output("Unix Time Stamp")]
        public ISpread<string> FOutput;

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = FInput.SliceCount = SpreadMax;

            for (int i = 0; i < FInput.SliceCount; i++)
            {
                try
                {
                    FOutput[i] = UnixTimestampFromDateTime(FInput[i].ZoneTime).ToString();
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FOutput[i] = "";
                }
            }
        }

        public static double UnixTimestampFromDateTime(DateTime date)
        {
            TimeSpan span = date - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return span.TotalSeconds;
        }
    }


    #region PluginInfo
    [PluginInfo(Name = "ChangeTimezone", Category = "Time", Help = "Changes time from the current timezone to a new timezone. This also converts the time!", Tags = "Timezone", Author = "tmp")]
    #endregion PluginInfo
    public class UpdateTimezoneNode : IPluginEvaluate
    {
        [Input("Time")]
        public ISpread<Time> FInput;

        [Input("Timezone", EnumName = "TimezoneEnum")]
        public IDiffSpread<EnumEntry> FTimezone;

        #region fields & pins
        [Output("Time")]
        public ISpread<Time> FOutput;

        #endregion fields & pins

        [ImportingConstructor]
        public UpdateTimezoneNode()
        {
            TimeZoneManager.Update();

        }

        public void Evaluate(int SpreadMax)
        {
            FOutput.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(FTimezone[i]);
                var dtwz = new Time(DateTime.SpecifyKind(FInput[i].OtherZoneTime(tz), DateTimeKind.Unspecified), tz);
                FOutput[i] = dtwz;
            }
        }
    }

}
