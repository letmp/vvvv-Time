#region usings

using System;
using System.ComponentModel.Composition;
using Time.Core;
using VVVV.Core.Logging;
using VVVV.Packs.Time;
using VVVV.PluginInterfaces.V2;

#endregion usings

namespace Time.Nodes
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


}
