using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace DotHome.Model
{
    public abstract class Block : INotifyPropertyChanged
    {
        private bool selected;
        private int x, y, id;
        private string debugString;

        public bool Selected { get => selected; set => SetAndRaise(ref selected, value, nameof(Selected)); }

        public Exception Exception { get; set; }

        public int X { get => x; set => SetAndRaise(ref x, value, nameof(X)); }

        public int Y { get => y; set => SetAndRaise(ref y, value, nameof(Y)); }

        public int Id { get => id; set => SetAndRaise(ref id, value, nameof(Id)); }

        private Color debugColor = Color.Black;

        public string DebugString { get => debugString; set => SetAndRaise(ref debugString, value, nameof(DebugString)); }

        public List<Value> Inputs { get; } = new List<Value>();
        public List<Value> Outputs { get; } = new List<Value>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Block()
        {
            foreach(PropertyInfo propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Input<>))
                {
                    Inputs.Add((Value)propertyInfo.GetValue(this));
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Output<>))
                {
                    Outputs.Add((Value)propertyInfo.GetValue(this));
                }
                //else if (propertyInfo.GetCustomAttribute<BlockParameterAttribute>() != null)
                //{
                //    ParameterDefinition parameterDefinition = new ParameterDefinition();
                //    parameterDefinition.PropertyInfo = propertyInfo;
                //    parameterDefinition.Name = propertyInfo.Name;
                //    parameterDefinition.Type = propertyInfo.PropertyType;
                //    parameterDefinition.ShowInBlock = propertyInfo.GetCustomAttribute<BlockParameterAttribute>().ShowInBlock;
                //    parameterDefinition.Description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
                //    parameterDefinition.DefaultValue = propertyInfo.GetValue(Activator.CreateInstance(type, Enumerable.Repeat<object>(null, type.GetConstructors().Single().GetParameters().Length).ToArray()));
                //    parameterDefinition.ValidationAttributes.AddRange(propertyInfo.GetCustomAttributes<ValidationAttribute>(true));
                //    blockDefinition.Parameters.Add(parameterDefinition);
                //    parameterDeclaringTypesDictionary[parameterDefinition] = propertyInfo.DeclaringType;
                //    parameterOriginalOrderDictionary[parameterDefinition] = i++; ;
                //}
            }
        }

        public abstract void Init();

        public abstract void Run();

        protected void DebugWriteLine(object parameter, Color color)
        {
            DebugWrite(parameter.ToString() + "\n", color);
        }

        protected void DebugWriteLine(object parameter)
        {
            DebugWriteLine(parameter, Color.Black);
        }

        protected void DebugWrite(object parameter, Color color)
        {
            if(color == debugColor)
            {
                DebugString += parameter.ToString();
            }
            else
            {
                debugColor = color;
                DebugString += ColorToEscapeString(color) + parameter.ToString();
            }
        }

        protected void DebugWrite(object parameter)
        {
            DebugWrite(parameter, Color.Black);
        }

        public void ClearDebugString()
        {
            DebugString = "";
            debugColor = Color.Black;
        }

        private string ColorToEscapeString(Color color)
        {
            return "\u001bc" + (char)color.R + (char)color.G + (char)color.B;
        }

        private void SetAndRaise<T>(ref T field, T value, string name)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
