using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DotHome.Config.Views
{
    public class BlockView : Border
    {
        private static Cursor cursor = new Cursor(StandardCursorType.SizeAll);

        public event EventHandler<PointerPressedEventArgs> InputPointerPressed;
        public event EventHandler<PointerReleasedEventArgs> InputPointerReleased;
        public event EventHandler<VisualTreeAttachmentEventArgs> InputAttachedToVisualTree;
        public event EventHandler<VisualTreeAttachmentEventArgs> InputDetachedFromVisualTree;
        public event EventHandler<PointerPressedEventArgs> OutputPointerPressed;
        public event EventHandler<PointerReleasedEventArgs> OutputPointerReleased;
        public event EventHandler<VisualTreeAttachmentEventArgs> OutputAttachedToVisualTree;
        public event EventHandler<VisualTreeAttachmentEventArgs> OutputDetachedFromVisualTree;

        public Block Block => (Block)DataContext;
        public new double Width => Bounds.Width;
        public new double Height => Bounds.Height;
                
        public BlockView()
        {
            this.InitializeComponent();

            Cursor = cursor;
            Padding = new Thickness(2);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Parent.Bind(Canvas.LeftProperty, new Binding("X"));
            Parent.Bind(Canvas.TopProperty, new Binding("Y"));
            Parent.Bind(ZIndexProperty, new Binding("Selected") { Converter = SelectedToZIndexConverter.Instance });
            this.Bind(BackgroundProperty, new Binding("Selected") { Converter = SelectedToBackgroundConverter.Instance });
        }

        private void Input_PointerPressed(object sender, PointerPressedEventArgs e) => InputPointerPressed?.Invoke(sender, e);
        private void Input_PointerReleased(object sender, PointerReleasedEventArgs e) => InputPointerReleased?.Invoke(sender, e);
        private void Input_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e) => InputDetachedFromVisualTree?.Invoke(sender, e);
        private void Input_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) => InputAttachedToVisualTree?.Invoke(sender, e);
        private void Output_PointerPressed(object sender, PointerPressedEventArgs e) => OutputPointerPressed?.Invoke(sender, e);
        private void Output_PointerReleased(object sender, PointerReleasedEventArgs e) => OutputPointerReleased?.Invoke(sender, e);
        private void Output_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e) => OutputDetachedFromVisualTree?.Invoke(sender, e);
        private void Output_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) => OutputAttachedToVisualTree?.Invoke(sender, e);
    }
}
