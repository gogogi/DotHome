using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using DotHome.Config.Tools;
using DotHome.Config.Windows;
using DotHome.ProgrammingModel;
using DotHome.RunningModel;
using DotHome.RunningModel.Attributes;
using DotHome.RunningModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace DotHome.Config.Views
{
    public class ParameterEditView : UserControl
    {
        private object lastValid;

        private Parameter Parameter => (Parameter)DataContext;

        private ProgrammingModel.Block Block => (ProgrammingModel.Block)this.ParentOfType<SelectionPropertiesView>().DataContext;


        protected override void OnDataContextChanged(EventArgs e)
        {
            lastValid = Parameter.Value;
            if (Parameter.Definition.Type == typeof(bool)) CreateBool();
            else if (Parameter.Definition.Type == typeof(List<User>)) CreateButton(async () => await new UsersWindow((List<User>)Parameter.Value).ShowDialog(ConfigTools.MainWindow));
            else if (Parameter.Definition.Type == typeof(List<DeviceValue>)) CreateButton(async () => await new GenericDeviceWindow(Block).ShowDialog(ConfigTools.MainWindow));
            else if (Parameter.Definition.Type == typeof(Room)) CreateComboBox(ConfigTools.MainWindow.Project.Rooms.Prepend(null));
            else if (Parameter.Definition.Type == typeof(Category)) CreateComboBox(ConfigTools.MainWindow.Project.Categories.Prepend(null));
            else if (Parameter.Definition.Type == typeof(int)) CreateString(nameof(Parameter.ValueAsInt));
            else if (Parameter.Definition.Type == typeof(uint)) CreateString(nameof(Parameter.ValueAsUint));
            else if (Parameter.Definition.Type == typeof(double)) CreateString(nameof(Parameter.ValueAsDouble));
            else if (Parameter.Definition.Type == typeof(string)) CreateString(nameof(Parameter.ValueAsString));
        }

        private void CreateComboBox(IEnumerable list)
        {
            ComboBox comboBox = new ComboBox() { Items = list };
            comboBox.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(Parameter.Value), BindingMode.TwoWay));
            Content = comboBox;
        }

        private void CreateButton(Action a)
        {
            Button button = new Button() { Content = "..." };
            button.Click += (s, e) => a();
            Content = button;
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
