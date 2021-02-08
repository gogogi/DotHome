using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.RunningModel.Tools;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotHome.StandardBlocks.Builtin
{
    public abstract class RefSink : ABlock
    {
        [BlockParameter(true), Unique]
        public string Reference { get; set; } = "Ref";

        public event Action Transfer;

        public override void Run()
        {
            Transfer?.Invoke();
        }
    }

    [Description("Transfers value to corresponding RefSources"), Category("Builtin"), Color("Green")]
    class RefSink<T> : RefSink
    {
        private RefProvider refProvider;

        [Description("Input")]
        public Input<T> I { get; set; }
                
        public RefSink(RefProvider refProvider) 
        {
            this.refProvider = refProvider;
        }


        public override void Init()
        {
            if(refProvider.RefSources.TryGetValue(Reference, out List<RefSource> sources))
            {
                foreach(RefSource source in sources)
                {
                    var action = RunningModelTools.GetTransferAction(I, source.Target);
                    if (action != null) Transfer += action;
                }
            }
            refProvider.RefSinks.TryAdd(Reference, this);
        }
    }
}
