using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using DotHome.Config.Tools;
using DotHome.ProgrammingModel;
using DotHome.RunningModel.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace DotHome.Config.Views
{
    public class ParameterEditView : UserControl
    {
        private object lastValid;

        public Parameter Parameter => (Parameter)DataContext;

        protected override void OnDataContextChanged(EventArgs e)
        {
            lastValid = Parameter.Value;
            string path = nameof(Parameter.Value);
            if (Parameter.Definition.Type == typeof(bool)) { CreateBool(); return; }
            else if (Parameter.Definition.Type == typeof(int)) path = nameof(Parameter.ValueAsInt);
            else if (Parameter.Definition.Type == typeof(uint)) path = nameof(Parameter.ValueAsUint);
            else if (Parameter.Definition.Type == typeof(double)) path = nameof(Parameter.ValueAsDouble);
            else if (Parameter.Definition.Type == typeof(string)) path = nameof(Parameter.ValueAsString);
            CreateString(path);
        }

        private void CreateBool()
        {
            CheckBox checkBox = new CheckBox() { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center };
            checkBox.Bind(CheckBox.IsCheckedProperty, new Binding(nameof(Parameter.Value), BindingMode.TwoWay));
            Content = checkBox;
        }

        private void CreateString(string path)
        {
            TextBox textBox = new TextBox() { BorderThickness = new Thickness(0), Background = Brushes.Transparent };
            textBox.Bind(TextBox.TextProperty, new Binding(path, BindingMode.TwoWay));
            textBox.PropertyChanged += (s, e) =>
            {
                if (e.Property == TextBox.TextProperty)
                {
                    Validate(textBox, Parameter.Value);
                }
            };
            textBox.LostFocus += (s, e) =>
            {
                if (!Validate(textBox, Parameter.Value)) Parameter.Value = lastValid;
            };
            Content = textBox;
            Validate(textBox, Parameter.Value);
        }

        private bool ExistsOtherParameterWithSameValue(object value)
        {
            return ConfigTools.MainWindow.Project.Pages.Any(
                p => p.Blocks.Any(
                    b => b.Parameters.Any(
                        p => p != Parameter 
                            && p.Definition.PropertyInfo.Name == Parameter.Definition.PropertyInfo.Name
                            && p.Definition.PropertyInfo.DeclaringType == Parameter.Definition.PropertyInfo.DeclaringType
                            && Equals(p.Value,value))));
        }

        private bool Validate(Control control, object value)
        {
            foreach (ValidationAttribute validationAttribute in Parameter.Definition.ValidationAttributes)
            {
                try
                {
                    if (validationAttribute is UniqueAttribute)
                    {
                        if (ExistsOtherParameterWithSameValue(value))
                        {
                            DataValidationErrors.SetErrors(control, new[] { "Parameter must be unique" });
                            return false;
                        }
                    }
                    else if (!validationAttribute.IsValid(value))
                    {
                        DataValidationErrors.SetErrors(control, new[] { validationAttribute.FormatErrorMessage(Parameter.Definition.Name) });
                        return false;
                    }
                }
                catch
                {
                    DataValidationErrors.SetErrors(control, new[] { "Some very ugly error happened" });
                    return false;
                }
            }
            lastValid = value;
            return true;
        }
    }
}
