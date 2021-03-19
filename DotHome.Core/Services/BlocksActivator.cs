using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using DotHome.StandardBlocks.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotHome.Core.Services
{
    public class BlocksActivator
    {
        private IServiceProvider serviceProvider;

        private Dictionary<Type, object> localServices = new Dictionary<Type, object>();

        public List<IBlockService> BlockServices => localServices.Values.OfType<IBlockService>().ToList();

        public BlocksActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void DisposeServices()
        {
            foreach(var o in localServices)
            {
                if (o.Value is IDisposable d) d.Dispose();
            }
            localServices.Clear();
        }

        private object CreateInstance(Type type)
        {
            var constructors = type.GetConstructors();
            if(constructors.Length == 1)
            {
                var constructor = constructors.Single();
                var argTypes = constructor.GetParameters().Select(pi => pi.ParameterType).ToArray();
                List<object> parameters = new List<object>();
                foreach(Type argType in argTypes)
                {
                    object globalService = serviceProvider.GetService(argType);
                    if(globalService != null)
                    {
                        parameters.Add(globalService);
                    }
                    else if(localServices.TryGetValue(argType, out object localService))
                    {
                        parameters.Add(localService);
                    }
                    else
                    {
                        localService = CreateInstance(argType);
                        if(localService != null)
                        {
                            localServices.Add(argType, localService);
                            parameters.Add(localService);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return constructor.Invoke(parameters.ToArray());
            }
            else
            {
                return null;
            }
        }

        public RunningModel.Block CreateBlock(ProgrammingModel.Block b)
        {
            Type type = b.Definition.Type;
            RunningModel.Block block = (RunningModel.Block)CreateInstance(type);
            block.Id = b.Id;

            foreach (Input i in b.Inputs)
            {
                BlockValue input = (BlockValue)Activator.CreateInstance(typeof(Input<>).MakeGenericType(i.Definition.Type));
                PropertyInfo pi = type.GetProperty(i.Definition.Name);
                input.Disabled = i.Disabled;
                block.Inputs.Add(input);
                if (pi != null) // If input is property
                {
                    pi.SetValue(block, input);   
                }
            }

            foreach (Output o in b.Outputs)
            {
                BlockValue output = (BlockValue)Activator.CreateInstance(typeof(Output<>).MakeGenericType(o.Definition.Type));
                PropertyInfo pi = type.GetProperty(o.Definition.Name);
                output.Disabled = o.Disabled;
                block.Outputs.Add(output);
                if (pi != null) // If output is property
                {
                    pi.SetValue(block, output);
                }
            }

            foreach (Parameter p in b.Parameters)
            {
                PropertyInfo po = type.GetProperty(p.Definition.Name);
                po.SetValue(block, p.Value);
            }

            return block;
        }

        public object GetService(Type type)
        {
            var globalService = serviceProvider.GetService(type);
            if (globalService != null) return globalService;
            if (localServices.TryGetValue(type, out object localService)) return localService;
            var newService = CreateInstance(type);
            localServices.Add(type, newService);
            return newService;
        }

        public void RemoveServices()
        {
            foreach(object service in localServices.Values)
            {
                if(service is IDisposable d)
                {
                    d.Dispose();
                }
            }
            localServices.Clear();
        }
    }
}
