using System;
using System.ComponentModel.Composition;
using Time.Core;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;
using VVVV.Packs.Time;

namespace Time.Nodes
{

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
}
