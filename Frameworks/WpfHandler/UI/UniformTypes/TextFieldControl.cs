//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfHandler.UI.UniformTypes
{
    /// <summary>
    /// Defines base members for all text field for providing uniform way to operate with GUI elemetns.
    /// </summary>
    public class TextFieldControl : UserControl
    {
        /// <summary>
        /// Mode of value operating.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Allow any string value.
            /// </summary>
            String,
            /// <summary>
            /// Allow only int formated values.
            /// </summary>
            Int,
            /// <summary>
            /// Allow only float values.
            /// </summary>
            Float,
            /// <summary>
            /// Allow only double values.
            /// </summary>
            Double,
            /// <summary>
            /// Use custom regex to define if value is valid.
            /// </summary>
            Regex
        }
        
        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(string), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty ValueModeProperty = DependencyProperty.Register(
          "ValueMode", typeof(Mode), typeof(TextFieldControl));

        /// <summary>
        /// Occurs when content changes in the text element.
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                // добавление обработчика
                base.AddHandler(TextBox.TextChangedEvent, value);
            }
            remove
            {
                // удаление обработчика
                base.RemoveHandler(TextBox.TextChangedEvent, value);
            }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public virtual string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public virtual Mode ValueMode
        {
            get { return (Mode)this.GetValue(ValueModeProperty); }
            set { this.SetValue(ValueModeProperty, value); }
        }

        /// <summary>
        /// Uniform value.
        /// Allowed type depends from @ValueMode.
        /// </summary>
        public virtual object Value
        {
            get
            {
                try
                {
                    switch (ValueMode)
                    {
                        case Mode.String:
                        case Mode.Regex: return Text;

                        case Mode.Int: return int.Parse(Text);
                        case Mode.Float: return float.Parse(Text);
                        case Mode.Double: return double.Parse(Text);
                    }
                }
                catch { }

                return null;
            }
            set
            {
                try
                {
                    if (ValueMode != Mode.Regex)
                    {
                        // Trying to conver value to the string.
                        Text = (string)value;
                    }
                    else
                    {
                        // TODO: Aplly after regex validation.
                        throw new NotSupportedException();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Initizlize base UserControl contructor.
        /// </summary>
        protected TextFieldControl(){}
    }
}
