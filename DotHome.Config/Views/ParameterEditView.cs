using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using DotHome.ProgrammingModel;
using System;
using System.Diagnostics;

namespace DotHome.Config.Views
{
    public class ParameterEditView : Panel
    {
        public Parameter Parameter => (Parameter)DataContext;

        protected override void OnDataContextChanged(EventArgs e)
        {
            Debug.WriteLine("DCC " + Parameter.Definition.Type);
            if (Parameter.Definition.Type == typeof(bool)) CreateBool();
            else if (Parameter.Definition.Type == typeof(int)) CreateInt();
            else if (Parameter.Definition.Type == typeof(string)) CreateString();
        }

        private void CreateBool()
        {
            CheckBox checkBox = new CheckBox() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
            checkBox.Bind(CheckBox.IsCheckedProperty, new Binding("Value", BindingMode.TwoWay));
            Children.Clear();
            Children.Add(checkBox);
        }

        private void CreateInt()
        {
            NumericUpDown numericUpDown = new NumericUpDown() { BorderThickness = new Thickness(0), Padding = new Thickness(0), Background = Brushes.Transparent };
            numericUpDown.ValueChanged += (s, e) => numericUpDown.Value = (int)Math.Round(numericUpDown.Value);
            numericUpDown.Bind(NumericUpDown.ValueProperty, new Binding("Value", BindingMode.TwoWay));
            Children.Clear();
            Children.Add(numericUpDown);
        }

        private void CreateString()
        {
            TextBox textBox = new TextBox() { BorderThickness = new Thickness(0), Background = Brushes.Transparent };
            textBox.Bind(TextBox.TextProperty, new Binding("Value", BindingMode.TwoWay));
            Children.Clear();
            Children.Add(textBox);
        }
    }
}
