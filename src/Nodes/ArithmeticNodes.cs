using System;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;
using VVVV.Packs.Time;

namespace VVVV.Packs.Time.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "Add", Category = "Time", Version = "TimeSpan", Help = "Adds a timespan to a given time", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class AddTimeTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<Time> FInput;

        [Input("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Time")]
        public ISpread<Time> FOutput;

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
                    var dtwz = new Time(FInput[i].ZoneTime.Add(FTimeSpan[i]), FInput[i].TimeZone);
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
    [PluginInfo(Name = "Subtract", Category = "Time", Version = "TimeSpan", Help = "Subtracts a timespan from a given time", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SubtractTimeTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<VVVV.Packs.Time.Time> FInput;

        [Input("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Time")]
        public ISpread<VVVV.Packs.Time.Time> FOutput;

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
                    var dtwz = new VVVV.Packs.Time.Time(FInput[i].ZoneTime.Subtract(FTimeSpan[i]), FInput[i].TimeZone);
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
    public class SubtractTimeTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time 1")]
        public ISpread<VVVV.Packs.Time.Time> FInput1;

        [Input("Time 2")]
        public ISpread<VVVV.Packs.Time.Time> FInput2;

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
                    FOutput[i] = FInput1[i] - FInput2[i];
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
    [PluginInfo(Name = "Add", Category = "Time", Version = "TimeSpan TimeSpan", Help = "Adds a timespan to a given timespan", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class AddTimeSpanTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("TimeSpan 1")]
        public ISpread<TimeSpan> FTimeSpan1;

        [Input("TimeSpan 2")]
        public ISpread<TimeSpan> FTimeSpan2;

        [Output("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FTimeSpan.SliceCount = FSuccess.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    FTimeSpan[i] = FTimeSpan1[i].Add(FTimeSpan2[i]);
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FTimeSpan[i] = FTimeSpan1[i];
                    FSuccess[i] = false;
                }
            }
        }
    }

    #region PluginInfo
    [PluginInfo(Name = "Subtract", Category = "Time", Version = "TimeSpan TimeSpan", Help = "Adds a timespan to a given timespan", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SubtractTimeSpanTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("TimeSpan 1")]
        public ISpread<TimeSpan> FTimeSpan1;

        [Input("TimeSpan 2")]
        public ISpread<TimeSpan> FTimeSpan2;

        [Output("TimeSpan")]
        public ISpread<TimeSpan> FTimeSpan;

        [Output("Success")]
        public ISpread<Boolean> FSuccess;

        [Import()]
        public ILogger FLogger;

        #endregion fields & pins
        public void Evaluate(int SpreadMax)
        {
            FTimeSpan.SliceCount = FSuccess.SliceCount = SpreadMax;
            for (int i = 0; i < SpreadMax; i++)
            {
                try
                {
                    FTimeSpan[i] = FTimeSpan1[i].Subtract(FTimeSpan2[i]);
                    FSuccess[i] = true;
                }
                catch (Exception e)
                {
                    FLogger.Log(LogType.Debug, e.ToString());
                    FTimeSpan[i] = FTimeSpan1[i];
                    FSuccess[i] = false;
                }
            }
        }
    }
}
