using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Block = DotHome.ProgrammingModel.Block;

namespace DotHome.Core.Tools
{
    public static class BlocksSorter
    {
        private class BlockWrapper
        {
            public Block Block { get; set; }
            public List<BlockWrapper> PrecedingBlocks { get; } = new List<BlockWrapper>();
            public List<BlockWrapper> SuccessingBlocks { get; } = new List<BlockWrapper>();
        }

        public static List<Block> SortTopological(List<Block> blocks, List<Wire> wires)
        {
            if (blocks.Count == 0) return new List<Block>();

            List<BlockWrapper> blockWrappers = blocks.Select(b => new BlockWrapper() { Block = b }).ToList();
            foreach (Wire wire in wires)
            {
                BlockWrapper inputBlockWrapper = blockWrappers.Single(bw => bw.Block.Inputs.Contains(wire.Input));
                BlockWrapper outputBlockWrapper = blockWrappers.Single(bw => bw.Block.Outputs.Contains(wire.Output));
                if(!outputBlockWrapper.Block.Definition.Type.IsAssignableTo(typeof(Device)))
                {
                    inputBlockWrapper.PrecedingBlocks.Add(outputBlockWrapper);
                    outputBlockWrapper.SuccessingBlocks.Add(inputBlockWrapper);
                }
            }

            Queue<BlockWrapper> blockWrappersWithoutPrecedors = new Queue<BlockWrapper>(blockWrappers.Where(bw => bw.PrecedingBlocks.Count == 0));
            //blockWrappersWithoutPrecedors.Enqueue(blockWrappers.First(bw => bw.PrecedingBlocks.Count == 0));

            List<Block> sortedBlocks = new List<Block>();

            while (blockWrappersWithoutPrecedors.TryDequeue(out BlockWrapper blockWrapper))
            {
                sortedBlocks.Add(blockWrapper.Block);
                foreach (BlockWrapper successingBlock in blockWrapper.SuccessingBlocks)
                {
                    successingBlock.PrecedingBlocks.Remove(blockWrapper);
                    if (successingBlock.PrecedingBlocks.Count == 0)
                    {
                        blockWrappersWithoutPrecedors.Enqueue(successingBlock);
                    }
                }
            }

            return sortedBlocks;
        }
    }
}