using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions.Tools;

namespace DotHome.Config.Views
{
    public class BlocksLibraryView : UserControl
    {
        private WrapPanel wrapPanelBuiltin;
        private StackPanel stackPanelCategories;

        public BlocksLibraryView()
        {
            this.InitializeComponent();

            wrapPanelBuiltin = this.FindControl<WrapPanel>("wrapPanelBuiltin");
            stackPanelCategories = this.FindControl<StackPanel>("stackPanelCategories");

            wrapPanelBuiltin.Children.Add(new ABlockButton(new RefSinkView() { Reference = "Ref" }));
            wrapPanelBuiltin.Children.Add(new ABlockButton(new RefSourceView() { Reference = "Ref" }));
            wrapPanelBuiltin.Children.Add(new ABlockButton(new ConstView() { Type = typeof(int), Value = 0 }));

            string path = @"C:\Users\Vojta\Desktop\Bakalarka\Assemblies";
            if(System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                path = "/mnt/shared/Assemblies";
            }
            var definitions = DefinitionsCreator.CreateDefinitions(path);

            foreach(var category in definitions.BlockCategories)
            {
                Expander expander = new Expander() { Header = category.Name };
                WrapPanel wrapPanel = new WrapPanel();
                expander.Content = wrapPanel;
                foreach(var block in category.BlockDefinitions)
                {
                    wrapPanel.Children.Add(new ABlockButton(new BlockView(block)));
                }
                stackPanelCategories.Children.Add(expander);
            }

            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
