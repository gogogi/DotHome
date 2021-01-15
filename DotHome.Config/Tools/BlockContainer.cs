using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotHome.Config.Tools
{
    [Serializable]
    public class BlockContainer
    {
        public List<ABlock> Blocks { get; } = new List<ABlock>();

        public List<Wire> Wires { get; } = new List<Wire>();

        public BlockViewContainer ToBlockViewContainer()
        {
            BlockViewContainer blocksViewContainer = new BlockViewContainer();

            Dictionary<IInput, InputView> inputsDictionary = new Dictionary<IInput, InputView>();
            Dictionary<IOutput, OutputView> outputsDictionary = new Dictionary<IOutput, OutputView>();

            foreach (var block in Blocks)
            {
                blocksViewContainer.Blocks.Add(ModelConverter.ABlockToABlockView(block, inputsDictionary, outputsDictionary));
            }
            foreach(var wire in Wires)
            {
                blocksViewContainer.Wires.Add(new WireView(inputsDictionary[wire.Input], outputsDictionary[wire.Output]));
            }
            return blocksViewContainer;
        }
    }
}
