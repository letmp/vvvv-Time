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
        public ISpread<DateTimeWithZone> FOutput;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins

        [ImportingConstructor]
        public AsTimeStringNode()
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
    public class AsTimeValueNode : IPluginEvaluate
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
        public AsTimeValueNode()
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
    public class AsStringTimeNode : IPluginEvaluate
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

}
