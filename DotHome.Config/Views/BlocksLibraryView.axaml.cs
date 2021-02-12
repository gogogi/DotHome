using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotHome.Config.Views
{
    public class BlocksLibraryView : UserControl
    {
        private List<Group> Groups { get; }

        public BlocksLibraryView()
        {
            this.InitializeComponent();
            Groups = CreateGroups();
            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private static List<Group> CreateGroups()
        {
            List<Group> groups = new List<Group>();
            foreach(string dll in Directory.GetFiles(AppConfig.Configuration["AssembliesPath"], "*.dll"))
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(dll));

                foreach(Type type in assembly.GetTypes())
                {
                    if(!type.IsAbstract && typeof(Block).IsAssignableFrom(type))
                    {
                        string categoryName = type.GetCustomAttribute<CategoryAttribute>()?.Category ?? "Default";
                        Group group = groups.SingleOrDefault(g => g.Name == categoryName);
                        if(group == null)
                        {
                            group = new Group() { Name = categoryName };
                            groups.Add(group);
                        }
                        if(type.GetConstructors().Count() == 1 && type.GetConstructors().Single().GetParameters().All(pi => typeof(IBlockService).IsAssignableFrom(pi.ParameterType)))
                        {
                            Type type2 = type;
                            if(type.IsGenericType)
                            {
                                type2 = type.MakeGenericType(typeof(object));
                            }
                            Block block = (Block)Activator.CreateInstance(type2, type2.GetConstructors().Single().GetParameters().Select<ParameterInfo, object>(pi => null).ToArray());
                            group.Blocks.Add(block);
                        }
                    }
                }
            }
            return groups;
        }

        private class Group
        {
            public string Name { get; set; }

            public List<Block> Blocks { get; } = new List<Block>();
        }
    }
}
