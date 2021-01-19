using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;

namespace DotHome.Config.Views
{
    public class RefSourceView : ABlockView
    {
        public event EventHandler<PointerPressedEventArgs> OutputPointerPressed;
        public event EventHandler<PointerReleasedEventArgs> OutputPointerReleased;
        public event EventHandler<VisualTreeAttachmentEventArgs> OutputAttachedToVisualTree;
        public event EventHandler<VisualTreeAttachmentEventArgs> OutputDetachedFromVisualTree;

        public RefSourceView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Output_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            OutputPointerPressed?.Invoke(sender, e);
        }

        private void Output_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            OutputPointerReleased?.Invoke(sender, e);
        }

        private void Output_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            OutputDetachedFromVisualTree?.Invoke(sender, e);
        }

        private void Output_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            OutputAttachedToVisualTree?.Invoke(sender, e);
        }
    }
}
