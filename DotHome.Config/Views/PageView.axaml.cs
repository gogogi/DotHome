﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using DotHome.Config.Tools;
using DotHome.ProgrammingModel;
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
    public class PageView : TabItem, IStyleable
    {
        Type IStyleable.StyleKey => typeof(TabItem);

        private TextBlock textBlockHeader;
        private ScrollViewer scrollViewer;
        private Panel panel;
        private LayoutTransformControl layoutTransformControl;
        private Canvas canvas;
        private Rectangle previewRectangle;
        private Line previewLine;
        private Canvas blocksCanvas;
        private Panel wiresPanel;

        private bool panning;
        private Point panningPoint;
        private bool dragging;
        private Point draggingPoint;
        private Point draggingOverflowDelta;
        private Point previewRectangleOrigin;
        private InputView draggingInput;
        private OutputView draggingOutput;
        private bool shouldUpdateWires;
        private bool shouldClose;

        public bool Visible { get => IsVisible; set { IsVisible = value; Command.ForceChanges(); scrollViewer.IsVisible = value; } }
        public new string Name { get => textBlockHeader.Text; set => textBlockHeader.Text = value; }
        public double Scale { get => layoutTransformControl.LayoutTransform?.Value.M11 ?? 1; set => layoutTransformControl.LayoutTransform = new ScaleTransform(value, value); }
        public new int Width { get => (int)canvas.Width; set => canvas.Width = value; }
        public new int Height { get => (int)canvas.Height; set => canvas.Height = value; }

        public IEnumerable<ABlockView> Blocks => blocksCanvas.Children.Cast<ABlockView>();

        public IEnumerable<WireView> Wires => wiresPanel.Children.Cast<WireView>();

        public PageView()
        {
            this.InitializeComponent();

            textBlockHeader = this.FindControl<TextBlock>("textBlockHeader");
            scrollViewer = this.FindControl<ScrollViewer>("scrollViewer");
            panel = this.FindControl<Panel>("panel1");
            layoutTransformControl = this.FindControl<LayoutTransformControl>("layoutTransformControl");
            canvas = this.FindControl<Canvas>("canvas");
            previewRectangle = this.FindControl<Rectangle>("previewRectangle");
            previewLine = this.FindControl<Line>("previewLine");
            blocksCanvas = this.FindControl<Canvas>("blocksCanvas");
            wiresPanel = this.FindControl<Panel>("wiresPanel");

            for(int i = 0; i < 100; i++)
            {
                if (i % 2 == 0)
                    AddBlock(new RefSinkView() { Reference = "Ahoj", X = 40 * (i % 20), Y = 25 * (i / 20) });
                else
                    AddBlock(new RefSourceView() { Reference = "Ahoj", X = 40 * (i % 20), Y = 25 * (i / 20) });
            }
            canvas.AddHandler(DragDrop.DropEvent, Canvas_Drop);
        }

        internal void Delete()
        {
            var blocksToRemove = Blocks.Where(b => b.Selected).ToArray();
            foreach (var block in blocksToRemove) RemoveBlock(block);
        }

        internal async void Paste()
        {
            try
            {
                var container = ContainerSerializer.DeserializeProject(await Application.Current.Clipboard.GetTextAsync()).ToBlockViewContainer();
                foreach(ABlockView block in Blocks)
                {
                    block.Selected = false;
                }
                foreach(ABlockView block in container.Blocks)
                {
                    AddBlock(block);
                    block.Selected = true;
                }
                foreach(WireView wireView in container.Wires)
                {
                    AddWire(wireView);
                }
            }
            catch
            {

            }
        }
        
        internal async void Copy()
        {
            await Application.Current.Clipboard.SetTextAsync(ContainerSerializer.SerializeContainer(CreateBlockViewContainer().ToBlockContainer()));
        }

        internal void Cut()
        {
            Copy();
            Delete();
        }

        internal void SelectAll()
        {
            foreach (var block in Blocks) block.Selected = true;
        }

        internal void Cancel()
        {
            foreach (var block in Blocks) block.Selected = false;
        }
        
        private BlockViewContainer CreateBlockViewContainer()
        {
            BlockViewContainer blockViewContainer = new BlockViewContainer();
            foreach(ABlockView block in Blocks)
            {
                if(block.Selected)
                {
                    blockViewContainer.Blocks.Add(block);
                }
            }
            foreach(WireView wire in Wires)
            {
                if(Blocks.Any(b => b.Selected && b.Inputs.Contains(wire.InputView)) && Blocks.Any(b => b.Selected && b.Outputs.Contains(wire.OutputView)))
                {
                    blockViewContainer.Wires.Add(wire);
                }
            }
            return blockViewContainer;
        }

        private void Header_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed) shouldClose = true;
        }

        private void Header_PointerMoved(object sender, PointerEventArgs e)
        {
            shouldClose = false;
        }
                
        private void Header_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if(shouldClose)
            {
                Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            TabControl tabControl = (TabControl)Parent;
            if (tabControl.SelectedItem == this)
            {
                int index = tabControl.SelectedIndex;
                if (index > 0) tabControl.SelectedIndex = index - 1;
                else tabControl.SelectedIndex = 0;
            }
            Visible = false;
        }

        public void AddBlock(ABlockView block)
        {
            blocksCanvas.Children.Add(block);
            block.PointerPressed += Block_PointerPressed;
            foreach (InputView inputView in block.Inputs)
            {
                inputView.PointerPressed += Input_PointerPressed;
                inputView.PointerReleased += Input_PointerReleased;
            }
            foreach (OutputView outputView in block.Outputs)
            {
                outputView.PointerPressed += Output_PointerPressed;
                outputView.PointerReleased += Output_PointerReleased;
            }

            block.LayoutUpdated += Block_LayoutUpdated;
        }

        public void RemoveBlock(ABlockView block)
        {
            blocksCanvas.Children.Remove(block);
            wiresPanel.Children.RemoveAll(wiresPanel.Children.Cast<WireView>().Where(ww => block.Inputs.Contains(ww.InputView) || block.Outputs.Contains(ww.OutputView)).ToList());
            Command.ForceChanges();
        }

        private void Block_LayoutUpdated(object sender, EventArgs e)
        {
            var block = (ABlockView)sender;
            block.LayoutUpdated -= Block_LayoutUpdated;
            if (block.X + block.Width > Width) block.X = Width - block.Width;
            if (block.Y + block.Height > Height) block.Y = Height - block.Height;
        }
                
        public void AddWire(WireView wire)
        {
            wiresPanel.Children.Add(wire);
            wire.PointerPressed += Wire_PointerPressed;
        }

        public void RemoveWire(WireView wire)
        {
            wiresPanel.Children.Remove(wire);
        }

        private void Wire_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            RemoveWire((WireView)sender);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Panel_Wheel(object sender, PointerWheelEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(scrollViewer);
            if (!point.Properties.IsLeftButtonPressed && !point.Properties.IsMiddleButtonPressed && !point.Properties.IsRightButtonPressed)
            {
                Point pos = point.Position;
                Vector v = (scrollViewer.Offset + e.GetPosition(scrollViewer)) / Scale;
                Scale *= Math.Pow(1.1, e.Delta.Y);
                Scale = Math.Min(Scale, 10);
                Scale = Math.Max(Scale, 0.1);
                if (Scale > 0.95 && Scale < 1.05) Scale = 1;
                scrollViewer.Offset = v * Scale - pos;
            }
            e.Handled = true;
        }

        private void Panel_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
            {
                panning = true;
                panningPoint = point.Position;
                e.Pointer.Capture(panel);
            }
            e.Handled = true;
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
                ABlockView block = (ABlockView)sender;
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    block.Selected = !block.Selected;
                }
                else
                {
                    if (!block.Selected)
                    {
                        foreach (ABlockView b in blocksCanvas.Children)
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
            }
        }

        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (dragging && point.Properties.IsLeftButtonPressed)
            {
                var delta = point.Position - draggingPoint + draggingOverflowDelta;
                double minX = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MinOrDefault(b => b.X);
                double minY = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MinOrDefault(b => b.Y);
                double maxX = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MaxOrDefault(b => b.X + b.Width);
                double maxY = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MaxOrDefault(b => b.Y + b.Height);
                draggingOverflowDelta = delta;
                if (minX + delta.X < 0) delta = delta.WithX(-minX);
                else if (maxX + delta.X > Width) delta = delta.WithX(Width - maxX);
                if (minY + delta.Y < 0) delta = delta.WithY(-minY);
                else if (maxY + delta.Y > Height) delta = delta.WithY(Height - maxY);
                draggingOverflowDelta = draggingOverflowDelta - delta;
                foreach (ABlockView b in blocksCanvas.Children)
                {
                    if(b.Selected)
                    {
                        b.X += delta.X;
                        b.Y += delta.Y;
                    }
                }
                ForceUpdateWires();
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
            if (dragging)
            {
                foreach (ABlockView b in blocksCanvas.Children)
                {
                    b.X = Math.Round(b.X);
                    b.Y = Math.Round(b.Y);
                }
                dragging = false;
            }
            if (previewRectangle.IsVisible)
            {
                foreach (ABlockView b in blocksCanvas.Children)
                {
                    b.Selected = (b.X >= Canvas.GetLeft(previewRectangle) && b.Y >= Canvas.GetTop(previewRectangle)
                        && b.X + b.Width <= Canvas.GetLeft(previewRectangle) + previewRectangle.Width
                        && b.Y + b.Height <= Canvas.GetTop(previewRectangle) + previewRectangle.Height);
                }
                previewRectangle.IsVisible = false;
            }
            previewLine.IsVisible = false;
        }

        private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                foreach (ABlockView b in blocksCanvas.Children)
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
            }
        }

        private void Input_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (previewLine.IsVisible && draggingOutput != null && point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                AddWire(new WireView((InputView)sender, draggingOutput));
            }
        }

        private void Input_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                draggingInput = (InputView)sender;
                previewLine.StartPoint = previewLine.EndPoint = draggingInput.Position;
                previewLine.IsVisible = true;
                point.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void Output_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (previewLine.IsVisible && draggingInput != null && point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                AddWire(new WireView(draggingInput, (OutputView)sender));
            }
        }

        private void Output_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                draggingOutput = (OutputView)sender;
                previewLine.StartPoint = previewLine.EndPoint = draggingOutput.Position;
                previewLine.IsVisible = true;
                point.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void ForceUpdateWires()
        {
            shouldUpdateWires = true;
        }

        private void PageView_LayoutUpdated(object sender, EventArgs e)
        {
            if(shouldUpdateWires)
            {
                UpdateWires();
                shouldUpdateWires = false;
            }
        }

        private void UpdateWires()
        {
            foreach(WireView wireView in wiresPanel.Children)
            {
                wireView.UpdatePoints();
            }
        }

        private void PageView_KeyDown(object sender, KeyEventArgs e)
        {
            Vector delta = default;
            switch(e.Key)
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
            }
            delta *= e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1;
            double minX = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MinOrDefault(b => b.X);
            double minY = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MinOrDefault(b => b.Y);
            double maxX = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MaxOrDefault(b => b.X + b.Width);
            double maxY = blocksCanvas.Children.Cast<ABlockView>().Where(b => b.Selected).MaxOrDefault(b => b.Y + b.Height);
            if (minX + delta.X < 0) delta = delta.WithX(-minX);
            else if (maxX + delta.X > Width) delta = delta.WithX(Width - maxX);
            if (minY + delta.Y < 0) delta = delta.WithY(-minY);
            else if (maxY + delta.Y > Height) delta = delta.WithY(Height - maxY);
            foreach (ABlockView b in blocksCanvas.Children)
            {
                if (b.Selected)
                {
                    b.X += delta.X;
                    b.Y += delta.Y;
                }
            }
            e.Handled = true;
            ForceUpdateWires();
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            var point = e.GetPosition(canvas);
            var data = e.Data.Get("add_block");
            if (data is RefSinkView) AddBlock(new RefSinkView() { Reference = "Ref", X = point.X, Y = point.Y });
            else if (data is RefSourceView) AddBlock(new RefSourceView() { Reference = "Ref", X = point.X, Y = point.Y });
            else if (data is ConstView) AddBlock(new ConstView() { Type = typeof(int), Value = 0, X = point.X, Y = point.Y });
            else if (data is BlockView bw) AddBlock(new BlockView(bw.BlockDefinition) { X = point.X, Y = point.Y });
        }
    }
}
