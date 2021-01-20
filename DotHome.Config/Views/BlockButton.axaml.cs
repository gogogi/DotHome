using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;

namespace DotHome.Config.Views
{
    public class BlockButton : Button, IStyleable
    {
        Type IStyleable.StyleKey => typeof(Button);

        public BlockButton()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            DataObject dataObject = new DataObject();
            dataObject.Set("add_block", DataContext);
            DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
        }
    }
}
