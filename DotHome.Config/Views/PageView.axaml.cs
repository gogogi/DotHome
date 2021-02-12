using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using DotHome.Config.Tools;
using DotHome.Config.Windows;
using DotHome.RunningModel;
using DotHome.RunningModel.Tools;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DotHome.Config.Views
{
    public class PageView : UserControl
    {
        public Page Page => (Page)DataContext;

        public Project Project => (Project)this.ParentOfType<ProjectView>().DataContext;

        private Dictionary<Block, BlockView> blockViewDictionary = new Dictionary<Block, BlockView>();
        private Dictionary<Value, InputView> inputViewDictionary = new Dictionary<Value, InputView>();
        private Dictionary<Value, OutputView> outputViewDictionary = new Dictionary<Value, OutputView>();

        private ScrollViewer scrollViewer;
        private Canvas canvas;
        private Rectangle previewRectangle;
        private Line previewLine;

        private bool panning;
        private Point panningPoint;
        private bool dragging;
        private Point draggingPoint;
        private Point draggingOverflowDelta;
        private Point previewRectangleOrigin;
        private Value draggingInput;
        private Value draggingOutput;


        public PageView()
        {
            this.InitializeComponent();

            scrollViewer = this.FindControl<ScrollViewer>("scrollViewer");
            canvas = this.FindControl<Canvas>("canvas");
            previewRectangle = this.FindControl<Rectangle>("previewRectangle");
            previewLine = this.FindControl<Line>("previewLine");

            canvas.AddHandler(DragDrop.DropEvent, Canvas_Drop);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Wire_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Page.Wires.Remove((Wire)((WireView)sender).DataContext);
        }

        private void Panel_Wheel(object sender, PointerWheelEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(scrollViewer);
            if (!point.Properties.IsLeftButtonPressed && !point.Properties.IsMiddleButtonPressed && !point.Properties.IsRightButtonPressed)
            {
                Point pos = point.Position;
                Vector v = (scrollViewer.Offset + e.GetPosition(scrollViewer)) / Page.Scale;
                Page.Scale *= Math.Pow(1.1, e.Delta.Y);
                Page.Scale = Math.Min(Page.Scale, 10);
                Page.Scale = Math.Max(Page.Scale, 0.1);
                if (Page.Scale > 0.95 && Page.Scale < 1.05) Page.Scale = 1;
                scrollViewer.Offset = v * Page.Scale - pos;
            }
            e.Handled = true;
        }

        private void Panel_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Focus();
            var point = e.GetCurrentPoint(this);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
            {
                panning = true;
                panningPoint = point.Position;
            }
            e.Handled = true;
        }

        internal async void Paste()
        {
            try
            {
                var container = ContainerSerializer.TryDeserializeContainer(await Application.Current.Clipboard.GetTextAsync()).Copy();
                var offset = scrollViewer.Offset / Page.Scale;
                if(Page.Width < container.MaxX - container.MinX || Page.Height < container.MaxY - container.MinY)
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Paste", "Page is too small", ButtonEnum.Ok, Icon.Error).ShowDialog(ConfigTools.MainWindow);
                    return;
                }
                if (Page.Width - offset.X < container.MaxX - container.MinX) offset = offset.WithX(Page.Width - (container.MaxX - container.MinX));
                if (Page.Height - offset.Y < container.MaxY - container.MinY) offset = offset.WithY(Page.Height - (container.MaxY - container.MinY));
                Debug.WriteLine(offset);
                foreach (Block b in Page.Blocks)
                {
                    b.Selected = false;
                }
                foreach (Block b in container.Blocks)
                {
                    b.X += (int)(offset.X - container.MinX);
                    b.Y += (int)(offset.Y - container.MinY);
                    Page.Blocks.Add(b);
                    b.Selected = true;
                }
                foreach (Wire w in container.Wires)
                {
                    Page.Wires.Add(w);
                }
            }
            catch
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Paste", "Failed to paste", ButtonEnum.Ok, Icon.Error).ShowDialog(ConfigTools.MainWindow);
            }
        }

        public async void Copy()
        {
            await Application.Current.Clipboard.SetTextAsync(ContainerSerializer.SerializeContainer(CreateBlockContainer().Copy()));
        }

        public void Cut()
        {
            Copy();
            Delete();
        }

        public void Delete()
        {
            var blocksToRemove = Page.Blocks.Where(b => b.Selected).ToArray();
            foreach (var block in blocksToRemove) Page.Blocks.Remove(block);
        }


        public void SelectAll()
        {
            foreach (var b in Page.Blocks) b.Selected = true;
        }

        public async void Cancel()
        {
            foreach (var b in Page.Blocks) b.Selected = false;
            await Application.Current.Clipboard.ClearAsync();
        }

        public BlockContainer CreateBlockContainer()
        {
            double minX = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.X);
            double minY = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.Y);
            double maxX = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.X + blockViewDictionary[b].Width);
            double maxY = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.Y + blockViewDictionary[b].Height);
            BlockContainer blockContainer = new BlockContainer() { MinX = minX, MaxX = maxX, MinY = minY, MaxY = maxY };
            foreach (Block b in Page.Blocks)
            {
                if (b.Selected)
                {
                    blockContainer.Blocks.Add(b);
                }
            }
            foreach (Wire w in Page.Wires)
            {
                if (Page.Blocks.Any(b => b.Selected && b.Inputs.Contains(w.Input)) && Page.Blocks.Any(b => b.Selected && b.Outputs.Contains(w.Output)))
                {
                    blockContainer.Wires.Add(w);
                }
            }
            return blockContainer;
        }

        private void Panel_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (e.InitialPressMouseButton == MouseButton.Middle)
            {
                panning = false;
                e.Pointer.Capture(null);
            }
            e.Handled = true;
        }

        private void Panel_PointerMoved(object sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (panning && !point.Properties.IsLeftButtonPressed && !point.Properties.IsRightButtonPressed && point.Properties.IsMiddleButtonPressed)
            {

                var delta = point.Position - panningPoint;
                scrollViewer.Offset -= delta;
                panningPoint = point.Position;
            }
            e.Handled = true;
        }

        private void Block_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                Block block = ((BlockView)sender).Block;
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    block.Selected = !block.Selected;
                }
                else
                {
                    if (!block.Selected)
                    {
                        foreach (Block b in Page.Blocks)
                        {
                            b.Selected = false;
                        }
                        block.Selected = true;
                    }
                    dragging = true;
                    draggingPoint = point.Position;
                    draggingOverflowDelta = default;
                }
                e.Handled = true;
                Command.ForceChanges();
            }
        }

        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (dragging && point.Properties.IsLeftButtonPressed)
            {
                var delta = point.Position - draggingPoint + draggingOverflowDelta;
                double minX = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.X);
                double minY = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.Y);
                double maxX = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.X + blockViewDictionary[b].Width);
                double maxY = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.Y + blockViewDictionary[b].Height);
                draggingOverflowDelta = delta;
                if (minX + delta.X < 0) delta = delta.WithX(-minX);
                else if (maxX + delta.X > Page.Width) delta = delta.WithX(Page.Width - maxX);
                if (minY + delta.Y < 0) delta = delta.WithY(-minY);
                else if (maxY + delta.Y > Page.Height) delta = delta.WithY(Page.Height - maxY);
                delta = new Point(Math.Round(delta.X), Math.Round(delta.Y));
                draggingOverflowDelta = draggingOverflowDelta - delta;
                foreach (Block b in Page.Blocks)
                {
                    if (b.Selected)
                    {
                        b.X += (int)delta.X;
                        b.Y += (int)delta.Y;
                    }
                }
                draggingPoint = point.Position;
            }
            else if (previewRectangle.IsVisible)
            {
                Canvas.SetLeft(previewRectangle, Math.Round(Math.Min(previewRectangleOrigin.X, point.Position.X)));
                Canvas.SetTop(previewRectangle, Math.Round(Math.Min(previewRectangleOrigin.Y, point.Position.Y)));
                previewRectangle.Width = Math.Abs(Math.Round(previewRectangleOrigin.X) - Math.Round(point.Position.X));
                previewRectangle.Height = Math.Abs(Math.Round(previewRectangleOrigin.Y) - Math.Round(point.Position.Y));
            }
            else if (previewLine.IsVisible)
            {
                previewLine.EndPoint = point.Position;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            dragging = false;
            if (previewRectangle.IsVisible)
            {
                foreach (Block b in Page.Blocks)
                {
                    b.Selected = (b.X >= Canvas.GetLeft(previewRectangle) && b.Y >= Canvas.GetTop(previewRectangle)
                        && b.X + blockViewDictionary[b].Width <= Canvas.GetLeft(previewRectangle) + previewRectangle.Width
                        && b.Y + blockViewDictionary[b].Height <= Canvas.GetTop(previewRectangle) + previewRectangle.Height);
                }
                previewRectangle.IsVisible = false;
                Command.ForceChanges();
            }
            previewLine.IsVisible = false;
        }

        private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                foreach (Block b in Page.Blocks)
                {
                    b.Selected = false;
                }
                previewRectangleOrigin = point.Position;
                previewRectangle.Width = 0;
                previewRectangle.Height = 0;
                Canvas.SetLeft(previewRectangle, Math.Round(point.Position.X));
                Canvas.SetTop(previewRectangle, Math.Round(point.Position.Y));
                previewRectangle.IsVisible = true;
                e.Handled = true;
                Command.ForceChanges();
            }
        }

        private void Block_InputPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (previewLine.IsVisible && draggingOutput != null && point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                InputView inputView = (InputView)sender;
                if (ModelTools.IsConversionSupported(draggingOutput.Type, inputView.Input.Type))
                    Page.AddWire(new Wire() { Input = inputView.Input, Output = draggingOutput });
                else
                    MessageBoxManager.GetMessageBoxStandardWindow("Create wire", "Cannot connect this types", ButtonEnum.Ok, Icon.Error);
            }
        }

        private void Block_InputPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                InputView inputView = (InputView)sender;
                draggingInput = inputView.Input;
                previewLine.StartPoint = previewLine.EndPoint = inputView.Position;
                previewLine.IsVisible = true;
                point.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void Block_OutputPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (previewLine.IsVisible && draggingInput != null && point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                OutputView outputView = (OutputView)sender;
                if (ModelTools.IsConversionSupported(outputView.Output.Type, draggingInput.Type))
                    Page.AddWire(new Wire() { Input = draggingInput, Output = outputView.Output });
                else
                    MessageBoxManager.GetMessageBoxStandardWindow("Create wire", "Cannot connect this types", ButtonEnum.Ok, Icon.Error);
            }
        }

        private void Block_OutputPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                OutputView outputView = (OutputView)sender;
                draggingOutput = outputView.Output;
                previewLine.StartPoint = previewLine.EndPoint = outputView.Position;
                previewLine.IsVisible = true;
                point.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void PageView_KeyDown(object sender, KeyEventArgs e)
        {
            Vector delta = default;
            switch (e.Key)
            {
                case Key.Left:
                    delta = -Vector.UnitX;
                    break;
                case Key.Right:
                    delta = Vector.UnitX;
                    break;
                case Key.Up:
                    delta = -Vector.UnitY;
                    break;
                case Key.Down:
                    delta = Vector.UnitY;
                    break;
                case Key.Delete:
                    Delete();
                    break;
                case Key.Escape:
                    Cancel();
                    break;
                case Key.A:
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) SelectAll();
                    break;
                case Key.C:
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) Copy();
                    break;
                case Key.X:
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) Cut();
                    break;
                case Key.V:
                    if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) Paste();
                    break;
            }
            if(delta != default)
            {
                delta *= e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
                double minX = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.X);
                double minY = Page.Blocks.Where(b => b.Selected).MinOrDefault(b => b.Y);
                double maxX = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.X + blockViewDictionary[b].Width);
                double maxY = Page.Blocks.Where(b => b.Selected).MaxOrDefault(b => b.Y + blockViewDictionary[b].Height);
                if (minX + delta.X < 0) delta = delta.WithX(-minX);
                else if (maxX + delta.X > Width) delta = delta.WithX(Width - maxX);
                if (minY + delta.Y < 0) delta = delta.WithY(-minY);
                else if (maxY + delta.Y > Height) delta = delta.WithY(Height - maxY);
                foreach (Block b in Page.Blocks)
                {
                    if (b.Selected)
                    {
                        b.X += (int)delta.X;
                        b.Y += (int)delta.Y;
                    }
                }
            }
            Command.ForceChanges();
            e.Handled = true;
        }

        private async void Canvas_Drop(object sender, DragEventArgs e)
        {
            var point = e.GetPosition(canvas);
            var data = e.Data.Get("add_block");
            foreach (Block b in Page.Blocks) b.Selected = false;
            if (data is Block b)
            {
                //if(b.GetType is GenericBlockDefinition gbd)
                //{
                //    Type type = await new SelectionWindow("Select type for which you want to create generic block", ModelTools.SupportedTypes).ShowDialog<Type>(ConfigTools.MainWindow);
                //    if(type != null)
                //    {
                //        Page.Blocks.Add(new Block(gbd.GetParticularBlockDefinition(type)) { X = (int)point.X, Y = (int)point.Y, Selected = true });
                //    }
                //}
                //else
                {
                    Page.Blocks.Add(new Block(bd) { X = (int)point.X, Y = (int)point.Y, Selected = true });
                }
            }

            Command.ForceChanges();
        }

        private void Block_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            BlockView aBlockView = (BlockView)sender;
            blockViewDictionary.Add(aBlockView.Block, aBlockView);
        }

        private void Block_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            BlockView aBlockView = (BlockView)sender;
            blockViewDictionary.Remove(aBlockView.Block);
        }

        private void Block_InputAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            InputView inputView = (InputView)sender;
            inputViewDictionary.Add(inputView.Input, inputView);
        }

        private void Block_InputDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            InputView inputView = (InputView)sender;
            inputViewDictionary.Remove(inputView.Input);
        }

        private void Block_OutputAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            OutputView outputView = (OutputView)sender;
            outputViewDictionary.Add(outputView.Output, outputView);
        }

        private void Block_OutputDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            OutputView outputView = (OutputView)sender;
            outputViewDictionary.Remove(outputView.Output);
            
        }

        private void Wire_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            WireView wireView = (WireView)sender;
            Wire wire = wireView.Wire;
            InputView inputView = inputViewDictionary[wire.Input];
            OutputView outputView = outputViewDictionary[wire.Output];

            var inputBlock = Page.Blocks.Single(b => b.Inputs.Contains(wire.Input));
            var outputBlock = Page.Blocks.Single(b => b.Outputs.Contains(wire.Output));


            wireView.Bind(WireView.PointsProperty, new MultiBinding()
            {
                Bindings =
                {
                    new Binding("TransformedBounds") { Source = inputView },
                    new Binding("TransformedBounds") { Source = outputView },
                },
                Converter = WirePointsConverter.Instance,
                ConverterParameter = new { InputView = inputView, OutputView = outputView }
            });

            wireView.Stroke = inputView.Input.Definition.Type == outputView.Output.Definition.Type ? Brushes.Black : Brushes.DarkRed;
        }

        private void Wire_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            
        }

        private void PageView_LostFocus(object sender, RoutedEventArgs e)
        {
            Command.ForceChanges();
        }

        private void PageView_GotFocus(object sender, GotFocusEventArgs e)
        {
            Command.ForceChanges();
        }
    }
}
