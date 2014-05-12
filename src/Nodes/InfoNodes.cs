#region usings

using System;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

#endregion usings

namespace VVVV.Packs.Time.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "CurrentTime", Category = "Time", Help = "Outputs the current time.", Tags = "Now", Author = "tmp")]
    #endregion PluginInfo
    public class CurrentTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Output("Time")]
        public ISpread<VVVV.Packs.Time.Time> FOutput;

        [Output("Daylight Saving Time")]
        public ISpread<bool> FDaylightSavingTime;

        [Import()]
        public ILogger FLogger;

        private static double currentFrame;
        private  static Time? currentTime = null;
            
        [Import()]
        #pragma warning disable 649
        IHDEHost FHDEHost;
        #pragma warning restore 649

        #endregion fields & pins

        public void Evaluate(int SpreadMax)
        {
            if (currentTime == null || currentFrame != FHDEHost.FrameTime)
            {
                currentFrame = FHDEHost.FrameTime;
                currentTime = Time.CurrentTime();
            }

            FOutput.SliceCount = FDaylightSavingTime.SliceCount = 1;
            var dtwz = currentTime.Value;
            
            FOutput[0] = dtwz;
            FDaylightSavingTime[0] = dtwz.ZoneTime.IsDaylightSavingTime();
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
}
