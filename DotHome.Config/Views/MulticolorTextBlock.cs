using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotHome.Config.Views
{
    public class MulticolorTextBlock : StackPanel
    {
        public static AvaloniaProperty<string> TextProperty = AvaloniaProperty.Register<MulticolorTextBlock, string>(nameof(Text));
        public static AvaloniaProperty<int> FontSizeProperty = AvaloniaProperty.Register<MulticolorTextBlock, int>(nameof(FontSize));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public int FontSize { get => (int)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if(change.Property == TextProperty)
            {
                Children.Clear();
                string[] lines = Text.Split("\n");
                Color color = Colors.Black;
                foreach(string line in lines)
                {
                    StackPanel stackPanel = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Horizontal };
                    Children.Add(stackPanel);
                    string[] parts = line.Split("\u001bc");
                    for(int i = 0; i < parts.Length; i++)
                    {
                        if(i == 0)
                        {
                            stackPanel.Children.Add(new TextBlock() { Text = parts[i], Foreground = new SolidColorBrush(color), FontSize = FontSize });
                        }
                        else if (parts[i].Length > 3)
                        {
                            byte r = (byte)parts[i][0];
                            byte g = (byte)parts[i][1];
                            byte b = (byte)parts[i][2];

                            color = Color.FromArgb(255, r, g, b);
                            stackPanel.Children.Add(new TextBlock() { Text = parts[i].Substring(3), Foreground = new SolidColorBrush(color), FontSize = FontSize });
                        }
                    }
                }
            }
        }
    }
}
