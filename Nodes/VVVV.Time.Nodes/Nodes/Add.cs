using System;
using System.ComponentModel.Composition;
using Time.Core;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.Time
{
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
}