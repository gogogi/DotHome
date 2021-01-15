using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Tools
{
    public class BlockViewContainer
    {
        public List<ABlockView> Blocks { get; } = new List<ABlockView>();

        public List<WireView> Wires { get; } = new List<WireView>();

        public BlockContainer ToBlockContainer()
        {
            BlockContainer blocksContainer = new BlockContainer();

            Dictionary<InputView, IInput> inputsDictionary = new Dictionary<InputView, IInput>();
            Dictionary<OutputView, IOutput> outputsDictionary = new Dictionary<OutputView, IOutput>();

            foreach (var block in Blocks)
            {
                blocksContainer.Blocks.Add(ModelConverter.ABlockViewToABlock(block, inputsDictionary, outputsDictionary));
            }
            foreach (var wire in Wires)
            {
                blocksContainer.Wires.Add(new Wire() { Input = inputsDictionary[wire.InputView], Output = outputsDictionary[wire.OutputView] });
            }
            return blocksContainer;
        }
    }
}
