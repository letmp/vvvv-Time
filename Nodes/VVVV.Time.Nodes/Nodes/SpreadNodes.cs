using System;
using System.Collections.Generic;
using VVVV.Nodes;
using VVVV.Packs.Time;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.Streams;

namespace VVVV.Packs.Time.Nodes
{


    #region PluginInfo
    [PluginInfo(Name = "Sort", Category = "Time", Help = "Sorts a spread of times. For that the internal UTC represantation is used.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SortDateTimeNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Time")]
        public ISpread<DateTimeWithZone> FInput;

        [Input("Input", DefaultEnumEntry = "Ascending")]
        public IDiffSpread<SortEnum> FEnum;

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
            if (Enum.GetName(typeof(SortEnum), FEnum[0]) == "Ascending")
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

    #region PluginInfo
    [PluginInfo(Name = "Zip", Category = "Time", Help = "Zip time", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class ZipTimeNode : ZipNode<IInStream<DateTimeWithZone>>
    {
    }

    #region PluginInfo
    [PluginInfo(Name = "Unzip", Category = "Time", Help = "Unzip time", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class UnZipTimeNode : UnzipNode<IInStream<DateTimeWithZone>>
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
            if (FInput.IsChanged)
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

    #region PluginInfo
    [PluginInfo(Name = "DeleteSlice", Category = "Time", Help = "Deletes a slice from a Spread at the given index.", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class DeleteSliceTimeNode : DeleteSlice<IInStream<DateTimeWithZone>> { }

    #region PluginInfo
    [PluginInfo(Name = "SetSlice", Category = "Time", Help = "Replace individual slices of the spread with the given input", Tags = "", Author = "tmp")]
    #endregion PluginInfo
    public class SetSliceTimeNode : SetSlice<IInStream<DateTimeWithZone>> { }

}
