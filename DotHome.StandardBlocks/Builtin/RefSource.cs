using DotHome.Model;
using DotHome.Model.Attributes;
using DotHome.Model.Tools;
using DotHome.StandardBlocks.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotHome.StandardBlocks.Builtin
{
    public abstract class RefSource : Block
    {
        [BlockParameter(true)]
        public string Reference { get; set; } = "Ref";

        public abstract Value Target { get; }
    }

    [Description("Provides value from corresponding RefSinks"), Category("Builtin"), Color("Green")]
    public class RefSource<T> : RefSource
    {
        private RefProvider refProvider;
        private Value<T> target;

        public override Value Target => target;

        [Description("Output")]
        public Output<T> O { get; set; }
        
        public RefSource(RefProvider refProvider)
        {
            this.refProvider = refProvider;
        }

        public override void Init()
        {
            if (refProvider.RefSinks.TryGetValue(Reference, out RefSink sink))
            {
                var action = ModelTools.GetTransferAction(sink.Inputs[0], target);
                if (action != null) sink.Transfer += action;
            }
            if (refProvider.RefSources.TryGetValue(Reference, out List<RefSource> sources))
            {
                sources.Add(this);
            }
            else
            {
                refProvider.RefSources.Add(Reference, new List<RefSource>() { this });
            }
        }

        public override void Run()
        {
            O.Val = target.Val;
        }
    }
}
