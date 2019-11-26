﻿//Copyright 2019 Volodymyr Podshyvalov
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHandler.UI.AutoLayout;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Defines base members for all text field for providing uniform way to operate with GUI elemetns.
    /// </summary>
    public abstract class TextFieldControl : UserControl, ILable
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
            /// TODO: Use custom regex to define if value is valid.
            /// </summary>
            Regex
        }

        #region Dependency properties
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
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LableProperty = DependencyProperty.Register(
          "Lable", typeof(string), typeof(TextFieldControl));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty LableWidthProperty = DependencyProperty.Register(
          "LableWidth", typeof(float), typeof(TextFieldControl), new PropertyMetadata(120.0f));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register(
          "TextBoxForeground", typeof(Brush), typeof(TextFieldControl),
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

        /// <summary>
        /// Property that bridging control's property between XAML and code.
        /// </summary>
        public static readonly DependencyProperty TextBoxBackgroundProperty = DependencyProperty.Register(
          "TextBoxBackground", typeof(Brush), typeof(TextFieldControl),
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a9e9"))));
        #endregion

        #region Properties
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
        /// Text in lable field.
        /// </summary>
        public string Lable
        {
            get { return (string)GetValue(LableProperty); }
            set { SetValue(LableProperty, value); }
        }

        /// <summary>
        /// Width of lable field.
        /// </summary>
        public float LableWidth
        {
            get { return (float)GetValue(LableWidthProperty); }
            set
            {
                // Buferize requested value.
                _LableWidth = value;

                // Set value but apply at least 25 point to input field.
                float appliedSize = (float)Math.Min(_LableWidth, ActualWidth - 25);

                // Appling value.
                SetValue(LableWidthProperty, appliedSize);
            }
        }

        /// <summary>
        /// Bufer that contains las requested lable width.
        /// </summary>
        protected float _LableWidth;

        /// <summary>
        /// Color of the text in textbox.
        /// </summary>
        public Brush TextBoxForeground
        {
            get { return (Brush)GetValue(TextBoxForegroundProperty); }
            set { SetValue(TextBoxForegroundProperty, value); }
        }

        /// <summary>
        /// Collor of the text box backplate.
        /// </summary>
        public Brush TextBoxBackground
        {
            get { return (Brush)GetValue(TextBoxBackgroundProperty); }
            set { SetValue(TextBoxBackgroundProperty, value); }
        }
        #endregion

        #region Customization block
        /// <summary>
        /// Returns reference to the lable block of UI element.
        /// </summary>
        public abstract FrameworkElement LableElement { get; }

        /// <summary>
        /// Returns reference to the field block of UI element.
        /// </summary>
        public abstract FrameworkElement FieldElement { get; }
        #endregion

        #region Constructor & desctructor
        /// <summary>
        /// Initizlize base UserControl contructor.
        /// </summary>
        protected TextFieldControl()
        {
            // Subscribing on the base events.
            SizeChanged += TextFieldControl_SizeChanged; 
            Loaded += TextFieldControl_Loaded;
        }

        /// <summary>
        /// Releasing unmanaged memory.
        /// </summary>
        ~TextFieldControl()
        {
            // Insubscribe from internal events.
            try { SizeChanged -= TextFieldControl_SizeChanged; } catch { }
            try { Loaded -= TextFieldControl_Loaded; } catch { }
        }
        #endregion

        #region API
        /// <summary>
        /// Recomputing dinamic layout values for providing hight quiality view.
        /// </summary>
        public virtual void RecomputeLayout()
        {
            var lableExist = !string.IsNullOrEmpty(Lable);

            if (lableExist)
            {
                // Show lable.
                LableElement.Visibility = Visibility.Visible;

                // Warping the input field.
                Grid.SetColumn(FieldElement, 2);
            }
            else
            {
                // Hide lable.
                LableElement.Visibility = Visibility.Collapsed;

                // Spreading the input field.
                Grid.SetColumn(FieldElement, 0);
            }

            // Reqcomputing width.
            LableWidth = _LableWidth;
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Init configs when all properties applied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Update default request lable size.
            _LableWidth = LableWidth;

            // Recomputing dinamic layout.
            RecomputeLayout();
        }

        /// <summary>
        /// Occurs when element size will shanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFieldControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RecomputeLayout();
            }
        }
        #endregion
    }
}
