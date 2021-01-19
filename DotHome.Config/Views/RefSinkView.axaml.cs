using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DotHome.Config.Tools;
using DotHome.ProgrammingModel;
using System;
using System.Diagnostics;
using System.Globalization;

namespace DotHome.Config.Views
{
    public class RefSinkView : ABlockView
    {
        public event EventHandler<PointerPressedEventArgs> InputPointerPressed;
        public event EventHandler<PointerReleasedEventArgs> InputPointerReleased;
        public event EventHandler<VisualTreeAttachmentEventArgs> InputAttachedToVisualTree;
        public event EventHandler<VisualTreeAttachmentEventArgs> InputDetachedFromVisualTree;

        public RefSinkView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Input_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            InputPointerPressed?.Invoke(sender, e);
        }

        private void Input_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            InputPointerReleased?.Invoke(sender, e);
        }

        private void Input_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            InputDetachedFromVisualTree?.Invoke(sender, e);
        }

        private void Input_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            InputAttachedToVisualTree?.Invoke(sender, e);
        }
    }    
}
