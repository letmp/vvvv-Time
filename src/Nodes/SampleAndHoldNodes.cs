using System;
using VVVV.PluginInterfaces.V2;
using VVVV.Packs.Time;

namespace VVVV.Packs.Time.Nodes
{

    #region PluginInfo
    [PluginInfo(Name = "S+H", Category = "Time", Help = "Sample and Hold - if set is 1 just passes the input through, but take a sample and hold it, as long as set is 0",
        Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SampleAndHoldTimeNode : IPluginEvaluate
    {
        #region fields & pins
#pragma warning disable 649
        [Input("Time", AutoValidate = false)]
        private ISpread<Time> FInput;

        [Input("Set", IsBang = true)]
        private ISpread<bool> FSetIn;
        [Output("Time")]
        private ISpread<Time> FOutput;
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
    [PluginInfo(Name = "S+H", 
                Category = "Time",
                Version = "TimeSpan",
                Help = "Sample and Hold - if set is 1 just passes the input through, but take a sample and hold it, as long as set is 0", 
                Author = "sebl")]
    #endregion PluginInfo
    public class SampleAndHoldTimeSpanNode : IPluginEvaluate
    {
        #region fields & pins
        #pragma warning disable 649
        [Input("TimeSpan", AutoValidate = false)]
        private ISpread<TimeSpan> FInput;

        [Input("Set", IsBang = true)]
        private ISpread<bool> FSetIn;

        [Output("TimeSpan")]
        private ISpread<TimeSpan> FOutput;
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
}
