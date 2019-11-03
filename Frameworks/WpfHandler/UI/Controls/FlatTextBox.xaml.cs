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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for FlatTextBox.xaml
    /// </summary>
    public partial class FlatTextBox : UserControl
    {
        #region Dependency properties
        public static readonly DependencyProperty LableProperty = DependencyProperty.Register(
          "Lable", typeof(string), typeof(FlatTextBox));

        public static readonly DependencyProperty LableWidthProperty = DependencyProperty.Register(
          "LableWidth", typeof(float), typeof(FlatTextBox));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          "Text", typeof(string), typeof(FlatTextBox));
        
        public static readonly DependencyProperty TextBoxForegroundProperty = DependencyProperty.Register(
          "TextBoxForeground", typeof(Brush), typeof(FlatTextBox), 
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

        public static readonly DependencyProperty TextBoxBackgroundProperty = DependencyProperty.Register(
          "TextBoxBackground", typeof(Brush), typeof(FlatTextBox),
          new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a9e9"))));
        #endregion

        #region Properties
        /// <summary>
        /// Text in lable field.
        /// </summary>
        public string Lable
        {
            get { return (string)this.GetValue(LableProperty); }
            set { this.SetValue(LableProperty, value); }
        }

        /// <summary>
        /// Width of lable field.
        /// </summary>
        public float LableWidth
        {
            get { return (float)this.GetValue(LableWidthProperty); }
            set { this.SetValue(LableWidthProperty, value); }
        }

        /// <summary>
        /// Text in textbox.
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Color of the text in textbox.
        /// </summary>
        public Brush TextBoxForeground
        {
            get { return (Brush)this.GetValue(TextBoxForegroundProperty); }
            set { this.SetValue(TextBoxForegroundProperty, value); }
        }

        /// <summary>
        /// Collor of the text box backplate.
        /// </summary>
        public Brush TextBoxBackground
        {
            get { return (Brush)this.GetValue(TextBoxBackgroundProperty); }
            set { this.SetValue(TextBoxBackgroundProperty, value); }
        }

        #endregion

        public FlatTextBox()
        {
            InitializeComponent();

            DataContext = this;

            // Try to load default style
            try
            {
                if (Application.Current.FindResource("FlatTextBox") is Style style)
                {
                    this.Style = style;
                }
            }
            catch
            {
                // Not found in dictionary. Not important.}
            }
        }
    }
}
