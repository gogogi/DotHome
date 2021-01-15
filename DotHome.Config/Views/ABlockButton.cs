using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Views
{
    public class ABlockButton : Button, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Button);

        private ABlockView block;

        public ABlockButton(ABlockView block)
        {
            this.block = block;

            block.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            block.IsEnabled = false;

            string title = null;
            if (block is RefSinkView) title = "RefSink";
            else if (block is RefSourceView) title = "RefSource";
            else if (block is ConstView) title = "Const";
            else if (block is BlockView bw)
            {
                title = bw.BlockDefinition.Name;
                ToolTip.SetTip(this, bw.BlockDefinition.Description);
            }


            var stackPanel = new StackPanel();
            stackPanel.Children.Add(block);
            stackPanel.Children.Add(new TextBlock() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Text = title });
            Content = stackPanel;
            MinWidth = 60;
            Margin = new Thickness(5);
            Padding = new Thickness(5);
        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            DataObject dataObject = new DataObject();
            dataObject.Set("add_block", block);
            DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
        }
    }
}
