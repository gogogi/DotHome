using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotHome.Config.Views
{
    public class ABlockButton : Button, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Button);

        private ABlockView blockView;

        public ABlockButton(ABlockView blockView)
        {
            this.blockView = blockView;

            blockView.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            blockView.IsEnabled = false;

            string title = null;
            if (blockView is RefSinkView) title = "RefSink";
            else if (blockView is RefSourceView) title = "RefSource";
            else if (blockView is ConstView) title = "Const";
            else if (blockView is BlockView bw)
            {
                title = ((Block)bw.Block).Definition.Name;
                ToolTip.SetTip(this, ((Block)bw.Block).Definition.Description);
            }


            var stackPanel = new StackPanel();
            stackPanel.Children.Add(blockView);
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
            dataObject.Set("add_block", blockView.DataContext);
            DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
        }
    }
}
